using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragEvent : UnityEvent<Card> { }

public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform _rectTransform;
    private Vector3 initialPos;

    public CardType cardType;

    public CardDragEvent dragBeginEvent;
    public CardDragEvent dragEndEvent;

    public CardArtSet artSet;

    private Image image;
    private Color defaultColor;

    [SerializeField]
    private Color dragColor;



    private void Awake()
    {
        dragBeginEvent = new CardDragEvent();
        dragEndEvent = new CardDragEvent();
        _rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragBeginEvent.Invoke(this);
        initialPos = _rectTransform.anchoredPosition;
        image.raycastTarget = false;
        image.color = dragColor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndEvent.Invoke(this);
        _rectTransform.anchoredPosition = initialPos;
        image.raycastTarget = true;
        image.color = defaultColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = eventData.position;
    }

    internal void Destroy() => Destroy(gameObject);
}
