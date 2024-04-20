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

        public void AssignRoles()
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
            // assign roles to players
            foreach (var player in Players)
            {
                var randomint = Random.Range(0, possiblePlayerTypes.Count);
                var type = possiblePlayerTypes[randomint];
                possiblePlayerTypes.RemoveAt(randomint);
                ScreenLog.Instance.SendEvent(TextType.Debug, $"player stuff: {player} {type}");
                AssignRoles(player, type);
                AssignCards(player, servercardmanager.CardOrder, servercardmanager.Cards);
            }

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
        public void AssignRoles(GameObject player, PlayerModel.TypeOfPlayer type)
        {
            player.GetComponent<PlayerModel>().PlayerRole = type;
        }

        [ObserversRpc]
        public void AssignCards(GameObject player, List<int> cardOrder, CardObject[] cards)
        {
            var card = player.GetComponent<CardManager>();

            card.Cards = cards;
            card.CardOrder = cardOrder;
        }

    }
}
