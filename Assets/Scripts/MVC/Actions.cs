using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // Card Actions
    public void BangAction(GameObject target) // Bang
    {
        PlayerModel targetPlayer = target.GetComponent<PlayerModel>();

        if (CalculateScope(target) >= 0) // calculate distance instead of 0 // before calling bang action
        {
            targetPlayer.CurrentBulletPoint--;

            foreach (var card in targetPlayer.openHand)
            {
                if (card.name.StartsWith("Missed")) // Karavana
                {
                    DiscardCard(card);
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
    void BeerAction() // Bitki çayý
    {
        GetComponent<PlayerModel>().CurrentBulletPoint++;
        if (GetComponent<PlayerModel>().CurrentBulletPoint > GetComponent<CharacterModel>().MaxBulletPoint)
        {
            GetComponent<PlayerModel>().CurrentBulletPoint = GetComponent<CharacterModel>().MaxBulletPoint;
        }
    }
    void DrawAction() // Fýçý
    {

    }
    void PanicAction() // Panik
    {

    }
    void SaloonAction() // Kahvehane
    {

    }
    void GatlingAction() // Makineli tüfek
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
    public void DiscardCard(GameObject card)
    {

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