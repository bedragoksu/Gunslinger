using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelect : MonoBehaviour
{
    private Actions _actions;
    private GameManager _gameManager;

    private GameObject _thisPlayerObject;
    private PlayerModel _thisPlayerModel;

    private Camera _camera;


    private void Start()
    {
        _actions = GameObject.Find("ActionController").GetComponent<Actions>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        var pls = GameObject.FindGameObjectsWithTag("Player");
        foreach (var pl in pls)
        {
            if (pl.GetComponent<PlayerModel>().enabled)
            {
                _thisPlayerObject = pl;
            }
        }
        _thisPlayerModel = _thisPlayerObject.GetComponent<PlayerModel>();

        // _camera = Camera.main; // duzelt
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // bang icin lazim
        {
            if (_camera)
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = _camera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
        }

        if(_gameManager.GetTurnInt() == _thisPlayerModel.PlayerID && !_thisPlayerModel.clicked)
        {
            this.GetComponent<Button>().interactable = true;
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
            if(_gameManager.GetTurnInt() != _thisPlayerModel.PlayerID)
            {
                _thisPlayerModel.clicked = false;
            }
        }

    }
    public void OnClick()
    {
        var cardName = this.GetComponent<CardDisplayer>().nameText.text;
        _camera = Camera.main;
        //Debug.Log(cardName);
        _thisPlayerModel.clicked = true;

        switch (cardName)
        {
            case "Bang":
                Debug.Log("TIKLANDIIIII");
                break;
        }


    }
}
