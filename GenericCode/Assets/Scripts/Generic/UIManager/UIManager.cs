using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : LazySingleton<UIManager>
{
    private Dictionary<System.Type, UIBaseController> uiDictionary = null;
    private Stack<UIBaseController> showedPanels = null;

    private const string UIDefaultPath = "Prefabs/UI";
    private const int defaultOrder = 100;

    UIManager()
    {
        uiDictionary = new Dictionary<System.Type, UIBaseController>();
        showedPanels = new Stack<UIBaseController>();

        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += ClearPanels;
    }

    ~UIManager()
    {
        uiDictionary.Clear();
        showedPanels.Clear();

        uiDictionary = null;
        showedPanels = null;
    }

    private void ClearPanels(UnityEngine.SceneManagement.Scene arg0)
    {
        uiDictionary.Clear();
        showedPanels.Clear();
    }

    public T Show<T>(string path, int sortingOrder = 0) where T : UIBaseController
    {
        var type = typeof(T);
        T panel;

        if (uiDictionary.ContainsKey(type))
        {
            panel = uiDictionary[type] as T;
        }
        else
        {
            panel = CreatePanel<T>(path);
            uiDictionary.Add(type, panel);
        }

        if (panel.IsShow())
            return panel;

        showedPanels.Push(panel);
        panel.Show();

        int order = sortingOrder == 0 ? defaultOrder + showedPanels.Count : sortingOrder;
        panel.SetSortingOrder(order);

        return panel;
    }

    private T CreatePanel<T>(string path) where T : UIBaseController
    {
        string resourcePath = $"{UIDefaultPath}/{path}";
        var prefab = Resources.Load<T>(resourcePath);
        var panel = UnityEngine.Object.Instantiate<T>(prefab);
        panel.Initialize();

        return panel;
    }

    public void Hide()
    {
        if (showedPanels.Count > 0)
        {
            var panel = showedPanels.Pop();
            panel.Hide();
        }
    }

    public T GetCachedPanel<T>(string path) where T : UIBaseController
    {
        var type = typeof(T);
        T panel;
        if (uiDictionary.ContainsKey(type))
        {
            panel = uiDictionary[type] as T;
            if (panel == null)
            {
                panel = CreatePanel<T>(path);
                uiDictionary[type] = panel;
            }
        }
        else
        {
            panel = CreatePanel<T>(path);
            uiDictionary.Add(type, panel);
        }

        return panel;
    }

    public T GetCachedPanel<T>() where T : UIBaseController
    {
        var type = typeof(T);
        T panel;
        if (uiDictionary.ContainsKey(type))
        {
            panel = uiDictionary[type] as T;
            return panel;
        }
        return null;
    }

    public T FindPanel<T>() where T : UIBaseController
    {
        T panel = GameObject.FindAnyObjectByType<T>();

        return panel;
    }

    public T FindPanelWithCache<T>() where T : UIBaseController
    {
        var type = typeof(T);

        T panel = GameObject.FindAnyObjectByType<T>();
        if (panel == null)
            return null;

        uiDictionary.Add(type, panel);

        return panel;
    }

    public void HideAllPanel()
    {
        int count = showedPanels.Count;
        var panels = showedPanels.ToArray();
        for (int i = 0; i < count; i++)
        {
            panels[i].Hide();
        }
    }
}
