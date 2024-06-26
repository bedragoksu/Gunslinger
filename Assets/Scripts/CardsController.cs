using FishNet.Managing;
using FishNet.Object;
//using NeptunDigital;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;
using System.Collections;

namespace Gunslinger.Controller
{
    public class CardsController : NetworkBehaviour
    {


        [SerializeField] private NetworkManager _networkManager;
        public GameManager gameManager;
        public PlayerRolesController prc;
        public CharacterDisplayer characterDisplayer;

        [SyncVar]public int CardPointer = 17; // bu sayi simdilik kalsin, assignroles parcalaninca pointer de�erinden alinacak
                                              // pointer cards lengthinden sonra 0'lanmal�
        private int _cardNum = 69;

        private GameObject _deck;


        private bool saved = false;

        [Header("Effects")]
        public Image splatterImage;
        public AudioSource gunAudioSource;
        public AudioClip gunAudio;
        public AudioClip missedAudio;

        public Material RedMaterial;
        public Material OriginalMaterial;


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

            if (plModel.hasGun) // silahi sil //
            {
                // stackhandden discard kart yap
                // stackten deck'e at
                GameObject foundItem = plModel.stackHand.Find(item => item.name.StartsWith("Volcanic") ||
                                                                        item.name.StartsWith("Remington") ||
                                                                        item.name.StartsWith("Rev. Carabine") ||
                                                                        item.name.StartsWith("Schofield") ||
                                                                        item.name.StartsWith("Winchester"));
                if(foundItem) foundItem.transform.SetParent(GameObject.Find("DeckPanel").transform);

            }

            //GameObject cardObj = null;
            //foreach(var c in plModel.openHand)
            //{
            //    if (c.name.StartsWith(cardName))
            //    {
                    
            //        cardObj = c;
            //        break;
            //    }
            //}
            

            //if (cardObj) 
            //{
            //    plModel.openHand.Remove(cardObj);
            //    plModel.stackHand.Add(cardObj);
            //}
            //else
            //{
            //    plModel.openHand.Remove(theCard);
            //    plModel.stackHand.Add(theCard);
            //}

            
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
            if (CardPointer == _cardNum) CardPointer = 0;
            var pointer = player.GetComponent<CardManager>().CardOrder[CardPointer];
            var child = GetChildOfDeck(pointer, _deck);
            child.SetActive(true);
            child.transform.SetParent(GameObject.Find("TheNextCard").transform);
            child.transform.localPosition = new Vector3(0f,0f,0f);

            // change saved
            Debug.Log(child.transform.Find("Symbol").GetComponent<Image>().sprite.name);
            //ScreenLog.Instance.SendEvent(TextType.Debug, child.transform.Find("Symbol").GetComponent<Image>().sprite.name);
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
            DrawCardsObservers(player,amount);
            return true;
        }


        [ServerRpc (RequireOwnership = false)]
        public void DrawCardsServer(GameObject player, int amount)
        {
            DrawCardsObservers(player, amount);
        }


