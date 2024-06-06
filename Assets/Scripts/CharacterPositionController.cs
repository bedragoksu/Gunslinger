using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPositionController : MonoBehaviour
{
    [SerializeField] CharacterCanvasController _characterCanvasController;
    public GameObject[] players;
    public GameManager gameManager;
    public void UpdateCharacterPositions()
    {
        GameObject[] players = null;
        if (gameManager._turns.Length != 0) players = gameManager._turns;
        else players = GameObject.FindGameObjectsWithTag("Player");
        Transform[] corners = CalculatePolygonCorners(Vector3.zero, players.Length, 5f);
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = corners[i].position;
            Debug.Log(players[i].GetComponent<PlayerModel>().PlayerName);
            Vector3 direction = (Vector3.zero - players[i].transform.position).normalized;

            Quaternion rotation = Quaternion.LookRotation(direction);

            players[i].transform.rotation = rotation;
        }        
        _characterCanvasController.UpdateCanvas();
    }



    Transform[] CalculatePolygonCorners(Vector3 origin, int numberOfEdges, float radius)
    {
        Transform[] corners = new Transform[numberOfEdges];

        float angleIncrement = 360f / numberOfEdges;
        for (int i = 0; i < numberOfEdges; i++)
        {
            float angle = i * angleIncrement;
            float x = origin.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = origin.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            GameObject cornerObject = new GameObject("SpawnPoint_" + i);
            cornerObject.transform.position = new Vector3(x, -0.2f, z);
            corners[i] = cornerObject.transform;
        }

        return corners;
    }
}
