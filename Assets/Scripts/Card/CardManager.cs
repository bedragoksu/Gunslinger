using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using FishNet.Managing;

public class CardManager : NetworkBehaviour
{
    public CardScriptableObject[] cards;
    public GameObject cardObject;
    [SerializeField] private GameObject _parentObject;

    public GameObject[] CardObjects { get; private set; }
    public List<int> CardOrder { get; private set; }

    private int _cardNum = 37;
    private int _cardCounter = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServer)
        {
            CardObjects = new GameObject[_cardNum];
            CardOrder = new List<int>();
            foreach (var card in cards)
            {
                GameObject sampleCard = createSampleCard(card);
                switch (card.name)
                {
                    case "Bang":
                        createCards(sampleCard, 25); break;
                    case "Missed":
                        createCards(sampleCard, 12); break;

                }
            }

            CardOrder = ShuffleList(CardOrder);
        }
    }

    GameObject createSampleCard(CardScriptableObject card)
    {
        var newCard = Instantiate(cardObject, _parentObject.transform);
        newCard.name = card.name;
        CardDisplayer cardDisplayer = newCard.GetComponent<CardDisplayer>();
        cardDisplayer.card = card;

        CardOrder.Add(_cardCounter);
        CardObjects[_cardCounter++] = newCard;

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

            CardOrder.Add(_cardCounter);
            CardObjects[_cardCounter++] = newCard;
        }

    }

    public List<int> ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }

}
