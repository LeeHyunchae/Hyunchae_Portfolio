using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneController : MonoBehaviour
{
    private const string MAINSCENE = "MainScene";

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if(Input.anyKey)
        {
            SceneController.getInstance.ChangeScene(MAINSCENE);
        }
    }
}
