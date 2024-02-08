using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FixedObstacle : BaseObstacle
{
    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FIXED;
    }
    public override void Action()
    {
        throw new System.NotImplementedException();
    }
}
