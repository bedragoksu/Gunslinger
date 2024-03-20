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

        private bool _canStart = true;
        private GameObject _thisPlayer;

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

            ScreenLog.Instance.SendEvent(TextType.Debug, $"onstartclienttt: {base.ObjectId}");


            Playerlist.Clear();
            Players = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < Players.Length; i++)
            {
                var _p = Players[i];
                ScreenLog.Instance.SendEvent(TextType.Debug, $"onstartclienttt: {_p.GetInstanceID()}");

                Playerlist.Add(_p.transform);
                if (i == Players.Length - 1)
                {
                    _thisPlayer = _p;
                }
            }
            //Debug.Log($"player num: {players.Length}");

            PlayerModel player = _thisPlayer.GetComponent<PlayerModel>();

            player.PlayerID = Players.Length - 1;
            player.PlayerRole = PlayerModel.TypeOfPlayer.Bos;

            string playerName = PlayerUIScript.PlayerName;
            player.PlayerName = (playerName.Equals("")) ? $"Player {player.PlayerID+1}":playerName;

            //AssignPlayerModelServer(_thisPlayer.GetComponent<PlayerModel>(), 
            //    Players.Length - 1, 
            //    (playerName.Equals("")) ? $"Player {player.PlayerID + 1}" : playerName);

            ScreenLog.Instance.SendEvent(TextType.Debug, $"helo {player.PlayerID}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player num: {Players.Length}");
        }

        [ServerRpc]
        public void AssignPlayerModelServer(PlayerModel player, int playerID, string name)
        {
            if (IsServer)
            {
                AssignPlayerModel(player, playerID, name);
            }
        }

        [ObserversRpc]
        public void AssignPlayerModel(PlayerModel player, int playerID, string name)
        {
            player.PlayerID = playerID;
            player.PlayerName = name;
            player.PlayerRole = PlayerModel.TypeOfPlayer.Bos;
        }



        private int _sheriffNumber = 1;
        private int _renegadeNumber = 1;
        private int _outlawNumber = 2;
        private int _deputyNumber = 0;

        List<PlayerModel.TypeOfPlayer> possiblePlayerTypes = new List<PlayerModel.TypeOfPlayer>();

        public void StartTheGame()
        {
            _canStart = false;

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

            // assign roles to players
            foreach (var player in Players)
            {
                var randomint = Random.Range(0, possiblePlayerTypes.Count);
                var type = possiblePlayerTypes[randomint];
                possiblePlayerTypes.RemoveAt(randomint);
                ScreenLog.Instance.SendEvent(TextType.Debug, $"player stuff: {player} {type}");
                AssignRolesServer(player, type);
            }

        }

        public void AssignRolesServer(GameObject player, PlayerModel.TypeOfPlayer type)
        {
            AssignRoles(player, type);
        }

        [ObserversRpc]
        public void AssignRoles(GameObject player, PlayerModel.TypeOfPlayer type)
        {
            player.GetComponent<PlayerModel>().PlayerRole = type;
        }

        void Update()
        {

            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.B) && _canStart && IsServer)
                {
                    Playerlist.Clear();
                    Players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var p in Players)
                    {
                        Playerlist.Add(p.transform);
                    }
                    ScreenLog.Instance.SendEvent(TextType.Debug, $"player num: {Players.Length}");
                    if (Players.Length >= 4 && Players.Length <= 7)
                    {
                        StartTheGame();
                    }
                }

                if (Input.GetKeyDown(KeyCode.N))
                {
                    if (_thisPlayer.GetComponent<PlayerModel>() != null)
                    {
                        ScreenLog.Instance.SendEvent(TextType.Debug, $"player id: {_thisPlayer.GetComponent<PlayerModel>().PlayerID}");
                        ScreenLog.Instance.SendEvent(TextType.Debug, $"player type: {_thisPlayer.GetComponent<PlayerModel>().PlayerRole}");
                    }
                }
            }
        }

        public void OnPlayerEntered()
        {

        }

        //void OnPlayerDisconnected(NetworkPlayer player)
        //{
        //    Transform playerTransform = GameObject.Find("Player_" + player.guid);
        //    if (playerTransform != null)
        //        Destroy(playerTransform.gameObject);

        //    Network.RemoveRPCs(networkPlayer);
        //    Network.DestroyPlayerObjects(networkPlayer);
        //}
    }
}
