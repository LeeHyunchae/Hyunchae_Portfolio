using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinController
{
    private const string COIN_PATH = "Prefabs/Coin";
    private const int MIN_INTERVAL = 1;

    private const int COIN_CAPACITY = 200;

    private Coin[] coins;

    private float screenLeft;
    private float floorReposX = 0;

    private float playerHalfSize = 0;

    private int prevCoinIdx;

    private Transform coinParent;

    private List<Coin> frontCoins;
    private Queue<Coin> curActivateCoins = new Queue<Coin>();

    public Action<List<Coin>> OnChangeCurCoins;

    public void SetScreenLeft(float _screenLeft) => screenLeft = _screenLeft;

    public void Init()
    {
        CreateCoins();

    }

    private void CreateCoins()
    {
        coinParent = new GameObject("Coins").transform;

        coinParent.transform.position = Vector2.zero;

        coins = new Coin[COIN_CAPACITY];
        frontCoins = new List<Coin>();

        CreateCoinObject();

        OnChangeCurCoins.Invoke(frontCoins);

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
        if (curActivateCoins.Count > 0 && CheckFrontCoin(curActivateCoins.Peek()))
        {
            curActivateCoins.Dequeue();

            if (OnChangeCurCoins != null)
            {
                frontCoins[0] = curActivateCoins.Peek();

                OnChangeCurCoins.Invoke(frontCoins);
            }
        }

        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            Coin coin = coins[i];

            if (!coin.GetActive)
            {
                continue;
            }

            if (CheckOutsideCoin(i))
            {
                coin.GetTransform.SetParent(coinParent);

                coin.SetActive(false);
            }

        }
    }
    private bool CheckFrontCoin(Coin _coin)
    {
        return _coin.GetTransform.position.x + _coin.GetWidth() * 0.5f + playerHalfSize <= Vector2.zero.x;
    }

    private bool CheckOutsideCoin(int _coinIdx)
    {
        return coins[_coinIdx].GetTransform.position.x + coins[_coinIdx].GetWidth() * 0.5f <= screenLeft;
    }

    public void OnRepositionFloor(Floor _rePosFloor, List<BaseObstacle> _obstacles)
    {
        if (frontCoins.Count == 0)
        {
            frontCoins.Add(coins[0]);
            OnChangeCurCoins.Invoke(frontCoins);
        }

        if (floorReposX == 0)
        {
            floorReposX = _rePosFloor.GetTransform.position.x;
        }

        if(_obstacles.Count == 0)
        {
            SetStrightRandomCoinPattern(_rePosFloor);
        }
        else
        {
            SetSemiCirclePattern(_rePosFloor, _obstacles);
        }

    }

    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }

    private void SetStrightRandomCoinPattern(Floor _floor)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        for (int i = 0; i < size; i++)
        {
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            coinPos.x = floor.GetTransform.position.x + -(size * 0.5f) + coin.GetWidth() * 0.5f + i;
            coinPos.y = floor.GetTransform.position.y + (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.position = coinPos;

            coin.SetFloorPosition(coinPos);

            coin.SetActive(true);

            curActivateCoins.Enqueue(coin);
        }

    }

    private void SetSemiCirclePattern(Floor _floor, List<BaseObstacle> _obstacles)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        int obstacleIdx = 0;

        int obstaclesCount = _obstacles.Count -1;

        float distance = 0;

        for (int i = 0; i < size; i++)
        {
            Vector2 obstaclePos = _obstacles[obstacleIdx].GetTransform.position;
            float obstacleHalfWidth = _obstacles[obstacleIdx].GetWidth() * 0.5f;
                
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            coinPos.x = floor.GetTransform.position.x + -(size * 0.5f) + coin.GetWidth() * 0.5f + i;

            if (coinPos.x > obstaclePos.x && obstacleIdx != obstaclesCount)
            {
                obstacleIdx = (obstacleIdx + 1) % _obstacles.Count;
                obstaclePos = _obstacles[obstacleIdx].GetTransform.position;
            }

            distance = Mathf.Abs(coinPos.x - obstaclePos.x);

            coinPos.y = floor.GetTransform.position.y + (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            
            if (distance < MIN_INTERVAL + obstacleHalfWidth)
            {
                coinPos.y += 2;
            }

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.position = coinPos;

            coin.SetFloorPosition(coinPos);

            coin.SetActive(true);

            curActivateCoins.Enqueue(coin);
        }

    }

    private void SetSquarePattern(Floor _floor)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        for (int i = 0; i < size; i++)
        {
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            coinPos.x = floor.GetTransform.position.x + -(size * 0.5f) + coin.GetWidth() * 0.5f + i;
            coinPos.y = floor.GetTransform.position.y + (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.position = coinPos;

            coin.SetFloorPosition(coinPos);

            coin.SetActive(true);

            curActivateCoins.Enqueue(coin);
        }

    }
}
