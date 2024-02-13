using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObstacle : BaseObstacle
{
    public float speedRate = 5;

    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FLY;
    }

    public override void Action()
    {
        Vector2 pos = _transform.position;

        pos.x += (speedRate) * -1f * Time.deltaTime;

        _transform.position = pos;
    }
}
