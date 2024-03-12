using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloorController
{
    private const int INITSPEED = 3;
    private const int MININTERVAL = 3;
    private const int MAXINTERVAL = 8;
    private const string FLOORPATH = "Prefabs/Floor";
    private const int FLOORCOUNT = 10;
    private const int FIRSTFLOORSIZE = 20;
    private const int MIN_FLOOR_HEIGHT = -4;
    private const int MAX_FLOOR_HEIGHT = 4;
    private const int MIN_FLOOR_SIZE = 1;
    private const int MAX_FLOOR_SIZE = 15;

    private Floor[] floors;
    private int floorCount;

    private float speedRate = INITSPEED;
    private float screenLeft;

    private int lastFloorIdx = 0;
    private int frontFloorIdx = 0;
    private float playerHalfSize = 0;

    private Transform floorParent;

    public Action<Floor> OnChaneCurFloor;
    public Action<Floor> OnRepositionFloor;

    public void SetScreenLeft(float _screenLeft) => screenLeft = _screenLeft;
    
    public void Init()
    {
        CreateFloor();

        InitFloorPos();
    }
    private void InitFloor(GameObject[] _floors)
    {
        floorCount = _floors.Length;

        floors = new Floor[floorCount];

        for (int i = 0; i < floorCount; i++)
        {
            int floorIdx = i;

            floors[floorIdx] = new Floor();
            floors[floorIdx].Init(_floors[floorIdx]);
        }
    }
    private void InitFloorPos()
    {
        Vector2 floorPos = floors[0].GetTransform.position;
        floorPos.x = 0;
        floorPos.y = -1;
        floors[0].GetTransform.position = floorPos;
        floors[0].SetMiddleSize(FIRSTFLOORSIZE);

        if (OnChaneCurFloor != null)
        {
            OnChaneCurFloor(floors[frontFloorIdx]);
        }

        for (int i = 1; i < floorCount; i++)
        {
            RepositionFloor(i);
        }
    }

    private void CreateFloor()
    {
        floorParent = new GameObject("Floors").transform;

        floorParent.transform.position = Vector2.zero;

        GameObject originFloor = (GameObject)Resources.Load(FLOORPATH);
        GameObject[] floorObjs = new GameObject[FLOORCOUNT];

        for (int i = 0; i < FLOORCOUNT; i++)
        {
            floorObjs[i] = GameObject.Instantiate<GameObject>(originFloor, Vector2.zero, Quaternion.identity, floorParent);
        }

        InitFloor(floorObjs);
    }

    public void SetSpeedRate(int _speed)
    {
        speedRate = _speed;
    }

    public void Update()
    {
        MoveFloor();
    }

    private void MoveFloor()
    {
        for (int i = 0; i < floorCount; i++)
        {
            Floor floor = floors[i];

            Vector2 floorPos = floor.GetTransform.position;

            floorPos.x += speedRate * -1f * Time.deltaTime;

            floor.GetTransform.position = floorPos;

            if(CheckFrontFloor(i))
            {
                frontFloorIdx = (i + 1) % floorCount;
                if(OnChaneCurFloor != null)
                {
                    OnChaneCurFloor.Invoke(floors[frontFloorIdx]);
                }
            }

            if (CheckOutsideFloor(i))
            {
                RepositionFloor(i);
            }
        }
    }

    private void RepositionFloor(int _index)
    {
        floors[_index].SetMiddleSize(Random.Range(MIN_FLOOR_SIZE, MAX_FLOOR_SIZE));

        Vector2 floorPos = floors[_index].GetTransform.position;

        int randomDistance = Random.Range(MININTERVAL, MAXINTERVAL);

        floorPos.x = floors[lastFloorIdx].GetTransform.position.x + floors[lastFloorIdx].GetFloorWidth() * 0.5f + randomDistance + floors[_index].GetFloorWidth() * 0.5f;
        floorPos.y = Random.Range(MIN_FLOOR_HEIGHT, MAX_FLOOR_HEIGHT);

        floors[_index].GetTransform.position = floorPos;

        floors[_index].SetPrevFloorPos(floors[lastFloorIdx].GetTransform.position,randomDistance);

        lastFloorIdx = _index;

        if(OnRepositionFloor != null)
        {
            OnRepositionFloor.Invoke(floors[_index]);
        }
    }

    private bool CheckFrontFloor(int _idx)
    {
        int curFloorIdx = (_idx + 1) % floorCount;

        if(frontFloorIdx == curFloorIdx)
        {
            return false;
        }

        return floors[_idx].GetTransform.position.x + floors[_idx].GetFloorWidth() * 0.5f + playerHalfSize <= Vector2.zero.x;
    }

    private bool CheckOutsideFloor(int _idx)
    {
        return floors[_idx].GetTransform.position.x + floors[_idx].GetFloorWidth() <= screenLeft;
    }
    public void SetPlayerHalfSize(float _halfSize)
    {
        playerHalfSize = _halfSize;
    }
}
