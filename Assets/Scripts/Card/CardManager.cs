using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardScriptableObject[] cards;
    public GameObject cardObject;
    [SerializeField] private GameObject _parentObject;
    void Start()
    {
        foreach (var card in cards)
        {
            GameObject sampleCard = createSampleCard(card);
            Debug.Log(sampleCard.name);
            switch (card.name)
            {
                case "Bang":
                    createCards(sampleCard, 25); break;
                case "Missed":
                    createCards(sampleCard, 12); break;

            }
        }
    }
    GameObject createSampleCard(CardScriptableObject card)
    {
        var newCard = Instantiate(cardObject, _parentObject.transform);
        newCard.name = card.name;
        CardDisplayer cardDisplayer = newCard.GetComponent<CardDisplayer>();
        cardDisplayer.card = card;

        return newCard;
    }
    void createCards(GameObject sampleCard, int numberOfCards)
    {
        string cardname = sampleCard.name;
        sampleCard.name += "_1";

        for (int i = 1; i < numberOfCards; i++)
        {
            //GameObject newCard = Instantiate(sampleCard, parentTransform); // Ebeveyni belirt
            GameObject newCard = Instantiate(sampleCard, _parentObject.transform);

            newCard.name = cardname + "_" + (i+1);
        }

    }

    
}
