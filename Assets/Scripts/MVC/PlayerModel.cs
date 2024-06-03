using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
using NeptunDigital;
using Gunslinger.Controller;

public class PlayerModel : NetworkBehaviour
{
    public int PlayerID;
    public string PlayerName;
    public int CurrentBulletPoint = 4;
    [SyncVar] public TypeOfPlayer PlayerRole;

    public bool CanPlayMultipleBangs = false;
    public bool PlayedBang = false;

    public GunModel gun; // can change
    public CharacterModel character;
    public List<GameObject> openHand;
    public List<GameObject> stackHand;
    public int position;

    public bool IsAgent = false;
    public bool IsAlive = true;
    public int Range = 1;
    public bool hasGun = false;

    public bool clicked = false;

    [SyncVar] public int magicNum = 0;
    public void ChangeMagicNum(int num) { 
        magicNum = num; 
    }

    public enum TypeOfPlayer
    {
        Sheriff, // Þerif
        Deputy, // Aynasýz
        Outlaw, // Haydut
        Renegade, // Hain
        Bos
    }

    int counter;
    public override void OnStartClient()
    {
        base.OnStartClient();

        ScreenLog.Instance.SendEvent(TextType.Debug, $"onstartclienttt: {base.ObjectId}");
        counter= base.ObjectId;
        openHand = new List<GameObject>();


        if (base.IsOwner)
        {
            PlayerModel player = GetComponent<PlayerModel>();
            _thisPlayer = player;

            var pl = GameObject.Find("PlayerUIHelper");
            PlayerUI ui = pl.GetComponent<PlayerUI>();
            string playerName = ui.PlayerName;

            ScreenLog.Instance.SendEvent(TextType.Debug, $"name and id: {playerName} {counter}");

            AssignPlayerModelServer(player,
                counter,
                (playerName.Equals("")) ? $"Player {counter}" : playerName);

            
        }
        else
        {
            GetComponent<PlayerModel>().enabled = false;
        }
    }

    //public override void OnStopClient()
    //{
    //    base.OnStopClient();

    //    ScreenLog.Instance.SendEvent(TextType.Debug, $"IM GOING BYEEE: {PlayerName}");
    //    CloneThePlayer();
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void CloneThePlayer()
    //{
    //    clone();
    //}
    //[ObserversRpc]
    //public void clone()
    //{
    //    GameObject cloned = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
    //    ServerManager.Spawn(cloned);
    //}


    public void cardchange(bool b)
    {
        Debug.Log("CARD CHANGE");
        GameObject handPanel = GameObject.Find("HandPanel");
        GameObject deckPanel = GameObject.Find("DeckPanel");

        Transform handPanelTransform = handPanel.transform;
        Transform deckPanelTransform = deckPanel.transform;

        foreach (Transform c in handPanelTransform)
        {
            c.SetParent(deckPanelTransform);
            c.gameObject.SetActive(false);
        }

        if (b)
        {
            foreach (var c in openHand)
            {
                Debug.Log("foreach");
                c.gameObject.SetActive(true);
                c.transform.SetParent(handPanelTransform);
            }
        }
    }

    private PlayerModel _thisPlayer;


    [ServerRpc]
    public void AssignPlayerModelServer(PlayerModel player, int playerID, string name)
    {
        AssignPlayerModel(player, playerID, name);

    }

    [ObserversRpc]
    public void AssignPlayerModel(PlayerModel player, int playerID, string name)
    {
        player.PlayerID = playerID;
        player.PlayerName = name;
        player.PlayerRole = TypeOfPlayer.Bos;
    }

    [ServerRpc]
    public void AssignMagicNumServer(PlayerModel player, int magicNum)
    {
        AssignMagicNum(player, magicNum);
    }

    [ObserversRpc]
    public void AssignMagicNum(PlayerModel player, int magicNum)
    {
        player.ChangeMagicNum(magicNum);
    }


    public GameObject gameObj;
    public override void OnStopClient() // bedra turnsdeki doðru yere koy yeni agentý
    {
        base.OnStopClient();

        //GameObject g = Instantiate(gameObj, gameObject.transform.position, gameObject.transform.rotation);

        //g.GetComponent<PlayerModel>().PlayerName = "xxagentxx";
        //var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //gameManager._turns[PlayerID] = g;

        //ServerManager.Spawn(g);
        //g.SetActive(true);

        //sts();

        if (base.IsServer)
        {

            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.oldPlayers = GameObject.FindGameObjectsWithTag("Player");


            gameManager.g = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);

            gameManager.GetComponent<GameManager>().SomeoneDestroyed = true;

            //GameObject g = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
            //ServerManager.Spawn(g);
        }


    }

    //[ServerRpc (RequireOwnership = false)]
    //public void sts()
    //{
    //    st();
    //}
    //[ObserversRpc]
    //public void st()
    //{
    //    gameObject.SetActive(true);
    //    PlayerName = "xxx";
    //}

    //[ServerRpc(RequireOwnership =false)]
    //public void SpawnAgentServer(GameObject g)
    //{
    //    SpawnAgent(g);
    //}
    //[ObserversRpc]
    //public void SpawnAgent(GameObject g)
    //{
    //    g.GetComponent<PlayerModel>().PlayerName = "xxagentxx";
    //    var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    //    gameManager._turns[PlayerID] = g;
    //}

}
