using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using FishNet;
using Gunslinger.Controller;
using Random = UnityEngine.Random;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using UnityEngine.UI;

public class AgentController : NetworkBehaviour
{

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Actions _actions;
    [SerializeField] private CardsController cardsController;
    [SerializeField] private AlertTextManager AlertTexts;

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
        while (OpenHandCardNames.Any(item => item.StartsWith("Mustang", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Mustang", OpenHandCardNames, AgentPlayerModel)));
        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Scope", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Scope", OpenHandCardNames, AgentPlayerModel)));

        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Barrel", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Barrel", OpenHandCardNames, AgentPlayerModel)));

        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Volcanic", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Volcanic", OpenHandCardNames, AgentPlayerModel)));

        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Remington", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Remington", OpenHandCardNames, AgentPlayerModel)));
        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Rev. Carabine", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Rev. Carabine", OpenHandCardNames, AgentPlayerModel)));
        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Schofield", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Schofield", OpenHandCardNames, AgentPlayerModel)));
        }
        while (OpenHandCardNames.Any(item => item.StartsWith("Winchester", StringComparison.OrdinalIgnoreCase)))
        {
            StartCoroutine(MoveRoutine(Tuple.Create("Winchester", OpenHandCardNames, AgentPlayerModel)));
        }

        // BANG
        BangDecideTree(OpenHandCardNames, CanHitPlayer, AgentPlayerModel);
        yield return new WaitForSeconds(_timeDelay);

        // HEALTH
        while (AgentPlayerModel.CurrentBulletPoint < MaxBulletPoint)
        {
            if (OpenHandCardNames.Any(item => item.StartsWith("Beer", StringComparison.OrdinalIgnoreCase)))
            {
                // play beer
                _actions.BeerAction(AgentPlayerModel);
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Beer", OpenHandCardNames, AgentPlayerModel)));
                yield return new WaitForSeconds(_timeDelay);
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Saloon", StringComparison.OrdinalIgnoreCase)))
            {
                // play saloon
                Debug.Log("HERKESE BENDEN ÇAY");
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
                Debug.Log("PANÝK DISCARD EDÝLDÝ");
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Cat Balou", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Cat Balou", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("CAT BALOU DISCARD EDÝLDÝ");
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Wells Fargo", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Wells Fargo", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("WELLS FARGO DISCARD EDÝLDÝ");
            }
            else if (OpenHandCardNames.Any(item => item.StartsWith("Stage Coach", StringComparison.OrdinalIgnoreCase)))
            {
                StartCoroutine(DiscardCardRoutine(Tuple.Create("Stage Coach", OpenHandCardNames, AgentPlayerModel)));
                Debug.Log("STAGE COACH DISCARD EDÝLDÝ");
            }
            else if(AgentPlayerModel.openHand.Count > AgentPlayerModel.CurrentBulletPoint)
            {
                Debug.Log($"{OpenHandCardNames[0]} DISCARD EDÝLDÝ");
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

        _gameManager.ChangeAlertServer(AlertTexts.GetBangText(AgentPlayer.PlayerName, targetPlayer.PlayerName));

        PlayerAnimationController targetAnimationController = target.GetComponent<PlayerAnimationController>();
        PlayerAnimationController playerAnimationController = AgentPlayer.gameObject.GetComponent<PlayerAnimationController>();
        playerAnimationController.playFire();
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

        var pointer = target.GetComponent<CardManager>().CardOrder[cardsController.CardPointer];
        var child = cardsController.GetChildOfDeck(pointer, GameObject.Find("DeckPanel"));

        if (hasBarrel && child.transform.Find("Symbol").GetComponent<Image>().sprite.name == "hearts")
        {
            _gameManager.ChangeAlertServer(AlertTexts.GetBarrelText(targetPlayer.PlayerName));
        }

        if ((hasBarrel && child.transform.Find("Symbol").GetComponent<Image>().sprite.name != "hearts") || !hasBarrel)
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
                    _gameManager.ChangeAlertServer(AlertTexts.GetMissedText(targetPlayer.PlayerName));
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
                            _gameManager.ChangeAlertServer(AlertTexts.GetBeerText(targetPlayer.PlayerName));
                            //MissedAction(targetPlayer);
                            break;
                        }
                    }
                }
                bool dodged = true;
                if (!hasBeer)
                {
                    if (targetPlayer.CurrentBulletPoint == 1)
                    {
                        targetAnimationController.playDeath();
                    }
                    else
                    {
                        targetAnimationController.playInjure();

                    }
                    dodged = false;
                    cardsController.UpdateHealthServer(targetPlayer, -1);
                }
                if (dodged)
                {
                    targetAnimationController.playDodge();
                }
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
    private IEnumerator MoveRoutine(Tuple<string, List<string>, PlayerModel> tuple)
    {
        yield return new WaitUntil(() => MoveToStackHand(tuple.Item1, tuple.Item2, tuple.Item3));
        Debug.Log("MOVE ENDEDD");
    }

    private bool MoveToStackHand(string CardName, List<string> OpenHandCardNames, PlayerModel AgentPlayerModel)
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
            MoveFromAgentServer(AgentPlayerModel, index);
            OpenHandCardNames.RemoveAt(index);
        }

        return true;
    }
    [ServerRpc(RequireOwnership = false)]
    private void MoveFromAgentServer(PlayerModel AgentPlayerModel, int index)
    {
        MoveFromAgent(AgentPlayerModel, index);
    }

    [ObserversRpc]
    private void MoveFromAgent(PlayerModel AgentPlayerModel, int index)
    {
        var c = AgentPlayerModel.openHand[index];
        AgentPlayerModel.openHand.RemoveAt(index);
        AgentPlayerModel.stackHand.Add(c);



        var stack = GameObject.Find("PlayerInfoStack").transform; // fatih
        foreach (Transform info in stack)
        {
            if (info.gameObject.name == AgentPlayerModel.PlayerName)
            {
                Debug.Log("move to stack þey iþte " + c.name);
                if (c.name.StartsWith("Mustang"))
                {
                    var m = info.Find("Mustang").gameObject;
                    m.SetActive(true);
                }
                else if (c.name.StartsWith("Scope"))
                {
                    info.Find("Scope").gameObject.SetActive(true);
                }
                else if (c.name.StartsWith("Barrel"))
                {
                    info.Find("Barrel").gameObject.SetActive(true);
                }




                if (c.name.StartsWith("Volcanic"))
                {
                    info.Find("Volcanic").gameObject.SetActive(true);
                    info.Find("Remington").gameObject.SetActive(false);
                    info.Find("Rev. Carabine").gameObject.SetActive(false);
                    info.Find("Schofield").gameObject.SetActive(false);
                    info.Find("Winchester").gameObject.SetActive(false);
                }
                else if (c.name.StartsWith("Remington"))
                {
                    info.Find("Volcanic").gameObject.SetActive(false);
                    info.Find("Remington").gameObject.SetActive(true);
                    info.Find("Rev. Carabine").gameObject.SetActive(false);
                    info.Find("Schofield").gameObject.SetActive(false);
                    info.Find("Winchester").gameObject.SetActive(false);
                }
                else if (c.name.StartsWith("Rev. Carabine"))
                {
                    info.Find("Volcanic").gameObject.SetActive(false);
                    info.Find("Remington").gameObject.SetActive(false);
                    info.Find("Rev. Carabine").gameObject.SetActive(true);
                    info.Find("Schofield").gameObject.SetActive(false);
                    info.Find("Winchester").gameObject.SetActive(false);
                }
                else if (c.name.StartsWith("Schofield"))
                {
                    info.Find("Volcanic").gameObject.SetActive(false);
                    info.Find("Remington").gameObject.SetActive(false);
                    info.Find("Rev. Carabine").gameObject.SetActive(false);
                    info.Find("Schofield").gameObject.SetActive(true);
                    info.Find("Winchester").gameObject.SetActive(false);
                }
                else if (c.name.StartsWith("Winchester"))
                {
                    info.Find("Volcanic").gameObject.SetActive(false);
                    info.Find("Remington").gameObject.SetActive(false);
                    info.Find("Rev. Carabine").gameObject.SetActive(false);
                    info.Find("Schofield").gameObject.SetActive(false);
                    info.Find("Winchester").gameObject.SetActive(true);
                }



            }
        }
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
