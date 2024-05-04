using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public GameObject card;
    public GameObject handPanel;
    public void addCard()
    {
        GameObject instantiatedPrefab = Instantiate(card, transform.position, Quaternion.identity, transform);

        instantiatedPrefab.transform.localPosition = Vector3.zero;
        instantiatedPrefab.transform.localRotation = Quaternion.identity;
    }

    public void handCardOnChange()
    {
        PlayerModel pl = new PlayerModel();
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<PlayerModel>().enabled)
            {
                pl = p.GetComponent<PlayerModel>();
            }
        }
        GameObject handPanel = GameObject.Find("HandPanel");
        foreach (var c in pl.openHand)
        {
            c.transform.SetParent(handPanel.transform);
        }
    }
}
