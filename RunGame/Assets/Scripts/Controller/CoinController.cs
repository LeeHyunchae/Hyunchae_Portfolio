using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ECoinPosType
{
    FLOOR = 0,
    PATTERN = 1
}

public class CoinController
{
    private const string COIN_PATH = "Prefabs/Coin";
    private const float MIN_OBSTACLE_INTERVAL = 0.6f;
    private const int MIN_FLOOR_INTERVAL = 4;

    private const int COIN_CAPACITY = 400;

    private Coin[] coins;

    private float screenLeft;
    private float screenRight;

    private float playerHalfSize = 0;

    private int prevCoinIdx;

    private Transform coinParent;


    public void Init()
    {
        CreateCoins();

    }

    public void SetScreenLeftRight(float _screenLeft, float _screenRight)
    {
        screenLeft = _screenLeft;
        screenRight = _screenRight;
    }

    public Coin[] GetCoins => coins;

    private void CreateCoins()
    {
        coinParent = new GameObject("Coins").transform;

        coinParent.transform.position = Vector2.zero;

        coins = new Coin[COIN_CAPACITY];

        CreateCoinObject();


    }

    private void CreateCoinObject()
    {
        GameObject originCoinObj = (GameObject)Resources.Load(COIN_PATH);
        GameObject[] coinObjs = new GameObject[COIN_CAPACITY];

        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            coinObjs[i] = GameObject.Instantiate<GameObject>(originCoinObj, Vector2.zero, Quaternion.identity, coinParent);
        }

        InitCoins(coinObjs);
    }

    private void InitCoins(GameObject[] _coins)
    {

        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            Coin coin = new Coin();

            coin.Init(_coins[i]);
            coin.SetActive(false);

            coins[i] = coin;
        }
    }

    public void Update()
    {
        CheckCoinPos();

    }

    private void CheckCoinPos()
    {

        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            Coin coin = coins[i];

            if (!coin.GetActive)
            {
                continue;
            }

            if(CheckInScreenCoin(coin))
            {
                coin.SetIsInScreen(true);
            }

            if (CheckOutsideCoin(coin))
            {
                coin.GetTransform.SetParent(coinParent);

                coin.SetActive(false);
                coin.SetIsInScreen(false);
            }

        }
    }
    private bool CheckInScreenCoin(Coin _coin)
    {
        return _coin.GetTransform.position.x < screenRight;
    }

    private bool CheckOutsideCoin(Coin _coin)
    {
        return _coin.GetTransform.position.x + _coin.GetWidth() * 0.5f <= screenLeft;
    }

    public void OnRepositionFloor(Floor _rePosFloor, List<BaseObstacle> _obstacles)
    {
        if (_rePosFloor.GetPrevFloorDistance > MIN_FLOOR_INTERVAL)
        {
            bool isSquarePattern = Random.value > 0.5f;

            SetPosSquarePattern(_rePosFloor);

            //if (isSquarePattern)
            //{
            //    SetPosSquarePattern(_rePosFloor);
            //}
        }

        if (_obstacles.Count == 0)
        {
            SetPosStrightRandomCoinPattern(_rePosFloor);
        }
        else
        {
            SetPosObstaclePattern(_rePosFloor, _obstacles);
        }


    }

    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }

    private void SetPosStrightRandomCoinPattern(Floor _floor)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        for (int i = 0; i <= size; i++)
        {
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            float result = -(size * 0.5f) + i;
            coinPos.x = (result < 0) ? (int)(result) : (int)(result + 0.5f);

            if(size % 2 != 0)
            {
                coinPos.x -= 0.5f;
            }
            coinPos.y = (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.localPosition = coinPos;

            coin.SetActive(true);
        }

    }

    private void SetPosObstaclePattern(Floor _floor, List<BaseObstacle> _obstacles)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        int obstacleIdx = 0;

        int obstaclesCount = _obstacles.Count -1;

        for (int i = 0; i <= size; i++)
        {
            Vector2 obstaclePos = _obstacles[obstacleIdx].GetTransform.localPosition;
            float obstacleHalfWidth = _obstacles[obstacleIdx].GetWidth() * 0.5f;
                
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            float result = -(size * 0.5f) + i;
            coinPos.x = (result < 0) ? (int)(result) : (int)(result + 0.5f);

            if (size % 2 != 0)
            {
                coinPos.x -= coin.GetWidth() * 0.5f;
            }

            coinPos.y = (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            float distance = Mathf.Abs(coinPos.x - obstaclePos.x);

            if (distance < MIN_OBSTACLE_INTERVAL + obstacleHalfWidth)
            {
                coinPos.y += 2;
            }
            
            if(coinPos.x > obstaclePos.x && obstacleIdx != obstaclesCount)
            {
                obstacleIdx++;
            }

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.localPosition = coinPos;

            coin.SetActive(true);
        }

    }

    private void SetPosSquarePattern(Floor _floor)
    {
        Floor floor = _floor;

        int squareSize = Random.Range(MIN_FLOOR_INTERVAL, _floor.GetPrevFloorDistance);

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        Vector2 coinPos = _floor.GetTransform.position;
        coinPos.x = coinPos.x - _floor.GetFloorWidth() * 0.5f - _floor.GetPrevFloorDistance * 0.5f - (squareSize * 0.5f);
        coinPos.x -= coins[0].GetWidth() * 0.5f;
        coinPos.y = _floor.GetPrevFloorPos.y + squareSize + MIN_FLOOR_INTERVAL;

        float startPosY = coinPos.y;

        for (int i = 0; i < squareSize * squareSize; i++)
        {
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            if (i % squareSize == 0)
            {
                coinPos.x += coin.GetWidth();
            }

            coinPos.y = startPosY - (coin.GetHeight() * (i % squareSize));

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.position = coinPos;

            coin.SetActive(true);
        }

    }
}
