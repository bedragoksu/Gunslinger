using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField]
    private CardModel card;
    [SerializeField]
    private TextMeshPro nameText;
    private Image pictureImage;
    private Image symbolImage;
    private Text numberText;

    void Start()
    {
        //nameText.text = card.name;

        //pictureImage.sprite = card.picture;
        //symbolImage.sprite = card.symbol;

        //numberText.text = card.number.ToString();
    }
}