        [ObserversRpc]
        public void DrawCardsObservers(GameObject player, int amount)
        {
            //ScreenLog.Instance.SendEvent(TextType.Debug, "DRAWING CARDS");
            //Debug.Log("DRAW CARDS");
            for(int i=0; i < amount; i++)
            {
                if (CardPointer == _cardNum) CardPointer = 0;
                //Debug.Log("card pointer: " + CardPointer);
                var pointer = player.GetComponent<CardManager>().CardOrder[CardPointer];
                var child = GetChildOfDeck(pointer,_deck);
                //Debug.Log("card: " + child.name);
                player.GetComponent<PlayerModel>().openHand.Add(child);
                child.SetActive(true);
                CardPointer++;
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

            if (player == gameManager._thisPlayer)
            {
                foreach (Transform c in GameObject.Find("HandPanel").transform)
                {
                    c.SetParent(GameObject.Find("DeckPanel").transform);
                }
            }

            player.GetComponent<PlayerModel>().openHand.Add(child);


            if (player == gameManager._thisPlayer)
            {
                foreach (var c in gameManager._thisPlayer.GetComponent<PlayerModel>().openHand)
                {
                    Debug.Log("foreach");
                    c.transform.SetParent(GameObject.Find("HandPanel").transform);
                }
            }

        }

        [HideInInspector] public bool discard = false; 
        [ServerRpc(RequireOwnership =false)]
        public void DiscardCardsServer(GameObject player, int i)
        {
            //ScreenLog.Instance.SendEvent(TextType.Debug, "discard card server");
            DiscardCards(player, i);
        }
        [ObserversRpc]
        public void DiscardCards(GameObject player, int i)
        {
            //ScreenLog.Instance.SendEvent(TextType.Debug, "discard card observer");
            var playermodel = player.GetComponent<PlayerModel>();

            //string Card = GameObject.Find("HandPanel").transform.GetChild(i).gameObject.name;
            //foreach (Transform c in GameObject.Find("HandPanel").transform)
            //{
            //    if(c.gameObject.name == Card)
            //    {
            //        c.SetParent(GameObject.Find("DeckPanel").transform);
            //        break;
            //    }

            //}

            if (player == gameManager._thisPlayer)
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
        public void MoveToStackServer(GameObject player, int i, string name)
        {
            //ScreenLog.Instance.SendEvent(TextType.Debug, "move to stack server");
            MoveToStack(player, i, name);
        }
        [ObserversRpc]
        public void MoveToStack(GameObject player, int i, string name)
        {
            // birer tane mustang scope vs olabilir bedra

            //ScreenLog.Instance.SendEvent(TextType.Debug, "move to stack observer");
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
                    c.transform.SetParent(GameObject.Find("HandPanel").transform);
                }
            }

            var stack = GameObject.Find("PlayerInfoStack").transform; // fatih
            foreach(Transform info in stack)
            {
                if(info.gameObject.name == playermodel.PlayerName)
                {
                    if (name.StartsWith("Mustang"))
                    {
                        var m = info.Find("Mustang").gameObject;
                        m.SetActive(true);
                    }
                    else if (name.StartsWith("Scope"))
                    {
                        info.Find("Scope").gameObject.SetActive(true);
                    }
                    else if (name.StartsWith("Barrel"))
                    {
                        info.Find("Barrel").gameObject.SetActive(true);
                    }




                    if (name.StartsWith("Volcanic"))
                    {
                        info.Find("Volcanic").gameObject.SetActive(true);
                        info.Find("Remington").gameObject.SetActive(false);
                        info.Find("Rev. Carabine").gameObject.SetActive(false);
                        info.Find("Schofield").gameObject.SetActive(false);
                        info.Find("Winchester").gameObject.SetActive(false);
                        if (player == gameManager._thisPlayer) characterDisplayer.UpdateRangeText(1);
                    }
                    else if (name.StartsWith("Remington"))
                    {
                        info.Find("Volcanic").gameObject.SetActive(false);
                        info.Find("Remington").gameObject.SetActive(true);
                        info.Find("Rev. Carabine").gameObject.SetActive(false);
                        info.Find("Schofield").gameObject.SetActive(false);
                        info.Find("Winchester").gameObject.SetActive(false);
                        if (player == gameManager._thisPlayer) characterDisplayer.UpdateRangeText(3);
                    }
                    else if (name.StartsWith("Rev. Carabine"))
                    {
                        info.Find("Volcanic").gameObject.SetActive(false);
                        info.Find("Remington").gameObject.SetActive(false);
                        info.Find("Rev. Carabine").gameObject.SetActive(true);
                        info.Find("Schofield").gameObject.SetActive(false);
                        info.Find("Winchester").gameObject.SetActive(false);
                        if (player == gameManager._thisPlayer) characterDisplayer.UpdateRangeText(4);
                    }
                    else if (name.StartsWith("Schofield"))
                    {
                        info.Find("Volcanic").gameObject.SetActive(false);
                        info.Find("Remington").gameObject.SetActive(false);
                        info.Find("Rev. Carabine").gameObject.SetActive(false);
                        info.Find("Schofield").gameObject.SetActive(true);
                        info.Find("Winchester").gameObject.SetActive(false);
                        if (player == gameManager._thisPlayer) characterDisplayer.UpdateRangeText(2);
                    }
                    else if (name.StartsWith("Winchester"))
                    {
                        info.Find("Volcanic").gameObject.SetActive(false);
                        info.Find("Remington").gameObject.SetActive(false);
                        info.Find("Rev. Carabine").gameObject.SetActive(false);
                        info.Find("Schofield").gameObject.SetActive(false);
                        info.Find("Winchester").gameObject.SetActive(true);
                        if (player == gameManager._thisPlayer) characterDisplayer.UpdateRangeText(5);
                    }



                }
            }

            move = true;
        }



