using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler , IPointerUpHandler
{
    public delegate void OnClickDelegate();
    public OnClickDelegate OnPointerClickEvent;
    public OnClickDelegate OnPointerDownEvent;

    private bool isPointerDown;
    private Image buttonImage;

    private Color pointerUpColor = Color.white;
    private Color pointerDownColor = Color.gray;

    public void SetEnable(bool _active) => buttonImage.raycastTarget = _active;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerClickEvent?.Invoke();
        isPointerDown = true;
        buttonImage.color = pointerDownColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        buttonImage.color = pointerUpColor;
    }

    private void Update()
    {
        if(isPointerDown)
        {
            //.. TODO :: 누른시간 대비 높이가 달라지는게 아니라면 누르고 있는 시간으로 체크해야 함 [jump / longjump 분기]
            OnPointerDownEvent?.Invoke();
        }
    }
}
