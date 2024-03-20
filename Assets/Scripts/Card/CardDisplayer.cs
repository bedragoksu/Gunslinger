using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CardDisplayer : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image picture;
    public Image symbol;
    public CardScriptableObject card;
    public TextMeshProUGUI range;
    //public GunCard gun;

    void Start()
    {
        if (card is GunCard)
        {
            GunCard gunCard = (GunCard)card;
            range.text = gunCard.range.ToString();
        }
        nameText.text = card.name;
        picture.sprite = card.picture;
        symbol.sprite = card.symbol;
    }

}
