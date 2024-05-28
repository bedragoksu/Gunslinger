using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunslinger.Controller;
using NeptunDigital;
using System;
using UnityEngine.UI;
using System.Xml.Linq;

public class Actions : MonoBehaviour
{
    public CardsController cardsController;

    // Card Actions
    public void BangAction(GameObject player, GameObject target) // Bang
    {
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();
        Debug.Log($"target of bang: {targetPlayer.PlayerName}");

        if (/*CalculateScope(target) >= 0*/ true) // calculate distance instead of 0 // before calling bang action
        {
            bool hasMissed = false;
            var hand = targetPlayer.openHand;
            for (int i=0; i< hand.Count; i++)
            {
                if (hand[i].name.StartsWith("Missed"))
                {
                    DiscardCard(target, i);
                    hasMissed = true;
                    //MissedAction(targetPlayer);
                    break;
                }
            }
            
            if(!hasMissed) cardsController.UpdateHealthServer(targetPlayer, -1);
            //foreach (var card in targetPlayer.openHand)
            //{
            //    if (card.name.StartsWith("Missed")) // Karavana
            //    {
            //        DiscardCard(target, card);
            //        MissedAction(targetPlayer);
            //        return;
            //    }
            //}
        }
    }
    void MissedAction(PlayerModel player) // Karavana // also discard card 
    {
        cardsController.UpdateHealthServer(player, 1);
        Debug.Log("target missed");
    }
    public void BeerAction(PlayerModel player, int playedCard) // Bitki çayý
    {
        var maxbullet = (player.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff)? 5 : 4;
        if (GetComponent<PlayerModel>().CurrentBulletPoint < maxbullet)
        {
            cardsController.UpdateHealthServer(player, 1);
        }
        DiscardCard(player.gameObject, playedCard);
    }

    public void WellsFargoAction(PlayerModel player, int playedCard)
    {
        var end = GameObject.Find("CardsController").GetComponent<CardsController>().DrawCards(player.gameObject, 3);
        DiscardCard(player.gameObject, playedCard);
    }
    public void StagecoachAction(PlayerModel player, int playedCard)
    {
        var end = GameObject.Find("CardsController").GetComponent<CardsController>().DrawCards(player.gameObject, 2);
        DiscardCard(player.gameObject, playedCard);
    }

    public void CatBalouAction( GameObject target) // Emrivaki
    {
        // target'in elinden random bir karti iskartaya cikar
        var targetmodel = target.GetComponent<PlayerModel>();
        var i = UnityEngine.Random.Range(0, targetmodel.openHand.Count);
        DiscardCard(target, i);
    }
    
    
    public bool PanicAction(GameObject player,int playedCard ,GameObject target) // Panik
    {
        var targetmodel = target.GetComponent<PlayerModel>();
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
        cardsController.DrawTheCardServer(player, indexOfDeck);
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
            cardsController.UpdateHealthServer(pl.GetComponent<PlayerModel>(), 1);
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

        var playerList = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> aliveList = new List<GameObject>();


        foreach(var pl in playerList)
        {
            if (pl.GetComponent<PlayerModel>().IsAlive) aliveList.Add(pl);
        }

        for(int i=0; i< aliveList.Count;i++)
        {
            var model = playerList[i].GetComponent<PlayerModel>();
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
        int scopeLevel = 1; // player'in infosundan cek
        int d = scopeLevel - CalculateDistance(thisPlayer, target);

        return (d>=0); // if >=0 can if <0 cannot
    }
    public void MoveToStackHand(GameObject player, int i)
    {
        StartCoroutine("MoveToStackRoutine", Tuple.Create(player, i));

        GameObject.Find("GameManager").GetComponent<GameManager>()._thisPlayer.GetComponent<PlayerModel>().clicked = false; // yuh bedra
        GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();
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
        ScreenLog.Instance.SendEvent(TextType.Debug, "discard card");
        StartCoroutine("DiscardCardRoutine", Tuple.Create(player, i));


        GameObject.Find("GameManager").GetComponent<GameManager>()._thisPlayer.GetComponent<PlayerModel>().clicked = false; // yuh bedra
        GameObject.Find("GameManager").GetComponent<GameManager>().PlayerInfoStack.GetComponent<PlayerInfoControllerUI>().UpdateStackCanvas();

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
    public void ChangeWeapon(GunModel gun)
    {
        PlayerModel thisPlayer = GetComponent<PlayerModel>();

        thisPlayer.gun = gun;
    }
    public bool CheckBullet(GameObject player)
    {
        if (player.GetComponent<PlayerModel>().CurrentBulletPoint > 0)
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