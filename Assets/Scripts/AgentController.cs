using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Gunslinger.Controller;
using Random = UnityEngine.Random;

public class AgentController : MonoBehaviour
{

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Actions _actions;
    [SerializeField] private CardsController cardsController;

    private class InfoPlayer
    {
        public string Name;
        public int CurrentBulletNumber;
        public bool IsAlive;
        public int CardNumber;
        public PlayerModel.TypeOfPlayer Role;

        public InfoPlayer(string Name, int CurrentBulletNumber, int CardNumber, PlayerModel.TypeOfPlayer Role = PlayerModel.TypeOfPlayer.Bos)
        {
            this.Name = Name;
            this.CurrentBulletNumber = CurrentBulletNumber;
            //this.IsAlive = IsAlive;
            this.CardNumber = CardNumber;
            this.Role = Role;
        }
    }

    // currentbulletpoint, IsAlive, cardnum, varsa rol, name,acik kartlar
    public void AgentDecideToPlay(GameObject AgentPlayer)
    {
        // about players
        PlayerModel AgentPlayerModel = AgentPlayer.GetComponent<PlayerModel>();
        Dictionary<int, InfoPlayer> PlayerInfos = CreatePlayerInfos(AgentPlayerModel);

        // helper lists
        List<PlayerModel> CanHitPlayer = _actions.CanHitAnyone(AgentPlayer);
        List<string> OpenHandCardNames = CardListByName(AgentPlayerModel);

        // DECIDE TREE

        // BANG

        bool containsBang = OpenHandCardNames.Any(item => item.StartsWith("bang", StringComparison.OrdinalIgnoreCase));

        if (containsBang && CanHitPlayer.Count != 0 && (AgentPlayerModel.CanPlayMultipleBangs || !AgentPlayerModel.PlayedBang))
        {
            int index = 0;
            switch (AgentPlayerModel.PlayerRole)
            {
                case PlayerModel.TypeOfPlayer.Sheriff:
                    // hit randomly
                    AgentPlayerModel.PlayedBang = true;
                    index = Random.Range(0, CanHitPlayer.Count);
                    BangForAgent(AgentPlayerModel, (CanHitPlayer[index].gameObject));
                    break;
                case PlayerModel.TypeOfPlayer.Deputy:
                    // hit randomly unless its not the sheriff
                    AgentPlayerModel.PlayedBang = true;
                    index = Random.Range(0, CanHitPlayer.Count);
                    BangForAgent(AgentPlayerModel, (CanHitPlayer[index].gameObject));
                    break;
                case PlayerModel.TypeOfPlayer.Outlaw:
                    // hit the sheriff
                    AgentPlayerModel.PlayedBang = true;
                    index = Random.Range(0, CanHitPlayer.Count);
                    BangForAgent(AgentPlayerModel, (CanHitPlayer[index].gameObject));
                    break;
                case PlayerModel.TypeOfPlayer.Renegade:
                    // if there is only sheriff then bang it, but there is someone else hit it
                    AgentPlayerModel.PlayedBang = true;
                    index = Random.Range(0, CanHitPlayer.Count);
                    BangForAgent(AgentPlayerModel, (CanHitPlayer[index].gameObject));
                    break;
            }

            // discard that bang
            DiscardCard("Bang", OpenHandCardNames, AgentPlayerModel);
        }



    }

    private bool barrelSaved = false;
    private void BangForAgent(PlayerModel AgentPlayer, GameObject target)
    {
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();

        Debug.Log($"{AgentPlayer.PlayerName} hits with bang to {targetPlayer.PlayerName}");

        bool hasBarrel = false;
        foreach (var stack in targetPlayer.stackHand)
        {
            if (stack.name.StartsWith("Barrel"))
            {
                hasBarrel = true;
                break;
            }
        }

        barrelSaved = false;
        if (hasBarrel)
        {
            // look out for the next card
            StartCoroutine("CheckTheNextCard", target);
        }


        if (!barrelSaved)
        {
            bool hasMissed = false;
            var hand = targetPlayer.openHand;
            for (int i = 0; i < hand.Count; i++)
            {
                var name = hand[i].name;
                if (name.StartsWith("Missed"))
                {
                    _actions.DiscardCard(target, i);
                    hasMissed = true;
                    //MissedAction(targetPlayer);
                    break;
                }
            }

            //bitki cayi
            if (!hasMissed)
            {
                var hasBeer = false;
                if (targetPlayer.CurrentBulletPoint == 1)
                {
                    for (int i = 0; i < hand.Count; i++)
                    {
                        var name = hand[i].name;
                        if (name.StartsWith("Beer"))
                        {
                            _actions.DiscardCard(target, i);
                            hasBeer = true;
                            //MissedAction(targetPlayer);
                            break;
                        }
                    }
                }
                if (!hasBeer) { cardsController.UpdateHealthServer(targetPlayer, -1); }
            }

        }
    }


    private List<string> CardListByName(PlayerModel player)
    {
        List<string> CardNames = new();

        foreach (GameObject card in player.openHand)
        {
            CardNames.Add(card.name);
        }

        return CardNames;
    }

    private Dictionary<int, InfoPlayer> CreatePlayerInfos(PlayerModel AgentPlayerModel)
    {
        Dictionary<int, InfoPlayer> PlayerInfos = new Dictionary<int, InfoPlayer>();
        foreach (GameObject player in _gameManager._turns)
        {
            PlayerModel playerModel = player.GetComponent<PlayerModel>();
            if (playerModel.PlayerID != AgentPlayerModel.PlayerID && playerModel.IsAlive)
            {
                InfoPlayer info = new(playerModel.PlayerName, playerModel.CurrentBulletPoint, playerModel.openHand.Count,
                                                    (playerModel.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff) ? PlayerModel.TypeOfPlayer.Sheriff : PlayerModel.TypeOfPlayer.Bos);
                PlayerInfos.Add(playerModel.PlayerID, info);
            }
        }

        return PlayerInfos;
    }

    private void DiscardCard(string CardName, List<string> OpenHandCardNames, PlayerModel AgentPlayerModel)
    {
        int index = -1;
        for (int i = 0; i < OpenHandCardNames.Count; i++)
        {
            if (OpenHandCardNames[i].StartsWith(CardName))
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            AgentPlayerModel.openHand.RemoveAt(index);
            OpenHandCardNames.RemoveAt(index);
        }
    }

}
