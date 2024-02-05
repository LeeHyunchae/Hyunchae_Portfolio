using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Translate 제거하기 transform 직접 작동하기

public enum PlayerState
{
    IDLE = 0,
    WALK = 1,
    JUMPUP = 2,
    JUMPDOWN = 3
}

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private const string PLAYERSTATE = "PlayerState";
    private const float GRAVITY = 0.15f;
    private const float LONGJUMPPOWER = 0.125f;
    private const float PLAYERHALFSIZE = 0.5f;

    private bool isGrounded;

    private float shortJumpPower = 0;
    private float shortJumpHeight = 0.125f;
    private float curLongJumpPower = 0;

    private PlayerState state;

    private SpriteRenderer playerSpriteRenderer;
    private Transform playerTM;
    public Floor floor;

    
    public void SetPlayer(GameObject _player)
    {
        anim = _player.GetComponent<Animator>();
        state = PlayerState.IDLE;

        playerTM = _player.GetComponent<Transform>();
        playerSpriteRenderer = _player.GetComponent<SpriteRenderer>();

    }

    public void Update()
    {
        if (!isGrounded)
        {
            shortJumpPower -= GRAVITY * Time.deltaTime;

            if (shortJumpPower < 0 && state != PlayerState.JUMPDOWN)
            {
                ChangeState(PlayerState.JUMPDOWN);
            }

        }

        playerTM.Translate(0, shortJumpPower + curLongJumpPower, 0);

        CheckGroundAABB();


        if (isGrounded && state != PlayerState.WALK)
        {
            shortJumpPower = 0;
            curLongJumpPower = 0;
            playerTM.position = new Vector2(0, floor.GetTransform.position.y + floor.GetFloorHeight());
            ChangeState(PlayerState.WALK);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKey(KeyCode.UpArrow) && shortJumpPower > 0)
        {
            curLongJumpPower = Mathf.Lerp(curLongJumpPower, LONGJUMPPOWER, 0.01f);
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
        floor = _floor;
    }

    private void CheckGroundAABB()
    {
        if(shortJumpPower > 0 || floor == null)
        {
            isGrounded = false;
            return;
        }

        Vector2 playerPos = playerTM.position;
        Vector2 floorPos = floor.GetTransform.position;


        if(floorPos.x - floor.GetFloorWidth() * 0.5 < playerPos.x + PLAYERHALFSIZE &&
            floorPos.x + floor.GetFloorWidth() * 0.5f > playerPos.x - PLAYERHALFSIZE &&
            floorPos.y - floor.GetFloorHeight() * 0.5f < playerPos.y + PLAYERHALFSIZE &&
            floorPos.y + floor.GetFloorHeight() * 0.5f >= playerPos.y - PLAYERHALFSIZE)
        {
            isGrounded = true;
            Debug.Log("충돌");
            
        }
        else
        {
            isGrounded = false;
            Debug.Log("비충돌");

        }
    }

    public Vector2 GetPlayerPos()
    {
        return playerTM.position;
    }
}
