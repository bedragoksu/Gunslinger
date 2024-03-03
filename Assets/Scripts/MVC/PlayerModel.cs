using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public string PlayerName;
    public int CurrentBulletPoint;
    public TypeOfPlayer PlayerRole;
    public bool CanPlayMultipleBangs = false;
    public GunModel gun; // can change
    public CharacterModel character;
    


    public enum TypeOfPlayer
    {
        Sheriff, // �erif
        Deputy, // Aynas�z
        Outlaw, // Haydut
        Renegade, // Hain
    }
}
