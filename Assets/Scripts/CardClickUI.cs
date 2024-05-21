using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClickUI : MonoBehaviour
{
    public GameObject selectedCard;


    public void OnSelectCard(GameObject card)
    {
        if (selectedCard != null && selectedCard != card)
        {
            CardUIManager cardUIManager = selectedCard.GetComponent<CardUIManager>();
            cardUIManager.ToOriginalScale();
            cardUIManager.ToGray();
            cardUIManager.isSelected = false;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            CardUIManager cardUIManager = child.GetComponent<CardUIManager>();
            if (child == card)
            {
                cardUIManager.ToHoverScale();
                cardUIManager.ToWhite();
                cardUIManager.isSelected = true;
                continue;
            }
            
            cardUIManager.ToGray();
        }
        selectedCard = card;

    }
}
