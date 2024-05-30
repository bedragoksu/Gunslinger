using FishNet.Managing;
using FishNet.Object;
using NeptunDigital;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;

namespace Gunslinger.Controller
{
    public class CardsController : NetworkBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        public GameManager gameManager;
        public PlayerRolesController prc;

        [SyncVar]public int CardPointer = 17; // bu sayi simdilik kalsin, assignroles parcalaninca pointer deðerinden alinacak
                                     // pointer cards lengthinden sonra 0'lanmalý

        private GameObject[] Players;
        private GameObject _deck;


        private bool saved = false;

        [ServerRpc(RequireOwnership =false)]
        public void AddGunServer(GameObject player, int rangeAmount, GameObject theCard, bool multipleBangs, string cardName)
        {
            AddGun(player, rangeAmount, theCard, multipleBangs, cardName);
        }
        [ObserversRpc]
        private void AddGun(GameObject player, int rangeAmount, GameObject theCard, bool multipleBangs, string cardName)
        {
            PlayerModel plModel = player.GetComponent<PlayerModel>();
            plModel.CanPlayMultipleBangs = false;

            if (plModel.hasGun) // silahi sil
            {
                // stackhandden discard kart yap
                // stackten deck'e at
                GameObject foundItem = plModel.stackHand.Find(item => item.name.StartsWith("Volcanic") ||
                                                                        item.name.StartsWith("Remington") ||
                                                                        item.name.StartsWith("Rev. Carabine") ||
                                                                        item.name.StartsWith("Schofield") ||
                                                                        item.name.StartsWith("Winchester"));
                foundItem.transform.SetParent(GameObject.Find("DeckPanel").transform);
            }

            GameObject cardObj = null;
            foreach(var c in plModel.openHand)
            {
                if (c.name.StartsWith(cardName))
                {
                    plModel.openHand.Remove(c);
                    cardObj = c;
                }
            }

            if (cardObj) 
            {
                plModel.stackHand.Add(cardObj);
            }
            else
            {
                plModel.stackHand.Add(theCard);
            }

            GameObject cardfrompanel = null;
            GameObject handpanel = GameObject.Find("HandPanel");
            for (int i = 0; i< handpanel.transform.childCount; i++)
            {
                if (handpanel.transform.GetChild(i).gameObject.name.StartsWith(cardName))
                {
                    cardfrompanel = handpanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
            
            if(cardfrompanel)
            {
                cardfrompanel.transform.SetParent(GameObject.Find("StackHandPanel").transform);
                cardfrompanel.SetActive(false);
            }
            plModel.Range = rangeAmount;
            plModel.hasGun = true;
            plModel.CanPlayMultipleBangs = multipleBangs;
        }


        public bool CheckNext(GameObject player)
        {
            CheckTheNextCardServer(player); // bedra, need coroutine?
            return saved;
        }

        [ServerRpc(RequireOwnership =false)]
        private void CheckTheNextCardServer(GameObject player)
        {
            CheckTheNextCard(player);
        }
        [ObserversRpc]
        private void CheckTheNextCard(GameObject player)
        {
            saved = false;

            var pointer = player.GetComponent<CardManager>().CardOrder[CardPointer];
            var child = GetChildOfDeck(pointer, _deck);
            child.SetActive(true);
            child.transform.SetParent(GameObject.Find("TheNextCard").transform);
            child.transform.localPosition = new Vector3(0f,0f,0f);

            // change saved
            if(child.transform.Find("Symbol").GetComponent<Image>().sprite.name == "hearts")
            {
                saved = true;
            }

            CardPointer++;
        }


        [ServerRpc(RequireOwnership = false)]
        public void CloseTheNextCardServer()
        {
            CloseTheNextCard();
        }
        [ObserversRpc]
        private void CloseTheNextCard()
        {
            var par = GameObject.Find("TheNextCard").transform;
            var deck = GameObject.Find("DeckPanel").transform;
            foreach (Transform c in par)
            {
                c.SetParent(deck);
            }
        }


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


        [ServerRpc(RequireOwnership =false)]
        public void DrawTheCardServer(GameObject player, int index)
        {
            DrawTheCardObs(player, index);

        }
        [ObserversRpc]
        public void DrawTheCardObs(GameObject player, int index)
        {
            var child = GetChildOfDeck(index, _deck);
            child.SetActive(true);
            player.GetComponent<PlayerModel>().openHand.Add(child);
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
            playermodel.openHand[i].SetActive(false);
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

        [HideInInspector] public bool move = false;
        [ServerRpc(RequireOwnership = false)]
        public void MoveToStackServer(GameObject player, int i)
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "move to stack server");
            MoveToStack(player, i);
        }
        [ObserversRpc]
        public void MoveToStack(GameObject player, int i)
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, "move to stack observer");
            var playermodel = player.GetComponent<PlayerModel>();

