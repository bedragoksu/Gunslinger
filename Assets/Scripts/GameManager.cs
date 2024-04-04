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
    private PlayerRolesController _prc;

    private void Start()
    {
        UpdateGameState(GameState.Lobby);
    }

    private void Update()
    {
        //if (Input.anyKeyDown)
        //{
        //    if (Input.GetKeyDown(KeyCode.CapsLock) && IsServer)
        //    {
        //        ScreenLog.Instance.SendEvent(TextType.Debug, $"we are at game manager btw");
        //    }
        //}


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
        _prc.AssignRoles();

        // deal the cards, wait until

        // who has the turn: ...
        
        // draw cards (who has the turn)
    }

    private void HandleLobby()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, $"LOBBY STATE");
        MixTheCards();

    }

    private void MixTheCards()
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "Mixing The Cards");

        // mixing process, wait until

        _canStart = true;
    }
}