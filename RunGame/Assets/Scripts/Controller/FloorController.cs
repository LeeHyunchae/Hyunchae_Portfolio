using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloorController
{
    private const int INITSPEED = 3;
    private const int MININTERVAL = 3;
    private const string FLOORPATH = "Prefabs/Floor";
    private const int FLOORCOUNT = 10;
    private const int FIRSTFLOORSIZE = 20;

    private Floor[] floors;
    private int floorCount;

    private float speedRate = INITSPEED;
    private float screenLeft;

    private int lastFloorIdx = 0;
    private int frontFloorIdx = 0;

    public Action<Floor> OnChaneCurFloor;
    public Action<Floor> OnRepositionFloor;
    
    public void Init()
    {
        CreateFloor();

        screenLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        InitFloorPos();
    }

    private void CreateFloor()
    {
        GameObject originFloor = (GameObject)Resources.Load(FLOORPATH);
        GameObject[] floorObjs = new GameObject[FLOORCOUNT];

        for (int i = 0; i < FLOORCOUNT; i++)
        {
            floorObjs[i] = GameObject.Instantiate<GameObject>(originFloor, Vector2.zero, Quaternion.identity);
        }

        InitFloor(floorObjs);
    }

    private void InitFloor(GameObject[] _floors)
    {
        floorCount = _floors.Length;

        floors = new Floor[floorCount];

        for(int i = 0; i<floorCount; i++)
        {
            int floorIdx = i;

            floors[floorIdx] = new Floor();
            floors[floorIdx].Init(_floors[floorIdx]);
        }
    }

    public void Update()
    {
        MoveFloor();
    }

    private void MoveFloor()
    {
        for (int i = 0; i < floorCount; i++)
        {
            int floorIdx = i;

            Vector2 floorPos = floors[floorIdx].GetTransform.position;

            floorPos.x += speedRate * -1f * Time.deltaTime;

            floors[floorIdx].GetTransform.position = floorPos;

            if(CheckFrontFloor(floorIdx))
            {
                frontFloorIdx = (floorIdx + 1) % floorCount;
                if(OnChaneCurFloor != null)
                {
                    OnChaneCurFloor.Invoke(floors[frontFloorIdx]);
                }
            }

            if (CheckOutsideFloor(floorIdx))
            {
                RepositionFloor(floorIdx);
            }
        }
    }

    private void RepositionFloor(int _index)
    {
        floors[_index].SetMiddleSize(Random.Range(1, 10));

        floors[_index].GetTransform.position =
            new Vector2(floors[lastFloorIdx].GetTransform.position.x + floors[lastFloorIdx].GetFloorWidth() * 0.5f + MININTERVAL + floors[_index].GetFloorWidth() * 0.5f
            ,Random.Range(-4, 4));

        lastFloorIdx = _index;

        if(OnRepositionFloor != null)
        {
            OnRepositionFloor(floors[_index]);
        }
    }

    private void InitFloorPos()
    {
        for(int i = 0; i<floorCount;i++)
        {
            int floorIdx = i;

            if(floorIdx == 0)
            {
                SetFirstFloor(floorIdx);
            }
            else
            {
                floors[floorIdx].SetMiddleSize(Random.Range(1, 10));

                RepositionFloor(floorIdx);

            }
        }
    }

    private void SetFirstFloor(int _idx)
    {
        floors[_idx].GetTransform.position = new Vector2(0, -1);
        floors[_idx].SetMiddleSize(FIRSTFLOORSIZE);

        if (OnChaneCurFloor != null)
        {
            OnChaneCurFloor(floors[frontFloorIdx]);
        }

    }

    private bool CheckFrontFloor(int _idx)
    {
        int curFloorIdx = (_idx + 1) % floorCount;

        if(frontFloorIdx == curFloorIdx)
        {
            return false;
        }

        return floors[_idx].GetTransform.position.x + floors[_idx].GetFloorWidth() * 0.5f <= Vector2.zero.x;
    }

    private bool CheckOutsideFloor(int _idx)
    {
        return floors[_idx].GetTransform.position.x + floors[_idx].GetFloorWidth() * 0.5f <= screenLeft;
    }

    public Floor GetFrontFloor()
    {
        return floors[frontFloorIdx];
    }

    public Floor[] GetAllfFloor()
    {
        return floors;
    }
}
