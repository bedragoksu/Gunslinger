using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDisplayer : MonoBehaviour
{
    public Image characterImage;
    public TextMeshProUGUI identify;
    public Image gunImage;
    public Image handImage;
    public TextMeshProUGUI range;
    public Image rangeImage;
    public GameObject bullet;
    public int bullets;
    void Awake()
    {
        bullets = 4;
    }
    public void addBullet()
    {
        GameObject instantiatedPrefab = Instantiate(bullet, transform.position, Quaternion.identity, transform);

        instantiatedPrefab.transform.localPosition = Vector3.zero;
        instantiatedPrefab.transform.localRotation = Quaternion.identity;
    }

    public void roleOnChange()
    {
        PlayerModel pl = new PlayerModel();
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<PlayerModel>().enabled)
            {
                pl = p.GetComponent<PlayerModel>();
            }
        }
        var t = pl.PlayerRole.ToString();
        if (t == "Sheriff")
            bullets++;
        identify.enabled = true;
        identify.text = t;
        characterImage.enabled = true;
        characterImage.sprite = Resources.Load<Sprite>(t);
        gunImage.enabled = true;
        gunImage.sprite = Resources.Load<Sprite>("Revolver");
        handImage.enabled = true;
        rangeImage.enabled = true;
        range.enabled = true;
        range.text = "1";
        for (int i = 0; i < bullets; i++)
            addBullet();
    }
}
