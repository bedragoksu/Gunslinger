using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunslinger.Controller;
using NeptunDigital;
using System;

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
            DiscardCard(player, playedCard);
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
    void DrawAction() // Fýçý
    {

    }
    void PanicAction() // Panik
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
    void GatlingAction(GameObject player, int playedCard) // Makineli tüfek
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(var pl in players)
        {
            if(pl != player)
            {
                BangAction(player,pl,playedCard);
            }
        }
    }

    // General Functions
    public int CalculateDistance(GameObject target)
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
    public void PullCard(GameObject card)
    {

    }
    public void DiscardCard(GameObject player, int i)
    {
        ScreenLog.Instance.SendEvent(TextType.Debug, "discard card");
        StartCoroutine("DiscardCardRoutine", Tuple.Create(player, i));
        
    }

    IEnumerator DiscardCardRoutine(Tuple<GameObject, int> tuple)
    {
        var player = tuple.Item1;
        var i = tuple.Item2;
        cardsController.DiscardCardsServer(player, i);
        yield return new WaitUntil(() => cardsController.discard);
        yield return new WaitForSeconds(1f);
        player.GetComponent<PlayerModel>().cardchange(false);
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