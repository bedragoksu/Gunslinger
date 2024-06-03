using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;
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

    private int _cardNum = 69;
    private int _cardCounter = 0;
    
    private Sprite[] _symbolSprites;
    private int _symbolCount = 0;
    private void Start()
    {
        // sampleCardObject = GameObject.Find("SampleCard");
        //_parentObject = GameObject.Find("DeckPanel");
        //Cards = GameObject.Find("CardManager").GetComponent<CardObject>().cards;
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _cardCounter = 0;
        _symbolSprites = new Sprite[]
                {
                    Resources.Load<Sprite>("diamonds"), // karo
                    Resources.Load<Sprite>("clubs"), // sinek
                    Resources.Load<Sprite>("hearts"), // kupa
                    Resources.Load<Sprite>("spades"), // maça
                };
        //if (!base.IsOwner)
        //{
        //    gameObject.GetComponent<CardManager>().enabled = false;
        //}
        _parentObject = GameObject.Find("DeckPanel");
        Cards = GameObject.Find("CardManager").GetComponent<CardObject>().cards;
        if (true)
        {
            
            CardObjects = new GameObject[_cardNum];
            foreach (var card in Cards)
            {
                GameObject sampleCard = createSampleCard(card);
                switch (card.cardName)
                {
                    case "Bang": // ok
                        createCards(sampleCard, 25); break;
                    case "Missed": // ok
                        createCards(sampleCard, 1); break;
                    case "Gatling": // ok
                        createCards(sampleCard, 1); break;
                    case "Cat Balou": // ok
                        createCards(sampleCard, 4); break;
                    case "Panic": // tt
                        createCards(sampleCard, 4); break;
                    case "Wells Fargo": // ok
                        createCards(sampleCard, 1); break;
                    case "Stage coach": // ok
                        createCards(sampleCard, 2); break;
                    case "Saloon": // ok
                        createCards(sampleCard, 1); break;
                    case "Beer": // ok
                        createCards(sampleCard, 16); break;
                    case "Mustang": // ok // onune ac
                        createCards(sampleCard, 2); break;
                    case "Scope": // ok
                        createCards(sampleCard, 1); break;
                    case "Barrel": // ok
                        createCards(sampleCard, 2); break;
                    case "Volcanic":
                        createCards(sampleCard, 2); break;
                    case "Remington":
                        createCards(sampleCard, 1); break;
                    case "Rev. Carabine":
                        createCards(sampleCard, 1); break;
                    case "Schofield":
                        createCards(sampleCard, 3); break;
                    case "Winchester":
                        createCards(sampleCard, 1); break;
                        //{ "Barrel", "Mustang", "Scope", "Volcanic" 12, "Remington" 13, "Rev. Carabine" 14, "Schofield" 15, "Winchester" 16 };


                }
                _symbolCount = 0;
            }

            InitializeCardOrder();
            CardOrder = ShuffleList(CardOrder);
        }

    }

    public void InitializeCardOrder()
    {
        CardOrder = new List<int>();
        for (int i = 0; i < _cardNum; i++)
        {
            CardOrder.Add(i);
        }
    }

    GameObject createSampleCard(CardObject card)
    {
        var newCard = Instantiate(sampleCardObject, _parentObject.transform);
        newCard.name = card.cardName;
        CardDisplayer cardDisplayer = newCard.GetComponent<CardDisplayer>();
        cardDisplayer.DisplayCard(card);

        CardObjects[_cardCounter++] = newCard;

        return newCard;
    }
    void createCards(GameObject sampleCard, int numberOfCards)
    {
        string cardname = sampleCard.name;
        sampleCard.name += "_1";
        
        for (int i = 1; i < numberOfCards; i++)
        {
            GameObject newCard = Instantiate(sampleCard, _parentObject.transform);

            newCard.name = cardname + "_" + (i + 1);
            AssignSymbol(newCard, i);
            CardObjects[_cardCounter++] = newCard;
        }
        AssignSymbol(sampleCard, 0);

    }
    private void AssignSymbol(GameObject card, int num)
    {
        GameObject symbol = card.transform.Find("Symbol").gameObject;
        Image symbolImage = symbol.GetComponent<Image>();
        
        if (card.name.StartsWith("Missed"))
        {
            if (num == 0 || num == 1)
            {
                symbolImage.sprite = _symbolSprites[1];
            }
        }
        else if (card.name.StartsWith("Panic"))
        {
            if (num == 0)
            {
                symbolImage.sprite = _symbolSprites[0];
            }
        }
        else if (card.name.StartsWith("Cat"))
        {
            if (num == 0)
            {
                symbolImage.sprite = _symbolSprites[2];
            }
        }
        else if (card.name.StartsWith("Bang"))
        {
            if (num == 0)
            {
                symbolImage.sprite = _symbolSprites[3];
            }
            else if (num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8)
            {
                symbolImage.sprite = _symbolSprites[1];
            }
            else if (num == 9 || num == 10 || num == 11)
            {
                symbolImage.sprite = _symbolSprites[2];
            }
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
    }


}