using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterCanvasController : MonoBehaviour
{
    GameObject _mainCamera;
    
    [SerializeField] GameObject _worldSpaceCanvas;

    [SerializeField] TextMeshProUGUI textInstance;

    public Vector3 offset;
    //Start is called before the first frame update
    public void UpdateCanvas(GameObject[] players)
    {

        _mainCamera = GameObject.Find("Main Camera");
        Camera camera = _mainCamera.GetComponent<Camera>();
        _worldSpaceCanvas.GetComponent<Canvas>().worldCamera = camera;
        Transform mainCam = _mainCamera.transform;
        foreach (Transform child in _worldSpaceCanvas.transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
        foreach (GameObject player in players)
        {
            Transform _unit = player.transform;
            createInstance(player.GetComponent<PlayerModel>().PlayerName, _unit, mainCam);
        }

    }
    private void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            UpdateCanvas(players);
        }

    }

    private void createInstance(string text, Transform _unit, Transform mainCam)
    {
        // Create a copy of the original TextMeshPro text
        TextMeshProUGUI copiedText = Instantiate(textInstance, _worldSpaceCanvas.transform);

        // Set the parent of the copied text to the world space canvas
        copiedText.transform.SetParent(_worldSpaceCanvas.transform);

        // Set the text content
        copiedText.text = text;
        copiedText.color = Color.red;

        // Calculate the direction from the text to the camera
        Vector3 directionToCamera = -mainCam.position + copiedText.transform.position;

        // Calculate the rotation to face the camera
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);

        // Apply the rotation
        copiedText.transform.rotation = rotationToCamera;

        // Set the position with offset
        copiedText.transform.position = _unit.position + offset;
    }


}
