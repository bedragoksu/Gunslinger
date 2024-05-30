using FishNet.Managing;
using FishNet.Object;
using NeptunDigital;
using System.Collections.Generic;
using UnityEngine;

namespace Gunslinger.Controller
{
    public class PlayerRolesControllerAI : NetworkBehaviour
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

        List<PlayerModelAI.TypeOfPlayer> possiblePlayerTypes = new List<PlayerModelAI.TypeOfPlayer>();

        public bool AssignRoles()
        {
            Debug.Log("we can start");

            possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Sheriff);
            possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Renegade);
            possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Outlaw);
            possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Outlaw);

            switch (Players.Length)
            {
                case 4: // might delete
                    break;
                case 5:
                    _deputyNumber = 1;
                    possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Deputy);
                    break;
                case 6:
                    _outlawNumber = 3;
                    _deputyNumber = 1;
                    possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Outlaw);
                    break;
                case 7:
                    _outlawNumber = 3;
                    _deputyNumber = 2;
                    possiblePlayerTypes.Add(PlayerModelAI.TypeOfPlayer.Deputy);
                    break;
            }
            var servercardmanager = Players[0].GetComponent<CardManagerAI>();
            int counter = 0;
            pointer = 0;
            // assign roles to players
            foreach (var player in Players)
            {
                var randomint = 0; // Random.Range(0, possiblePlayerTypes.Count)
                var type = possiblePlayerTypes[randomint];
                possiblePlayerTypes.RemoveAt(randomint);
                ScreenLog.Instance.SendEvent(TextType.Debug, $"player stuff: {player} {type}");
                var plmodel = player.GetComponent<PlayerModelAI>();
                AssignRolesServer(player, type, counter, plmodel.PlayerName);
                //AssignCards(player, servercardmanager.CardOrder);
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
        public void AssignRolesServer(GameObject player, PlayerModelAI.TypeOfPlayer type, int id, string name)
        {
            var model = player.GetComponent<PlayerModelAI>();
            model.PlayerRole = type;
            model.PlayerID = id;
            model.PlayerName = name;
            model.CurrentBulletPoint = 4;
            if(type == PlayerModelAI.TypeOfPlayer.Sheriff)
            {
                model.CurrentBulletPoint = 5;
            }
        }

        

    }
}
