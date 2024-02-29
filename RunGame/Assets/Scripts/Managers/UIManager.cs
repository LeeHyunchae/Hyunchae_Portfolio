using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager uiManager;
    public static UIManager instance
    {
        get
        {
            if (uiManager == null)
            {
                uiManager = new UIManager();
            }

            return uiManager;
        }
    }

    private Stack<UIBaseController> uiStack = new Stack<UIBaseController>();
    private List<UIBaseController> uiList = new List<UIBaseController>();

    private const int DEFAULTLAYER = 100;
    private int curLayer = 0;

    public T Show<T>(string _filePath) where T : UIBaseController
    {
        int listCount = uiList.Count;

        UIBaseController ui;

        for (int i = 0; i < listCount; i++)
        {
            ui = uiList[i];

            if (typeof(T).Equals(ui.GetType()))
            {
                if (ui.IsShow())
                {
                    Debug.Log("this Panel already Showing");
                    return (T)ui;
                }

                uiStack.Push(ui);
                ui.Show();

                ui.SetSortOrder(DEFAULTLAYER + curLayer);
                curLayer++;

                return (T)ui;
            }
        }

        ui = Find<T>();

        if (ui == null)
        {
            ui = CreateUIPanel<T>(_filePath);
        }

        uiStack.Push(ui);
        ui.Show();

        ui.SetSortOrder(DEFAULTLAYER + curLayer);
        curLayer++;

        return (T)ui;
    }

    public void Hide()
    {
        if (uiStack.Count > 0)
        {
            UIBaseController ui = uiStack.Pop();
            ui.Hide();

            curLayer--;
        }
        else
        {
            Debug.Log("Not exist Showing UIPanel");
        }
    }

    public T GetUIPanel<T>() where T : UIBaseController
    {
        int listCount = uiList.Count;

        UIBaseController ui;

        for (int i = 0; i < listCount; i++)
        {
            ui = uiList[i];
            if (typeof(T).Equals(ui.GetType()))
            {
                return (T)ui;
            }
        }

        Debug.Log("Not exist UIPanel");
        return null;
    }


    public T CreateUIPanel<T>(string _filePath) where T : UIBaseController
    {
        T uiPanelCtrl = UnityEngine.Object.Instantiate(Resources.Load<T>(_filePath));
        uiList.Add(uiPanelCtrl);

        return uiPanelCtrl;
    }

    public void UnloadScene()
    {
        uiStack.Clear();
        uiList.Clear();
    }

    public T Find<T>() where T : UIBaseController
    {
        T ui = UnityEngine.GameObject.FindObjectOfType<T>();

        return ui;
    }
}