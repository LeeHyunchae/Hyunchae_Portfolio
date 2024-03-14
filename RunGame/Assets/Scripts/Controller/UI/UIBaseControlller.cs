using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseController : MonoBehaviour
{
    protected Canvas myCanvas;
    protected bool isShow = false;

    protected void Awake()
    {
        myCanvas = this.GetComponent<Canvas>();

        Init();
    }

    public void Show()
    {
        myCanvas.enabled = true;
        isShow = true;
    }

    public void Hide()
    {
        myCanvas.enabled = false;
        isShow = false;
    }

    public void SetSortOrder(int _sortorder)
    {
        myCanvas.sortingOrder = _sortorder;
    }

    public bool IsShow() { return isShow; }

    public virtual void Init() { }

}