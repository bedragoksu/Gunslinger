using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelect : MonoBehaviour
{
    private Actions _actions;
    private GameManager _gameManager;

    private GameObject _thisPlayerObject;
    [HideInInspector]public PlayerModel _thisPlayerModel; //private bedra

    private GameObject _target;

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
                    if (hit.collider.gameObject.GetComponent<PlayerModel>())
                    {
                        _target = hit.collider.gameObject;
                    }
                }
            }
        }

        if(_gameManager.GetTurnInt() == _thisPlayerModel.PlayerID && !_thisPlayerModel.clicked)
        {
            this.GetComponent<Button>().interactable = true;
            _target = null;
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
    public void OnClick() // discard islemlerini burada yapabilirsin
    {
        var cardName = this.GetComponent<CardDisplayer>().nameText.text;
        _camera = Camera.main;
        //Debug.Log(cardName);
        _thisPlayerModel.clicked = true;
        int index = 0;

        if(_gameManager.CurrentGameState == GameManager.GameState.PlayCard)
        {
            switch (cardName)
            {
                case "Bang":
                    Debug.Log("BANG TIKLANDIIIII");
                    StartCoroutine("BangRoutine", this.gameObject);
                    break;
                case "Missed":
                    Debug.Log("MISSED TIKLANDII");
                    break;
                case "Saloon":
                    Debug.Log("KAHVEHNE TIKLANDIII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.SaloonAction(_thisPlayerObject, index);
                    break;
                case "Beer":
                    Debug.Log("BEER KARTI TIKLANDI");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.BeerAction(_thisPlayerModel, index);
                    break;
                case "Wells Fargo":
                    Debug.Log("WELLS FARGO TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.WellsFargoAction(_thisPlayerModel, index);
                    break;
                case "Stage coach":
                    Debug.Log("WELLS FARGO TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.StagecoachAction(_thisPlayerModel, index);
                    break;
                case "Gatling":
                    Debug.Log("WELLS FARGO TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.GatlingAction(_thisPlayerObject, index);
                    break;
                case "Mustang":
                    Debug.Log("MUSTANG TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index);
                    break;
                case "Scope":
                    Debug.Log("SCOPE TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index);
                    break;
                case "Barrel":
                    Debug.Log("BARREL TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index);
                    break; 
                case "Cat Balou": // herhangi bir oyuncunun open handi
                    Debug.Log("EMR�VAK� TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    StartCoroutine("CatBalouRoutine", index);
                    break;
                case "Panic": // 1 mesafedeki oyuncunun open handi
                    Debug.Log("PAN�K TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.PanicAction(_thisPlayerObject, index);
                    break;

            }
        }
        if (_gameManager.CurrentGameState == GameManager.GameState.DiscardCard)
        {
            _gameManager.ActivateNextButton();
            index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
            _actions.DiscardCard(_thisPlayerObject, index);
        }

    }

    private IEnumerator CatBalouRoutine(int index)
    {
        yield return new WaitUntil(() => _target != null);
        _actions.CatBalouAction(_thisPlayerObject, index, _target);
        yield return new WaitForSeconds(0.3f);
        _actions.DiscardCard(_thisPlayerObject, index);
        _target = null;
    }

    private IEnumerator BangRoutine(GameObject card)
    {
        yield return new WaitUntil(() => _target != null);
        Debug.Log($"bang to: {_target.name}");

        int index = FindIndexInOpenHand(_thisPlayerModel, card); // check to // bedra
        // slm ben z�b, ay�ayla bilgisayar�n�z� �ald�k. �ok e�leniyoruz. pc b�yle mi b�rak�l�r ajsfasjfgasakj
        // ehehe salak bilgisayar�n�n �ifresi yok???
        //art�k gelinbizi polise �ikayet edecekler
        
        _actions.BangAction(_thisPlayerObject,_target, index);
        yield return new WaitForSeconds(0.3f);
        _actions.DiscardCard(_thisPlayerObject, index);
        _target = null;
    }

    private int FindIndexInOpenHand(PlayerModel player, GameObject card)
    {
        var hand = player.openHand;
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] == card)
            {
                return i;
            }
        }
        return 0;
    }

}
