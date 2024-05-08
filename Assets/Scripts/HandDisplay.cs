using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public GameObject card;
    public void AddCard()
    {
        GameObject instantiatedPrefab = Instantiate(card, transform.position, Quaternion.identity, transform);

        instantiatedPrefab.transform.localPosition = Vector3.zero;
        instantiatedPrefab.transform.localRotation = Quaternion.identity;
    }

    public void handCardOnChange(GameObject thisPlayer)
    {
        GameObject handPanel = GameObject.Find("HandPanel");
        var hand = thisPlayer.GetComponent<PlayerModel>().openHand;
        foreach (var c in hand)
        {
            Debug.Log("foreach");
            c.transform.SetParent(handPanel.transform);
        }
    }
}
