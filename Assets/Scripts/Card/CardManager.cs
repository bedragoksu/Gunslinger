using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using FishNet.Managing;
using Gunslinger.Controller;
using System.CodeDom.Compiler;
using NeptunDigital;

public class CardManager : NetworkBehaviour
{
    public CardObject[] Cards;
    public GameObject sampleCardObject;
    [SerializeField] private GameObject _parentObject;

    public GameObject[] CardObjects;
    public List<int> CardOrder;

    private int _cardNum = 37;
    private int _cardCounter = 0;
    public GameObject[] Players;

    private void Start()
    {
        // sampleCardObject = GameObject.Find("SampleCard");
        _parentObject = GameObject.Find("DeckPanel");
        Cards = GameObject.Find("CardManager").GetComponent<CardObject>().cards;

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            gameObject.GetComponent<CardManager>().enabled = false;
        }
        if (base.IsServer && base.IsOwner)
        {
            
            CardObjects = new GameObject[_cardNum];
            CardOrder = new List<int>();
            foreach (var card in Cards)
            {
                GameObject sampleCard = createSampleCard(card);
                switch (card.cardName)
                {
                    case "Bang":
                        createCards(sampleCard, 25); break;
                    case "Missed":
                        createCards(sampleCard, 12); break;

                }
            }

            CardOrder = ShuffleList(CardOrder);
        }
        //if (IsOwner && !IsServer)
        //{
        //    Players = GameObject.FindGameObjectsWithTag("Player");
        //    if (Players.Length != 0)
        //    {
        //        CardObjects = Players[0].GetComponent<CardManager>().CardObjects;
        //        Debug.Log(Players[0].GetComponent<CardManager>().CardObjects);
        //        CardOrder = Players[0].GetComponent<CardManager>().CardOrder;
        //        GenerateCards();
        //    }

        //}


    }

    GameObject createSampleCard(CardObject card)
    {
        var newCard = Instantiate(sampleCardObject, _parentObject.transform);
        newCard.name = card.cardName;
        CardDisplayer cardDisplayer = newCard.GetComponent<CardDisplayer>();
        cardDisplayer.ChangeCard(card);

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

            newCard.name = cardname + "_" + (i + 1);

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

    private void GenerateCards()
    {
        Debug.Log(CardObjects);
        Debug.Log(CardOrder);
        //for (int i = 0; i < CardObjects.Length; i++)
        //{
        //    GameObject newCard = Instantiate(CardObjects[CardOrder[i]], _parentObject.transform);
        //}
    }


}