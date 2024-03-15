using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button shopBtn;
    private SceneController sceneCtrl;

    private void Awake()
    {
        sceneCtrl = SceneController.getInstance;

        gameStartBtn.onClick.AddListener(OnClickGameStartBtn);
        shopBtn.onClick.AddListener(OnClickShopBtn);
    }

    private void OnClickGameStartBtn()
    {
        UIManager.getInstance.UnloadScene();
        sceneCtrl.ChangeScene("InGameScene");
    }

    private void OnClickShopBtn()
    {
        UIManager.getInstance.Show<ShopPanelController>("Prefabs/UI/ShopPanel");
    }
}
