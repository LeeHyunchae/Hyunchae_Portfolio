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
    private const float PLAYERHALFSIZE = 0.5f;
    private const string PLAYERPATH = "Prefabs/Player";
    private const float PLAYER_HEIGHT_COLLECTION_VALUE = 0.1f;
    private const float FLOOR_HEIGHT_COLLECTION_VALUE = 0.2f;

    private bool isGrounded;
    private bool isDoubleJump;

    private float shortJumpPower = 0;
    private float shortJumpHeight = 0.225f;
    private float curLongJumpPower = 0;
    private float floorWidth = 0;
    private float floorHeight = 0;
    private float coinWidth = 0;
    private float coinHeight = 0;

    private PlayerState state;

    private Transform playerTM;
    private Floor curFloor;
    private BaseObstacle[] obstacles;
    private Coin[] coins;
    private Vector2 playerPos;

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
        else if (Input.GetKey(KeyCode.UpArrow) && shortJumpPower > 0)
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

        playerPos.y += shortJumpPower + curLongJumpPower;

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
        shortJumpPower -= GRAVITY * Time.deltaTime;

        if (shortJumpPower < 0 && state != PlayerState.JUMPDOWN)
        {
            ChangeState(PlayerState.JUMPDOWN);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            shortJumpPower = shortJumpHeight;
            ChangeState(PlayerState.JUMPUP);
            return;
        }
        else if(!isDoubleJump)
        {
            isDoubleJump = true;
            shortJumpPower = shortJumpHeight;
            Debug.Log("더블점프");
            ChangeState(PlayerState.JUMPUP);
        }
    }

    private void LongJump()
    {
        if(state != PlayerState.JUMPDOWN)
        {
            curLongJumpPower = Mathf.Lerp(curLongJumpPower, LONGJUMPPOWER, 0.01f);
        }

    }

    private void Land()
    {
        shortJumpPower = 0;
        curLongJumpPower = 0;
        isDoubleJump = false;

        ChangeState(PlayerState.WALK);

        playerPos = playerTM.position;
        playerPos.y = (int)(curFloor.GetTransform.position.y + curFloor.GetFloorHeight() * 0.5f + PLAYERHALFSIZE);
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
        floorWidth = curFloor.GetFloorWidth();
        floorHeight = curFloor.GetFloorHeight();
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
        if(shortJumpPower > 0 || curFloor == null)
        {
            isGrounded = false;
            return;
        }

        Vector2 playerPos = playerTM.position;
        Vector2 floorPos = curFloor.GetTransform.position;

        if (floorPos.x - floorWidth * 0.5 < playerPos.x + PLAYERHALFSIZE &&
            floorPos.x + floorWidth * 0.5f > playerPos.x - PLAYERHALFSIZE &&
            floorPos.y + floorHeight * FLOOR_HEIGHT_COLLECTION_VALUE < playerPos.y - PLAYERHALFSIZE + PLAYER_HEIGHT_COLLECTION_VALUE &&
            floorPos.y + floorHeight * 0.5f >= playerPos.y - PLAYERHALFSIZE)
        {
            isGrounded = true;
            
        }
        else
        {
            isGrounded = false;
        }

    }

    private void CheckObstacleAABB()
    {
        if(obstacles == null)
        {
            return;
        }

        int count = obstacles.Length;

        BaseObstacle obstacle;

        for(int i = 0; i < count; i++)
        {
            obstacle = obstacles[i];

            if(!obstacle.GetActive || !obstacle.GetIsInScreen)
            {
                continue;
            }

            coinWidth = obstacle.GetWidth();
            coinHeight = obstacle.GetHeight();

            Vector2 playerPos = playerTM.position;
            Vector2 obstaclePos = obstacle.GetPosition();

            if (obstaclePos.x - coinWidth * 0.5 < playerPos.x + PLAYERHALFSIZE &&
                obstaclePos.x + coinWidth * 0.5f > playerPos.x - PLAYERHALFSIZE &&
                obstaclePos.y - coinHeight * 0.5f < playerPos.y + PLAYERHALFSIZE &&
                obstaclePos.y + coinHeight * 0.5f >= playerPos.y - PLAYERHALFSIZE)
            {
                Debug.Log("장애물 충돌!!");
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

        for (int i = 0; i < count; i++)
        {
            coin = coins[i];

            if(!coin.GetActive || !coin.GetIsInScreen)
            {
                continue;
            }

            coinWidth = coin.GetWidth();
            coinHeight = coin.GetHeight();

            Vector2 playerPos = playerTM.position;
            Vector2 coinPos = coin.GetTransform.position;

            if (coinPos.x - coinWidth * 0.3f < playerPos.x + PLAYERHALFSIZE &&
                coinPos.x + coinWidth * 0.3f > playerPos.x - PLAYERHALFSIZE &&
                coinPos.y - coinHeight * 0.3f < playerPos.y + PLAYERHALFSIZE &&
                coinPos.y + coinHeight * 0.3f >= playerPos.y - PLAYERHALFSIZE)
            {
                Debug.Log("동전 충돌!!");
                coin.SetActive(false);
            }
        }


    }
}
