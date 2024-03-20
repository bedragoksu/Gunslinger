using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Character,
    Action,
    Gun
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Card")]
public class CardScriptableObject : ScriptableObject
{
    public string name;
    public Sprite picture;
    public string description;
    public Sprite background;
    public CardType cardType;
    public Sprite symbol;
}

//[System.Serializable]
//[CreateAssetMenu(fileName = "New CharcterCard", menuName = "Card/CharcterCard")]
//public class CharacterCard : CardScriptableObject
//{
//    public int maxBullet;
//}

//[System.Serializable]
//[CreateAssetMenu(fileName = "New ActionCard", menuName = "Card/ActionCard")]
//public class ActionCard : CardScriptableObject
//{
//    public Sprite symbol;
//}

//[System.Serializable]
//[CreateAssetMenu(fileName = "New GunCard", menuName = "Card/GunCard")]
//public class GunCard : CardScriptableObject
//{
//    public int range;
//}