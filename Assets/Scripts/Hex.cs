using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HexEventArgs : EventArgs
{
    public Hex Hex { get; }
    public PointerEventData PointerData { get; }

    public HexEventArgs(Hex hex, PointerEventData pointer)
    {
        Hex = hex;
        PointerData = pointer;
    }
}

[Serializable]
public class HexSelectionEvent : UnityEvent<bool> { }

public class Hex : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public HexCoords coords;



    #region EVENTS
    public event EventHandler<HexEventArgs> OnEnter;
    public event EventHandler<HexEventArgs> OnDropped;
    public event EventHandler<HexEventArgs> OnExit;

    public HexSelectionEvent OnSelected;
    public HexSelectionEvent OnDeselected;
    public void Activate()
        => OnSelected.Invoke(true);

    public void Deactivate()
        => OnDeselected.Invoke(false);

    public void OnDrop(PointerEventData eventData)
    {
        var handler = OnDropped;
        handler?.Invoke(this, new HexEventArgs(this, eventData));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var handler = OnEnter;
        handler?.Invoke(this, new HexEventArgs(this, eventData));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var handler = OnExit;
        handler?.Invoke(this, new HexEventArgs(this, eventData));
    }
    #endregion
}
