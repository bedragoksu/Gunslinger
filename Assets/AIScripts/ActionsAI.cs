using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunslinger.Controller;
using NeptunDigital;
using System;
using UnityEngine.UI;
using System.Xml.Linq;

public class ActionsAI : MonoBehaviour
{
    public CardsControllerAI CardsControllerAI;

    // Card Actions

    bool barrelSaved = false;
    public void BangAction(GameObject player, GameObject target) // Bang
    {
        PlayerModelAI targetPlayer = target.GetComponent<PlayerModelAI>();
        Debug.Log($"target of bang: {targetPlayer.PlayerName}");

        bool hasBarrel = false;
        foreach(var stack in targetPlayer.stackHand)
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
            for (int i=0; i< hand.Count; i++)
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
            if(!hasMissed)
            {
                var hasBeer = false;
                if(targetPlayer.CurrentBulletPoint == 1)
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
                if (!hasBeer) { CardsControllerAI.UpdateHealthServer(targetPlayer, -1); }
            }
            
        }
    }



    private IEnumerator CheckTheNextCard(GameObject player)
    {
        // next card to (TheNextCard) panel
        var b = CardsControllerAI.CheckNext(player);

        // wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // if it has heart then barrelSaved = true otherwise false
        barrelSaved = b;

        // next card to deck
        CardsControllerAI.CloseTheNextCardServer();

    }

    public void AddingGunToPlayer(GameObject player, int rangeAmount, GameObject theCard, bool multipleBangs, string cardName)
    {
        CardsControllerAI.AddGunServer(player, rangeAmount, theCard, multipleBangs, cardName);
    }

    void MissedAction(PlayerModelAI player) // Karavana // also discard card 
    {
        CardsControllerAI.UpdateHealthServer(player, 1);
        Debug.Log("target missed");
    }
    public void BeerAction(PlayerModelAI player, int playedCard) // Bitki çayý
    {
        var maxbullet = (player.PlayerRole == PlayerModelAI.TypeOfPlayer.Sheriff)? 5 : 4;
        if (player.CurrentBulletPoint < maxbullet)
        {
            CardsControllerAI.UpdateHealthServer(player, 1);
        }
        DiscardCard(player.gameObject, playedCard);
    }

    public void WellsFargoAction(PlayerModelAI player, int playedCard)
    {
        var end = GameObject.Find("CardsControllerAI").GetComponent<CardsControllerAI>().DrawCards(player.gameObject, 3);
        DiscardCard(player.gameObject, playedCard);
    }
    public void StagecoachAction(PlayerModelAI player, int playedCard)
    {
        var end = GameObject.Find("CardsControllerAI").GetComponent<CardsControllerAI>().DrawCards(player.gameObject, 2);
        DiscardCard(player.gameObject, playedCard);
    }

    public void CatBalouAction( GameObject target) // Emrivaki
    {
        // target'in elinden random bir karti iskartaya cikar
        var targetmodel = target.GetComponent<PlayerModelAI>();
        var i = UnityEngine.Random.Range(0, targetmodel.openHand.Count);
        DiscardCard(target, i);
    }
    
    
    public bool PanicAction(GameObject player,int playedCard ,GameObject target) // Panik
    {
        var targetmodel = target.GetComponent<PlayerModelAI>();
        var i = UnityEngine.Random.Range(0, targetmodel.openHand.Count);
        var card = targetmodel.openHand[i];
        DiscardCard(target, i); // target'in elinden random bir kart silindi.
        DiscardCard(player, playedCard); // panic karti silindi.

        // deck'in altindaki o card draw the card icine atilmali
        var t = GameObject.Find("DeckPanel").transform;

        int indexOfDeck = 0;
        for(int j=0; j<69; j++) // 69 = cardNum bedra
        {
            if(t.GetChild(j).name == card.name)
            {
                indexOfDeck = j;
                break;
            }
        }
        CardsControllerAI.DrawTheCardServer(player, indexOfDeck);
        //var handpanel = GameObject.Find("HandPanel").transform;
        //t.GetChild(indexOfDeck).transform.SetParent(handpanel);
        
        return true;
    }

    


    void DrawAction() // Fýçý
    {

    }
    public void SaloonAction(GameObject player, int playedCard) // Kahvehane
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var pl in players)
        {
            CardsControllerAI.UpdateHealthServer(pl.GetComponent<PlayerModelAI>(), 1);
        }
        DiscardCard(player, playedCard);
    }
    public void GatlingAction(GameObject player, int playedCard) // Makineli tüfek
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(var pl in players)
        {
            if(pl != player)
            {
                BangAction(player,pl); // bedra playedInt?
            }
        }
        DiscardCard(player, playedCard);
    }


    public void MustangAction(GameObject player, int playedCard) // Makineli tüfek
    {
        MoveToStackHand(player, playedCard);
    }

    // General Functions
    public int CalculateDistance(GameObject thisObj, GameObject target) // bedra
    {
        PlayerModelAI thisPlayer = thisObj.GetComponent<PlayerModelAI>();
        PlayerModelAI targetPlayer = target.GetComponent<PlayerModelAI>();

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

        PlayerModelAI plModel = player.GetComponent<PlayerModelAI>();
        PlayerModelAI targetModel = target.GetComponent<PlayerModelAI>();

        var playerList = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> aliveList = new List<GameObject>();


        foreach(var pl in playerList)
        {
            if (pl.GetComponent<PlayerModelAI>().IsAlive) aliveList.Add(pl);
        }

        for(int i=0; i< aliveList.Count;i++)
        {
            var model = playerList[i].GetComponent<PlayerModelAI>();
            if (model.PlayerName == plModel.PlayerName)
            {
                indexA = i;
            }else if (model.PlayerName == targetModel.PlayerName)
            {
                indexB = i;
            }
        }

        int directDistance = Mathf.Abs(indexA - indexB);
        int circularDistance = aliveList.Count - directDistance;

        dist = Mathf.Min(directDistance, circularDistance);
        return dist;
    }
    public bool CalculateScopeCanHit(GameObject thisPlayer,GameObject target)
    {
        int scopeLevel = thisPlayer.GetComponent<PlayerModelAI>().Range; // player'in infosundan cek
        int d = scopeLevel - CalculateDistance(thisPlayer, target);

        return (d>=0); // if >=0 can if <0 cannot
    }
    public void MoveToStackHand(GameObject player, int i)
    {
        StartCoroutine("MoveToStackRoutine", Tuple.Create(player, i));

        GameObject.Find("GameManager").GetComponent<GameManager>()._thisPlayer.GetComponent<PlayerModelAI>().clicked = false; // yuh bedra
        GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();
    }
    IEnumerator MoveToStackRoutine(Tuple<GameObject, int> tuple)
    {
        var player = tuple.Item1;
        var i = tuple.Item2;
        CardsControllerAI.MoveToStackServer(player, i);
        //var c = GameObject.Find("HandPanel").transform.GetChild(i);
        //c.SetParent(GameObject.Find("DeckPanel").transform);
        yield return new WaitUntil(() => CardsControllerAI.move);
        yield return new WaitForSeconds(1f);
        //player.GetComponent<PlayerModelAI>().cardchange(true);
        CardsControllerAI.move = false;
    }

    public void DiscardCard(GameObject player, int i)
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "discard card");
        StartCoroutine("DiscardCardRoutine", Tuple.Create(player, i));


        GameObject.Find("GameManager").GetComponent<GameManager>()._thisPlayer.GetComponent<PlayerModelAI>().clicked = false; // yuh bedra
        GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();

    }

    IEnumerator DiscardCardRoutine(Tuple<GameObject, int> tuple)
    {
        var player = tuple.Item1;
        var i = tuple.Item2;
        CardsControllerAI.DiscardCardsServer(player, i);
        //var c = GameObject.Find("HandPanel").transform.GetChild(i);
        //c.SetParent(GameObject.Find("DeckPanel").transform);
        yield return new WaitUntil(() => CardsControllerAI.discard);
        yield return new WaitForSeconds(1f);
        //player.GetComponent<PlayerModelAI>().cardchange(true);
        CardsControllerAI.discard = false;
    }
    public void ChangeWeapon(GunModel gun)
    {
        PlayerModelAI thisPlayer = GetComponent<PlayerModelAI>();

        thisPlayer.gun = gun;
    }
    public bool CheckBullet(GameObject player)
    {
        if (player.GetComponent<PlayerModelAI>().CurrentBulletPoint > 0)
        {
            return true;
        }
        return false;
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