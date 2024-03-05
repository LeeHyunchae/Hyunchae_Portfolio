using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObstacle : BaseObstacle
{
    private float curSpeedRate;
    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FLY;
        curSpeedRate = obstacleSpeedRate;

        boundsX = obstacleSprite.bounds.size.x;
        boundsY = obstacleSprite.bounds.size.y;
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

}
