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

public class PlayerController : MonoBehaviour
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

    private PlayerState state;

    private Transform playerTM;
    public Floor floor;
    private Vector2 playerPos;

    public void Init()
    {
        SetPlayer(Instantiate<GameObject>((GameObject)Resources.Load(PLAYERPATH), Vector2.zero, Quaternion.identity));
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
            shortJumpPower -= GRAVITY * Time.deltaTime;

            if (shortJumpPower < 0 && state != PlayerState.JUMPDOWN)
            {
                ChangeState(PlayerState.JUMPDOWN);
            }

        }

        playerPos.y += shortJumpPower + curLongJumpPower;

        playerTM.position = playerPos;

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
        floorWidth = floor.GetFloorWidth();
        floorHeight = floor.GetFloorHeight();
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

    public Vector2 GetPlayerPos()
    {
        return playerTM.position;
    }
}
