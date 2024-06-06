using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunslinger.Controller;
//using NeptunDigital;
using System;
using UnityEngine.UI;
using System.Xml.Linq;
using FishNet.Object;

public class Actions : MonoBehaviour
{
    public CardsController cardsController;
    public GameManager gameManager;

    // Card Actions

    bool barrelSaved = false;
    public void BangAction(GameObject player, GameObject target) // Bang
    {
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();
        Debug.Log($"target of bang: {targetPlayer.PlayerName}");
        PlayerAnimationController targetAnimationController = target.GetComponent<PlayerAnimationController>();
        PlayerAnimationController playerAnimationController = player.GetComponent<PlayerAnimationController>();

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
            StartCoroutine("CheckTheNextCard", target);
        }

        var pointer = player.GetComponent<CardManager>().CardOrder[cardsController.CardPointer];
        var child = cardsController.GetChildOfDeck(pointer, GameObject.Find("DeckPanel"));

        if (child.transform.Find("Symbol").GetComponent<Image>().sprite.name != "hearts")
        {
            bool hasMissed = false;
            var hand = targetPlayer.openHand;
            for (int i = 0; i < hand.Count; i++)
            {
                var name = hand[i].name;
                if (name.StartsWith("Missed"))
                {
                    DiscardCard(target, i);
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
                            DiscardCard(target, i);
                            hasBeer = true;
                            //MissedAction(targetPlayer);
                            break;
                        }
                    }
                }
                bool dodged = true;
                if (!hasBeer) { 
                    if (targetPlayer.CurrentBulletPoint == 1)
                    {
                        
                        targetAnimationController.playAnimDeathServer();
                    } else
                    {
                        targetAnimationController.playAnimInjureServer();
                            
                    }
                    dodged = false;
                    cardsController.UpdateHealthServer(targetPlayer, -1); 
                }
                if (dodged)
                {
                    Debug.Log("Target dogded");
                    targetAnimationController.playAnimDodgeServer();
                }
            } else
            {
                targetAnimationController.playAnimDodgeServer();
            }

        } else { targetAnimationController.playAnimDodgeServer(); }
    }

    public List<PlayerModel> CanHitAnyone(GameObject player)
    {
        var plList = gameManager._turns;
        List<PlayerModel> CanHitList = new List<PlayerModel>();

        foreach (var pl in plList)
        {
            if (pl != player && pl.GetComponent<PlayerModel>().IsAlive)
            {
                if (CalculateScopeCanHit(player, pl))
                {
                    CanHitList.Add(pl.GetComponent<PlayerModel>());
                }
            }
        }

        return CanHitList;
    }

    public IEnumerator CheckTheNextCard(GameObject player)
    {
        // next card to (TheNextCard) panel
        var b = cardsController.CheckNext(player);
        barrelSaved = b;

        // wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // if it has heart then barrelSaved = true otherwise false
        

        // next card to deck
        cardsController.CloseTheNextCardServer();

    }

    public void AddingGunToPlayer(GameObject player, int rangeAmount, GameObject theCard, bool multipleBangs, string cardName)
    {
        cardsController.AddGunServer(player, rangeAmount, theCard, multipleBangs, cardName);
        //GameObject cardfrompanel = null;
        //GameObject handpanel = GameObject.Find("HandPanel");
        //for (int i = 0; i < handpanel.transform.childCount; i++)
        //{
        //    if (handpanel.transform.GetChild(i).gameObject.name.StartsWith(cardName))
        //    {
        //        cardfrompanel = handpanel.transform.GetChild(i).gameObject;
        //        break;
        //    }
        //}

        //if (cardfrompanel)
        //{
        //    cardfrompanel.transform.SetParent(GameObject.Find("StackHandPanel").transform);
        //    cardfrompanel.SetActive(false);
        //}


    }

    public void BeerAction(PlayerModel player) // Bitki çayý // bedra 2 kere ayni sey
    {
        var maxbullet = (player.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff) ? 5 : 4;
        if (player.CurrentBulletPoint < maxbullet)
        {
            cardsController.UpdateHealthServer(player, 1);
        }
    }

    public void WellsFargoAction(PlayerModel player, int playedCard) // bedra
    {
        cardsController.DrawCardsServer(player.gameObject, 3);
        DiscardCard(player.gameObject, playedCard);
    }
    public void StagecoachAction(PlayerModel player, int playedCard)
    {
        cardsController.DrawCardsServer(player.gameObject, 2);
        DiscardCard(player.gameObject, playedCard);
    }

    public void CatBalouAction(GameObject target) // Emrivaki
    {
        // target'in elinden random bir karti iskartaya cikar
        var targetmodel = target.GetComponent<PlayerModel>();
        var i = UnityEngine.Random.Range(0, targetmodel.openHand.Count);
        DiscardCard(target, i);
    }


    public bool PanicAction(GameObject player, int playedCard, GameObject target) // Panik
    {
        var targetmodel = target.GetComponent<PlayerModel>();
        var i = UnityEngine.Random.Range(0, targetmodel.openHand.Count);
        var card = targetmodel.openHand[i];
        DiscardCard(target, i); // target'in elinden random bir kart silindi.
        DiscardCard(player, playedCard); // panic karti silindi.

        // deck'in altindaki o card draw the card icine atilmali
        var t = GameObject.Find("DeckPanel").transform;

        int indexOfDeck = 0;
        for (int j = 0; j < 69; j++) // 69 = cardNum bedra
        {
            if (t.GetChild(j).name == card.name)
            {
                indexOfDeck = j;
                break;
            }
        }
        cardsController.DrawTheCardServer(player, indexOfDeck);
        var handpanel = GameObject.Find("HandPanel").transform;
        t.GetChild(indexOfDeck).transform.SetParent(handpanel);


        gameManager.PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();

        return true;
    }


    public void SaloonAction() // Kahvehane
    {
        var players = gameManager._turns;
        foreach (var pl in players)
        {
            if (pl.GetComponent<PlayerModel>().IsAlive)
            {
                cardsController.UpdateHealthServer(pl.GetComponent<PlayerModel>(), 1);
            }
        }
        
    }
    public void GatlingAction(GameObject player) // Makineli tüfek
    {
        var players = gameManager._turns;

        foreach (var pl in players)
        {
            if (pl != player)
            {
                BangAction(player, pl);
            }
        }
        
    }


    public void MustangAction(GameObject player, int playedCard) // Makineli tüfek
    {
        MoveToStackHand(player, playedCard);
    }

    // General Functions
    public int CalculateDistance(GameObject thisObj, GameObject target) // bedra
    {
        PlayerModel thisPlayer = thisObj.GetComponent<PlayerModel>();
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();

        int dist = GetCircularDistance(thisObj, target);



        foreach (var card in thisPlayer.stackHand)
        {
            if (card.name.StartsWith("Scope")) // Dürbün
            {
                dist--;
            }
        }

        foreach (var card in targetPlayer.stackHand)
        {
            if (card.name.StartsWith("Mustang")) // Mustang
            {
                dist++;
            }
        }

        return dist;
    }

    public int GetCircularDistance(GameObject player, GameObject target)
    {
        int dist = 0;
        int indexA = 0;
        int indexB = 0;

        PlayerModel plModel = player.GetComponent<PlayerModel>();
        PlayerModel targetModel = target.GetComponent<PlayerModel>();

        List<PlayerModel> aliveList = cardsController.GetAlivePlayers();


        for (int i = 0; i < aliveList.Count; i++)
        {
            var model = aliveList[i];
            if (model.PlayerName == plModel.PlayerName)
            {
                indexA = i;
            }
            else if (model.PlayerName == targetModel.PlayerName)
            {
                indexB = i;
            }
        }

        int directDistance = Mathf.Abs(indexA - indexB);
        int circularDistance = aliveList.Count - directDistance;

        dist = Mathf.Min(directDistance, circularDistance);
        return dist;
    }
    public bool CalculateScopeCanHit(GameObject thisPlayer, GameObject target)
    {
        int scopeLevel = thisPlayer.GetComponent<PlayerModel>().Range; // player'in infosundan cek
        int d = scopeLevel - CalculateDistance(thisPlayer, target);

        return (d >= 0); // if >=0 can if <0 cannot
    }


    public void MoveToStackHand(GameObject player, int i)
    {
        StartCoroutine("MoveToStackRoutine", Tuple.Create(player, i));

        gameManager._thisPlayer.GetComponent<PlayerModel>().clicked = false;
        gameManager.PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();
    }
    IEnumerator MoveToStackRoutine(Tuple<GameObject, int> tuple)
    {
        var player = tuple.Item1;
        var i = tuple.Item2;
        cardsController.MoveToStackServer(player, i);
        //var c = GameObject.Find("HandPanel").transform.GetChild(i);
        //c.SetParent(GameObject.Find("DeckPanel").transform);
        yield return new WaitUntil(() => cardsController.move);
        yield return new WaitForSeconds(1f);
        //player.GetComponent<PlayerModel>().cardchange(true);
        cardsController.move = false;
    }

    public void DiscardCard(GameObject player, int i)
    {
        //ScreenLog.Instance.SendEvent(TextType.Debug, "discard card");
        StartCoroutine("DiscardCardRoutine", Tuple.Create(player, i));


        gameManager._thisPlayer.GetComponent<PlayerModel>().clicked = false;
        gameManager.PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();

    }

    IEnumerator DiscardCardRoutine(Tuple<GameObject, int> tuple)
    {
        var player = tuple.Item1;
        var i = tuple.Item2;
        cardsController.DiscardCardsServer(player, i);
        //var c = GameObject.Find("HandPanel").transform.GetChild(i);
        //c.SetParent(GameObject.Find("DeckPanel").transform);
        yield return new WaitUntil(() => cardsController.discard);
        yield return new WaitForSeconds(1f);
        //player.GetComponent<PlayerModel>().cardchange(true);
        cardsController.discard = false;
    }


    // increase/decrease bullet points
    // pull card
    // miss (karavana)
    // discard card(s)
    // Bang!


    // calculate distance (mustang + scope)
    // end of the game (player roles)

    // actions on the cards...
}