using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnEnterHandler = null;
    public Action<PointerEventData> OnExitHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            Managers.Sound.Play(ESound.Effect, "SFX_click");

			OnClickHandler.Invoke(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnterHandler != null)
        {
            OnEnterHandler.Invoke(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnExitHandler != null)
        {
            OnExitHandler.Invoke(eventData);
        }
    }
}
