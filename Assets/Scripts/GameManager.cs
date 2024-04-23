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

    private GameObject[] _turns;

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
                //HandleDrawCard();
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

    private void HandleInitialization()
    {
        StartCoroutine(InitializationRoutine());
    }
    public bool _rolesAssigned = false;
    private IEnumerator InitializationRoutine()
    {
        _prc.AssignRoles();

        yield return new WaitUntil(() => _rolesAssigned);
        yield return new WaitForSecondsRealtime(1f); // bu bir f hiç olmadý ya :( neyse düzeltcez
        // deal the cards, wait until

        // who has the turn: ...
        Debug.Log("assign turns start");
        _turns = AssignTurns();
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

    private GameObject[] AssignTurns()
    {
        var Players = GameObject.FindGameObjectsWithTag("Player");
        //foreach (var pl in Players)
        //{
        //    Debug.Log(pl.GetComponent<PlayerModel>().PlayerRole);
        //}
        Debug.Log(Players[0].GetComponent<PlayerModel>().PlayerRole);
        while (Players[0].GetComponent<PlayerModel>().PlayerRole != PlayerModel.TypeOfPlayer.Sheriff)
        {
            Debug.Log(Players[0].GetComponent<PlayerModel>().PlayerRole);
            var first = Players[0];
            for (int i = 0; i < Players.Length - 1; i++)
            {
                Players[i] = Players[i + 1];
            }
            Players[Players.Length - 1] = first;
        }
        return Players;
    }
}