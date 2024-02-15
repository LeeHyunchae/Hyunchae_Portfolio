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

    public override void SetSprite(Sprite _sprite)
    {
        base.SetSprite(_sprite);
        boundsX = obstacleSprite.bounds.size.x;
        boundsY = obstacleSprite.bounds.size.y;
    }

    public override void Action()
    {
        return;
    }
}
