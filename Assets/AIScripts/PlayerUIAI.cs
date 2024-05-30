using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIAI : MonoBehaviour
{
    public string PlayerName { get; private set; }
    public TMP_InputField InputFieldPlayerName;

    public void SetPlayerName()
    {
        PlayerName = InputFieldPlayerName.text;
    }
}
