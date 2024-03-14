using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    private Player player;
    private const float GRAVITY = 0.45f;
    private const float LONGJUMPPOWER = 0.125f;
    private const float SHORTJUMPPOWER = 0.225f;
    private const float HIT_IGNORE_TIME = 3;
    private const int PLAYERSIZE = 1;
    private const float PLAYERHALFSIZE = 0.5f;
    private const string PLAYERPATH = "Prefabs/Player";
    private const float AABB_COLLECTION_VALUE = 0.25f;
    private const int PLAYER_MAX_HP = 10;

    private bool isGrounded;
    private bool isDoubleJump;
    private bool isHit;
    private bool isMagnet;

    private float curShortJumpPower = 0;
    private float curLongJumpPower = 0;
    private float curHitIgnoreTime = 0;
    private float curMagnetTime = 0;
    private float curDinoTime = 0;

    private Floor curFloor;
    private float floorLandPosY;
    private BaseObstacle[] obstacles;
    private Coin[] coins;
    private BaseItem[] items;
    private Vector2 playerPos;
    private int coinSpeed;

    public Action<ECoinType> OnGetCoin;
    public Action<int> OnIncreaseHP;
    public Action<int> OnDecreaseHP;

    public float GetPlayerHalfSize => PLAYERHALFSIZE;

    public void Init()
    {
        SetPlayer(GameObject.Instantiate<GameObject>((GameObject)Resources.Load(PLAYERPATH), Vector2.zero, Quaternion.identity));
    }
    
    private void SetPlayer(GameObject _player)
    {
        player = new Player();
        player.Init(_player);
        player.SetHP(PLAYER_MAX_HP);
        playerPos = player.GetTranstorm.position;

    }
    public void IncreasePlayerHP()
    {
        int playerHP = player.GetHP;
        player.SetHP(++playerHP);
        OnIncreaseHP?.Invoke(player.GetHP);
    }

    public void DecreasePlayerHP()
    {
        int playerHP = player.GetHP;
        player.SetHP(--playerHP);
        OnDecreaseHP?.Invoke(player.GetHP);
    }

    public void SetCoinSpeed(int _speed)
    {
        coinSpeed = _speed;
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


        if (isMagnet)
        {
            curMagnetTime += Time.deltaTime;

            if (curMagnetTime > ItemManager.getInstance.GetItemModel(EItemType.MAGNET).itemDuration)
            {
                curMagnetTime = 0;
                isMagnet = false;
            }
        }

    }

    public void FixedUpdate()
    {
        if (!isGrounded)
        {
            OnJumping();
        }

        playerPos.y += curShortJumpPower + curLongJumpPower;

        player.SetPosition(playerPos);

        CheckGroundAABB();
        CheckCoinAABB();
        CheckItemAABB();

        if (player.GetPlayerState == EPlayerState.DINO)
        {
            curDinoTime += Time.deltaTime;
            if(curDinoTime > ItemManager.getInstance.GetItemModel(EItemType.DINO).itemDuration)
            {
                curDinoTime = 0;
                ChangeState(EPlayerState.DINOEND);
            }
        }
        else if (isHit)
        {
            curHitIgnoreTime += Time.deltaTime;
            if(curHitIgnoreTime >= HIT_IGNORE_TIME)
            {
                curHitIgnoreTime = 0;
                isHit = false;
            }
        }
        else
        {
            CheckObstacleAABB();
        }

        if (isGrounded && player.GetPlayerState != EPlayerState.WALK)
        {
            Land();
        }
    }

    private void OnJumping()
    {
        curShortJumpPower -= GRAVITY * Time.deltaTime;

        if (curShortJumpPower < 0 && player.GetPlayerState != EPlayerState.JUMPDOWN)
        {
            ChangeState(EPlayerState.JUMPDOWN);
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            curShortJumpPower = SHORTJUMPPOWER;
            ChangeState(EPlayerState.JUMPUP);
            return;
        }
        else if(!isDoubleJump)
        {
            isDoubleJump = true;
            curShortJumpPower = SHORTJUMPPOWER;
            ChangeState(EPlayerState.JUMPUP);
        }
    }

    public void LongJump()
    {
        if(player.GetPlayerState != EPlayerState.JUMPDOWN)
        {
            curLongJumpPower = Mathf.Lerp(curLongJumpPower, LONGJUMPPOWER, 0.01f);
        }

    }

    private void Land()
    {
        curShortJumpPower = 0;
        curLongJumpPower = 0;
        isDoubleJump = false;

        ChangeState(EPlayerState.WALK);

        playerPos.y = floorLandPosY;
        player.SetPosition(playerPos);
    }

    private void ChangeState(EPlayerState _state)
    {
        if(player.GetPlayerState == EPlayerState.DINO && _state != EPlayerState.DINOEND)
        {
            return;
        }

        player.SetState(_state);
        PlayCurStateAnim(_state);
    }

    private void PlayCurStateAnim(EPlayerState _state)
    {
        player.ChangeAnimation((int)_state);
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

    public void SetItems(BaseItem[] _items)
    {
        items = _items;
    }

    public void OnGetMagnet()
    {
        isMagnet = true;
    }

    public void OnGetDino()
    {
        ChangeState(EPlayerState.DINO);
    }

    private void CheckGroundAABB()
    {
        if(curShortJumpPower > 0 || curFloor == null)
        {
            isGrounded = false;
            return;
        }

        if (playerPos.y == floorLandPosY)
        {
            isGrounded = true;
            return;
        }

        Vector2 floorPos = curFloor.GetTransform.position;

        float floorWidthHalf = curFloor.GetFloorWidth() * 0.5f;
        float floorHeightHalf = curFloor.GetFloorHeight() * 0.5f;

        Rect playerRect = new Rect(playerPos.x - PLAYERHALFSIZE, playerPos.y - PLAYERHALFSIZE + AABB_COLLECTION_VALUE, PLAYERSIZE, AABB_COLLECTION_VALUE);
        Rect floorRect = new Rect(floorPos.x - floorWidthHalf, floorPos.y + floorHeightHalf, curFloor.GetFloorWidth(), AABB_COLLECTION_VALUE);

        floorRect.DrawDebugLine();

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

            obstacleRect.DrawDebugLine();

            if (playerRect.Overlaps(obstacleRect))
            {
                isHit = true;
                DecreasePlayerHP();
                return;
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
            if (isMagnet)
            {
                float distance = Vector2.Distance(coinPos, playerPos);

                if (distance <= ItemManager.getInstance.GetItemModel(EItemType.MAGNET).itemValue)
                {
                    Vector2 direction = (playerPos - coinPos).normalized;

                    coinPos += direction * coinSpeed * Time.deltaTime;
                    coin.GetTransform.position = coinPos;
                }
            }

            float coinWidthHalf = coin.GetWidth() * 0.5f;
            float coinHeightHalf = coin.GetHeight() * 0.5f;

            float coinRectX = coinPos.x - coinWidthHalf + AABB_COLLECTION_VALUE;
            float coinRectY = coinPos.y + coinHeightHalf - AABB_COLLECTION_VALUE;
            float coinRectWidth = coin.GetWidth() - AABB_COLLECTION_VALUE * 2;
            float coinRectHeight = coin.GetHeight() - AABB_COLLECTION_VALUE * 2;

            coinRect.Set(coinRectX, coinRectY, coinRectWidth, coinRectHeight);

            coinRect.DrawDebugLine();

            if(playerRect.Overlaps(coinRect))
            {
                coin.SetActive(false);
                OnGetCoin.Invoke(coin.GetCoinType);
            }
        }
    }

    private void CheckItemAABB()
    {
        if (items == null)
        {
            return;
        }

        int count = items.Length;

        BaseItem item;

        Rect playerRect = new Rect(playerPos.x - PLAYERHALFSIZE, playerPos.y + PLAYERHALFSIZE, PLAYERSIZE, PLAYERSIZE);
        Rect itemRect = new Rect();

        for (int i = 0; i < count; i++)
        {
            item = items[i];

            if (!item.GetActive || !item.GetIsInScreen)
            {
                continue;
            }

             Vector2 itemPos = item.GetTransform.position;

            float itemWidthHalf = item.GetWidth() * 0.5f;
            float itemHeightHalf = item.GetHeight() * 0.5f;

            float itemRectX = itemPos.x - itemWidthHalf;
            float itemRectY = itemPos.y + itemHeightHalf;
            float itemRectWidth = item.GetWidth();
            float itemRectHeight = item.GetHeight();

            itemRect.Set(itemRectX, itemRectY, itemRectWidth, itemRectHeight);

            itemRect.DrawDebugLine();

            if (playerRect.Overlaps(itemRect))
            {
                item.OnGetItem(this);
                item.SetActive(false);
            }
        }
    }
}
