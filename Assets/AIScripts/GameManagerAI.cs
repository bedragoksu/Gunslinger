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

public class GameManagerAI : NetworkBehaviour
{
    public int NumberOfPlayers;

    [HideInInspector] public int SheriffNumber = 1;
    [HideInInspector] public int RenegadeNumber = 1;
    [HideInInspector] public int OutlawNumber = 2;
    [HideInInspector] public int DeputyNumber = 0;

    [SyncVar] public GameState CurrentGameState; // unnecessary sync?
    private static event Action<GameState> _onGameStateChanged;

    [SerializeField] private PlayerRolesControllerAI _prc;
    [SerializeField] private CardsControllerAI _cc;

    private GameObject[] _turns;
    [SyncVar] private int _turnInt; public int GetTurnInt() { return _turnInt; }

    private bool _canStart = false;

    public GameObject CharacterDisplayer; // to assign character roles to canvas/life
    public GameObject CardDisplayer; // to make changes on cards to canvas
    public GameObject CharacterPositionController; // to assign new positions to characters
    private bool _isRoleAssinged = false;

    private Button _discardButton;
    public void OpenCloseDiscardButton(bool open)
    {
        _discardButton.interactable = open;
    }
    public bool IsActiveButton()
    {
        return _discardButton.interactable;
    }

    public GameObject _thisPlayer;

    

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
        }

    }

    // butonla alakali kisimlar burada buna script ac.!
    public void DiscardUIButton(GameObject ButtonsObj)
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "discard ui button");

        Button[] buttons = ButtonsObj.GetComponentsInChildren<Button>();
        buttons[0].interactable = false;

        AssignStateServer(GameState.DiscardCard);

        var pl = _thisPlayer.GetComponent<PlayerModelAI>();
        if (pl.openHand.Count <= pl.CurrentBulletPoint)
        {
            buttons[1].interactable = true;
        }
        
        
    }
    public void NextUIButton()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "next ui button");

        _turns[_turnInt].GetComponent<PlayerModelAI>().PlayedBang = false;
        StartCoroutine(r());

        AssignStateServer(GameState.DrawCard);
    }
    public void ActivateNextButton()
    {
        var pl = _thisPlayer.GetComponent<PlayerModelAI>();
        if (pl.openHand.Count == pl.CurrentBulletPoint+1)
        {
            Button[] buttons = GameObject.Find("Buttons").GetComponentsInChildren<Button>();
            buttons[1].interactable = true;
        }
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
        server();
        b = true;
    }
    [ObserversRpc]
    public void server()
    {
        do
        {
            _turnInt++;
            _turnInt %= _turns.Length;
        } while (!_turns[_turnInt].GetComponent<PlayerModelAI>().IsAlive);
        
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

    private void HandleDiscardCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "DISCARD CARD STATE");
    }


    private void HandlePlayCard()
    {
        StartCoroutine(PlayRoutine());
    }
    private IEnumerator PlayRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        ScreenLog.Instance.SendEvent(TextType.Debug, "PLAY CARD STATE");
        ScreenLog.Instance.SendEvent(TextType.Debug, $"turnint: {_turnInt}, {_thisPlayer.GetComponent<PlayerModelAI>().PlayerID}");
        if (_thisPlayer.GetComponent<PlayerModelAI>().PlayerID == _turnInt) //true
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "activa");
            //Activate(_discardButton);
            OpenCloseDiscardButton(true);
            // activate the open hands clickable
        }
        _thisPlayer.GetComponent<PlayerModelAI>().cardchange(true);
    }


    private void HandleDrawCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD STATE");


        StartCoroutine(DrawCardRoutine());
    }
    private IEnumerator DrawCardRoutine()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD ROUTINE");
        var currentPlayer = _turns[_turnInt];
        var end = GameObject.Find("CardsControllerAI").GetComponent<CardsControllerAI>().DrawCards(currentPlayer, 2);
        yield return new WaitUntil(() => end);
        AssignStateServer(GameState.PlayCard);
    } 

    
    private void HandleInitialization() // bir kez cagiriliyor her client icin
    {
        _turns = GameObject.FindGameObjectsWithTag("Player");
        _discardButton = GameObject.Find("discard").GetComponent<Button>();
        // herkesin player modelini g�rs�n herkes
        foreach (var pl in _turns)
        {
            if (pl.GetComponent<PlayerModelAI>().enabled)
            {
                _thisPlayer = pl;
            }
        }
        StartCoroutine(InitializationRoutine());
    }
    private IEnumerator InitializationRoutine()
    {
        yield return new WaitUntil(() => _prc.AssignRoles());  // bu fonksiyonu par�ala
        yield return new WaitForSecondsRealtime(0.5f); // bu bir f hi� olmad� ya :( neyse d�zeltcez

        // who has the turn: ...
        yield return new WaitUntil(() => AssignTurns());
        yield return new WaitForSecondsRealtime(0.5f);

        yield return new WaitUntil(() => _cc.DealCards());
        
        
        UpdateGameState(GameState.DrawCard);
        //AssignStateServer(GameState.DrawCard);
    }
    

    private void HandleLobby()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"LOBBY STATE");
        _canStart = true;
        AssignTurnIndex(0);
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

    private bool AssignTurns()
    {
        for(int i=0; i< _turns.Length;i++)
        {
            if(_turns[i].GetComponent<PlayerModelAI>().PlayerRole == PlayerModelAI.TypeOfPlayer.Sheriff)
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


    public void DrawTheCard(GameObject player, GameObject card)
    {
        DrawTheCardServer(player, card);
    }
    [ServerRpc(RequireOwnership = false)]
    public void DrawTheCardServer(GameObject player, GameObject card)
    {
        DrawTheCardObserver(player, card);
    }
    [ObserversRpc]
    public void DrawTheCardObserver(GameObject player, GameObject card)
    {
        player.GetComponent<PlayerModelAI>().openHand.Add(card);
    }

}