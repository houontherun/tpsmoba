using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickEventListener : MonoBehaviour, IPointerClickHandler
{
    public delegate void VoidDelegate(PointerEventData e);

    public VoidDelegate onClick;
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
            onClick(eventData);
    }

    public static ClickEventListener Get(GameObject go)
    {
        ClickEventListener listener = go.GetComponent<ClickEventListener>();
        if (listener == null) listener = go.AddComponent<ClickEventListener>();
        var g = go.GetComponent<Graphic>();
        if (g != null)
        {
            g.raycastTarget = true;
        }
        return listener;
    }

    public static void Remove(GameObject go)
    {
        ClickEventListener listener = go.GetComponent<ClickEventListener>();
        if (listener)
        {
            GameObject.DestroyImmediate(listener);
        }
    }
}
