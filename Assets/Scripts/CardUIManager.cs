using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardUIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Scale factors
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float scaleDuration = 0.2f;

    // Original scale
    private Vector3 originalScale;

    private void Start()
    {
        // Save the original scale
        originalScale = transform.localScale;
    }

    // When the pointer enters the card area
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up the card
        transform.DOScale(hoverScale, scaleDuration);
    }

    // When the pointer exits the card area
    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale back to original size
        transform.DOScale(originalScale, scaleDuration);
    }
}
