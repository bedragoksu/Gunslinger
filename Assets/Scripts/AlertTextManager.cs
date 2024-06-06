using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertTextManager : MonoBehaviour
{
    public string GetBangText(string PlayerName, string TargetName)
    {
        return $"{PlayerName} targetted {TargetName}";
    }

    public string GetBarrelText(string PlayerName)
    {
        return $"Your barrel saved you, {PlayerName}";
    }

    public string GetMissedText(string PlayerName)
    {
        return $"Missed card saved you, {PlayerName}";
    }

    public string GetBeerText(string PlayerName)
    {
        return $"Bottoms up for {PlayerName}!";
    }

    public string GetSaloonText(string PlayerName)
    {
        return $"Drinks are on {PlayerName}!";
    }


    public string GetGatlingText(string PlayerName)
    {
        return $"Ratatatat! The Gatling sprays bullets!";
    }

    public string GetCatBalouText(string PlayerName, string targetName)
    {
        return $"Cat Balou by {PlayerName}! {targetName}'s card is discarded!";
    }

    public string GetPanicText(string PlayerName, string targetName)
    {
        return $"Panic in the air! {PlayerName} steals a card from {targetName}!";
    }

    public string GetRangeForBangText()
    {
        return "Out of range! Your target is too far.";
    }

    public string GetBangNobodyIsInRangeText()
    {
        return "No one in range! All targets are too far.";
    }

    public string GetPanicRangeText()
    {
        return "Pick left or right! Target only your immediate neighbors.";
    }

}
