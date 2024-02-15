using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObstacle : BaseObstacle
{
    private const float baseSpeedRate = 8;
    private float curSpeedRate;

    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FLY;
        curSpeedRate = baseSpeedRate;
    }

    public override void Action()
    {
        Vector2 pos = _transform.position;

        pos.x += (curSpeedRate) * -1f * Time.deltaTime;

        _transform.position = pos;
    }

    public void SetSpeedRate(int _speedRate)
    {
        curSpeedRate = baseSpeedRate + _speedRate;
    }
}
