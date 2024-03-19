using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

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
}
