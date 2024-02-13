using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleController
{
    private const string FIXED_OBSTACLE_PATH = "Prefabs/Obstacle";
    private const string FIXED_OBSTACLE_SPRITEPATH = "Sprites/Obstacle_";
    private const string JUMP_OBSTACLE_PATH = "Prefabs/Enemy_BigDino";
    private const string FLY_OBSTACLE_PATH = "Prefabs/Enemy_SmallDino";

    private const int INITSPEED = 3;
    private const int OBSTACLECOUNT = 60;
    private const int FIXED_OBSTACLE_COUNT = 10;
    private const int JUMP_OBSTACLE_COUNT = 10;
    private const int FLY_OBSTACLE_COUNT = 10;
    private const int ONE_OBSTACLE_SIZE = 3;
    private const int TWO_OBSTACLE_SIZE = 5;
    private const int THREE_OBSTACLE_SIZE = 7;
    private const int SPRITECOUNT = 4;
    private const int FLOOR_WIDTH_CORRECTION = 1;

    private BaseObstacle[] obstacles;
    private Queue<BaseObstacle> standbyLandObstacles = new Queue<BaseObstacle>();
    private Queue<BaseObstacle> standbyFlyObstacles = new Queue<BaseObstacle>();
    private List<BaseObstacle> rePosObstacleList = new List<BaseObstacle>(OBSTACLECOUNT);

    private Sprite[] sprites;
    private int obstacleCount = 0;

    private float speedRate = INITSPEED;
    private float screenLeft;
    private float flyObstacleInterval;
    private float curFlyObstacleTime = 0f;

    private int frontObstacleIdx = 0;
    private float playerHalfSize = 0;

    private Transform obstacleParent;

    public Action<BaseObstacle> OnChangeCurLandObstacle;
    public Action<BaseObstacle> OnRepositionObstacle;

    public void SetScreenLeft(float _screenLeft) => screenLeft = _screenLeft;

    public void Init()
    {
        CreateObstacles();

        InitObstacleSprites();
    }

    public void InitObstacleSprites()
    {
        sprites = new Sprite[SPRITECOUNT];

        for(int i = 0; i<SPRITECOUNT;i++)
        {
            sprites[i] = Resources.Load<Sprite>(FIXED_OBSTACLE_SPRITEPATH + i);

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

    private void CreateObstacles()
    {
        obstacleParent = new GameObject("Obstacles").transform;

        obstacleParent.transform.position = Vector2.zero;

        obstacles = new BaseObstacle[OBSTACLECOUNT];

        CreateFixedObstacles();
        CreateJumpObstacles();
        CreateFlyObstacle();

        OnChangeCurLandObstacle.Invoke(obstacles[0]);
    }

    private void CreateFixedObstacles()
    {
        GameObject originObstacleObj = (GameObject)Resources.Load(FIXED_OBSTACLE_PATH);
        GameObject[] obstacleObjs = new GameObject[FIXED_OBSTACLE_COUNT];

        for (int i = 0; i < FIXED_OBSTACLE_COUNT; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObstacleObj, Vector2.zero, Quaternion.identity, obstacleParent);
        }

        InitObstacles<FixedObstacle>(obstacleObjs);
    }

    private void CreateJumpObstacles()
    {
        GameObject originObstacleObj = (GameObject)Resources.Load(JUMP_OBSTACLE_PATH);
        GameObject[] obstacleObjs = new GameObject[JUMP_OBSTACLE_COUNT];

        for (int i = 0; i < JUMP_OBSTACLE_COUNT; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObstacleObj, Vector2.zero, Quaternion.identity, obstacleParent);
        }

        InitObstacles<JumpObstacle>(obstacleObjs);
    }

    private void CreateFlyObstacle()
    {
        GameObject originObstacleObj = (GameObject)Resources.Load(FLY_OBSTACLE_PATH);
        GameObject[] obstacleObjs = new GameObject[FLY_OBSTACLE_COUNT];

        for (int i = 0; i < JUMP_OBSTACLE_COUNT; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObstacleObj, Vector2.zero, Quaternion.identity, obstacleParent);
        }

        InitObstacles<FlyObstacle>(obstacleObjs);
    }

    private void InitObstacles<T>(GameObject[] _obstacles) where T : BaseObstacle , new()
    {
        int curArrayCount = _obstacles.Length + obstacleCount;

        for (int i = obstacleCount; i <  curArrayCount; i++)
        {
            BaseObstacle obstacle = new T();

            obstacle.Init(_obstacles[i - obstacleCount]);
            obstacle.SetActive(false);

            if(obstacle.GetObstacleType != EObstacleType.FLY)
            {
                standbyLandObstacles.Enqueue(obstacle);
            }
            else if(obstacle.GetObstacleType == EObstacleType.FLY)
            {
                standbyFlyObstacles.Enqueue(obstacle);
            }

            obstacles[i] = obstacle;
        }

        obstacleCount += _obstacles.Length;
    }

    public void Update()
    {
        CheckObstaclePos();
        OnActionObstacle();

        if (CheckFlyObstacleInterval())
        {
            RepositionFlyObstacle();
        }
    }

    private void OnActionObstacle()
    {
        for(int i = 0; i<obstacleCount; i++)
        {
            BaseObstacle obstacle = obstacles[i];

            if(obstacle.GetObstacleType != EObstacleType.FIXED)
            {
                obstacle.Action();
            }
        }
    }

    private void CheckObstaclePos()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            BaseObstacle obstacle = obstacles[i];

            if(!obstacle.GetActive)
            {
                continue;
            }

            if (obstacle.GetObstacleType != EObstacleType.FLY)
            {
                if (CheckFrontObstacle(i))
                {
                    frontObstacleIdx = (i + 1) % obstacleCount;
                    if (OnChangeCurLandObstacle != null)
                    {
                        OnChangeCurLandObstacle.Invoke(obstacles[frontObstacleIdx]);
                    }
                }

                if (CheckOutsideObstacle(i))
                {
                    obstacle.GetTransform.SetParent(obstacleParent);


                    standbyLandObstacles.Enqueue(obstacle);

                    obstacle.SetActive(false);
                }
            }
            else
            {
                
                if (CheckOutsideObstacle(i))
                {
                    standbyFlyObstacles.Enqueue(obstacle);

                    obstacle.SetActive(false);
                }
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

    private bool CheckFlyObstacleInterval()
    {
        curFlyObstacleTime += Time.deltaTime;
        Debug.Log(curFlyObstacleTime);

        if(curFlyObstacleTime >= flyObstacleInterval)
        {
            curFlyObstacleTime = 0;

            return true;
        }

        return false;
    }

    public List<BaseObstacle> OnRepositionFloor(Floor _rePosFloor)
    {
        if(standbyLandObstacles.Count == 0)
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
            BaseObstacle obstacle = standbyLandObstacles.Dequeue();

            Vector2 obstaclePos = floor.GetTransform.position;

            obstacle.SetSprite(sprites[Random.Range(0, SPRITECOUNT)]);

            int rndMinX = (int)(-(size * 0.5f) + obstacle.GetWidth() * 0.5f + FLOOR_WIDTH_CORRECTION);
            int rndMaxX = (int)(size * 0.5f - obstacle.GetWidth() * 0.5f + FLOOR_WIDTH_CORRECTION);

            int posX = (int)(obstaclePos.x + Random.Range(rndMinX,rndMaxX));
            float posY = floor.GetTransform.position.y + (floor.GetFloorHeight() * 0.5f) + (obstacle.GetHeight() * 0.5f);
            obstaclePos.x = posX;
            obstaclePos.y = posY;
            
            obstacle.GetTransform.SetParent(_rePosFloor.GetTransform);
            obstacle.GetTransform.position = obstaclePos;

            obstacle.SetFloorPosition(obstaclePos);

            obstacle.SetActive(true);

            rePosObstacleList.Add(obstacle);
        }

        return rePosObstacleList;
    }

    public void RepositionFlyObstacle()
    {
        if(standbyFlyObstacles.Count == 0)
        {
            return;
        }

        BaseObstacle obstacle = standbyFlyObstacles.Dequeue();

        Vector2 obstaclePos = Vector2.zero;

        float posY = Random.Range(-2, 3);

        obstaclePos.x = 100;
        obstaclePos.y = posY;

        obstacle.GetTransform.position = obstaclePos;

        obstacle.SetFloorPosition(obstaclePos);

        obstacle.SetActive(true);

    }

    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }

    public void SetFlyObstacleInterval(float _intervalTime)
    {
        flyObstacleInterval = _intervalTime;
    }
}
