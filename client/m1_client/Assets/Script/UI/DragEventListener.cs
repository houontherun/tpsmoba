using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragEventListener : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public delegate void VoidDelegate(PointerEventData e);
    public delegate void Vector2Delegate(PointerEventData e, Vector2 v);

    public VoidDelegate onBeginDrag;
    public VoidDelegate onDrag;
    public VoidDelegate onEndDrag;
    public Vector2Delegate onForwardDrag;
    Vector2 forwardDelta;
    bool forwardDrag;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null)
            onBeginDrag(eventData);
        forwardDrag = true;
        forwardDelta = Vector2.zero;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
            onEndDrag(eventData);
        if (forwardDrag && onForwardDrag != null)
            onForwardDrag(eventData, forwardDelta);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
            onDrag(eventData);

        if (forwardDelta.y * eventData.delta.y < 0 || forwardDelta.x * eventData.delta.x < 0)
        {
            forwardDrag = false;
        }
        else if (forwardDrag)
        {
            forwardDelta += eventData.delta;
        }
    }

    public static DragEventListener Get(GameObject go)
    {
        DragEventListener listener = go.GetComponent<DragEventListener>();
        if (listener == null) listener = go.AddComponent<DragEventListener>();
        var g = go.GetComponent<Graphic>();
        if (g != null)
        {
            g.raycastTarget = true;
        }
        return listener;
    }
}
