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
    private const float GRAVITY = 0.15f;
    private const float LONGJUMPPOWER = 0.1f;
    private const float PLAYERHALFSIZE = 0.5f;
    private const string PLAYERPATH = "Prefabs/Player";
    private const float PLAYER_HEIGHT_COLLECTION_VALUE = 0.1f;
    private const float FLOOR_HEIGHT_COLLECTION_VALUE = 0.4f;

    private bool isGrounded;

    private float shortJumpPower = 0;
    private float shortJumpHeight = 0.1f;
    private float curLongJumpPower = 0;
    private float floorWidth = 0;
    private float floorHeight = 0;
    private float obsWidth = 0;
    private float obsHeight = 0;

    private PlayerState state;

    private Transform playerTM;
    private Floor curFloor;
    private List<BaseObstacle> curObstacles;
    private List<Coin> curCoins;
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
        if (!isGrounded)
        {
            Jumping();
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

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKey(KeyCode.UpArrow) && shortJumpPower > 0)
        {
            LongJump();
        }


    }

    private void Jumping()
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
        }
    }

    private void LongJump()
    {
        curLongJumpPower = Mathf.Lerp(curLongJumpPower, LONGJUMPPOWER, 0.01f);
    }

    private void Land()
    {
        shortJumpPower = 0;
        curLongJumpPower = 0;

        Vector2 playerPos = playerTM.position;
        playerPos.y = curFloor.GetTransform.position.y + curFloor.GetFloorHeight() * 0.5f + PLAYERHALFSIZE;
        playerTM.position = playerPos;
        ChangeState(PlayerState.WALK);
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

    public void SetCurObstacle(List<BaseObstacle> _obstacles)
    {
        curObstacles = _obstacles;
        
    }

    public void SetCurCoin(List<Coin> _coins)
    {
        curCoins = _coins;
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
        if(curObstacles == null)
        {
            return;
        }

        int count = curObstacles.Count;

        BaseObstacle obstacle;

        for(int i = 0; i < count; i++)
        {
            obstacle = curObstacles[i];

            obsWidth = obstacle.GetWidth();
            obsHeight = obstacle.GetHeight();

            Vector2 playerPos = playerTM.position;
            Vector2 obsPos = obstacle.GetTransform.position;

            if (obsPos.x - obsWidth * 0.5 < playerPos.x + PLAYERHALFSIZE &&
                obsPos.x + obsWidth * 0.5f > playerPos.x - PLAYERHALFSIZE &&
                obsPos.y - obsHeight * 0.5f < playerPos.y + PLAYERHALFSIZE &&
                obsPos.y + obsHeight * 0.5f >= playerPos.y - PLAYERHALFSIZE)
            {
                Debug.Log("장애물 충돌!!");
            }
        }

        
    }

    private void CheckCoinAABB()
    {
        if (curCoins == null)
        {
            return;
        }

        int count = curCoins.Count;

        Coin coin;

        for (int i = 0; i < count; i++)
        {
            coin = curCoins[i];

            obsWidth = coin.GetWidth();
            obsHeight = coin.GetHeight();

            Vector2 playerPos = playerTM.position;
            Vector2 obsPos = coin.GetTransform.position;

            if (obsPos.x - obsWidth * 0.5 < playerPos.x + PLAYERHALFSIZE &&
                obsPos.x + obsWidth * 0.5f > playerPos.x - PLAYERHALFSIZE &&
                obsPos.y - obsHeight * 0.5f < playerPos.y + PLAYERHALFSIZE &&
                obsPos.y + obsHeight * 0.5f >= playerPos.y - PLAYERHALFSIZE)
            {
                Debug.Log("동전 충돌!!");
                coin.SetActive(false);
            }
        }


    }

    public Vector2 GetPlayerPos()
    {
        return playerTM.position;
    }
}
