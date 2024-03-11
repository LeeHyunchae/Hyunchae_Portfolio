using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl
{
    private const int ITEM_CAPACITY = 10;
    private const int HEART_ITEM_START_NUM = ITEM_CAPACITY * (int)EItemType.HEART;
    private const int DINO_ITEM_START_NUM = ITEM_CAPACITY * (int)EItemType.DINO;
    private const int MAGNET_ITEM_START_NUM = ITEM_CAPACITY * (int)EItemType.MAGNET;

    private int itemCount = 0;

    private int prevHeartItemIdx = HEART_ITEM_START_NUM;
    private int prevDinoItemIdx = DINO_ITEM_START_NUM;
    private int prevMagnetItemIdx = MAGNET_ITEM_START_NUM;

    private const string ITEM_PATH = "Prefabs/Item_";

    private ItemManager itemManager;
    private BaseItem[] items;

    private float screenLeft;
    private float screenRight;

    private Transform itemParent;

    private float itemDropInterval;
    private float curItemDropInterval;
    private bool isDropReady;

    public BaseItem[] GetItems => items;

    public void Init()
    {
        CreateItems();
    }
    #region Init && CreateObstacleGameObject

    private void InitObstacles<T>(GameObject[] _itemObjs) where T : BaseItem, new()
    {
        int curArrayCount = _itemObjs.Length + itemCount;

        for (int i = itemCount; i < curArrayCount; i++)
        {
            BaseItem item = new T();

            item.Init(_itemObjs[i - itemCount]);
            item.SetActive(false);

            items[i] = item;

            item.SetParentTm(itemParent);
            item.GetTransform.SetParent(itemParent);
        }

        itemCount += _itemObjs.Length;
    }

    private void CreateItems()
    {
        itemParent = new GameObject("Items").transform;

        itemParent.transform.position = Vector2.zero;

        items = new BaseItem[ITEM_CAPACITY * (int)EItemType.END];

        CreateHeartItem();
        CreateDinoItem();
        CreateMagnetItem();
    }

    private void CreateHeartItem()
    {
        GameObject originItemObj = (GameObject)Resources.Load(ITEM_PATH + "Heart");
        GameObject[] itemObjs = new GameObject[ITEM_CAPACITY];

        for (int i = 0; i < ITEM_CAPACITY; i++)
        {
            itemObjs[i] = GameObject.Instantiate<GameObject>(originItemObj, Vector2.zero, Quaternion.identity, itemParent);
        }

        InitObstacles<HeartItem>(itemObjs);
    }

    private void CreateDinoItem()
    {
        GameObject originItemObj = (GameObject)Resources.Load(ITEM_PATH + "Dino");
        GameObject[] itemObjs = new GameObject[ITEM_CAPACITY];

        for (int i = 0; i < ITEM_CAPACITY; i++)
        {
            itemObjs[i] = GameObject.Instantiate<GameObject>(originItemObj, Vector2.zero, Quaternion.identity, itemParent);
        }

        InitObstacles<DinoItem>(itemObjs);
    }

    private void CreateMagnetItem()
    {
        GameObject originItemObj = (GameObject)Resources.Load(ITEM_PATH + "Magnet");
        GameObject[] itemObjs = new GameObject[ITEM_CAPACITY];

        for (int i = 0; i < ITEM_CAPACITY; i++)
        {
            itemObjs[i] = GameObject.Instantiate<GameObject>(originItemObj, Vector2.zero, Quaternion.identity, itemParent);
        }

        InitObstacles<MagnetItem>(itemObjs);

    }

    #endregion

    public void SetScreenLeftRight(float _screenLeft, float _screenRight)
    {
        screenLeft = _screenLeft;
        screenRight = _screenRight;
    }

    public void Update()
    {
        CheckItemPos();
        CheckItemDropInterval();
    }

    private void CheckItemPos()
    {

        for (int i = 0; i < (int)EItemType.END; i++)
        {
            BaseItem item = items[i];

            if (!item.GetActive)
            {
                continue;
            }

            if (CheckInScreenCoin(item))
            {
                item.SetIsInScreen(true);
            }

            if (CheckOutsideCoin(item))
            {
                item.GetTransform.SetParent(itemParent);

                item.SetActive(false);
                item.SetIsInScreen(false);
            }
        }
    }
    private bool CheckInScreenCoin(BaseItem _item)
    {
        return _item.GetTransform.position.x < screenRight;
    }

    private bool CheckOutsideCoin(BaseItem _item)
    {
        return _item.GetTransform.position.x + _item.GetWidth() * 0.5f <= screenLeft;
    }

    public void OnRepositionFloor(Floor _rePosFloor, List<Coin> _coins)
    {
        if (_coins.Count == 0 || !isDropReady)
        {
            return;
        }

        int halfPosCoin = (int)(_coins.Count * 0.5f);

        int randomItem = Random.Range(0, (int)EItemType.END);

        BaseItem item;

        if (randomItem == (int)EItemType.HEART)
        {
            item = items[prevHeartItemIdx];
            prevHeartItemIdx = (prevHeartItemIdx + 1) % ITEM_CAPACITY + HEART_ITEM_START_NUM;
        }
        else if (randomItem == (int)EItemType.DINO)
        {
            item = items[prevDinoItemIdx];
            prevDinoItemIdx = (prevDinoItemIdx + 1) % ITEM_CAPACITY + DINO_ITEM_START_NUM;
        }
        else
        {
            item = items[prevMagnetItemIdx];
            prevMagnetItemIdx = (prevMagnetItemIdx + 1) % ITEM_CAPACITY + MAGNET_ITEM_START_NUM;
        }

        Coin coin = _coins[halfPosCoin];

        item.GetTransform.SetParent(_rePosFloor.GetTransform);
        item.GetTransform.localPosition = coin.GetTransform.localPosition;
        item.SetActive(true);

        coin.SetActive(false);
        coin.SetIsInScreen(false);

        isDropReady = false;

    }

    private void CheckItemDropInterval()
    {
        curItemDropInterval += Time.deltaTime;

        if (curItemDropInterval > itemDropInterval)
        {
            curItemDropInterval = 0;
            isDropReady = true;
        }
    }    
}
