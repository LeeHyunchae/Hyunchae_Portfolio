using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleController
{
    private const int INITSPEED = 3;
    private const string OBSTACLEPATH = "Prefabs/Obstacle";
    private const int OBSTACLECOUNT = 20;
    private const int ONE_OBSTACLE_SIZE = 3;
    private const int TWO_OBSTACLE_SIZE = 5;
    private const int THREE_OBSTACLE_SIZE = 7;
    private const string SPRITEPATH = "Sprites/Obstacle_";
    private const int SPRITECOUNT = 4;
    private const int FLOOR_WIDTH_CORRECTION = 1;

    private FixedObstacle[] obstacles;
    private Queue<FixedObstacle> standbyObstacles = new Queue<FixedObstacle>();
    private List<FixedObstacle> rePosObstacleList = new List<FixedObstacle>(OBSTACLECOUNT);

    private Sprite[] sprites;
    private int obstacleCount;

    private float speedRate = INITSPEED;
    private float screenLeft;

    private int frontObstacleIdx = 0;
    private float playerHalfSize = 0;

    private Transform obstacleParent;

    public Action<FixedObstacle> OnChangeCurObstacle;
    public Action<FixedObstacle> OnRepositionObstacle;

    public void SetScreenLeft(float _screenLeft) => screenLeft = _screenLeft;

    public void Init()
    {
        CreateObstacle();

        InitObstacleSprites();
    }

    public void InitObstacleSprites()
    {
        sprites = new Sprite[SPRITECOUNT];

        for(int i = 0; i<SPRITECOUNT;i++)
        {
            sprites[i] = Resources.Load<Sprite>(SPRITEPATH + i);

        }
    }

    public void InitFirstObstacle(Floor[] _floors)
    {
        int count = _floors.Length;

        int startFloorIdx = 0;

        for(int i = 0; i < count; i ++)
        {
            if(i == startFloorIdx)
            {
                continue;
            }

            OnRepositionFloor(_floors[i]);
        }

        
    }

    public void SetSpeedRate(int _speed)
    {
        speedRate = _speed;
    }

    private void CreateObstacle()
    {
        obstacleParent = new GameObject("Obstacles").transform;

        obstacleParent.transform.position = Vector2.zero;

        GameObject originObj = (GameObject)Resources.Load(OBSTACLEPATH);
        GameObject[] obstacleObjs = new GameObject[OBSTACLECOUNT];

        for (int i = 0; i < OBSTACLECOUNT; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObj, Vector2.zero, Quaternion.identity,obstacleParent);
        }

        InitObstacles(obstacleObjs);
        OnChangeCurObstacle.Invoke(obstacles[0]);
    }

    private void InitObstacles(GameObject[] _obstacles)
    {
        obstacleCount = _obstacles.Length;

        obstacles = new FixedObstacle[obstacleCount];

        for (int i = 0; i < obstacleCount; i++)
        {
            FixedObstacle obstacle = new FixedObstacle();

            obstacle.Init(_obstacles[i]);
            obstacle.SetActive(false);

            standbyObstacles.Enqueue(obstacle);

            obstacles[i] = obstacle;
        }
    }

    public void Update()
    {
        MoveObstacle();
    }

    private void MoveObstacle()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            FixedObstacle obstacle = obstacles[i];

            if(!obstacle.GetActive)
            {
                continue;
            }

            Vector2 pos = obstacle.GetTransform.position;

            pos.x += (speedRate + obstacle.GetSpeedRate) * -1f * Time.deltaTime;

            obstacle.GetTransform.position = pos;

            if (CheckFrontObstacle(i))
            {
                frontObstacleIdx = (i + 1) % obstacleCount;
                if (OnChangeCurObstacle != null)
                {
                    OnChangeCurObstacle.Invoke(obstacles[frontObstacleIdx]);
                }
            }

            if(CheckOutsideObstacle(i))
            {
                standbyObstacles.Enqueue(obstacle);
                obstacle.SetActive(false);
            }
        }
    }


    private bool CheckFrontObstacle(int _idx)
    {
        int curFloorIdx = (_idx + 1) % obstacleCount;

        if (frontObstacleIdx == curFloorIdx)
        {
            return false;
        }

        return obstacles[_idx].GetTransform.position.x + obstacles[_idx].GetWidth() * 0.5f + playerHalfSize <= Vector2.zero.x;
    }

    private bool CheckOutsideObstacle(int _idx)
    {
        return obstacles[_idx].GetTransform.position.x + obstacles[_idx].GetWidth() * 0.5f <= screenLeft;
    }

    public FixedObstacle GetFrontFloor()
    {
        return obstacles[frontObstacleIdx];
    }

    public List<FixedObstacle> OnRepositionFloor(Floor _rePosFloor)
    {
        if(standbyObstacles.Count == 0)
        {
            return null;
        }

        rePosObstacleList.Clear();

        Floor floor = _rePosFloor;

        int size = floor.GetFloorMiddleSize();

        //if(size >= THREE_OBSTACLE_SIZE)
        //{

        //}
        //else if(size >= TWO_OBSTACLE_SIZE)
        //{

        //}
        //else
        if(size >= ONE_OBSTACLE_SIZE)
        {
            FixedObstacle obstacle = standbyObstacles.Dequeue();

            Vector2 obstaclePos = floor.GetTransform.position;

            obstacle.SetSprite(sprites[Random.Range(0, SPRITECOUNT)]);

            int rndMinX = (int)(-(size * 0.5f) + obstacle.GetWidth() * 0.5f + FLOOR_WIDTH_CORRECTION);
            int rndMaxX = (int)(size * 0.5f - obstacle.GetWidth() * 0.5f + FLOOR_WIDTH_CORRECTION);

            int posX = (int)(obstaclePos.x + Random.Range(rndMinX,rndMaxX));
            float posY = floor.GetTransform.position.y + (floor.GetFloorHeight() * 0.5f) + (obstacle.GetHeight() * 0.5f);

            obstaclePos.x = posX;
            obstaclePos.y = posY;
            
            obstacle.GetTransform.position = obstaclePos;


            obstacle.SetActive(true);

            rePosObstacleList.Add(obstacle);
        }

        return rePosObstacleList;
    }

    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }
}
