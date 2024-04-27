using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using NeptunDigital;
using Gunslinger.Controller;
using FishNet.Object.Synchronizing;

public class GameManager : NetworkBehaviour
{
    public int NumberOfPlayers;

    [HideInInspector] public int SheriffNumber = 1;
    [HideInInspector] public int RenegadeNumber = 1;
    [HideInInspector] public int OutlawNumber = 2;
    [HideInInspector] public int DeputyNumber = 0;

    [HideInInspector] public GameState CurrentGameState;
    private static event Action<GameState> _onGameStateChanged;

    [SerializeField] private PlayerRolesController _prc;

    private GameObject[] _turns;
    [SyncVar] private int _turnInt;

    private bool _canStart = false;

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
                    UpdateGameState(GameState.Initialization);
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

    private bool b = false;
    private IEnumerator r()
    {
        nextplayer();
        AssignState(GameState.PlayCard);
        yield return new WaitUntil(() => b);
        yield return new WaitForSeconds(0.5f);
        b = false;
        AssignState(GameState.DrawCard);
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
                //HandlePlayCard();
                break;
            case GameState.DiscardCard:
                //HandleDiscardCard();
                break;
            case GameState.EndOfGame:
                //HandleEndOfGame();
                break;
        }
        _onGameStateChanged?.Invoke(newState);
    }

    private void HandleDrawCard()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD STATE");
        StartCoroutine(DrawCardRoutine());
    }
    private IEnumerator DrawCardRoutine()
    {
        var currentPlayer = _turns[_turnInt];
        var end = GameObject.Find("CardsController").GetComponent<CardsController>().DrawCards(currentPlayer, 2);
        yield return new WaitUntil(() => end);
        AssignState(GameState.PlayCard);
    } 
    
    private void HandleInitialization()
    {
        _turns = GameObject.FindGameObjectsWithTag("Player");
        // herkesin player modelini görsün herkes

        StartCoroutine(InitializationRoutine());
    }
    private IEnumerator InitializationRoutine()
    {
        yield return new WaitUntil(() => _prc.AssignRoles());  // bu fonksiyonu parçala
        yield return new WaitForSecondsRealtime(1f); // bu bir f hiç olmadý ya :( neyse düzeltcez

        // who has the turn: ...
        Debug.Log("assign turn int start");
        yield return new WaitUntil(() => AssignTurns());
        
        //UpdateGameState(GameState.DrawCard);
        AssignState(GameState.DrawCard);
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
        //Debug.Log(Players[0].GetComponent<PlayerModel>().PlayerRole);
        //while (Players[0].GetComponent<PlayerModel>().PlayerRole != PlayerModel.TypeOfPlayer.Sheriff)
        //{
        //    Debug.Log(Players[0].GetComponent<PlayerModel>().PlayerRole);
        //    var first = Players[0];
        //    for (int i = 0; i < Players.Length - 1; i++)
        //    {
        //        Players[i] = Players[i + 1];
        //    }
        //    Players[Players.Length - 1] = first;
        //}
    }
    
    [ObserversRpc]
    public void AssignTurnIndex(int i)
    {
        _turnInt = i;
    }
}