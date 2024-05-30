using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterCanvasControllerAI : MonoBehaviour
{
    GameObject _mainCamera;
    
    [SerializeField] GameObject _worldSpaceCanvas;

    [SerializeField] TextMeshProUGUI textInstance;

    [SerializeField] GameObject worldSpaceCanvasInstance;

    public Vector3 offset;
    //Start is called before the first frame update
    public void UpdateCanvas()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
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
            createInstance(player.GetComponent<PlayerModel>(), _unit, mainCam);
        }

    }

    private void createInstance(PlayerModel pl, Transform _unit, Transform mainCam)
    {
        string text = pl.PlayerName;
        // Create a copy of the original TextMeshPro text
        GameObject copiedObject = Instantiate(worldSpaceCanvasInstance, _worldSpaceCanvas.transform);

        // Set the parent of the copied text to the world space canvas
        copiedObject.transform.SetParent(_worldSpaceCanvas.transform);
        string role = "";
        if (pl.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff)
        {
            role = "Sheriff";
        } else if (pl.CurrentBulletPoint == 0)
        {
            role += $"{pl.PlayerRole}";
        }
        copiedObject.transform.Find("Name/TextName").gameObject.GetComponent<TextMeshProUGUI>().text = text;
        copiedObject.transform.Find("Role/RoleText").gameObject.GetComponent<TextMeshProUGUI>().text = role;
        copiedObject.transform.Find("Bullet/NumberOfBullets").gameObject.GetComponent<TextMeshProUGUI>().text = "x" + pl.CurrentBulletPoint.ToString();

        // Set the text content
        //copiedText.text = text;
        //copiedText.color = Color.red;

        // Calculate the direction from the text to the camera
        Vector3 directionToCamera = -mainCam.position + copiedObject.transform.position;

        // Calculate the rotation to face the camera
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);

        // Apply the rotation
        copiedObject.transform.rotation = rotationToCamera;

        // Set the position with offset
        copiedObject.transform.position = _unit.position + offset;
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UpdateCanvas();
            }

        }
        
    }

}
