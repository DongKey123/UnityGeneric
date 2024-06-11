using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerDemo : MonoBehaviour
{

    private UIManager uiManager = null;

    private const string UIDemoPrefabPath = "UIManagerExample/Example1";

    private void Awake()
    {
        uiManager = UIManager.getInstance;
        uiManager.Show<UIDemoController>(UIDemoPrefabPath);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
