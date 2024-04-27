using FishNet.Managing;
using FishNet.Object;
using NeptunDigital;
using System.Collections.Generic;
using UnityEngine;

namespace Gunslinger.Controller
{
    public class CardsController : NetworkBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        public GameManager gameManager;
        public PlayerRolesController prc;

        public int CardPointer = 17; // bu sayi simdilik kalsin, assignroles parcalaninca pointer deðerinden alinacak
                                    // pointer cards lengthinden sonra 0'lanmalý

        public bool DrawCards(GameObject player,int amount)
        {
            DrawCardsServer(player,amount);
            return true;
        }

        [ObserversRpc]
        public void DrawCardsServer(GameObject player, int amount)
        {
            for(int i=0; i < amount; i++)
            {
                var pointer = player.GetComponent<CardManager>().CardOrder[CardPointer];
                var child = prc.GetChildOfDeck(pointer, GameObject.Find("DeckPanel"));
                child.SetActive(true);
                player.GetComponent<PlayerModel>().openHand.Add(child);
                CardPointer++; // herkes icin guncelle
            }
            
        }


    }
}
