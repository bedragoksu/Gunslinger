using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PlayerRolesController : NetworkBehaviour
{
    public List<Transform> playerlist { get; private set; }
    public GameObject[] players { get; private set; }
    bool CanStart = true;

    void Start()
	{
		playerlist = new List<Transform>();
		players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject p in players)
		{
			playerlist.Add(p.transform);
		}
	}

    //public override void OnStartClient()
    //   {
    //       base.OnStartClient();
    //       Debug.Log("in server connect");
    //	playerlist.Clear();
    //	players = GameObject.FindGameObjectsWithTag("Player");
    //	foreach (GameObject p in players)
    //	{
    //		playerlist.Add(p.transform);
    //	}
    //}


    private int sheriffNumber = 1;
    private int renegadeNumber = 1;
    private int outlawNumber = 2;
    private int deputyNumber = 0;

    List<PlayerModel.TypeOfPlayer> possiblePlayerTypes = new List<PlayerModel.TypeOfPlayer>();

    public void StartTheGame()
    {
		if(players.Length>=4 && players.Length <= 7 && CanStart)
        {
			Debug.Log("we can start");
            CanStart = false;

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
                var randomint = Random.Range(0,possiblePlayerTypes.Count);
                var type = possiblePlayerTypes[randomint];
                possiblePlayerTypes.RemoveAt(randomint);
                player.GetComponent<PlayerAnimationController>().PlayerType = type;
            }
        }
    }

    void Update()
    {
        
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                playerlist.Clear();
                players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject p in players)
                {
                    playerlist.Add(p.transform);
                }
                Debug.Log($"player num: {players.Length}");
                StartTheGame();
            }
        }
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
