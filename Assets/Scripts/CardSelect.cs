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
    [HideInInspector] public PlayerModel _thisPlayerModel; //private bedra

    private GameObject _target;
    private int _distanceBetweenTarget = -1;
    private int _circularDistanceBetweenTarget = -1;

    private Camera _camera;

    public bool MovedOn = false;

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
                        if (hit.collider.gameObject != _thisPlayerObject)
                        {
                            if (hit.collider.gameObject.GetComponent<PlayerModel>().IsAlive)
                            {
                                _target = hit.collider.gameObject;
                                _distanceBetweenTarget = _actions.CalculateDistance(_thisPlayerObject, _target);
                                _circularDistanceBetweenTarget = _actions.GetCircularDistance(_thisPlayerObject, _target);
                            }

                        }
                    }
                }
            }
        }

        //if(_gameManager.GetTurnInt() == _thisPlayerModel.PlayerID && !_thisPlayerModel.clicked)
        //{
        //    this.GetComponent<Button>().interactable = true;
        //    _target = null;
        //    _distanceBetweenTarget = -1;
        //}
        //else
        //{
        //    this.GetComponent<Button>().interactable = false;
        //    if(_gameManager.GetTurnInt() != _thisPlayerModel.PlayerID)
        //    {
        //        _thisPlayerModel.clicked = false;
        //    }
        //}

    }
    public void OnClick() // discard islemlerini burada yapabilirsin
    {
        if (_gameManager._thisPlayer != _gameManager._turns[_gameManager.GetTurnInt()]) return;
        
        MovedOn = true;
        var cardName = this.GetComponent<CardDisplayer>().nameText.text;
        _camera = Camera.main;
        //Debug.Log(cardName);
        _thisPlayerModel.clicked = true;
        int index = 0;
        _target = null;
        _distanceBetweenTarget = -1;
        _circularDistanceBetweenTarget = -1;

        if (_gameManager.CurrentGameState == GameManager.GameState.PlayCard)
        {
            switch (cardName)
            {
                case "Bang":
                    Debug.Log("BANG TIKLANDIIIII");
                    if (_actions.CanHitAnyone(_thisPlayerObject).Count != 0)
                    {
                        if (_thisPlayerModel.CanPlayMultipleBangs || !_thisPlayerModel.PlayedBang)
                        {
                            _thisPlayerModel.PlayedBang = true;
                            StartCoroutine("BangRoutine", this.gameObject);
                        }
                        else
                        {
                            _gameManager.ChangeAlert("Already fired a Bang!");
                        }
                    }
                    else
                    {
                        _gameManager.ChangeAlert(GameObject.Find("AlertTextManager").GetComponent<AlertTextManager>().GetBangNobodyIsInRangeText());
                    }
                    break;
                //case "Missed":
                //    Debug.Log("MISSED TIKLANDII");
                //    break;
                case "Saloon":
                    Debug.Log("KAHVEHNE TIKLANDIII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.SaloonAction();
                    _gameManager._cardClickUI.makeAllOfTheCardsWhite();
                    _actions.DiscardCard(_thisPlayerObject, index);
                    break;
                case "Beer":
                    Debug.Log("BEER KARTI TIKLANDI");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.BeerAction(_thisPlayerModel);
                    _gameManager._cardClickUI.makeAllOfTheCardsWhite();
                    _actions.DiscardCard(_thisPlayerObject, index);
                    break;
                case "Wells Fargo":
                    Debug.Log("WELLS FARGO TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.WellsFargoAction(_thisPlayerModel, index);
                    break;
                case "Stage Coach":
                    Debug.Log("Stage Coach TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.StagecoachAction(_thisPlayerModel, index);
                    break;
                case "Gatling":
                    Debug.Log("GATLING TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.GatlingAction(_thisPlayerObject);
                    _actions.DiscardCard(_thisPlayerObject, index);
                    break;
                case "Mustang":
                    Debug.Log("MUSTANG TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Mustang");
                    break;
                case "Scope":
                    Debug.Log("SCOPE TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Scope");
                    break;
                case "Barrel":
                    Debug.Log("BARREL TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Barrel");
                    break;
                case "Cat Balou": // herhangi bir oyuncunun open handi
                    Debug.Log("EMRÝVAKÝ TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    StartCoroutine("CatBalouRoutine", index);
                    break;
                case "Panic": // 1 mesafedeki oyuncunun open handi
                    Debug.Log("PANÝK TIKLANDII");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    StartCoroutine("PanicRoutine", index);
                    break;
                case "Volcanic":
                    _actions.AddingGunToPlayer(_thisPlayerObject, 1, this.gameObject, true, "Volcanic");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Volcanic");
                    break;
                case "Remington":
                    _actions.AddingGunToPlayer(_thisPlayerObject, 3, this.gameObject, false, "Remington");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Remington");
                    break;
                case "Rev. Carabine":
                    _actions.AddingGunToPlayer(_thisPlayerObject, 4, this.gameObject, false, "Rev. Carabine");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Rev. Carabine");
                    break;
                case "Schofield":
                    _actions.AddingGunToPlayer(_thisPlayerObject, 2, this.gameObject, false, "Schofield");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Schofield");
                    break;
                case "Winchester":
                    _actions.AddingGunToPlayer(_thisPlayerObject, 5, this.gameObject, false, "Winchester");
                    index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
                    _actions.MustangAction(_thisPlayerObject, index, "Winchester");
                    break;

            }
        }
        if (_gameManager.CurrentGameState == GameManager.GameState.DiscardCard)
        {
            _gameManager.ActivateNextButton();
            index = FindIndexInOpenHand(_thisPlayerModel, this.gameObject);
            _actions.DiscardCard(_thisPlayerObject, index);
            _gameManager._cardClickUI.makeAllOfTheCardsWhite();
        }
        //_thisPlayerModel.clicked = false;

        
    }

    // kimseye vuramýyorsak??


    private IEnumerator PanicRoutine(int index)
    {
        _target = null;
        _distanceBetweenTarget = -1;
        _circularDistanceBetweenTarget = -1;

        _gameManager.ChangeAlert(GameObject.Find("AlertTextManager").GetComponent<AlertTextManager>().GetPanicRangeText());


        var before = _gameManager.IsActiveButton();
        _gameManager.OpenCloseDiscardButton(false);
        yield return new WaitUntil(() => _target != null);
        yield return new WaitUntil(() => _circularDistanceBetweenTarget == 1);
        _gameManager.OpenCloseDiscardButton(before);

        var name = _actions.PanicAction(_thisPlayerObject, index, _target);

        yield return new WaitUntil(() => name);
        yield return new WaitForSeconds(1f);

        _target = null;
        _distanceBetweenTarget = -1;
        _circularDistanceBetweenTarget = -1;

    }

    private IEnumerator CatBalouRoutine(int index)
    {
        _target = null;
        _distanceBetweenTarget = -1;
        _circularDistanceBetweenTarget = -1;
        var before = _gameManager.IsActiveButton();

        _gameManager.OpenCloseDiscardButton(false);
        yield return new WaitUntil(() => _target != null);
        _gameManager.OpenCloseDiscardButton(before);

        _actions.CatBalouAction(_target);
        yield return new WaitForSeconds(0.3f);
        _actions.DiscardCard(_thisPlayerObject, index);
        _target = null;
        _distanceBetweenTarget = -1;
    }

    private IEnumerator BangRoutine(GameObject card)
    {
        _target = null;
        _distanceBetweenTarget = -1;
        _circularDistanceBetweenTarget = -1;
        var before = _gameManager.IsActiveButton();
        _gameManager.OpenCloseDiscardButton(false);
        yield return new WaitUntil(() => _target != null);
        yield return new WaitUntil(() => _actions.CalculateScopeCanHit(_thisPlayerObject, _target, true));
        _gameManager.OpenCloseDiscardButton(before);
        Debug.Log($"bang to: {_target.name}");

        int index = FindIndexInOpenHand(_thisPlayerModel, card); // check to // bedra
        // slm ben züb, ayçayla bilgisayarýnýzý çaldýk. çok eðleniyoruz. pc böyle mi býrakýlýr ajsfasjfgasakj
        // ehehe salak bilgisayarýnýn þifresi yok???
        //artýk gelinbizi polise þikayet edecekler

        _actions.BangAction(_thisPlayerObject, _target, false);
        yield return new WaitForSeconds(0.3f);
        _actions.DiscardCard(_thisPlayerObject, index);
        _target = null;
        _distanceBetweenTarget = -1;
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
