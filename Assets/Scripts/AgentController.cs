using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using FishNet;
using Gunslinger.Controller;
using Random = UnityEngine.Random;
using FishNet.Object;

public class AgentController : NetworkBehaviour
{

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Actions _actions;
    [SerializeField] private CardsController cardsController;

    private float _timeDelay = 2f;

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
    public IEnumerator AgentDecideToPlay(GameObject AgentPlayer)
    {
        // about players
        PlayerModel AgentPlayerModel = AgentPlayer.GetComponent<PlayerModel>();
        int MaxBulletPoint = (AgentPlayerModel.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff) ? 5 : 4;
        Dictionary<int, InfoPlayer> PlayerInfos = CreatePlayerInfos(AgentPlayerModel);

        // helper lists
        List<PlayerModel> CanHitPlayer = _actions.CanHitAnyone(AgentPlayer);
        List<string> OpenHandCardNames = CardListByName(AgentPlayerModel);

        // DECIDE TREE
        AgentPlayerModel.PlayedBang = false;


        // STACK HAND'E ATILABILECEK KARTLAR

        // BANG
        BangDecideTree(OpenHandCardNames, CanHitPlayer, AgentPlayerModel);
        yield return new WaitForSeconds(_timeDelay);

        // HEALTH
        while (AgentPlayerModel.CurrentBulletPoint < MaxBulletPoint)
        {
            if (OpenHandCardNames.Any(item => item.StartsWith("Beer", StringComparison.OrdinalIgnoreCase)))
            {
                // play beer
                Debug.Log("B�TK� �AY ���YORUM");
                _actions.BeerAction(AgentPlayerModel);
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Beer", OpenHandCardNames, AgentPlayerModel)));
                yield return new WaitForSeconds(_timeDelay);
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Saloon", StringComparison.OrdinalIgnoreCase)))
            {
                // play saloon
                Debug.Log("HERKESE BENDEN �AY");
                _actions.SaloonAction();
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Saloon", OpenHandCardNames, AgentPlayerModel)));
                yield return new WaitForSeconds(_timeDelay);
            }
            else
            {
                break;
            }
        }

        // gatling bang
        if (OpenHandCardNames.Any(item => item.StartsWith("Gatling", StringComparison.OrdinalIgnoreCase)))
        {
            Debug.Log("GATLING ATTII");
            _actions.GatlingAction(AgentPlayer);
            StartCoroutine(DiscardCardRoutine(Tuple.Create("Gatling", OpenHandCardNames, AgentPlayerModel)));
            yield return new WaitForSeconds(_timeDelay);
        }

        BangDecideTree(OpenHandCardNames, CanHitPlayer, AgentPlayerModel);
        yield return new WaitForSeconds(_timeDelay);

        // panic cat balou


        // wells fargo, stage coach

        // discard
        while (AgentPlayerModel.openHand.Count > AgentPlayerModel.CurrentBulletPoint)
        {
            Debug.Log($"open hand count: {AgentPlayerModel.openHand.Count} , CurrentBulletPoint: {AgentPlayerModel.CurrentBulletPoint}");
            if(OpenHandCardNames.Any(item => item.StartsWith("Panic", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Panic", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("PAN�K DISCARD ED�LD�");
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Cat Balou", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Cat Balou", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("CAT BALOU DISCARD ED�LD�");
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Wells Fargo", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Wells Fargo", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("WELLS FARGO DISCARD ED�LD�");
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Stage Coach", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Stage Coach", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("STAGE COACH DISCARD ED�LD�");
            }
            else if(AgentPlayerModel.openHand.Count > AgentPlayerModel.CurrentBulletPoint)
            {
                Debug.Log($"{OpenHandCardNames[0]} DISCARD ED�LD�");
                StartCoroutine(DiscardCardRoutine(Tuple.Create(OpenHandCardNames[0], OpenHandCardNames, AgentPlayerModel)));
            }
            yield return new WaitForSeconds(_timeDelay);
        }

        yield return new WaitForSeconds(_timeDelay);

        Debug.Log("NEXXTTTT");
        _gameManager.NextUIButton();

    }



    private void BangDecideTree(List<string> OpenHandCardNames, List<PlayerModel> CanHitPlayer, PlayerModel AgentPlayerModel)
    {
        bool containsBang = OpenHandCardNames.Any(item => item.StartsWith("Bang", StringComparison.OrdinalIgnoreCase));

        if (containsBang && CanHitPlayer.Count != 0 && (AgentPlayerModel.CanPlayMultipleBangs || !AgentPlayerModel.PlayedBang))
        {
            int index = 0;
            List<PlayerModel> HitListWithoutSheriff = new();
            switch (AgentPlayerModel.PlayerRole)
            {
                case PlayerModel.TypeOfPlayer.Sheriff:
                    // hit randomly
                    AgentPlayerModel.PlayedBang = true;
                    StartCoroutine(DiscardCardRoutine(Tuple.Create("Bang", OpenHandCardNames, AgentPlayerModel)));
                    index = Random.Range(0, CanHitPlayer.Count);
                    StartCoroutine(BangRoutine(Tuple.Create(AgentPlayerModel, (CanHitPlayer[index].gameObject))));
                    break;
                case PlayerModel.TypeOfPlayer.Deputy:
                    // hit randomly unless its not the sheriff
                    HitListWithoutSheriff = CanHitPlayer.Where(player => player.PlayerRole != PlayerModel.TypeOfPlayer.Sheriff).ToList();
                    if (HitListWithoutSheriff.Count != 0)
                    {
                        AgentPlayerModel.PlayedBang = true;
                        StartCoroutine(DiscardCardRoutine(Tuple.Create("Bang", OpenHandCardNames, AgentPlayerModel)));
                        index = Random.Range(0, HitListWithoutSheriff.Count);
                        StartCoroutine(BangRoutine(Tuple.Create(AgentPlayerModel, (HitListWithoutSheriff[index].gameObject))));
                    }
                    break;
                case PlayerModel.TypeOfPlayer.Outlaw:
                    // hit the sheriff
                    AgentPlayerModel.PlayedBang = true;
                    StartCoroutine(DiscardCardRoutine(Tuple.Create("Bang", OpenHandCardNames, AgentPlayerModel)));

                    PlayerModel Sheriff = CanHitPlayer.FirstOrDefault(player => player.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff);

                    if (Sheriff)
                    {
                        StartCoroutine(BangRoutine(Tuple.Create(AgentPlayerModel, Sheriff.gameObject)));
                    }
                    else
                    {
                        index = Random.Range(0, CanHitPlayer.Count);
                        StartCoroutine(BangRoutine(Tuple.Create(AgentPlayerModel, (CanHitPlayer[index].gameObject))));
                    }
                    break;
                case PlayerModel.TypeOfPlayer.Renegade:
                    // if there is only sheriff then bang it, but there is someone else hit it
                    AgentPlayerModel.PlayedBang = true;
                    StartCoroutine(DiscardCardRoutine(Tuple.Create("Bang", OpenHandCardNames, AgentPlayerModel)));

                    HitListWithoutSheriff = CanHitPlayer.Where(player => player.PlayerRole != PlayerModel.TypeOfPlayer.Sheriff).ToList();
                    if (HitListWithoutSheriff.Count != 0)
                    {
                        AgentPlayerModel.PlayedBang = true;
                        StartCoroutine(DiscardCardRoutine(Tuple.Create("Bang", OpenHandCardNames, AgentPlayerModel)));
                        index = Random.Range(0, HitListWithoutSheriff.Count);
                        StartCoroutine(BangRoutine(Tuple.Create(AgentPlayerModel, (HitListWithoutSheriff[index].gameObject))));
                    }
                    else
                    {
                        index = Random.Range(0, CanHitPlayer.Count);
                        StartCoroutine(BangRoutine(Tuple.Create(AgentPlayerModel, (CanHitPlayer[index].gameObject))));
                    }
                    break;
            }

        }
    }

    private bool barrelSaved = false;

    private IEnumerator BangRoutine(Tuple<PlayerModel, GameObject> tuple)
    {

        yield return new WaitUntil(() => BangForAgent(tuple.Item1, tuple.Item2));

        yield return new WaitForSeconds(_timeDelay/10);
        barrelSaved = false;
    }

    private bool BangForAgent(PlayerModel AgentPlayer, GameObject target)
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
            StartCoroutine(_actions.CheckTheNextCard(target));
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
        return true;
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

    private IEnumerator DiscardCardRoutine(Tuple<string, List<string>, PlayerModel> tuple)
    {
        yield return new WaitUntil(() => DiscardCard(tuple.Item1, tuple.Item2, tuple.Item3));
        Debug.Log("DISCARD ENDEDD");
    }

    private bool DiscardCard(string CardName, List<string> OpenHandCardNames, PlayerModel AgentPlayerModel)
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
            RemoveCardFromAgentServer(AgentPlayerModel,index);
            OpenHandCardNames.RemoveAt(index);
        }

        return true;
    }

    [ServerRpc (RequireOwnership = false)]
    private void RemoveCardFromAgentServer(PlayerModel AgentPlayerModel, int index)
    {
        RemoveCardFromAgent(AgentPlayerModel, index);
    }

    [ObserversRpc]
    private void RemoveCardFromAgent(PlayerModel AgentPlayerModel, int index)
    {
        AgentPlayerModel.openHand.RemoveAt(index);
    }


}
