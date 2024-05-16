using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    // 0 = bang, 1 = missed
    [SerializeField] private Sprite[] _cardSprites;
    // 0 = karo
    [SerializeField] private Sprite[] _symbolSprites;
    // 0 = brown, 1 = blueS
    [SerializeField] private Sprite[] _backgroundSprites;
    [HideInInspector] public string cardName;
    [HideInInspector] public Sprite picture;
    [HideInInspector] public string description;
    [HideInInspector] public Sprite background;
    [HideInInspector] public Sprite symbol;
    private static int numberOfCardGenres = 12;
    //private static string[] _brownCardNames = new string[] { "Bang", "Missed" };
    private static string[] _brownCardNames = new string[] { "Bang", "Missed", "Beer", "Cat Balou", "Gatling", "Panic", "Saloon", "Stage Coach", "Wells Fargo" };
    //private static string[] _blueCardNames = new string[] {  };


    private static string[] _blueCardNames = new string[] { "Barrel", "Mustang", "Scope" };
    [HideInInspector] public CardObject[] cards;

    private int _backgroundIndex = 0;
    private void Start()
    {
        cards = new CardObject[numberOfCardGenres];
        int i = 0;
        foreach (var card in _brownCardNames)
        {
            cards[i] = getCard(card);
            i++;
        }

        _backgroundIndex = 1;
        foreach (var card in _blueCardNames)
        {
            cards[i] = getCard(card);
            i++;
        }
    }

    private void Awake()
    {
        _cardSprites = new Sprite[] {
            Resources.Load<Sprite>("Bang"),
            Resources.Load<Sprite>("Missed"),
            Resources.Load<Sprite>("Gatling"),
            Resources.Load<Sprite>("CatBalou"),
            Resources.Load<Sprite>("Panic"),
            Resources.Load<Sprite>("WellsFargo"),
            Resources.Load<Sprite>("Stagecoach"),
            Resources.Load<Sprite>("Saloon"),
            Resources.Load<Sprite>("Beer"),
            Resources.Load<Sprite>("Mustang"),
            Resources.Load<Sprite>("Scope"),
            Resources.Load<Sprite>("Barrel")
        };
        _backgroundSprites = new Sprite[]
        {
            Resources.Load<Sprite>("_brown_card"),
            Resources.Load<Sprite>("_blue_card")
        };
        _symbolSprites = new Sprite[]
        {
            Resources.Load<Sprite>("diamonds"), // karo
            Resources.Load<Sprite>("clubs"), // sinek
            Resources.Load<Sprite>("hearts"), // kupa
            Resources.Load<Sprite>("spades"), // maça
        };
    }

    private CardObject getCard(string cardName)
    {
        CardObject card = new CardObject();
        switch (cardName.ToLower())
        {
            case "bang":
                card.cardName = cardName;
                card.picture = _cardSprites[0];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "missed":
                card.cardName = cardName;
                card.picture = _cardSprites[1];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "gatling":
                card.cardName = cardName;
                card.picture = _cardSprites[2];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "cat balou":
                card.cardName = cardName;
                card.picture = _cardSprites[3];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "panic":
                card.cardName = cardName;
                card.picture = _cardSprites[4];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "wells fargo":
                card.cardName = cardName;
                card.picture = _cardSprites[5];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "stage coach":
                card.cardName = cardName;
                card.picture = _cardSprites[6];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "saloon":
                card.cardName = cardName;
                card.picture = _cardSprites[7];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "beer":
                card.cardName = cardName;
                card.picture = _cardSprites[8];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "mustang":
                card.cardName = cardName;
                card.picture = _cardSprites[9];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "scope":
                card.cardName = cardName;
                card.picture = _cardSprites[10];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;
            case "barrel":
                card.cardName = cardName;
                card.picture = _cardSprites[11];
                card.background = _backgroundSprites[_backgroundIndex];
                card.symbol = _symbolSprites[0];
                break;

        }
        return card;
    }
}
