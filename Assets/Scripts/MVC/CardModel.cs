using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardModel : ScriptableObject
{
    public int Number;
    public SuitOfTheCards Suit;
    public string Name;
    public string Action; //change the type
    public int Type; // 0 -> brown, 1 -> blue
    public Sprite picture;
    public Sprite symbol;
    public char number;


    public enum SuitOfTheCards
    {
        diamonds,
        clubs,
        hearts,
        spades
    }

}
