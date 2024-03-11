using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System;

public class Actions : NetworkBehaviour
{
    // Card Actions
    void BangAction(GameObject target) // Bang
    {
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();

        if (CalculateScope(target) >= 0)
        {
            targetPlayer.CurrentBulletPoint--; 
            
            foreach (CardModel card in targetPlayer.openHand)
            {
                if (card.Name == "Missed") // Karavana
                {
                    MissedAction(targetPlayer);
                    return;
                }
            }
        }
    }
    void MissedAction(PlayerModel player) // Karavana
    {
        player.CurrentBulletPoint++;
    }
    void BeerAction() // Bitki �ay�
    {
        GetComponent<PlayerModel>().CurrentBulletPoint++;
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

        foreach (CardModel card in thisPlayer.openHand)
        {
            if (card.Name == "Appaloosa") // D�rb�n
            {
                dist--;
            }
        }

        foreach (CardModel card in targetPlayer.openHand)
        {
            if (card.Name == "Mustang") // Mustang
            {
                dist++;
            }
        }

        return dist;
    }
    public int CalculateScope(GameObject target)
    {
        PlayerModel thisPlayer = GetComponent<PlayerModel>();

        return Mathf.Abs(thisPlayer.gun.ScopeLevel - CalculateDistance(target)); // if >=0 can if <0 cannot
    }
    void PullCard()
    {

    }
    void DiscardCard()
    {

    }
    public void ChangeWeapon(GunModel gun)
    {
        PlayerModel thisPlayer = GetComponent<PlayerModel>();

        thisPlayer.gun = gun;
    }
    public bool CheckBullet(GameObject player)
    {
        if(player.GetComponent<PlayerModel>().CurrentBulletPoint > 0)
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
    // multiple bangs
    // end of the game (player roles)


    // in-game states (pull cards -> action -> discard cards)
    // game states (start -> play -> end)

    // actions on the cards...
}