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

    public int magicNum = 0;


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

    private PlayerModel _thisPlayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player id: {_thisPlayer.PlayerID}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player type: {_thisPlayer.PlayerRole}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"player name: {_thisPlayer.PlayerName}");
            ScreenLog.Instance.SendEvent(TextType.Debug, $"magic number: {_thisPlayer.magicNum}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            var prc = GameObject.Find("RoleController").GetComponent<PlayerRolesController>();
            foreach(var player in prc.Players)
            {
                
                Assign5AsMagicNumServer(player.GetComponent<PlayerModel>(), 5);
            }
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

    [ServerRpc]
    public void Assign5AsMagicNumServer(PlayerModel player, int magicNum)
    {
        Assign5AsMagicNum(player, magicNum);

    }

    [ObserversRpc]
    public void Assign5AsMagicNum(PlayerModel player, int magicNum)
    {
        player.magicNum = magicNum;
    }

}
