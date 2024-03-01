using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObstacle : BaseObstacle
{
    private const float GRAVITY = 0.15f; 
    private float jumpHeight = 0.1f;
    private float jumpPower = 0;

    private float jumpInterval = 0;
    private float jumpTime = 0;
    private bool isJumping = false;
    private float posY;

    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.JUMP;
        posY = _transform.position.y;

        ResetData();
    }

    public override void Action()
    {
        if(!obstacleSprite.enabled)
        {
            return;
        }

        if(!isJumping)
        {
            jumpTime += Time.deltaTime;
        }
        else if(isJumping)
        {
            jumpPower -= GRAVITY * Time.deltaTime;

            posY += jumpPower;

            _transform.localPosition = new Vector2(floorPosX,floorPosY + posY);
        }

        if(jumpTime >= jumpInterval && !isJumping)
        {
            jumpPower = jumpHeight;
            isJumping = true;
            jumpTime = 0;
        }

        if(_transform.localPosition.y <= floorPosY && jumpPower < 0)
        {
            isJumping = false;
            _transform.localPosition = new Vector2(floorPosX, floorPosY);
        }    

    }

    public override void SetFloorPosition(Vector2 _floorPos)
    {
        base.SetFloorPosition(_floorPos);
    }

    public override void ResetData()
    {
        base.ResetData();

        jumpInterval = Random.Range(1, 5);
        jumpHeight = Random.Range(0.1f, 0.15f);
    }
}
