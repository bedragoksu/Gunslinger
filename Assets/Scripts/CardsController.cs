using FishNet.Managing;
using FishNet.Object;
using NeptunDigital;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;

namespace Gunslinger.Controller
{
    public class CardsController : NetworkBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        public GameManager gameManager;
        public PlayerRolesController prc;

        [SyncVar]public int CardPointer = 17; // bu sayi simdilik kalsin, assignroles parcalaninca pointer de�erinden alinacak
                                     // pointer cards lengthinden sonra 0'lanmal�

        private GameObject[] Players;
        private GameObject _deck;

        public bool DrawCards(GameObject player,int amount)
        {
            DrawCardsServer(player,amount);
            return true;
        }

        [ObserversRpc]
        public void DrawCardsServer(GameObject player, int amount)
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "DRAWING CARDS");
            for(int i=0; i < amount; i++)
            {
                var pointer = player.GetComponent<CardManager>().CardOrder[CardPointer];
                var child = GetChildOfDeck(pointer,_deck);
                child.SetActive(true);
                player.GetComponent<PlayerModel>().openHand.Add(child);
                CardPointer++; // herkes icin guncelle
            }
            
        }

        [HideInInspector] public bool discard = false; 
        [ServerRpc(RequireOwnership =false)]
        public void DiscardCardsServer(GameObject player, int i)
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "discard card server");
            DiscardCards(player, i);
        }
        [ObserversRpc]
        public void DiscardCards(GameObject player, int i)
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "discard card observer");
            var playermodel = player.GetComponent<PlayerModel>();

            if(player == gameManager._thisPlayer)
            {
                foreach (Transform c in GameObject.Find("HandPanel").transform)
                {
                    c.SetParent(GameObject.Find("DeckPanel").transform);
                }
            }

            playermodel.openHand.RemoveAt(i);

            if (player == gameManager._thisPlayer)
            {
                foreach (var c in gameManager._thisPlayer.GetComponent<PlayerModel>().openHand)
                {
                    Debug.Log("foreach");
                    c.transform.SetParent(GameObject.Find("HandPanel").transform);
                }
            }
            
            
            discard = true;
        }

        public bool DealCards()
        {
            Players = GameObject.FindGameObjectsWithTag("Player");
            _deck = GameObject.Find("DeckPanel");
            var servercardmanager = Players[0].GetComponent<CardManager>();
            int counter = 0;
            // assign roles to players
            foreach (var player in Players)
            {
                var plmodel = player.GetComponent<PlayerModel>();
                AssignCards(player, servercardmanager.CardOrder);
                //DrawCards(player, plmodel.CurrentBulletPoint);
                counter++;
            }
            return true;
        }

        private int pointer = 0;
        [ObserversRpc]
        public void AssignCards(GameObject player, List<int> cardOrder)
        {
            var card = player.GetComponent<CardManager>();

            //card.CardObjects = cards;
            card.CardOrder = cardOrder;

            var pl = player.GetComponent<PlayerModel>();
            
            for (int i = 0; i < pl.CurrentBulletPoint; i++)
            {
                Debug.Log($"pointer value is {pointer}");
                var a = cardOrder[pointer];
                Debug.Log($"order value is {a}");

                var child = GetChildOfDeck(a, _deck);
                child.SetActive(true);
                pl.openHand.Add(child);
                pointer++;
            }

            //DrawCards(player, pl.CurrentBulletPoint);

        }

        public GameObject GetChildOfDeck(int index, GameObject parent)
        {
            var counter = 0;
            foreach (Transform c in parent.transform)
            {
                if (counter == index)
                {
                    return c.gameObject;
                }
                counter++;
            }
            return parent.transform.GetChild(index).gameObject;
        }

        [ServerRpc(RequireOwnership =false)]
        public void UpdateHealthServer(PlayerModel player, int amount)
        {
            UpdateHealth(player, amount);
        }
        [ObserversRpc]
        public void UpdateHealth(PlayerModel player, int amount)
        {
            player.CurrentBulletPoint += amount; // max olup olmadigina bak !! bedra
        }


    }
}
