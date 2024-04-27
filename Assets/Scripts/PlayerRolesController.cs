using FishNet.Managing;
using FishNet.Object;
using NeptunDigital;
using System.Collections.Generic;
using UnityEngine;

namespace Gunslinger.Controller
{
    public class PlayerRolesController : NetworkBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;

        public List<Transform> Playerlist { get; private set; } //assign game manager to this value, at gamemanagerscript, when game started
        public GameObject[] Players { get; private set; } //assign game manager to this value, at gamemanagerscript, when game started
        // also needed for turn queue

        public PlayerUI PlayerUIScript;
        public GameManager gameManager;

        private int pointer;

        void Start()
        {

            Playerlist = new List<Transform>();
            Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in Players)
            {
                Playerlist.Add(p.transform);
            }
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            Playerlist.Clear();
            Players = GameObject.FindGameObjectsWithTag("Player");
        }





        private int _sheriffNumber = 1;
        private int _renegadeNumber = 1;
        private int _outlawNumber = 2;
        private int _deputyNumber = 0;

        List<PlayerModel.TypeOfPlayer> possiblePlayerTypes = new List<PlayerModel.TypeOfPlayer>();

        public bool AssignRoles()
        {
            Debug.Log("we can start");

            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Sheriff);
            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Renegade);
            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Outlaw);
            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Outlaw);

            switch (Players.Length)
            {
                case 4: // might delete
                    break;
                case 5:
                    _deputyNumber = 1;
                    possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Deputy);
                    break;
                case 6:
                    _outlawNumber = 3;
                    _deputyNumber = 1;
                    possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Outlaw);
                    break;
                case 7:
                    _outlawNumber = 3;
                    _deputyNumber = 2;
                    possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Deputy);
                    break;
            }
            var servercardmanager = Players[0].GetComponent<CardManager>();
            int counter = 0;
            pointer = 0;
            // assign roles to players
            foreach (var player in Players)
            {
                var randomint = Random.Range(0, possiblePlayerTypes.Count);
                var type = possiblePlayerTypes[randomint];
                possiblePlayerTypes.RemoveAt(randomint);
                ScreenLog.Instance.SendEvent(TextType.Debug, $"player stuff: {player} {type}");
                var plmodel = player.GetComponent<PlayerModel>();
                AssignRolesServer(player, type, counter, plmodel.PlayerName);
                AssignCards(player, servercardmanager.CardOrder);
                counter++;
            }

            return true;
        }


        public int PlayersUpdate()
        {
            Playerlist.Clear();
            Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in Players)
            {
                Playerlist.Add(p.transform);
            }
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player num: {Players.Length}");

            return Players.Length;
        }

        [ObserversRpc]
        public void AssignRolesServer(GameObject player, PlayerModel.TypeOfPlayer type, int id, string name)
        {
            var model = player.GetComponent<PlayerModel>();
            model.PlayerRole = type;
            model.PlayerID = id;
            model.PlayerName = name;
            model.CurrentBulletPoint = 4;
            if(type == PlayerModel.TypeOfPlayer.Sheriff)
            {
                model.CurrentBulletPoint = 5;
            }
        }

        [ObserversRpc]
        public void AssignCards(GameObject player, List<int> cardOrder)
        {
            var card = player.GetComponent<CardManager>();

            //card.CardObjects = cards;
            card.CardOrder = cardOrder;

            var pl = player.GetComponent<PlayerModel>();
            var cardss = GameObject.Find("DeckPanel");
            for (int i = 0; i < pl.CurrentBulletPoint; i++)
            {
                Debug.Log($"pointer value is {pointer}");
                var a = cardOrder[pointer];
                Debug.Log($"order value is {a}");
                
                var child = GetChildOfDeck(a, cardss);
                child.SetActive(true);
                pl.openHand.Add(child);
                pointer++;
            }

        }

        public GameObject GetChildOfDeck(int index, GameObject parent)
        {
            var counter = 0;
            foreach(Transform c in parent.transform)
            {
                if(counter == index)
                {
                    return c.gameObject;
                }
                counter++;
            }
            return parent.transform.GetChild(index).gameObject;
        }

    }
}
