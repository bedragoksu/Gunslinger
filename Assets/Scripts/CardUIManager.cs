using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using DG.Tweening;

public class CardUIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Scale factors
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float duration = 0.2f;
    public bool isSelected = false;

    private GameObject background;
    private GameObject picture;

    // Original scale
    private Vector3 originalScale;

    private void Start()
    {
        // Save the original scale
        originalScale = transform.localScale;

        background = transform.Find("Background").gameObject;
        picture = transform.Find("Picture").gameObject;
    }

    // When the pointer enters the card area
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            ToHoverScale();
        }

    }

    // When the pointer exits the card area
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            ToOriginalScale();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelected) { 
        GameObject parent = transform.parent.gameObject;
        if (parent != null && parent.name == "HandPanel")
        {
            parent.GetComponent<CardClickUI>().OnSelectCard(gameObject);
        }
        }
    }

        public void ToHoverScale()
    {
        transform.DOScale(hoverScale, duration);
    }

    public void ToOriginalScale()
    {
        transform.DOScale(originalScale, duration);
    }

    public void ToGray()
    {
        Image pictureImage = picture.GetComponent<Image>();
        Image backgroundImage = background.GetComponent<Image>();

        if (pictureImage != null)
        {
            pictureImage.DOColor(Color.gray, duration);
        }

        if (backgroundImage != null)
        {
            backgroundImage.DOColor(Color.gray, duration);
        }
    }

    public void ToWhite()
    {
        Image pictureImage = picture.GetComponent<Image>();
        Image backgroundImage = background.GetComponent<Image>();

        if (pictureImage != null)
        {
            pictureImage.DOColor(Color.white, duration);
        }

        if (backgroundImage != null)
        {
            backgroundImage.DOColor(Color.white, duration);
        }
    }
}
