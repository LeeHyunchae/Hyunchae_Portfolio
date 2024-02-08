using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObstacle : BaseObstacle
{
    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.JUMP;
    }

    public override void Action()
    {
        //점프하기
        throw new System.NotImplementedException();
    }
}
