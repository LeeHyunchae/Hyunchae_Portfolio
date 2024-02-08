using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    private const int LEFT = 0;
    private const int MIDDLE = 1;
    private const int RIGHT = 2;
    private const float FLOOR_HALF_SIZE = 0.5f;

    private SpriteRenderer[] floors = new SpriteRenderer[3];
    private Transform _transform;

    public Transform GetTransform => _transform;
    
    public void Init(GameObject _floorObj)
    {
        floors = _floorObj.GetComponentsInChildren<SpriteRenderer>();
        _transform = _floorObj.GetComponent<Transform>();
        SetMiddleSize(1);
    }

    public void SetMiddleSize(int _middleSize)
    {
        floors[MIDDLE].size = new Vector2(_middleSize, 1);

        floors[LEFT].transform.localPosition = new Vector2(-_middleSize * 0.5f - 0.5f, 0);
        floors[RIGHT].transform.localPosition = new Vector2(_middleSize * 0.5f + 0.5f, 0);
    }

    public int GetFloorWidth()
    {
        return (int)(floors[LEFT].size.x + floors[MIDDLE].size.x + floors[RIGHT].size.x);
    }

    public float GetFloorHeight()
    {
        return 1;
    }

    public int GetFloorMiddleSize()
    {
        return (int)floors[MIDDLE].size.x;
    }
}
