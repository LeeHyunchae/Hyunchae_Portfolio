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
    private const int MINFLOORSIZE = 5;
    private const string SPRITEPATH = "Sprites/Obstacle_";
    private const int SPRITECOUNT = 4;

    private Obstacle[] obstacles;
    private Queue<Obstacle> standbyObstacles = new Queue<Obstacle>();

    private Sprite[] sprites;
    private int obstacleCount;

    private float speedRate = INITSPEED;
    private float screenLeft;

    private int frontObstacleIdx = 0;
    private float playerHalfSize = 0;

    public Action<Obstacle> OnChangeCurObstacle;

    
    public void Init(FloorController _floorCtrl)
    {
        _floorCtrl.OnRepositionFloor = OnRepositionFloor;
        CreateObstacle();

        screenLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

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

        OnChangeCurObstacle.Invoke(obstacles[0]);
    }

    public void SetSpeedRate(int _speed)
    {
        speedRate = _speed;
    }

    private void CreateObstacle()
    {
        Transform obstacleParent = new GameObject("Obstacles").transform;

        obstacleParent.transform.position = Vector2.zero;

        GameObject originObj = (GameObject)Resources.Load(OBSTACLEPATH);
        GameObject[] obstacleObjs = new GameObject[OBSTACLECOUNT];

        for (int i = 0; i < OBSTACLECOUNT; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObj, Vector2.zero, Quaternion.identity,obstacleParent);
        }

        InitObstacles(obstacleObjs);
    }

    private void InitObstacles(GameObject[] _obstacles)
    {
        obstacleCount = _obstacles.Length;

        obstacles = new Obstacle[obstacleCount];

        for (int i = 0; i < obstacleCount; i++)
        {
            Obstacle obs = new Obstacle();

            obs.Init(_obstacles[i]);
            obs.SetActive(false);

            standbyObstacles.Enqueue(obs);

            obstacles[i] = obs;
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
            Obstacle obs = obstacles[i];

            if(!obs.GetActive)
            {
                continue;
            }

            Vector2 pos = obs.GetTransform.position;

            pos.x += speedRate * -1f * Time.deltaTime;

            obs.GetTransform.position = pos;

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
                standbyObstacles.Enqueue(obs);
                obs.SetActive(false);
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

    public Obstacle GetFrontFloor()
    {
        return obstacles[frontObstacleIdx];
    }

    private void OnRepositionFloor(Floor _reposFloor)
    {
        if(standbyObstacles.Count == 0)
        {
            return;
        }

        Floor floor = _reposFloor;

        int size = floor.GetFloorMiddleSize();

        if(size >= MINFLOORSIZE)
        {
            Obstacle obs = standbyObstacles.Dequeue();

            Vector2 obsPos = floor.GetTransform.position;

            obs.SetSprite(sprites[Random.Range(0, SPRITECOUNT)]);

            obsPos.x = obsPos.x + Random.Range(-(size * 0.5f) + obs.GetWidth() * 0.5f, size * 0.5f - obs.GetWidth() * 0.5f);
            obsPos.y = floor.GetTransform.position.y + (floor.GetFloorHeight() * 0.5f) + (obs.GetHeight() * 0.5f);
            
            obs.GetTransform.position = obsPos;


            obs.SetActive(true);


        } 
    }

    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }
}
