using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObstacle : BaseObstacle
{
    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.FLY;
    }

    public override void Action()
    {
        //날기,,
        throw new System.NotImplementedException();
    }
}
