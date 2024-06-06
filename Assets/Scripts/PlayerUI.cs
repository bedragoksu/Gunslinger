using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public string PlayerName { get; private set; }
    public TMP_InputField InputFieldPlayerName;
    [SerializeField] public TMP_Text LobbyText;

    public void SetPlayerName()
    {
        PlayerName = InputFieldPlayerName.text;
        LobbyText.enabled = true;
    }
}
