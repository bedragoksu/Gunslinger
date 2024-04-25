using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using NeptunDigital;
using Gunslinger.Controller;


public class GameManager : NetworkBehaviour
{
    public int NumberOfPlayers;

    [HideInInspector] public int SheriffNumber = 1;
    [HideInInspector] public int RenegadeNumber = 1;
    [HideInInspector] public int OutlawNumber = 2;
    [HideInInspector] public int DeputyNumber = 0;

    [HideInInspector] public GameState CurrentGameState;
    private static event Action<GameState> _onGameStateChanged;

    private bool _canStart = false;
    [SerializeField] private PlayerRolesController _prc;

    public bool _rolesAssigned = false;
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
                break;
            case GameState.EndOfGame:
                //HandleEndOfGame();
                break;
        }
        _onGameStateChanged?.Invoke(newState);
    }

    private void HandleDrawCard()
    {
        var currentPlayer = _turns[_turnInt];
    }

    private void HandleInitialization()
    {
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
        AssignTurns();

        // draw cards (who has the turn)
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

    private void AssignTurns()
    {
        _turns = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i< _turns.Length;i++)
        {
            if(_turns[i].GetComponent<PlayerModel>().PlayerRole == PlayerModel.TypeOfPlayer.Sheriff)
            {
                _turnInt = i;
                break;
            }
        }
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