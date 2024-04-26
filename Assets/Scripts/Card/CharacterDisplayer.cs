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
    public TextMeshProUGUI range;
    public Character character;
    public GameObject bullet;
    public int bullets;
    public GameObject player;
    public string role;

    void Start()
    {
        for (int i = 0; i < bullets; i++)
        {
            // Instantiate the prefab as a child of the current object
            GameObject instantiatedPrefab = Instantiate(bullet, transform.position, Quaternion.identity, transform);

            // Optionally, you can adjust the instantiated prefab's position and rotation relative to the parent
            // You can modify these adjustments as needed based on your requirements
            instantiatedPrefab.transform.localPosition = Vector3.zero;
            instantiatedPrefab.transform.localRotation = Quaternion.identity;
        }

        characterImage.sprite = Resources.Load<Sprite>("cowboy");
        gunImage.sprite = character.gunImage;
        range.text = character.range.ToString();
    }
    //public override void OnStartClient()
    //{
    //    base.OnStartClient(); 
    //    if (!base.IsOwner)
    //    {
    //        //GetComponent<CharacterDisplayer>().enabled = false;
    //    }

    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //    foreach (GameObject p in players)
    //    {
    //        if (p.GetComponent<PlayerModel>().enabled)
    //        {
    //            player = p;
    //        }
    //    }
    //}
    public void addBullet()
    {
        GameObject instantiatedPrefab = Instantiate(bullet, transform.position, Quaternion.identity, transform);

        instantiatedPrefab.transform.localPosition = Vector3.zero;
        instantiatedPrefab.transform.localRotation = Quaternion.identity;
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                addBullet();
            }
        }
        if(GameObject.Find("GameManager") != null && GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState == GameManager.GameState.Initialization)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerModel>().enabled)
                {
                    role = player.GetComponent<PlayerModel>().PlayerRole.ToString();
                    identify.text = player.GetComponent<PlayerModel>().PlayerRole.ToString();
                }
            }

        }
    }
}
