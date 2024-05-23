using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoControllerUI : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private GameObject[] _players;
    [SerializeField] private GameObject _playerInfoPrefab;
    private List<GameObject> _playersInfoStack = new List<GameObject>();
    private GameObject _thisPlayer;

    public void InitializeStackCanvas()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        _thisPlayer = _gameManager._thisPlayer;
        foreach (var player in _players)
        {
            if (player != _thisPlayer) {

                GameObject instantiatedObject = 
                    Instantiate(_playerInfoPrefab, _playerInfoPrefab.transform.position, _playerInfoPrefab.transform.rotation);
                
                instantiatedObject.transform.SetParent(gameObject.transform, false);

                GameObject nameText = instantiatedObject.transform.Find("Name/TextName").gameObject;
                PlayerModel playerModel = player.GetComponent<PlayerModel>();
                nameText.GetComponent<TextMeshProUGUI>().text = playerModel.PlayerName;

                changeNumberOfCards(instantiatedObject, player);
                _playersInfoStack.Add(instantiatedObject);
            }
        }
    }

    public void UpdateStackCanvas()
    {
        int count = 0;
        for (int i = 0; i < _players.Length; i++) { 
            var player = _players[i];
            if (player != _thisPlayer)
            {
                changeNumberOfCards(_playersInfoStack[count], player);
                count++;
            }
        }
    }

    private void changeNumberOfCards(GameObject stackObject, GameObject player)
    {
        GameObject numberOfCards = stackObject.transform.Find("Cards/NumberOfCards").gameObject;
        PlayerModel playerModel = player.GetComponent<PlayerModel>();
        numberOfCards.GetComponent<TextMeshProUGUI>().text = "x" + playerModel.openHand.Count.ToString();
    }
}
