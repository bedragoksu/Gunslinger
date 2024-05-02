using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using NeptunDigital;
using Gunslinger.Controller;
using FishNet.Object.Synchronizing;
using TMPro;

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

    
    public void DiscardUIButton()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "discard ui button");
        AssignStateServer(GameState.DiscardCard);
    }

    private bool b = false;
    private IEnumerator r()
    {
        nextplayer();
        yield return new WaitUntil(() => b);
        yield return new WaitForSeconds(0.5f);
        b = false;
        AssignStateServer(GameState.DrawCard);
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
        AssignStateServer(GameState.PlayCard);
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
                ScreenLog.Instance.SendEvent(TextType.Debug, "PLAY CARD STATE");
                //HandlePlayCard();
                break;
            case GameState.DiscardCard:
                ScreenLog.Instance.SendEvent(TextType.Debug, "DISCARD CARD STATE");
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
        CharacterDisplayer.GetComponent<CharacterDisplayer>().roleOnChange();
        CharacterPositionController.GetComponent<CharacterPositionController>().UpdateCharacterPositions();
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
        // herkesin player modelini g�rs�n herkes

        StartCoroutine(InitializationRoutine());
    }
    private IEnumerator InitializationRoutine()
    {
        yield return new WaitUntil(() => _prc.AssignRoles());  // bu fonksiyonu par�ala
        yield return new WaitForSecondsRealtime(0.5f); // bu bir f hi� olmad� ya :( neyse d�zeltcez
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