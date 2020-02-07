using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using SLua;
[CustomLuaClassAttribute]
public class EventListener : MonoBehaviour, IEventSystemHandler, IPointerClickHandler, ISubmitHandler,IDragHandler,IEndDragHandler
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void BaseEventDelegate(BaseEventData eventData);
    public delegate void PointerEventDelegate(PointerEventData eventData);
    public delegate void StringDelegate(string arg);

    public PointerEventDelegate onClick;
    public PointerEventDelegate onDown;
    public PointerEventDelegate onEnter;
    public PointerEventDelegate onExit;
    public PointerEventDelegate onUp;
    public BaseEventDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public BaseEventDelegate onSubmit;
    public PointerEventDelegate onDrag;
    public PointerEventDelegate onEndDrag;
    public StringDelegate onAnimEvent;


    static public EventListener Get(GameObject go)
    {
        EventListener listener = go.GetComponent<EventListener>();
        if (listener == null)
            listener = go.AddComponent<EventListener>();
        return listener;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(onClick != null)
            onClick(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(eventData);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(eventData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(eventData);

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(eventData);
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(eventData);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (onSubmit != null) onSubmit(eventData);
    }


    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
            onDrag(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
            onEndDrag(eventData);
    }

    
    private void AnimEvent(string arg)
    {
        if(onAnimEvent != null)
        {
            onAnimEvent(arg);
        }
    }
}
