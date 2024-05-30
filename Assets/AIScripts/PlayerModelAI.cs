using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
using NeptunDigital;
using Gunslinger.Controller;

public class PlayerModelAI : NetworkBehaviour
{
    public int PlayerID;
    public string PlayerName;
    public int CurrentBulletPoint = 4;
    [SyncVar] public TypeOfPlayer PlayerRole;

    public bool CanPlayMultipleBangs = false;
    public bool PlayedBang = false;

    public GunModel gun; // can change
    public List<GameObject> openHand;
    public List<GameObject> stackHand;

    public bool IsAlive = true;
    public int Range = 1;
    public bool hasGun = false;

    public bool clicked = false;


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
            PlayerModelAI player = GetComponent<PlayerModelAI>();
            _thisPlayer = player;

            var pl = GameObject.Find("PlayerUIHelper");
            PlayerUI ui = pl.GetComponent<PlayerUI>();
            string playerName = ui.PlayerName;

            ScreenLog.Instance.SendEvent(TextType.Debug, $"name and id: {playerName} {counter}");

            AssignPlayerModelAIServer(player,
                counter,
                (playerName.Equals("")) ? $"Player {counter}" : playerName);

            
        }
        else
        {
            GetComponent<PlayerModelAI>().enabled = false;
        }


    }

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

    private PlayerModelAI _thisPlayer;

    private void Update()
    {
        var Players = GameObject.FindGameObjectsWithTag("Player");
        if (Input.GetKeyDown(KeyCode.N))
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player id: {_thisPlayer.PlayerID}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player type: {_thisPlayer.PlayerRole}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player name: {_thisPlayer.PlayerName}");

            ScreenLog.Instance.SendEvent(TextType.Debug, $"card order: {_thisPlayer.GetComponentInParent<CardManager>().CardOrder[0]}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"card order: {_thisPlayer.GetComponentInParent<CardManager>().Cards[0]}");
        }
        


    }



    [ServerRpc]
    public void AssignPlayerModelAIServer(PlayerModelAI player, int playerID, string name)
    {
        AssignPlayerModelAI(player, playerID, name);

    }

    [ObserversRpc]
    public void AssignPlayerModelAI(PlayerModelAI player, int playerID, string name)
    {
        player.PlayerID = playerID;
        player.PlayerName = name;
        player.PlayerRole = PlayerModelAI.TypeOfPlayer.Bos;
    }

    

}
