using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerState
{
    IDLE = 0,
    WALK = 1,
    JUMPUP = 2,
    JUMPDOWN = 3,
    DINO = 4,
    DINOEND = 5
}

public class Player
{
    private const string PLAYERSTATE = "PlayerState";

    private Animator anim;
    private EPlayerState state;
    private int playerHP;
    private Transform playerTM;
    private SpriteRenderer sprite;

    public Transform GetTranstorm => playerTM;
    public EPlayerState GetPlayerState => state;
    public int GetHP => playerHP;
    public int SetHP(int _hp) => playerHP = _hp;


    public void Init(GameObject _playerObj)
    {
        anim = _playerObj.GetComponent<Animator>();
        sprite = _playerObj.GetComponent<SpriteRenderer>();
        state = EPlayerState.IDLE;

        playerTM = _playerObj.GetComponent<Transform>();
    }
    
    public void ChangeAnimation(int _animNum)
    {
        anim.SetInteger(PLAYERSTATE, _animNum);
    }

    public void SetState(EPlayerState _state)
    {
        state = _state;
    }

    public void SetPosition(Vector2 _pos)
    {
        playerTM.position = _pos;
    }

    public void SetSpriteColor(Color _color)
    {
        sprite.color = _color;
    }
}
