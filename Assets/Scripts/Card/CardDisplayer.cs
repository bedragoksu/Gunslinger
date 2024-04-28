using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CardDisplayer : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI nameText;
    public Image picture;
    public Image symbol;
    public TextMeshProUGUI descriptionText;

    public void DisplayCard(CardObject cardObject)
    {
        background.sprite = cardObject.background;
        nameText.text = cardObject.cardName;
        picture.sprite = cardObject.picture;
        symbol.sprite = cardObject.symbol;
        descriptionText.text = cardObject.description;
    }

}
