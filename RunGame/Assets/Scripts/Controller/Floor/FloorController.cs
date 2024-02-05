using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController
{
    private const int INITSPEED = 3;
    private const int MININTERVAL = 3;

    private Floor[] floors;
    private int floorCount;

    private float speedRate = INITSPEED;
    private float screenLeft;

    private int lastFloorIdx = 0;
    private int frontFloorIdx = 0;

    public FloorController(GameObject[] _floors)
    {
        InitFloor(_floors);

        screenLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        InitFloorPos();
    }

    private void InitFloor(GameObject[] _floors)
    {
        floorCount = _floors.Length;

        floors = new Floor[floorCount];

        for(int i = 0; i<floorCount; i++)
        {
            floors[i] = new Floor();
            floors[i].Init(_floors[i]);
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
            floors[i].GetTransform.Translate(speedRate * -1f * Time.deltaTime, 0, 0);

            if(CheckFrontFloor(i))
            {
                frontFloorIdx = (i + 1) % floorCount;
            }

            if (CheckOutsideFloor(i))
            {
                RepositionFloor(i);
            }
        }
    }

    private void RepositionFloor(int _index)
    {
        floors[_index].SetMiddleSize(Random.Range(1, 10));

        floors[_index].GetTransform.position = new Vector2(floors[lastFloorIdx].GetTransform.position.x + floors[lastFloorIdx].GetFloorWidth() * 0.5f + MININTERVAL + floors[_index].GetFloorWidth() * 0.5f,
                    Random.Range(-2, 3));

        lastFloorIdx = _index;
    }

    private void InitFloorPos()
    {
        for(int i = 0; i<floorCount;i++)
        {
            if(i == 0)
            {
                floors[i].GetTransform.position = Vector2.zero;
                floors[i].SetMiddleSize(20);
            }
            else
            {
                floors[i].SetMiddleSize(Random.Range(1, 10));

                RepositionFloor(i);

            }
        }
    }

    private bool CheckFrontFloor(int _idx)
    {
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
}
