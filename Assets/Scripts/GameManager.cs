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
    [SerializeField] private CharacterCanvasController _ccc;

    public GameObject[] _turns;
    [SyncVar] private int _turnInt; public int GetTurnInt() { return _turnInt; }

    private bool _canStart = false;

    public GameObject CharacterDisplayer; // to assign character roles to canvas/life
    public GameObject CardDisplayer; // to make changes on cards to canvas
    public GameObject CharacterPositionController; // to assign new positions to characters
    public GameObject PlayerInfoStack; // to initialize stack ui
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
                _turns = GameObject.FindGameObjectsWithTag("Player");

                if (_turns.Length >= 4 && _turns.Length <= 7)
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

        var pl = _thisPlayer.GetComponent<PlayerModel>();
        if (pl.openHand.Count <= pl.CurrentBulletPoint)
        {
            buttons[1].interactable = true;
        }

        _ccc.UpdateCanvas();
    }
    public void NextUIButton()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "next ui button");

        _turns[_turnInt].GetComponent<PlayerModel>().PlayedBang = false;
        StartCoroutine(r());

        AssignStateServer(GameState.DrawCard);
    }
    public void ActivateNextButton()
    {
        var pl = _thisPlayer.GetComponent<PlayerModel>();
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
        } while (!_turns[_turnInt].GetComponent<PlayerModel>().IsAlive);
        
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
                HandleEndOfGame();
                break;
        }
        _onGameStateChanged?.Invoke(newState);
    }

    private void HandleEndOfGame()
    {
        List<PlayerModel> alivePlayers = _cc.GetAlivePlayers();
        bool sheriff = false;
        bool deputy = false;
        bool outlaw = false;
        bool renegade = false;

        foreach(var a in alivePlayers)
        {
            switch (a.PlayerRole)
            {
                case PlayerModel.TypeOfPlayer.Sheriff:
                    sheriff = true;
                    break;
                case PlayerModel.TypeOfPlayer.Deputy:
                    deputy = true;
                    break;
                case PlayerModel.TypeOfPlayer.Outlaw:
                    outlaw = true;
                    break;
                case PlayerModel.TypeOfPlayer.Renegade:
                    renegade = true;
                    break;
            }
        }

        if (outlaw)
        {
            // haydutlar kazandi
        }else if (sheriff)
        {
            // aynasizlar ve serif kazandi
        }else if (renegade)
        {
            // hain kazandi
        }

        // beklet ve finito
        
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
        ScreenLog.Instance.SendEvent(TextType.Debug, $"turnint: {_turnInt}, {_thisPlayer.GetComponent<PlayerModel>().PlayerID}");
        if (_thisPlayer.GetComponent<PlayerModel>().PlayerID == _turnInt) //true
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "activa");
            //Activate(_discardButton);
            OpenCloseDiscardButton(true);
            // activate the open hands clickable
        }
        _thisPlayer.GetComponent<PlayerModel>().cardchange(true);
        PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();
        _ccc.UpdateCanvas();
    }


    private void HandleDrawCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD STATE");
        if (!_isRoleAssinged) // canvas duzeni icin
        {
            CharacterDisplayer.GetComponent<CharacterDisplayer>().roleOnChange();
            CharacterPositionController.GetComponent<CharacterPositionController>().UpdateCharacterPositions();
            PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().InitializeStackCanvas();
            _isRoleAssinged = true;
        }
        //CardDisplayer.GetComponent<HandDisplay>().handCardOnChange();
        _ccc.UpdateCanvas();

        StartCoroutine(DrawCardRoutine());
    }
    private IEnumerator DrawCardRoutine()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD ROUTINE");
        var currentPlayer = _turns[_turnInt];
        var end = GameObject.Find("CardsController").GetComponent<CardsController>().DrawCards(currentPlayer, 2);
        yield return new WaitUntil(() => end);
        AssignStateServer(GameState.PlayCard);
    } 

    
    private void HandleInitialization() // bir kez cagiriliyor her client icin
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
        player.GetComponent<PlayerModel>().openHand.Add(card);
    }

}