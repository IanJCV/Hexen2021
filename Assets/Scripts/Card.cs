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

    private void Awake()
    {
        dragBeginEvent = new CardDragEvent();
        dragEndEvent = new CardDragEvent();
        _rectTransform = GetComponent<RectTransform>();
        initialPos = _rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragBeginEvent.Invoke(this);
        //GetComponent<Image>().raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndEvent.Invoke(this);
        //_rectTransform.position = initialPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_rectTransform.position = eventData.position;
    }

    internal void Destroy() => Destroy(gameObject);
}
