using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObstacle : BaseObstacle
{
    private float curSpeedRate;
    private float strightCount = 1;

    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FLY;
        curSpeedRate = obstacleSpeedRate;
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
}
