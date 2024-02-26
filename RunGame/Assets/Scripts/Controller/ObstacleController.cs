using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EObstaclePosType
{
    FLOOR = 0,
    FLY = 1
}

public class ObstacleController
{
    private const string FIXED_OBSTACLE_PATH = "Prefabs/Obstacle";
    private const string FIXED_OBSTACLE_SPRITEPATH = "Sprites/Obstacle_";
    private const string JUMP_OBSTACLE_PATH = "Prefabs/Enemy_BigDino";
    private const string FLY_OBSTACLE_PATH = "Prefabs/Obstacle_Arrow";

    private const int OBSTACLE_CAPACITY = 30;
    private const int FIXED_OBSTACLE_START_NUM = OBSTACLE_CAPACITY * (int)EObstacleType.FIXED;
    private const int JUMP_OBSTACLE_START_NUM = OBSTACLE_CAPACITY * (int)EObstacleType.JUMP;
    private const int FLY_OBSTACLE_START_NUM = OBSTACLE_CAPACITY * (int)EObstacleType.FLY;

    private const int MIN_FLOOR_SIZE = 4;

    private const int SPRITECOUNT = 4;
    private const int FLOOR_WIDTH_CORRECTION = 1;

    private BaseObstacle[] obstacles;
    private List<BaseObstacle> rePosObstacleList;

    private Sprite[] sprites;
    private int obstacleCount = 0;

    private float screenLeft;
    private float floorReposX = 0;
    private float flyObstacleInterval;
    private float curFlyObstacleTime = 0f;

    private int frontFlyObstacleIdx = 0;
    private float playerHalfSize = 0;

    private int prevFixedObstacleIdx = FIXED_OBSTACLE_START_NUM;
    private int prevJumpObstacleIdx = JUMP_OBSTACLE_START_NUM;
    private int prevFlyObstacleIdx = FLY_OBSTACLE_START_NUM;

    private Transform obstacleParent;

    private List<BaseObstacle> frontObstacles;
    private Queue<BaseObstacle> curActivateFloorObstacles = new Queue<BaseObstacle>();

    public Action<List<BaseObstacle>> OnChangeCurObstacles;
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

    public void SetSpeedRate(int _speed)
    {
        int count = obstacles.Length;
        int firstFlyObstacleIdx = FLY_OBSTACLE_START_NUM;

        for (int i = firstFlyObstacleIdx; i < count; i++)
        {
            BaseObstacle obstacle = obstacles[i];

            if (obstacle.GetObstacleType == EObstacleType.FLY)
            {
                obstacle.SetSpeed(_speed);
            }
        }
    }

    private void CreateObstacles()
    {
        obstacleParent = new GameObject("Obstacles").transform;

        obstacleParent.transform.position = Vector2.zero;

        obstacles = new BaseObstacle[OBSTACLE_CAPACITY * (int)EObstacleType.END];
        frontObstacles = new List<BaseObstacle>();
        rePosObstacleList = new List<BaseObstacle>();

        CreateFixedObstacles();
        CreateJumpObstacles();
        CreateFlyObstacle();

        OnChangeCurObstacles.Invoke(frontObstacles);
    }

    #region CreateObstacleGameObject
    private void CreateFixedObstacles()
    {
        GameObject originObstacleObj = (GameObject)Resources.Load(FIXED_OBSTACLE_PATH);
        GameObject[] obstacleObjs = new GameObject[OBSTACLE_CAPACITY];

        for (int i = 0; i < OBSTACLE_CAPACITY; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObstacleObj, Vector2.zero, Quaternion.identity, obstacleParent);
        }

