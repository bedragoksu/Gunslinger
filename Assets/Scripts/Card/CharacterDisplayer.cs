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

    void Start()
    {
        for (int i = 0; i < character.maxBullet; i++)
        {
            // Instantiate the prefab as a child of the current object
            GameObject instantiatedPrefab = Instantiate(bullet, transform.position, Quaternion.identity, transform);

            // Optionally, you can adjust the instantiated prefab's position and rotation relative to the parent
            // You can modify these adjustments as needed based on your requirements
            instantiatedPrefab.transform.localPosition = Vector3.zero;
            instantiatedPrefab.transform.localRotation = Quaternion.identity;
        }

        characterImage.sprite = character.characterImage;
        identify.text = character.identify;
        gunImage.sprite = character.gunImage;
        range.text = character.range.ToString();
    }

   
}
