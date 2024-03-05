using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    IDLE = 0,
    WALK = 1,
    JUMPUP = 2,
    JUMPDOWN = 3
}

public class PlayerController
{
    private Animator anim;
    private const string PLAYERSTATE = "PlayerState";
    private const float GRAVITY = 0.45f;
    private const float LONGJUMPPOWER = 0.125f;
    private const float SHORTJUMPPOWER = 0.225f;
    private const int PLAYERSIZE = 1;
    private const float PLAYERHALFSIZE = 0.5f;
    private const string PLAYERPATH = "Prefabs/Player";
    private const float AABB_COLLECTION_VALUE = 0.25f;

    private bool isGrounded;
    private bool isDoubleJump;

    private float curShortJumpPower = 0;
    private float curLongJumpPower = 0;

    private PlayerState state;

    private Transform playerTM;
    private Floor curFloor;
    private float floorLandPosY;
    private BaseObstacle[] obstacles;
    private Coin[] coins;
    private Vector2 playerPos;

    public Action<ECoinType> OnGetCoin;

    public float GetPlayerHalfSize => PLAYERHALFSIZE;

    public void Init()
    {
        SetPlayer(GameObject.Instantiate<GameObject>((GameObject)Resources.Load(PLAYERPATH), Vector2.zero, Quaternion.identity));
    }
    
    private void SetPlayer(GameObject _player)
    {
        anim = _player.GetComponent<Animator>();
        state = PlayerState.IDLE;

        playerTM = _player.GetComponent<Transform>();
        playerPos = playerTM.position;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKey(KeyCode.UpArrow) && curShortJumpPower > 0)
        {
            LongJump();
        }
    }

    public void FixedUpdate()
    {
        if (!isGrounded)
        {
            OnJumping();
        }

        playerPos.y += curShortJumpPower + curLongJumpPower;

        playerTM.position = playerPos;

        CheckGroundAABB();
        CheckObstacleAABB();
        CheckCoinAABB();

        if (isGrounded && state != PlayerState.WALK)
        {
            Land();
        }
    }

    private void OnJumping()
    {
        curShortJumpPower -= GRAVITY * Time.deltaTime;

        if (curShortJumpPower < 0 && state != PlayerState.JUMPDOWN)
        {
            ChangeState(PlayerState.JUMPDOWN);
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            curShortJumpPower = SHORTJUMPPOWER;
            ChangeState(PlayerState.JUMPUP);
            return;
        }
        else if(!isDoubleJump)
        {
            isDoubleJump = true;
            curShortJumpPower = SHORTJUMPPOWER;
            ChangeState(PlayerState.JUMPUP);
        }
    }

    public void LongJump()
    {
        if(state != PlayerState.JUMPDOWN)
        {
            curLongJumpPower = Mathf.Lerp(curLongJumpPower, LONGJUMPPOWER, 0.01f);
        }

    }

    private void Land()
    {
        curShortJumpPower = 0;
        curLongJumpPower = 0;
        isDoubleJump = false;

        ChangeState(PlayerState.WALK);

        playerPos = playerTM.position;
        playerPos.y = floorLandPosY;
        playerTM.position = playerPos;
    }

    private void ChangeState(PlayerState _state)
    {
        state = _state;
        PlayCurStateAnim(state);
    }

    private void PlayCurStateAnim(PlayerState _state)
    {
        anim.SetInteger(PLAYERSTATE, (int)_state);
    }

    public void SetCurFloor(Floor _floor)
    {
        curFloor = _floor;
        floorLandPosY = curFloor.GetTransform.position.y + curFloor.GetFloorHeight() * 0.5f + PLAYERHALFSIZE;
    }

    public void SetObstacles(BaseObstacle[] _obstacles)
    {
        obstacles = _obstacles;
    }

    public void SetCoins(Coin[] _coins)
    {
        coins = _coins;
    }

    private void CheckGroundAABB()
    {
        if(curShortJumpPower > 0 || curFloor == null)
        {
            isGrounded = false;
            return;
        }

        Vector2 playerPos = playerTM.position;

        if(playerPos.y == floorLandPosY)
        {
            isGrounded = true;
            return;
        }

        Vector2 floorPos = curFloor.GetTransform.position;

        float floorWidthHalf = curFloor.GetFloorWidth() * 0.5f;
        float floorHeightHalf = curFloor.GetFloorHeight() * 0.5f;

        Rect playerRect = new Rect(playerPos.x - PLAYERHALFSIZE, playerPos.y - PLAYERHALFSIZE + AABB_COLLECTION_VALUE, PLAYERSIZE, AABB_COLLECTION_VALUE);
        Rect floorRect = new Rect(floorPos.x - floorWidthHalf, floorPos.y + floorHeightHalf, curFloor.GetFloorWidth(), AABB_COLLECTION_VALUE);

        isGrounded = playerRect.Overlaps(floorRect);

    }

    private void CheckObstacleAABB()
    {
        if(obstacles == null)
        {
            return;
        }

        int count = obstacles.Length;

        BaseObstacle obstacle;

        Vector2 playerPos = playerTM.position;
        Rect playerRect = new Rect(playerPos.x - PLAYERHALFSIZE, playerPos.y + PLAYERHALFSIZE, PLAYERSIZE, PLAYERSIZE);
        Rect obstacleRect = new Rect();

        for(int i = 0; i < count; i++)
        {
            obstacle = obstacles[i];

            if(!obstacle.GetActive || !obstacle.GetIsInScreen)
            {
                continue;
            }
            Vector2 obstaclePos = obstacle.GetPosition();

            float obstacleWidthHalf = obstacle.GetWidth() * 0.5f;
            float obstacleHeightHalf = obstacle.GetHeight() * 0.5f;

            obstacleRect.Set(obstaclePos.x - obstacleWidthHalf , obstaclePos.y + obstacleHeightHalf, obstacle.GetWidth(), obstacle.GetHeight());

            if (playerRect.Overlaps(obstacleRect))
            {
                Debug.Log("장애물 충돌!");
            }
        }

        
    }

    private void CheckCoinAABB()
    {
        if (coins == null)
        {
            return;
        }

        int count = coins.Length;

        Coin coin;

        Vector2 playerPos = playerTM.position;
        Rect playerRect = new Rect(playerPos.x - PLAYERHALFSIZE, playerPos.y + PLAYERHALFSIZE, PLAYERSIZE, PLAYERSIZE);
        Rect coinRect = new Rect();

        for (int i = 0; i < count; i++)
        {
            coin = coins[i];

            if(!coin.GetActive || !coin.GetIsInScreen)
            {
                continue;
            }

            Vector2 coinPos = coin.GetTransform.position;

            float coinWidthHalf = coin.GetWidth() * 0.5f;
            float coinHeightHalf = coin.GetHeight() * 0.5f;

            float coinRectX = coinPos.x - coinWidthHalf + AABB_COLLECTION_VALUE * 2;
            float coinRectY = coinPos.y + coinHeightHalf - AABB_COLLECTION_VALUE * 2;
            float coinRectWidth = coin.GetWidth() - AABB_COLLECTION_VALUE;
            float coinRectHeight = coin.GetHeight() - AABB_COLLECTION_VALUE;

            coinRect.Set(coinRectX, coinRectY, coinRectWidth, coinRectHeight);

            if(playerRect.Overlaps(coinRect))
            {
                coin.SetActive(false);
                OnGetCoin.Invoke(coin.GetCoinType);
            }
        }


    }
}
