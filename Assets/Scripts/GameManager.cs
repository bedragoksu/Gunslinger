using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using NeptunDigital;
using Gunslinger.Controller;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public int NumberOfPlayers;

    [HideInInspector] public int SheriffNumber = 1;
    [HideInInspector] public int RenegadeNumber = 1;
    [HideInInspector] public int OutlawNumber = 2;
    [HideInInspector] public int DeputyNumber = 0;

    [SyncVar] public GameState CurrentGameState; // unnecessary sync?
    private static event Action<GameState> _onGameStateChanged;

    [SerializeField] private PlayerRolesController _prc;
    [SerializeField] private CardsController _cc;

    private GameObject[] _turns;
    [SyncVar] private int _turnInt;

    private bool _canStart = false;

    public GameObject CharacterDisplayer; // to assign character roles to canvas/life
    public GameObject CharacterPositionController; // to assign new positions to characters
    private bool _isRoleAssinged = false;

    private Button _discardButton;

    private GameObject _thisPlayer;

    private void Start()
    {
        UpdateGameState(GameState.Lobby);
    }

    private void Update()
    {

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.B) && _canStart && IsServer)
            {
                var len =_prc.PlayersUpdate();
                
                if (len >= 4 && len <= 7)
                {
                    _canStart = false;
                    AssignStateServer(GameState.Initialization);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ScreenLog.Instance.SendEvent(TextType.Debug, $"curr state: { CurrentGameState}");
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                StartCoroutine(r());
            }
        }
    }

    // butonla alakali kisimlar burada buna script ac.!
    public void DiscardUIButton()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "discard ui button");
        AssignStateServer(GameState.DiscardCard);
    }
    public void NextUIButton()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "next ui button");
        StartCoroutine(r());
        AssignStateServer(GameState.DrawCard);
    }
    public void Activate(Button button)
    {
        button.interactable = true;
    }

    private bool b = false;
    private IEnumerator r()
    {
        nextplayer();
        yield return new WaitUntil(() => b);
        yield return new WaitForSeconds(0.5f);
        b = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void nextplayer()
    {
        //_turnInt = ++_turnInt%4;
        //ScreenLog.Instance.SendEvent(TextType.Debug, $"_turnInt= {_turnInt}");
        server();
        b = true;
    }
    [ObserversRpc]
    public void server()
    {
        _turnInt++;
        _turnInt %= 4;
        ScreenLog.Instance.SendEvent(TextType.Debug, $"_turnInt= {_turnInt}");
    }

    public enum GameState
    {
        Lobby,
        Initialization,
        DrawCard,
        PlayCard,
        DiscardCard,
        EndOfGame
    }

    public void UpdateGameState(GameState newState)
    {
        CurrentGameState = newState;

        switch (CurrentGameState)
        {
            case GameState.Lobby:
                HandleLobby();
                break;
            case GameState.Initialization:
                HandleInitialization();
                break;
            case GameState.DrawCard:
                HandleDrawCard();
                break;
            case GameState.PlayCard:
                HandlePlayCard();
                break;
            case GameState.DiscardCard:
                HandleDiscardCard();
                break;
            case GameState.EndOfGame:
                //HandleEndOfGame();
                break;
        }
        _onGameStateChanged?.Invoke(newState);
    }

    private void HandlePlayCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "PLAY CARD STATE");
        ScreenLog.Instance.SendEvent(TextType.Debug, $"turnint: {_turnInt}, {_thisPlayer.GetComponent<PlayerModel>().PlayerID}");
        if (_thisPlayer.GetComponent<PlayerModel>().PlayerID == _turnInt) //true
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "activa");
            Activate(_discardButton);
        }
    }

    private void HandleDiscardCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "DISCARD CARD STATE");
    }
    private void HandleDrawCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD STATE");
        if (!_isRoleAssinged)
        {
            CharacterDisplayer.GetComponent<CharacterDisplayer>().roleOnChange();
            CharacterPositionController.GetComponent<CharacterPositionController>().UpdateCharacterPositions();
            _isRoleAssinged = true; 
        }
        
        StartCoroutine(DrawCardRoutine());
    }
    private IEnumerator DrawCardRoutine()
    {
        var currentPlayer = _turns[_turnInt];
        var end = GameObject.Find("CardsController").GetComponent<CardsController>().DrawCards(currentPlayer, 2);
        yield return new WaitUntil(() => end);
        AssignStateServer(GameState.PlayCard);
    } 
    
    private void HandleInitialization()
    {
        _turns = GameObject.FindGameObjectsWithTag("Player");
        _discardButton = GameObject.Find("discard").GetComponent<Button>();
        // herkesin player modelini görsün herkes
        foreach (var pl in _turns)
        {
            if (pl.GetComponent<PlayerModel>().enabled)
            {
                _thisPlayer = pl;
            }
        }

        StartCoroutine(InitializationRoutine());
    }
    private IEnumerator InitializationRoutine()
    {
        yield return new WaitUntil(() => _prc.AssignRoles());  // bu fonksiyonu parçala
        yield return new WaitForSecondsRealtime(0.5f); // bu bir f hiç olmadý ya :( neyse düzeltcez
        yield return new WaitUntil(() => _cc.DealCards());
        yield return new WaitForSecondsRealtime(0.5f);

        // who has the turn: ...
        Debug.Log("assign turn int start");
        yield return new WaitUntil(() => AssignTurns());
        
        //UpdateGameState(GameState.DrawCard);
        AssignStateServer(GameState.DrawCard);
    }
    [ServerRpc(RequireOwnership = false)]
    public void AssignStateServer(GameState newState)
    {
        AssignState(newState);
    }
    [ObserversRpc]
    public void AssignState(GameState newState)
    {
        UpdateGameState(newState);
    }

    private void HandleLobby()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"LOBBY STATE");
        //MixTheCards();
        _canStart = true;
        AssignTurnIndex(0);
    }

    private void MixTheCards()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "Mixing The Cards");

        // mixing process, wait until

        _canStart = true;
    }

    private bool AssignTurns()
    {
        for(int i=0; i< _turns.Length;i++)
        {
            if(_turns[i].GetComponent<PlayerModel>().PlayerRole == PlayerModel.TypeOfPlayer.Sheriff)
            {
                AssignTurnIndex(i);
                break;
            }
        }
        return true;
    }
    
    [ObserversRpc]
    public void AssignTurnIndex(int i)
    {
        _turnInt = i;
    }

    
}