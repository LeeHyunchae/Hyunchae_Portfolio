using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObstacle : BaseObstacle
{
    private float curSpeedRate;
    private float strightCount = 0;
    private Vector2 centerPos;

    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FLY;
        curSpeedRate = obstacleSpeedRate;
    }

    public override Vector2 GetPosition()
    {
        if(strightCount != 0)
        {
            centerPos.x = _transform.position.x;
            return centerPos;
        }
        else
        {
            return base.GetPosition();
        }
    }
    public override void Action()
    {
        Vector2 pos = _transform.position;

        pos.x += (curSpeedRate) * -1f * Time.deltaTime;

        _transform.position = pos;
    }

    public override void SetSpeed(int _speedRate)
    {
        curSpeedRate = obstacleSpeedRate + _speedRate;
    }

    public void SetStrightCount(int _count)
    {
        strightCount = _count;
    }

    public void SetStrightCenterPos(Vector2 _centerPos)
    {
        centerPos = _centerPos;
    }

    public override void ResetData()
    {
        strightCount = 0;
        centerPos = Vector2.zero;
    }

    public override float GetHeight()
    {
        if (strightCount != 0)
        {
            return boundsY * strightCount;
        }
        else
        {
            return base.GetHeight();
        }
    }
}
