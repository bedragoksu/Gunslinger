using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Example;
using NeptunDigital;

namespace Gunslinger.Controller
{
    public class PlayerRolesController : NetworkBehaviour
    {
        public List<Transform> playerlist { get; private set; }
        public GameObject[] players { get; private set; }
        bool CanStart = true;

        private GameObject _thisPlayer;

        [SerializeField]
        private NetworkManager _networkManager;

        void Start()
        {
            playerlist = new List<Transform>();
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                playerlist.Add(p.transform);
            }
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            ScreenLog.Instance.SendEvent(TextType.Debug, $"onstartclienttt: {base.ObjectId}");


            playerlist.Clear();
            players = GameObject.FindGameObjectsWithTag("Player");
            
            for(int i=0; i<players.Length;i++)
            {
                var _p = players[i];
                ScreenLog.Instance.SendEvent(TextType.Debug, $"onstartclienttt: {_p.GetInstanceID()}");

                playerlist.Add(_p.transform);
                if(i == players.Length - 1)
                {
                    _thisPlayer = _p;
                }
            }
            //Debug.Log($"player num: {players.Length}");

            PlayerModel player = _thisPlayer.GetComponent<PlayerModel>();

            player.PlayerID = players.Length - 1;
            player.PlayerRole = PlayerModel.TypeOfPlayer.Bos;

            ScreenLog.Instance.SendEvent(TextType.Debug, $"helo {player.PlayerID}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player num: {players.Length}");


        }

        private int sheriffNumber = 1;
        private int renegadeNumber = 1;
        private int outlawNumber = 2;
        private int deputyNumber = 0;

        List<PlayerModel.TypeOfPlayer> possiblePlayerTypes = new List<PlayerModel.TypeOfPlayer>();

        public void StartTheGame()
        {

            //if (!IsServer)
            //{
            //    return;
            //}

            CanStart = false;

            Debug.Log("we can start");

            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Sheriff);
            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Renegade);
            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Outlaw);
            possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Outlaw);

            switch (players.Length)
            {
                case 4: // might delete
                    break;
                case 5:
                    deputyNumber = 1;
                    possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Deputy);
                    break;
                case 6:
                    outlawNumber = 3;
                    deputyNumber = 1;
                    possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Outlaw);
                    break;
                case 7:
                    outlawNumber = 3;
                    deputyNumber = 2;
                    possiblePlayerTypes.Add(PlayerModel.TypeOfPlayer.Deputy);
                    break;
            }

            // assign roles to players
            foreach (var player in players)
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
                if (Input.GetKeyDown(KeyCode.B) && CanStart && IsServer)
                {
                    playerlist.Clear();
                    players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var p in players)
                    {
                        playerlist.Add(p.transform);
                    }
                    ScreenLog.Instance.SendEvent(TextType.Debug, $"player num: {players.Length}");
                    if (players.Length >= 4 && players.Length <= 7)
                    {
                        StartTheGame();
                    }
                }

                if (Input.GetKeyDown(KeyCode.N))
                {
                    if(_thisPlayer.GetComponent<PlayerModel>() != null)
                        ScreenLog.Instance.SendEvent(TextType.Debug, $"player id: {_thisPlayer.GetComponent<PlayerModel>().PlayerID}");
                        ScreenLog.Instance.SendEvent(TextType.Debug, $"player type: {_thisPlayer.GetComponent<PlayerModel>().PlayerRole}");
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