        public bool DealCards()
        {
            var Players = gameManager._turns;
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
                var a = cardOrder[pointer];

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
                 maxpoint = 3;
            }
            else
            {
                maxpoint = 2;
            }
            if(player.CurrentBulletPoint > maxpoint) player.CurrentBulletPoint = maxpoint;
            if(player.CurrentBulletPoint < 0) player.CurrentBulletPoint = 0;

            characterDisplayer.UpdateBullets();

            if(amount < 0)
            {
                if(player.gameObject == gameManager._thisPlayer)
                {
                    StartCoroutine(DamageEffect(player.gameObject));
                }
                else
                {
                    StartCoroutine(DamageOnWorld(player.gameObject));
                }
            }

            if (player.CurrentBulletPoint <= 0) // dead 
            {
                player.IsAlive = false;
                player.CurrentBulletPoint = 0;

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

        [ServerRpc (RequireOwnership = false)]
        public void MissedEffectServer()
        {
            MissedEffect();
        }
        [ObserversRpc]
        private void MissedEffect()
        {
            StartCoroutine(MissedEffectRoutine());
        }
        IEnumerator MissedEffectRoutine()
        {
            yield return new WaitForSeconds(1f);
            gunAudioSource.PlayOneShot(missedAudio);
            yield return new WaitForSeconds(2f);
        }
        IEnumerator DamageEffect(GameObject player)
        {
            Color c = splatterImage.color;
            SkinnedMeshRenderer meshRenderer = player.transform.Find("sheriff/Character_Sheriff_01").GetComponent<SkinnedMeshRenderer>();
            yield return new WaitForSeconds(1f);
            gunAudioSource.PlayOneShot(gunAudio);
            yield return new WaitForSeconds(2f);
            c.a = 0.6f;
            splatterImage.color = c;
            //meshRenderer.material = RedMaterial; // highlight
            yield return new WaitForSeconds(1f);
            c.a = 0f;
            splatterImage.color = c;
            //meshRenderer.materials[0] = OriginalMaterial;
        }
        IEnumerator DamageOnWorld(GameObject player)
        {
            SkinnedMeshRenderer meshRenderer = player.transform.Find("sheriff/Character_Sheriff_01").GetComponent<SkinnedMeshRenderer>();
            yield return new WaitForSeconds(1f);
            gunAudioSource.PlayOneShot(gunAudio);
            yield return new WaitForSeconds(2f);
            //meshRenderer.material = RedMaterial;
            yield return new WaitForSeconds(1f);
            //meshRenderer.materials[0] = OriginalMaterial;
        }

        public List<PlayerModel> GetAlivePlayers()
        {
            var playerList = gameManager._turns;
            List<PlayerModel> aliveList = new List<PlayerModel>();


            foreach (var pl in playerList)
            {
                if (pl.GetComponent<PlayerModel>().IsAlive) aliveList.Add(pl.GetComponent<PlayerModel>());
            }

            return aliveList;
        }

    }
}
