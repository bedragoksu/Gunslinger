using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
using NeptunDigital;
using Gunslinger.Controller;

public class CardDisplayer : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image picture;
    public Image symbol;
    public CardObject card;
    public Image background;
    public TextMeshProUGUI range;

    //public GunCard gun;

    void Start()
    {
        //if (card is GunCard)
        //{
        //    GunCard gunCard = (GunCard)card;
        //    range.text = gunCard.range.ToString();
        //}
        //nameText.text = "CardName";
        //picture.sprite = Resources.Load<Sprite>("Panic");
        //symbol.sprite = Resources.Load<Sprite>("Hearts");
        //background.sprite = Resources.Load<Sprite>("_blue_card");
        if (card != null)
        {
            nameText.text = card.name;
            picture.sprite = card.picture;
            symbol.sprite = card.symbol;
            background.sprite = card.background;
        }
        
    }
    public void ChangeCard(CardObject cardObject)
    {
        nameText.text = cardObject.cardName;
        picture.sprite = cardObject.picture;
        symbol.sprite = cardObject.symbol;
        background.sprite = cardObject.background;
    }

}
