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
    public void BangAction(GameObject player, GameObject target, int playedCard) // Bang
    {
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();
        Debug.Log($"target of bang: {targetPlayer.PlayerName}");

        if (/*CalculateScope(target) >= 0*/ true) // calculate distance instead of 0 // before calling bang action
        {
            cardsController.UpdateHealthServer(targetPlayer, -1);
            var hand = targetPlayer.openHand;
            for (int i=0; i< hand.Count; i++)
            {
                if (hand[i].name.StartsWith("Missed"))
                {
                    DiscardCard(target, i);
                    MissedAction(targetPlayer);
                    break;
                }
            }
            
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

    public void CatBalouAction(GameObject player, int playedCard, GameObject target) // Emrivaki
    {
        // target'in elinden random bir karti iskartaya cikar
        var targetmodel = target.GetComponent<PlayerModel>();
        var i = UnityEngine.Random.Range(0, targetmodel.openHand.Count);
        DiscardCard(target, i);
    }
    
    
    public void PanicAction(GameObject player, int playedCard) // Panik
    {

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
                BangAction(player,pl,playedCard);
            }
        }
        DiscardCard(player, playedCard);
    }


    public void MustangAction(GameObject player, int playedCard) // Makineli tüfek
    {
        MoveToStackHand(player, playedCard);
    }

    // General Functions
    public int CalculateDistance(GameObject target) // bedra
    {
        PlayerModel thisPlayer = GetComponent<PlayerModel>();
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();

        int dist = Mathf.Abs(targetPlayer.position - thisPlayer.position);

        foreach (var card in thisPlayer.openHand)
        {
            if (card.name.StartsWith("Appaloosa")) // Dürbün
            {
                dist--;
            }
        }

        foreach (var card in targetPlayer.openHand)
        {
            if (card.name.StartsWith("Mustang")) // Mustang
            {
                dist++;
            }
        }

        return dist;
    }
    public int CalculateScope(GameObject target)
    {
        PlayerModel thisPlayer = GetComponent<PlayerModel>();

        return (thisPlayer.gun.ScopeLevel - CalculateDistance(target)); // if >=0 can if <0 cannot
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