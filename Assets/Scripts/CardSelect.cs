using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelect : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log(this.GetComponent<CardDisplayer>().nameText.text);
    }
}
