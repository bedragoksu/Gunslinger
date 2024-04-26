using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;
    public Sprite picture;
    public Sprite symbol;
    public char number;

    public void Print()
    {
        Debug.Log(name);
    }

}
