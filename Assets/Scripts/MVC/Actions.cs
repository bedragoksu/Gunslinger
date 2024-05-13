using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunslinger.Controller;
using NeptunDigital;

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
                    DiscardCard(player, playedCard);
                    return;
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
    void BeerAction() // Bitki �ay�
    {
        GetComponent<PlayerModel>().CurrentBulletPoint++;
        if (GetComponent<PlayerModel>().CurrentBulletPoint > GetComponent<CharacterModel>().MaxBulletPoint)
        {
            GetComponent<PlayerModel>().CurrentBulletPoint = GetComponent<CharacterModel>().MaxBulletPoint;
        }
    }
    void DrawAction() // F���
    {

    }
    void PanicAction() // Panik
    {

    }
    void SaloonAction() // Kahvehane
    {

    }
    void GatlingAction() // Makineli t�fek
    {

    }

    // General Functions
    public int CalculateDistance(GameObject target)
    {
        PlayerModel thisPlayer = GetComponent<PlayerModel>();
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();

        int dist = Mathf.Abs(targetPlayer.position - thisPlayer.position);

        foreach (var card in thisPlayer.openHand)
        {
            if (card.name.StartsWith("Appaloosa")) // D�rb�n
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
        cardsController.DiscardCardsServer(player, i);
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