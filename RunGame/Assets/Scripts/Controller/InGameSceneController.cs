using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//랜덤액세스 3회이상이면 캐싱해주기

public class InGameSceneController : MonoBehaviour
{
    private PlayerController playerCtrl;
    private FloorController floorCtrl;
    private ObstacleController obstacleCtrl;
    private int curSpeed = 3;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        InitPlayerCtrl();
        InitObstacleCtrl();
        InitFloorCtrl();
        SetFirstObstacle();
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
        obstacleCtrl.OnChangeCurObstacle = ChangeCurObstacle;
        obstacleCtrl.SetScreenLeft(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x);
        obstacleCtrl.Init();
        obstacleCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
    }

    private void Update()
    {
        playerCtrl.Update();
        floorCtrl.Update();
        obstacleCtrl.Update();

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpSpeedRate();
        }
    }

    private void UpSpeedRate()
    {
        curSpeed++;
        floorCtrl.SetSpeedRate(curSpeed);
        obstacleCtrl.SetSpeedRate(curSpeed);
    }

    private void ChangeCurFloor(Floor _curFloor)
    {
        playerCtrl.SetCurFloor(_curFloor);
    }

    private void ChangeCurObstacle(FixedObstacle _obstacle)
    {
        playerCtrl.SetCurObstacle(_obstacle);
    }

    private void SetFirstObstacle()
    {
        //obstacleCtrl.InitFirstObstacle(floorCtrl.GetAllfFloor());
    }

    private void OnRepositionFloor(Floor _floor)
    {
        List<FixedObstacle> obstacles = obstacleCtrl.OnRepositionFloor(_floor);
    }

}
