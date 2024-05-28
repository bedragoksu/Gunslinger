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
    public GunModel gun; // can change
    public CharacterModel character;
    public List<GameObject> openHand;
    public List<GameObject> stackHand;
    public int position;

    public bool IsAlive = true;

    public bool clicked = false;

    [SyncVar] public int magicNum = 0;
    public void ChangeMagicNum(int num) { 
        magicNum = num; 
    }

    public enum TypeOfPlayer
    {
        Sheriff, // �erif
        Deputy, // Aynas�z
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

    private void Update()
    {
        var Players = GameObject.FindGameObjectsWithTag("Player");
        if (Input.GetKeyDown(KeyCode.N))
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player id: {_thisPlayer.PlayerID}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player type: {_thisPlayer.PlayerRole}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player name: {_thisPlayer.PlayerName}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"magic number: {_thisPlayer.magicNum}");

            ScreenLog.Instance.SendEvent(TextType.Debug, $"card order: {_thisPlayer.GetComponentInParent<CardManager>().CardOrder[0]}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"card order: {_thisPlayer.GetComponentInParent<CardManager>().Cards[0]}");
        }
        


    }



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

}
