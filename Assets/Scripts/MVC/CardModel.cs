using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    public int Number;
    public SuitOfTheCards Suit;
    public string Name;
    public string Action; //change the type
    public int Type; // 0 -> brown, 1 -> blue


    public enum SuitOfTheCards
    {
        diamonds,
        clubs,
        hearts,
        spades
    }

}
