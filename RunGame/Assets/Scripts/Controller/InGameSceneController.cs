using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//랜덤액세스 3회이상이면 캐싱해주기

public class InGameSceneController : MonoBehaviour
{
    private PlayerController playerCtrl;
    private FloorController floorCtrl;
    private ObstacleController obstacleCtrl;
    private CoinController coinCtrl;
    private int curGameSpeed = 3;
    private float flyObstacleInterval = 3f;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        InitPlayerCtrl();
        InitObstacleCtrl();
        InitCoinCtrl();
        InitFloorCtrl();
    }

    private void InitPlayerCtrl()
    {
        playerCtrl = new PlayerController();
        playerCtrl.Init();
    }

    private void InitFloorCtrl()
    {
        floorCtrl = new FloorController();
        floorCtrl.OnChaneCurFloor = ChangeCurFloor;
        floorCtrl.OnRepositionFloor = OnRepositionFloor;
        floorCtrl.SetScreenLeft(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x);
        floorCtrl.Init();
        floorCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
        
    }
    private void InitObstacleCtrl()
    {
        obstacleCtrl = new ObstacleController();
        obstacleCtrl.OnChangeCurObstacles = ChangeCurObstacle;
        obstacleCtrl.SetScreenLeft(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x);
        obstacleCtrl.SetFlyObstacleInterval(flyObstacleInterval);
        obstacleCtrl.Init();
        obstacleCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
    }

    private void InitCoinCtrl()
    {
        coinCtrl = new CoinController();
        coinCtrl.OnChangeCurCoins = ChangeCurCoins;
        coinCtrl.SetScreenLeft(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x);
        coinCtrl.Init();
        coinCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
    }

    private void Update()
    {
        playerCtrl.Update();
        floorCtrl.Update();
        obstacleCtrl.Update();
        coinCtrl.Update();

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpSpeedRate();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DownSpeedRate();
        }
    }

    private void UpSpeedRate()
    {
        curGameSpeed++;
        floorCtrl.SetSpeedRate(curGameSpeed);
        obstacleCtrl.SetSpeedRate(curGameSpeed);
    }

    private void DownSpeedRate()
    {
        curGameSpeed--;
        floorCtrl.SetSpeedRate(curGameSpeed);
        obstacleCtrl.SetSpeedRate(curGameSpeed);
    }

    private void ChangeCurFloor(Floor _curFloor)
    {
        playerCtrl.SetCurFloor(_curFloor);
    }

    private void ChangeCurObstacle(List<BaseObstacle> _obstacles)
    {
        playerCtrl.SetCurObstacle(_obstacles);
    }

    private void ChangeCurCoins(List<Coin> _coins)
    {

    }

    private void OnRepositionFloor(Floor _floor)
    {
        List<BaseObstacle> obstacles = obstacleCtrl.OnRepositionFloor(_floor);
        coinCtrl.OnRepositionFloor(_floor);
    }

}
