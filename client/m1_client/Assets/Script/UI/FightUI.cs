using UnityEngine;
using UnityEngine.EventSystems;

public class FightUI : MonoBehaviour {

    public GameObject joyStickNood;
    public RectTransform joyStickNoodRT;
    public float joyMaxRange = 160;

    Vector2 initialPos;
    Vector2 controlDirection = Vector2.zero;
    Canvas cavas;

    void Start () {
        DragEventListener.Get(joyStickNood).onDrag = OnDrag;
        DragEventListener.Get(joyStickNood).onEndDrag = OnEndDrag;
        joyStickNoodRT = joyStickNood.GetComponent<RectTransform>();
        initialPos = joyStickNoodRT.anchoredPosition;
        cavas = this.transform.parent.GetComponent<Canvas>();
    }

    void Update()
    {
        if(controlDirection != Vector2.zero)
        {
            OnHeroMove();
        }
    }

    void OnDrag(PointerEventData e)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joyStickNoodRT.parent as RectTransform, e.position, cavas.worldCamera, out pos);
        if ((pos - initialPos).magnitude > joyMaxRange)
        {
            pos = (pos - initialPos).normalized * joyMaxRange + initialPos;
        }
        joyStickNoodRT.anchoredPosition = pos;
        controlDirection = (pos - initialPos).normalized;
    }



    void OnEndDrag(PointerEventData e)
    {
        joyStickNood.GetComponent<RectTransform>().anchoredPosition = initialPos;
        controlDirection = Vector2.zero;
    }

    void OnHeroMove()
    {
        if(BattleDirector.Instance.hero)
        {
            var direction = new Vector3(-controlDirection.y, 0, controlDirection.x);
            BattleDirector.Instance.hero.Move(direction);
        }
    }
}
