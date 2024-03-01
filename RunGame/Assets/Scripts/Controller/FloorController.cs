using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// new Vector 지우기 //
// 약어 수정하기 //
// transform.position 가능하면 curPos같은걸로 캐싱해서 사용하기
// Camera.main 피하기(캐싱해서 사용하기) //
// 수식 변수화하기 //
// Rect 사용하기(AABB) ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// 플로어 관련 클래스들 구조 변경하기

public class FloorController
{
    private const int INITSPEED = 3;
    private const int MININTERVAL = 3;
    private const int MAXINTERVAL = 8;
    private const string FLOORPATH = "Prefabs/Floor";
    private const int FLOORCOUNT = 10;
    private const int FIRSTFLOORSIZE = 20;

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
        for (int i = 0; i < floorCount; i++)
        {
            int floorIdx = i;

            if (floorIdx == 0)
            {
                SetFirstFloor(floorIdx);
            }
            else
            {
                RepositionFloor(floorIdx);
            }
        }
    }

    private void CreateFloor()
    {
        floorParent = new GameObject("Floos").transform;

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
        floors[_index].SetMiddleSize(Random.Range(1, 15));

        Vector2 floorPos = floors[_index].GetTransform.position;

        int randomDistance = Random.Range(MININTERVAL, MAXINTERVAL);

        floorPos.x = floors[lastFloorIdx].GetTransform.position.x + floors[lastFloorIdx].GetFloorWidth() * 0.5f + randomDistance + floors[_index].GetFloorWidth() * 0.5f;
        floorPos.y = Random.Range(-4, 4);

        floors[_index].GetTransform.position = floorPos;

        floors[_index].SetPrevFloorPos(floors[lastFloorIdx].GetTransform.position,randomDistance);

        lastFloorIdx = _index;

        if(OnRepositionFloor != null)
        {
            OnRepositionFloor.Invoke(floors[_index]);
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
