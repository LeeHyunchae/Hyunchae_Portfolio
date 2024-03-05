using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObstacle : BaseObstacle
{
    private const float GRAVITY = 0.15f;
    private const int MININTERVAL = 1;
    private const int MAXINTERVAL = 5;
    private const float MIN_JUMP_HEIGHT = 0.1f;
    private const float MAX_JUMP_HEIGHT = 0.15f;

    private float jumpHeight = 0.1f;
    private float jumpPower = 0;

    private float jumpInterval = 0;
    private float jumpTime = 0;
    private bool isJumping = false;
    private float posY;
    private Vector2 obstacleLocalPos;

    public override void Init(GameObject _obstacleObj)
    {
        base.Init(_obstacleObj);
        obstacleType = EObstacleType.JUMP;
        posY = _transform.position.y;

        boundsX = obstacleSprite.bounds.size.x;
        boundsY = obstacleSprite.bounds.size.y;

        obstacleLocalPos = _transform.position;
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

            obstacleLocalPos.x = floorPosX;
            obstacleLocalPos.y = floorPosY + posY;

            _transform.localPosition = obstacleLocalPos;
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

            obstacleLocalPos.x = floorPosX;
            obstacleLocalPos.y = floorPosY;

            _transform.localPosition = obstacleLocalPos;
        }    

    }

    public override void ResetData()
    {
        base.ResetData();

        jumpInterval = Random.Range(MININTERVAL, MAXINTERVAL);
        jumpHeight = Random.Range(MIN_JUMP_HEIGHT, MAX_JUMP_HEIGHT);
    }
}