        InitObstacles<FixedObstacle>(obstacleObjs);
    }

    private void CreateJumpObstacles()
    {
        GameObject originObstacleObj = (GameObject)Resources.Load(JUMP_OBSTACLE_PATH);
        GameObject[] obstacleObjs = new GameObject[OBSTACLE_CAPACITY];

        for (int i = 0; i < OBSTACLE_CAPACITY; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObstacleObj, Vector2.zero, Quaternion.identity, obstacleParent);
        }

        InitObstacles<JumpObstacle>(obstacleObjs);
    }

    private void CreateFlyObstacle()
    {
        GameObject originObstacleObj = (GameObject)Resources.Load(FLY_OBSTACLE_PATH);
        GameObject[] obstacleObjs = new GameObject[OBSTACLE_CAPACITY];

        for (int i = 0; i < OBSTACLE_CAPACITY; i++)
        {
            obstacleObjs[i] = GameObject.Instantiate<GameObject>(originObstacleObj, Vector2.zero, Quaternion.identity, obstacleParent);
        }

        InitObstacles<FlyObstacle>(obstacleObjs);

        frontFlyObstacleIdx = FLY_OBSTACLE_START_NUM;
    }

    #endregion

    private void InitObstacles<T>(GameObject[] _obstacles) where T : BaseObstacle , new()
    {
        int curArrayCount = _obstacles.Length + obstacleCount;

        for (int i = obstacleCount; i <  curArrayCount; i++)
        {
            BaseObstacle obstacle = new T();

            obstacle.Init(_obstacles[i - obstacleCount]);
            obstacle.SetActive(false);

            obstacles[i] = obstacle;
        }

        obstacleCount += _obstacles.Length;
    }

    public void Update()
    {
        CheckObstaclePos();

        CheckFlyObstacleInterval();


        //화살뭉치 박스그리기
        //Debug.DrawLine(new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x - obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y + obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1),
        //    new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x + obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y + obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1),Color.red);

        //Debug.DrawLine(new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x - obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y - obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1),
        //    new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x + obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y - obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1),Color.red);

        //Debug.DrawLine(new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x - obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y + obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1),
        //    new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x - obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y - obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1), Color.red);

        //Debug.DrawLine(new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x + obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y + obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1),
        //    new Vector3(obstacles[frontFlyObstacleIdx].GetPosition().x + obstacles[frontFlyObstacleIdx].GetWidth() * 0.5f, obstacles[frontFlyObstacleIdx].GetPosition().y - obstacles[frontFlyObstacleIdx].GetHeight() * 0.5f, 1), Color.red);

    }

    public void FixedUpdate()
    {
        OnActionObstacle();
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
        if (curActivateFloorObstacles.Count > 0 && CheckFrontObstacle(curActivateFloorObstacles.Peek()))
        {
            curActivateFloorObstacles.Dequeue();

            if (OnChangeCurObstacles != null)
            {
                frontObstacles[(int)EObstaclePosType.FLOOR] = curActivateFloorObstacles.Peek();

                OnChangeCurObstacles.Invoke(frontObstacles);
            }
        }

        if (CheckFrontObstacle(frontFlyObstacleIdx))
        {
            frontFlyObstacleIdx = (frontFlyObstacleIdx + 1) % OBSTACLE_CAPACITY + FLY_OBSTACLE_START_NUM;

            if (OnChangeCurObstacles != null && obstacles[frontFlyObstacleIdx].GetActive)
            {
                frontObstacles[(int)EObstaclePosType.FLY] = obstacles[frontFlyObstacleIdx];
                
                OnChangeCurObstacles.Invoke(frontObstacles);
            }
        }

        for (int i = 0; i < obstacleCount; i++)
        {
            BaseObstacle obstacle = obstacles[i];

            if(!obstacle.GetActive)
            {
                continue;
            }

            if (CheckOutsideObstacle(i))
            {
                obstacle.GetTransform.SetParent(obstacleParent);

                obstacle.SetActive(false);

                obstacle.ResetData();
            }

        }
    }
    private bool CheckFrontObstacle(BaseObstacle _obstacle)
    {
        return _obstacle.GetTransform.position.x + _obstacle.GetWidth() * 0.5f + playerHalfSize <= Vector2.zero.x;
    }

    private bool CheckFrontObstacle(int _obstacleIdx)
    {
        return obstacles[_obstacleIdx].GetTransform.position.x + obstacles[_obstacleIdx].GetWidth() * 0.5f + playerHalfSize <= Vector2.zero.x;
    }

    private bool CheckOutsideObstacle(int _obstacleIdx)
    {
        return obstacles[_obstacleIdx].GetTransform.position.x + obstacles[_obstacleIdx].GetWidth() * 0.5f <= screenLeft;
    }

    private void CheckFlyObstacleInterval()
    {
        curFlyObstacleTime += Time.deltaTime;

        if(curFlyObstacleTime >= flyObstacleInterval)
        {
            curFlyObstacleTime = 0;

            RepositionFlyObstacle();
        }
    }

    public List<BaseObstacle> OnRepositionFloor(Floor _rePosFloor)
    {
        rePosObstacleList.Clear();

        if(floorReposX == 0)
        {
            floorReposX = _rePosFloor.GetTransform.position.x;
        }

        if(_rePosFloor.GetFloorMiddleSize() > MIN_FLOOR_SIZE)
        {
            bool isRandomPos = Random.value > 0.5f;

            if (isRandomPos)
            {
                SetPosRandomPatternObstacle(_rePosFloor);
            }
            else
            {
                SetPosStrightPatternFloorObstacle(_rePosFloor);
            }

            if (frontObstacles.Count == 0 && rePosObstacleList.Count > 0)
            {
                frontObstacles.Add(rePosObstacleList[0]);
                OnChangeCurObstacles.Invoke(frontObstacles);
            }
        }

        return rePosObstacleList;
    }

    public void RepositionFlyObstacle()
    {
        if(floorReposX == 0)
        {
            return;
        }

        bool isOneObstacle = Random.value > 0.5f;

        if(isOneObstacle)
        {
            SetPosOneFlyObstacle();
        }
        else
        {
            SetPosStrightPatternFlyObstacle();
        }

        if (frontObstacles.Count == 1)
        {
            frontFlyObstacleIdx = FLY_OBSTACLE_START_NUM;
           
            frontObstacles.Add(obstacles[frontFlyObstacleIdx]);
            OnChangeCurObstacles.Invoke(frontObstacles);
        }
    }

    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }

    public void SetFlyObstacleInterval(float _intervalTime)
    {
        flyObstacleInterval = _intervalTime;
    }

    #region Patterns
    private void SetPosRandomPatternObstacle(Floor _floor)
    {
        Floor floor = _floor;

        int size = floor.GetFloorMiddleSize();

        int obstacleCount = size / MIN_FLOOR_SIZE;

        int floorDivideSize = size / obstacleCount;

        for (int i = 0; i < obstacleCount; i++)
        {
            bool isFixedObstacle = (Random.value > 0.5f);

            BaseObstacle obstacle;

            if (isFixedObstacle)
            {
                obstacle = obstacles[prevFixedObstacleIdx];
                prevFixedObstacleIdx = (prevFixedObstacleIdx + 1) % OBSTACLE_CAPACITY + FIXED_OBSTACLE_START_NUM;

                obstacle.SetSprite(sprites[Random.Range(0, SPRITECOUNT)]);

            }
            else
            {
                obstacle = obstacles[prevJumpObstacleIdx];
                prevJumpObstacleIdx = (prevJumpObstacleIdx + 1) % OBSTACLE_CAPACITY + JUMP_OBSTACLE_START_NUM;
            }

            Vector2 obstaclePos = floor.GetTransform.position;

            int halfSize = size / 2;
            float minX = i * floorDivideSize + obstacle.GetWidth() * 0.5f - halfSize;
            float maxX = i * floorDivideSize + obstacle.GetWidth() * 0.5f + floorDivideSize - FLOOR_WIDTH_CORRECTION - halfSize;
            int posX = (int)(Random.Range(minX,maxX));
            float posY = (floor.GetFloorHeight() * 0.5f) + (obstacle.GetHeight() * 0.5f);

            obstaclePos.x = posX;
            obstaclePos.y = posY;

            obstacle.GetTransform.SetParent(floor.GetTransform);
            obstacle.GetTransform.localPosition = obstaclePos;

            obstacle.SetFloorPosition(obstaclePos);

            obstacle.SetActive(true);

            rePosObstacleList.Add(obstacle);

            curActivateFloorObstacles.Enqueue(obstacle);
        }

    }

    private void SetPosStrightPatternFloorObstacle(Floor _floor)
    {
        Floor floor = _floor;

        int size = floor.GetFloorMiddleSize();

        int obstacleCount = size / MIN_FLOOR_SIZE;

        Vector2 obstaclePos = floor.GetTransform.position;

        float startPosX = (int)Random.Range(-size * 0.5f + FLOOR_WIDTH_CORRECTION, size * 0.5f - obstacleCount - FLOOR_WIDTH_CORRECTION);

        for (int i = 0; i < obstacleCount; i++)
        {
            bool isFixedObstacle = (Random.value > 0.5f);

            BaseObstacle obstacle;

            if (isFixedObstacle)
            {
                obstacle = obstacles[prevFixedObstacleIdx];
                prevFixedObstacleIdx = (prevFixedObstacleIdx + 1) % OBSTACLE_CAPACITY + FIXED_OBSTACLE_START_NUM;

                obstacle.SetSprite(sprites[Random.Range(0, SPRITECOUNT)]);

            }
            else
            {
                obstacle = obstacles[prevJumpObstacleIdx];
                prevJumpObstacleIdx = (prevJumpObstacleIdx + 1) % OBSTACLE_CAPACITY + JUMP_OBSTACLE_START_NUM;
            }

            startPosX += obstacle.GetWidth() * 0.5f;

            float posY = (floor.GetFloorHeight() * 0.5f) + (obstacle.GetHeight() * 0.5f);

            obstaclePos.x = startPosX;
            obstaclePos.y = posY;

            obstacle.GetTransform.SetParent(floor.GetTransform);
            obstacle.GetTransform.localPosition = obstaclePos;

            startPosX += obstacle.GetWidth() * 0.5f;

            obstacle.SetFloorPosition(obstaclePos);

            obstacle.SetActive(true);

            rePosObstacleList.Add(obstacle);

            curActivateFloorObstacles.Enqueue(obstacle);

        }
    }

    private void SetPosOneFlyObstacle()
    {
        BaseObstacle obstacle = obstacles[prevFlyObstacleIdx];

        Vector2 obstaclePos = Vector2.zero;

        obstaclePos.x = floorReposX;
        obstaclePos.y = Random.Range(-2, 3);

        obstacle.GetTransform.position = obstaclePos;

        obstacle.SetFloorPosition(obstaclePos);

        obstacle.SetActive(true);

        prevFlyObstacleIdx = (prevFlyObstacleIdx + 1) % OBSTACLE_CAPACITY + FLY_OBSTACLE_START_NUM;

    }

    private void SetPosStrightPatternFlyObstacle()
    {
        int obstacleCount = Random.Range(2, 5);
        int startPosY = Random.Range(-3, 7);

        FlyObstacle[] flyobstacles = new FlyObstacle[obstacleCount];

        for (int i = 0; i<obstacleCount; i++)
        {
            BaseObstacle obstacle = obstacles[prevFlyObstacleIdx];

            Vector2 obstaclePos = Vector2.zero;

            obstaclePos.x = floorReposX;
            obstaclePos.y = startPosY - i * obstacle.GetHeight();

            obstacle.GetTransform.position = obstaclePos;

            obstacle.SetFloorPosition(obstaclePos);

            obstacle.SetActive(true);

            prevFlyObstacleIdx = (prevFlyObstacleIdx + 1) % OBSTACLE_CAPACITY + FLY_OBSTACLE_START_NUM;

            flyobstacles[i] = (FlyObstacle)obstacle;
        }

        Vector2 centerPos = flyobstacles[0].GetPosition();

        centerPos.y = (flyobstacles[0].GetPosition().y + flyobstacles[obstacleCount - 1].GetPosition().y) * 0.5f;

        flyobstacles[0].SetStrightCount(obstacleCount);
        flyobstacles[0].SetStrightCenterPos(centerPos);

    }
    #endregion
}
