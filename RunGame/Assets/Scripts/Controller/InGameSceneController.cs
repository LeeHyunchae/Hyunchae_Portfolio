using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//랜덤액세스 3회이상이면 캐싱해주기

public class InGameSceneController : MonoBehaviour
{
    private PlayerController playerCtrl;
    private FloorController floorCtrl;
    private ObstacleController obstacleCtrl;

    private void Awake()
    {
        InitPlayerCtrl();
        InitFloorCtrl();
        InitObstacleCtrl();
        SetFirstObstacle();
    }

    private void Update()
    {
        playerCtrl.Update();
        floorCtrl.Update();
        obstacleCtrl.Update();
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
        floorCtrl.Init();
    }

    private void ChangeCurFloor(Floor _curFloor)
    {
        playerCtrl.SetCurFloor(_curFloor);
    }

    private void InitObstacleCtrl()
    {
        obstacleCtrl = new ObstacleController();
        obstacleCtrl.OnChangeCurObstacle = ChangeCurObstacle;
        obstacleCtrl.Init(floorCtrl);
    }

    private void ChangeCurObstacle(Obstacle _obstacle)
    {

    }

    private void SetFirstObstacle()
    {
        obstacleCtrl.InitFirstObstacle(floorCtrl.GetAllfFloor());
    }
}
