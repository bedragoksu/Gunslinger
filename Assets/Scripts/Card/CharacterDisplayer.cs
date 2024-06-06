using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using NeptunDigital;
using static GameManager;
using System;

public class CharacterDisplayer : MonoBehaviour
{
    public Image characterImage;
    public TextMeshProUGUI identify;
    public Image gunImage;
    public Image handImage;
    public TextMeshProUGUI range;
    public Image rangeImage;
    public GameObject bullet;
    private PlayerModel pl;
    public GameManager gameManager;

    private void addBullet(int num)
    {


        int count = 0;
        foreach(Transform child in transform)
        {
            if(!child.gameObject.activeSelf)
            {
                count++;
                child.gameObject.SetActive(true);
            }
            if (count == num) break;
        }

    }
    private void removeBullet(int num)
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                count++;
                child.gameObject.SetActive(false);
            }
            if (count == num) break;
        }
    }

    public void roleOnChange()
    {
        pl = new PlayerModel();
        GameObject[] pls = null;
        if (gameManager._turns.Length != 0) pls = gameManager._turns;
        else pls = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in pls)
        {
            if (p.GetComponent<PlayerModel>().enabled)
            {
                pl = p.GetComponent<PlayerModel>();
            }
        }
        var t = pl.PlayerRole.ToString();
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
        for (int i = 0; i < pl.CurrentBulletPoint; i++)
            CreateBullet();


        b = true;

    }
    bool b = false;
    private void CreateBullet()
    {
        GameObject instantiatedPrefab = Instantiate(bullet, transform.position, Quaternion.identity, transform);

        instantiatedPrefab.transform.localPosition = Vector3.zero;
        instantiatedPrefab.transform.localRotation = Quaternion.identity;
    }

    public void UpdateBullets()
    {
        var numberOfChildren = getActiveChildren();
        if (numberOfChildren == pl.CurrentBulletPoint ) {
            return;
        }
        else if (numberOfChildren < pl.CurrentBulletPoint)
        {
            addBullet(pl.CurrentBulletPoint - numberOfChildren);
        }
        else if (numberOfChildren > pl.CurrentBulletPoint)
        {
            removeBullet(numberOfChildren - pl.CurrentBulletPoint);
        }
        
    }

    public void UpdateRangeText(int rang)
    {
        range.text = rang.ToString();
    } 
    private int getActiveChildren()
    {
        int num = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                num++;
        }
        return num;
    }
    private void Update()
    {
        if(b) UpdateBullets();
    }
}
