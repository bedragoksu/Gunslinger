using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using NeptunDigital;
using Gunslinger.Controller;
using NeptunDigital;
using FishNet.Object.Synchronizing;

public class GameManager : NetworkBehaviour
{
    public int NumberOfPlayers;

    [HideInInspector] public int SheriffNumber = 1;
    [HideInInspector] public int RenegadeNumber = 1;
    [HideInInspector] public int OutlawNumber = 2;
    [HideInInspector] public int DeputyNumber = 0;

    [SyncVar][HideInInspector] public GameState CurrentGameState;
    private static event Action<GameState> _onGameStateChanged;

    private bool _canStart = false;
    [SerializeField] private PlayerRolesController _prc;

    [HideInInspector] public bool _rolesAssigned = false;
    private GameObject[] _turns;
    private int _turnInt = 0;

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
            else if (Input.GetKeyDown(KeyCode.DownArrow)){
                ScreenLog.Instance.SendEvent(TextType.Debug, $"curr state: { CurrentGameState}");
            }
        }
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
                _turnInt++;
                if (_turnInt == _turns.Length) _turnInt = 0;
                break;
            case GameState.EndOfGame:
                //HandleEndOfGame();
                break;
        }
        _onGameStateChanged?.Invoke(newState);
    }

    private void HandleDrawCard() // herkesin draw card state olmasý mý lazým?
    {
        //var currentPlayer = _turns[_turnInt];
        ScreenLog.Instance.SendEvent(TextType.Debug, $"DRAW CARD STATE");
    }

    

    
    private void HandleInitialization()
    {
        _turns = GameObject.FindGameObjectsWithTag("Player");
        // herkesin player modelini görsün herkes

        StartCoroutine(InitializationRoutine());
    }
    private IEnumerator InitializationRoutine()
    {
        _prc.AssignRoles();

        yield return new WaitUntil(() => _rolesAssigned);
        yield return new WaitForSecondsRealtime(1f); // bu bir f hiç olmadý ya :( neyse düzeltcez
        // deal the cards, wait until

        // who has the turn: ...
        Debug.Log("assign turn int start");
        yield return new WaitUntil(() => AssignTurns());
        // draw cards (who has the turn)
        // herkes draw card state'ine geçsin ve turn index paylaþýlsýn
        //UpdateGameState(GameState.DrawCard);
        AssignState(GameState.DrawCard, _turnInt);
    }
    [ObserversRpc]
    public void AssignState(GameState newState, int index)
    {
        UpdateGameState(newState);
        //gameManager._turnInt = index;
    }

    private void HandleLobby()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"LOBBY STATE");
        //MixTheCards();
        _canStart = true;
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
                _turnInt = i;
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
}