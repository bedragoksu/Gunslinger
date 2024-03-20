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
    public int CurrentBulletPoint;
    [SyncVar] public TypeOfPlayer PlayerRole;
    public bool CanPlayMultipleBangs = false;
    public GunModel gun; // can change
    public CharacterModel character;
    public List<CardModel> openHand = new List<CardModel>();
    public int position;


    public enum TypeOfPlayer
    {
        Sheriff, // Þerif
        Deputy, // Aynasýz
        Outlaw, // Haydut
        Renegade, // Hain
        Bos
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        ScreenLog.Instance.SendEvent(TextType.Debug, $"onstartclienttt: {base.ObjectId}");

        if (base.IsOwner)
        {
            PlayerModel player = GetComponent<PlayerModel>();

            //player.PlayerID = Players.Length - 1;
            //player.PlayerRole = PlayerModel.TypeOfPlayer.Bos;

            PlayerUI ui = GameObject.Find("PlayerUIHelper").GetComponent<PlayerUI>();
            string playerName = ui.PlayerName;
            //player.PlayerName = (playerName.Equals("")) ? $"Player {player.PlayerID + 1}" : playerName;
            int id = GameObject.Find("RoleManager").GetComponent<PlayerRolesController>().Players.Length;

            AssignPlayerModelServer(GetComponent<PlayerModel>(),
                id,
                (playerName.Equals("")) ? $"Player {id + 1}" : playerName);

            //ScreenLog.Instance.SendEvent(TextType.Debug, $"helo {player.PlayerID}");
            //ScreenLog.Instance.SendEvent(TextType.Debug, $"player num: {Players.Length}");
        }
        else
        {
            GetComponent<PlayerModel>().enabled = false;
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
        player.PlayerRole = PlayerModel.TypeOfPlayer.Bos;
    }

}
