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
    private const float GRAVITY = 0.49f;
    private const float LONGJUMPPOWER = 0.125f;
    private const int GROUNDSPRITESIZE = 16;
    private const float AABBHEIGHTLIMIT = 0.5f;

    private bool isGrounded;

    private float shortJumpPower = 0;
    private float shortJumpHeight = 0.125f;
    private float curLongJumpPower = 0;

    private PlayerState state;

    private SpriteRenderer playerSpriteRenderer;
    private Transform playerTM;
    public Transform groundTM;

    private float playerHalfWidth;
    private float playerHalfHeight;
    private float playerPivotY;

    private float groundHalfWidth;
    private float groundHalfHeight;
    private float groundSpritePivotY;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        state = PlayerState.IDLE;

        playerTM = GetComponent<Transform>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        SetPlayerBoxSize();
        SetCurGround(groundTM);

    }

    private void Update()
    {
        //스타트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = PlayerState.WALK;
        }

        if (!isGrounded)
        {
            shortJumpPower -= GRAVITY * Time.deltaTime;

            if(shortJumpPower < 0 && state != PlayerState.JUMPDOWN)
            {
                ChangeState(PlayerState.JUMPDOWN);
            }

        }

        playerTM.Translate(0,shortJumpPower + curLongJumpPower,0);

        CheckGroundAABB();

        //Debug.DrawLine(new Vector3(groundTM.position.x - groundHalfWidth,groundTM.position.y -( groundHalfHeight * (1 - groundSpritePivotY)), 1),
        //    new Vector3(groundTM.position.x + groundHalfWidth, groundTM.position.y - (groundHalfHeight * (1 - groundSpritePivotY)), 1),Color.red);

        //Debug.DrawLine(new Vector3(groundTM.position.x - groundHalfWidth, groundTM.position.y, 1),
        //    new Vector3(groundTM.position.x + groundHalfWidth, groundTM.position.y, 1), Color.black);

        if (isGrounded && state != PlayerState.WALK)
        {
            shortJumpPower = 0;
            curLongJumpPower = 0;
            playerTM.position = Vector2.zero;
            ChangeState(PlayerState.WALK);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
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

    private void SetPlayerBoxSize()
    {
        playerHalfWidth = playerSpriteRenderer.bounds.extents.x;
        playerHalfHeight = playerSpriteRenderer.bounds.extents.y;
        playerPivotY = playerSpriteRenderer.sprite.pivot.y / playerSpriteRenderer.sprite.rect.height;
        
    }

    private void SetCurGround(Transform _ground)
    {
        groundTM = _ground;
        int groundCount = groundTM.childCount;
        groundHalfWidth = groundCount * 0.5f;
        //height 바꿔야함
        groundHalfHeight = 0;
        groundSpritePivotY = 1 / GROUNDSPRITESIZE;
    }

    private void CheckGroundAABB()
    {
        if(shortJumpPower > 0)
        {
            isGrounded = false;
            return;
        }

        Vector2 playerPos = playerTM.position;
        Vector2 groundPos = groundTM.position;

        //playerPos.y -= playerHalfHeight * (1 - playerPivotY);
        //groundPos.y -= groundHalfHeight * (1 - groundSpritePivotY);

        // 두 사각형의 중심 간의 거리 계산
        float deltaX = Mathf.Abs(playerPos.x - groundPos.x);
        float deltaY = Mathf.Abs(playerPos.y - groundPos.y);

        // 각 축별로 겹치는지 여부 확인
        isGrounded = deltaX < (playerHalfWidth + groundHalfWidth);
        isGrounded &= deltaY < (playerHalfHeight + groundHalfHeight);

        isGrounded &= playerPos.y > groundPos.y - AABBHEIGHTLIMIT;

        // 양 축 모두 겹치면 충돌이 발생한 것으로 간주
       
    }
}