            if (player == gameManager._thisPlayer)
            {
                foreach (Transform c in GameObject.Find("HandPanel").transform)
                {
                    c.SetParent(GameObject.Find("DeckPanel").transform);
                }
            }

            var o = playermodel.openHand[i];
            //stack panele at bedra
            o.SetActive(false);
            o.transform.SetParent(GameObject.Find("StackHandPanel").transform);
            playermodel.stackHand.Add(o);

            playermodel.openHand.RemoveAt(i);

            if (player == gameManager._thisPlayer)
            {
                foreach (var c in gameManager._thisPlayer.GetComponent<PlayerModel>().openHand)
                {
                    Debug.Log("foreach");
                    c.transform.SetParent(GameObject.Find("HandPanel").transform);
                }
            }

            var stack = GameObject.Find("PlayerInfoStack").transform;
            foreach(Transform info in stack)
            {
                if(info.gameObject.name == playermodel.PlayerName)
                {
                    if (o.name.StartsWith("Mustang"))
                    {
                        var m = info.Find("Mustang").gameObject;
                        m.SetActive(true);
                        Debug.Log(m.activeSelf);
                    }
                    else if (o.name.StartsWith("Scope"))
                    {
                        info.Find("Scope").gameObject.SetActive(true);
                    }
                    else if (o.name.StartsWith("Barrel"))
                    {
                        info.Find("Barrel").gameObject.SetActive(true);
                    }
                }
            }

            move = true;
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
        public void UpdateHealthServer(PlayerModel player, int amount) // 0 ve max burada kontrol et
        {
            UpdateHealth(player, amount);
        }
        [ObserversRpc]
        public void UpdateHealth(PlayerModel player, int amount)
        {
            player.CurrentBulletPoint += amount;// can 0 olmasi durumu/ dead !! bedra

            int maxpoint = 0;
            if (player.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff)
            {
                 maxpoint = 5;
            }
            else
            {
                maxpoint = 4;
            }
            if(player.CurrentBulletPoint > maxpoint) player.CurrentBulletPoint = maxpoint;

            if(player.CurrentBulletPoint <= 0) // dead 
            {
                player.IsAlive = false;


                var currentAlives = GetAlivePlayers();
                bool lastSheriff = true;
                foreach(var a in currentAlives)
                {
                    if(a.PlayerRole == PlayerModel.TypeOfPlayer.Outlaw || a.PlayerRole == PlayerModel.TypeOfPlayer.Renegade)
                    {
                        lastSheriff = false;
                    }
                }

                if(player.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff || lastSheriff)
                {
                    gameManager.UpdateGameState(GameManager.GameState.EndOfGame);
                }


            }

        }

        public List<PlayerModel> GetAlivePlayers()
        {
            var playerList = GameObject.FindGameObjectsWithTag("Player");
            List<PlayerModel> aliveList = new List<PlayerModel>();


            foreach (var pl in playerList)
            {
                if (pl.GetComponent<PlayerModel>().IsAlive) aliveList.Add(pl.GetComponent<PlayerModel>());
            }

            return aliveList;
        }

    }
}
