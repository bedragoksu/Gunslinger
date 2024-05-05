using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterCanvasController : MonoBehaviour
{
    Camera _mainCamera;
    
    [SerializeField] GameObject _worldSpaceCanvas;

    [SerializeField] TextMeshProUGUI textInstance;

    public Vector3 offset;
    // Start is called before the first frame update
    public void UpdateCanvas(GameObject[] players)
    {
        
         _mainCamera = Camera.main;
        
        if (_worldSpaceCanvas.GetComponent<Canvas>().worldCamera = _mainCamera) { }
        Transform mainCam = _mainCamera.transform;
        foreach (GameObject player in players)
        {
            Transform _unit = player.transform;
            creataInstance(player.GetComponent<PlayerModel>().PlayerName, _unit, mainCam);
        }

    }
    //private void Update()
    //{
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //    if (players.Length > 0)
    //    {
    //        _mainCamera = Camera.main;

    //        if (_worldSpaceCanvas.GetComponent<Canvas>().worldCamera = _mainCamera) { }
    //        Transform mainCam = _mainCamera.transform;
    //        foreach (GameObject player in players)
    //        {
    //            Transform _unit = player.transform;
    //            creataInstance(player.GetComponent<PlayerModel>().PlayerName, _unit, mainCam);
    //        }
    //    }
        
    //}

    private void creataInstance(string text, Transform _unit, Transform mainCam)
    {
        // Create a copy of the original TextMeshPro text
        TextMeshProUGUI copiedText = Instantiate(textInstance, _worldSpaceCanvas.transform);

        // Set the parent of the copied text to the world space canvas
        copiedText.transform.SetParent(_worldSpaceCanvas.transform);

        // Set the text content
        copiedText.text = text;
        copiedText.color = Color.red;

        // Calculate the direction from the text to the camera
        Vector3 directionToCamera = mainCam.position - copiedText.transform.position;

        // Calculate the rotation to face the camera
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);

        // Apply the rotation
        copiedText.transform.rotation = rotationToCamera;

        // Set the position with offset
        copiedText.transform.position = _unit.position + offset;
    }


}
