using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CardObject : NetworkBehaviour
{
    // 0 = bang, 1 = missed
    [SerializeField] private Sprite[] _cardSprites;
    // 0 = karo
    [SerializeField] private Sprite[] _symbolSprites;
    // 0 = brown, 1 = blue
    [SerializeField] private Sprite[] _backgroundSprites;
    [HideInInspector] public string name;
    [HideInInspector] public Sprite picture;
    [HideInInspector] public string description;
    [HideInInspector] public Sprite background;
    [HideInInspector] public Sprite symbol;
    private static int numberOfCardGenres = 2;
    private static string[] _brownCardNames = new string[] { "Bang", "Missed" };
    //private string[] _brownCardNames = new string[] { "Bang", "Missed", "Beer", "Cat Balou", "Gatling", "Panic", "Saloon", "Stage Coach", "Wells Fargo" };
    private static string[] _blueCardNames = new string[] {  };


    //private string[] _blueCardNames = new string[] { "Barrel", "Mustang", "Scope" };
    [HideInInspector] public CardObject[] cards;
    private void Start()
    {
        cards = new CardObject[numberOfCardGenres];
        int i = 0;
        foreach (var card in _brownCardNames)
        {
            cards[i] = getCard(card);
            i++;
        }

        foreach (var card in _blueCardNames)
        {
            cards[i] = getCard(card);
            i++;
        }
    }
    
    private CardObject getCard(string name)
    {
        CardObject card = new CardObject();
        switch (name.ToLower())
        {
            case "bang":
                card.name = name;
                card.picture = _cardSprites[0];
                card.background = _backgroundSprites[0];
                card.symbol = _symbolSprites[0];
                break;
            case "missed":
                card.name = name;
                card.picture = _cardSprites[1];
                card.background = _backgroundSprites[0];
                card.symbol = _symbolSprites[0];
                break;
        }
        return card;
    }
}
