using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NeptunDigital;
using UnityEngine.UI;
using FishNet.Object;

public class ButtonController : MonoBehaviour
{
    //[SerializeField] private GameManager _gameManager;

    //// butonla alakali kisimlar burada buna script ac.!
    //public void DiscardUIButton()
    //{
    //    ScreenLog.Instance.SendEvent(TextType.Debug, "discard ui button");
    //    _gameManager.AssignStateServer(GameManager.GameState.DiscardCard);
    //}
    //public void NextUIButton()
    //{
    //    ScreenLog.Instance.SendEvent(TextType.Debug, "next ui button");
    //    StartCoroutine(r());
    //    _gameManager.AssignStateServer( GameManager.GameState.DrawCard);
    //}
    //public void Activate(Button button)
    //{
    //    button.interactable = true;
    //}

    //private bool b = false;
    //private IEnumerator r()
    //{
    //    nextplayer();
    //    yield return new WaitUntil(() => b);
    //    yield return new WaitForSeconds(0.5f);
    //    b = false;
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void nextplayer()
    //{
    //    server();
    //    b = true;
    //}
    //[ObserversRpc]
    //public void server()
    //{
    //    var i = _gameManager.GetTurnInt();
    //    i++;
    //    i %= 4;
    //    ScreenLog.Instance.SendEvent(TextType.Debug, $"_turnInt= {i}");
    //    _gameManager.AssignTurnIndex(i);
    //}
}
