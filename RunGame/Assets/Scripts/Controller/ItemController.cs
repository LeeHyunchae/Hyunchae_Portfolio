using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController
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

        itemManager = ItemManager.getInstance;
        itemDropInterval = itemManager.GetItemModel(EItemType.ITEM_DROP_INTERVAL).itemDuration;
        itemDropInterval = 10;
    }
    #region Init && CreateObstacleGameObject

    private void InitItems<T>(GameObject[] _itemObjs) where T : BaseItem, new()
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

        items = new BaseItem[ITEM_CAPACITY * (int)EItemType.ITEM_DROP_INTERVAL];

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

        InitItems<HeartItem>(itemObjs);
    }

    private void CreateDinoItem()
    {
        GameObject originItemObj = (GameObject)Resources.Load(ITEM_PATH + "Dino");
        GameObject[] itemObjs = new GameObject[ITEM_CAPACITY];

        for (int i = 0; i < ITEM_CAPACITY; i++)
        {
            itemObjs[i] = GameObject.Instantiate<GameObject>(originItemObj, Vector2.zero, Quaternion.identity, itemParent);
        }

        InitItems<DinoItem>(itemObjs);
    }

    private void CreateMagnetItem()
    {
        GameObject originItemObj = (GameObject)Resources.Load(ITEM_PATH + "Magnet");
        GameObject[] itemObjs = new GameObject[ITEM_CAPACITY];

        for (int i = 0; i < ITEM_CAPACITY; i++)
        {
            itemObjs[i] = GameObject.Instantiate<GameObject>(originItemObj, Vector2.zero, Quaternion.identity, itemParent);
        }

        InitItems<MagnetItem>(itemObjs);

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

        for (int i = 0; i < itemCount; i++)
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

        //int randomItem = Random.Range(0, (int)EItemType.END);
        int randomItem = 1;

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

//    private const string ITEM_PATH = "Prefabs/Item_";

//    private ItemManager itemManager;
//    private BaseItem[] items;

//    private float screenLeft;
//    private float screenRight;

//    private Transform itemParent;

//    private float itemDropInterval;
//    private float curItemDropInterval;
//    private bool isDropReady;

//    public BaseItem[] GetItems => items;

//    public void Init()
//    {
//        CreateItems();

//        itemManager = ItemManager.getInstance;
//        itemDropInterval = itemManager.GetItemDropInterval;
//    }
//    private void InitItems(GameObject[] itemObjs)
//    {
//        items = new BaseItem[(int)EItemType.END];

//        items[(int)EItemType.HEART] = new HeartItem();
//        items[(int)EItemType.HEART].Init(itemObjs[(int)EItemType.HEART]);
//        items[(int)EItemType.HEART].SetParentTm(itemParent);
//        items[(int)EItemType.HEART].SetActive(false);

//        items[(int)EItemType.DINO] = new DinoItem();
//        items[(int)EItemType.DINO].Init(itemObjs[(int)EItemType.DINO]);
//        items[(int)EItemType.DINO].SetParentTm(itemParent);
//        items[(int)EItemType.DINO].SetActive(false);

//        items[(int)EItemType.MAGNET] = new MagnetItem();
//        items[(int)EItemType.MAGNET].Init(itemObjs[(int)EItemType.MAGNET]);
//        items[(int)EItemType.MAGNET].SetParentTm(itemParent);
//        items[(int)EItemType.MAGNET].SetActive(false);

//    }

//    public void SetScreenLeftRight(float _screenLeft, float _screenRight)
//    {
//        screenLeft = _screenLeft;
//        screenRight = _screenRight;
//    }

//    private void CreateItems()
//    {
//        itemParent = new GameObject("Items").transform;

//        itemParent.transform.position = Vector2.zero;

//        CreateItemObject();
//    }

//    private void CreateItemObject()
//    {
//        GameObject[] itemObjs = new GameObject[(int)EItemType.END];

//        GameObject originHeartObj = (GameObject)Resources.Load(ITEM_PATH + "Heart");
//        itemObjs[(int)EItemType.HEART] = GameObject.Instantiate<GameObject>(originHeartObj, Vector2.zero, Quaternion.identity, itemParent);

//        GameObject originDinoObj = (GameObject)Resources.Load(ITEM_PATH + "Dino");
//        itemObjs[(int)EItemType.DINO] = GameObject.Instantiate<GameObject>(originDinoObj, Vector2.zero, Quaternion.identity, itemParent);

//        GameObject originMagnetObj = (GameObject)Resources.Load(ITEM_PATH + "Magnet");
//        itemObjs[(int)EItemType.MAGNET] = GameObject.Instantiate<GameObject>(originMagnetObj, Vector2.zero, Quaternion.identity, itemParent);

//        InitItems(itemObjs);
//    }
//    public void Update()
//    {
//        CheckItemPos();
//        CheckItemDropInterval();
//    }

//    private void CheckItemPos()
//    {

//        for (int i = 0; i < (int)EItemType.END; i++)
//        {
//            BaseItem item = items[i];

//            if (!item.GetActive)
//            {
//                continue;
//            }

//            if (CheckInScreenCoin(item))
//            {
//                item.SetIsInScreen(true);
//            }

//            if (CheckOutsideCoin(item))
//            {
//                item.GetTransform.SetParent(itemParent);

//                item.SetActive(false);
//                item.SetIsInScreen(false);
//            }
//        }
//    }
//    private bool CheckInScreenCoin(BaseItem _item)
//    {
//        return _item.GetTransform.position.x < screenRight;
//    }

//    private bool CheckOutsideCoin(BaseItem _item)
//    {
//        return _item.GetTransform.position.x + _item.GetWidth() * 0.5f <= screenLeft;
//    }

//    public void OnRepositionFloor(Floor _rePosFloor, List<Coin> _coins)
//    {
//        if (_coins.Count == 0 || !isDropReady)
//        {
//            return;
//        }

//        int halfPosCoin = (int)(_coins.Count * 0.5f);

//        int randomItem = Random.Range(0, (int)EItemType.END);

//        BaseItem item = items[randomItem];
//        Coin coin = _coins[halfPosCoin];

//        item.GetTransform.SetParent(_rePosFloor.GetTransform);
//        item.GetTransform.localPosition = coin.GetTransform.localPosition;
//        item.SetActive(true);

//        coin.SetActive(false);
//        coin.SetIsInScreen(false);

//        isDropReady = false;

//    }

//    private void CheckItemDropInterval()
//    {
//        curItemDropInterval += Time.deltaTime;

//        if(curItemDropInterval > itemDropInterval)
//        {
//            curItemDropInterval = 0;
//            isDropReady = true;
//        }
//    }

//    //#region Patterns
//    //private void SetPosStrightRandomCoinPattern(Floor _floor)
//    //{
//    //    Floor floor = _floor;

//    //    int size = floor.GetFloorWidth();

//    //    int coinGrade = Random.Range(0, (int)ECoinType.END);

//    //    float remainder = size % 2;

//    //    float floorHalfSize = (size - remainder) * 0.5f;

//    //    float coinStartposX = -floorHalfSize;

//    //    if (remainder != 0)
//    //    {
//    //        coinStartposX -= 0.5f;
//    //    }

//    //    for (int i = 0; i <= size; i++)
//    //    {
//    //        Coin coin;

//    //        coin = coins[prevCoinIdx];
//    //        prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

//    //        coin.SetCoinGrade(coinGrade);

//    //        Vector2 coinPos = floor.GetTransform.position;

//    //        coinPos.x = coinStartposX + i;

//    //        coinPos.y = (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

//    //        coin.GetTransform.SetParent(floor.GetTransform);
//    //        coin.GetTransform.localPosition = coinPos;

//    //        coin.SetActive(true);
//    //    }

//    //}

//    //private void SetPosObstaclePattern(Floor _floor, List<BaseObstacle> _obstacles)
//    //{
//    //    Floor floor = _floor;

//    //    int size = floor.GetFloorWidth();

//    //    int coinGrade = Random.Range(0, (int)ECoinType.END);

//    //    int obstacleIdx = 0;

//    //    int obstaclesCount = _obstacles.Count - 1;

//    //    float remainder = size % 2;

//    //    float floorHalfSize = (size - remainder) * 0.5f;

//    //    float coinStartposX = -floorHalfSize;

//    //    if (remainder != 0)
//    //    {
//    //        coinStartposX -= 0.5f;
//    //    }

//    //    for (int i = 0; i <= size; i++)
//    //    {
//    //        Vector2 obstaclePos = _obstacles[obstacleIdx].GetTransform.localPosition;
//    //        float obstacleHalfWidth = _obstacles[obstacleIdx].GetWidth() * 0.5f;

//    //        Coin coin;

//    //        coin = coins[prevCoinIdx];
//    //        prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

//    //        coin.SetCoinGrade(coinGrade);

//    //        Vector2 coinPos = floor.GetTransform.position;

//    //        coinPos.x = coinStartposX + i;

//    //        coinPos.y = (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

//    //        float distance = Mathf.Abs(coinPos.x - obstaclePos.x);

//    //        if (distance < MIN_OBSTACLE_INTERVAL + obstacleHalfWidth)
//    //        {
//    //            coinPos.y += 2;
//    //        }

//    //        if (coinPos.x > obstaclePos.x && obstacleIdx != obstaclesCount)
//    //        {
//    //            obstacleIdx++;
//    //        }

//    //        coin.GetTransform.SetParent(floor.GetTransform);
//    //        coin.GetTransform.localPosition = coinPos;

//    //        coin.SetActive(true);
//    //    }

//    //}

//    //private void SetPosSquarePattern(Floor _floor)
//    //{
//    //    Floor floor = _floor;

//    //    int squareSize = Random.Range(MIN_FLOOR_INTERVAL, _floor.GetPrevFloorDistance);

//    //    int coinGrade = Random.Range(0, (int)ECoinType.END);

//    //    Vector2 coinStartPos = _floor.GetTransform.position;

//    //    float floorWidthHalf = _floor.GetFloorWidth() * 0.5f;
//    //    float prevFloorDistanceHalf = _floor.GetPrevFloorDistance * 0.5f;
//    //    float squareSizeHalf = squareSize * 0.5f;
//    //    float coinWidthHalf = coins[0].GetWidth() * 0.5f;

//    //    coinStartPos.x = coinStartPos.x - floorWidthHalf - prevFloorDistanceHalf - squareSizeHalf - coinWidthHalf;
//    //    coinStartPos.y = _floor.GetPrevFloorPos.y + squareSize + MIN_FLOOR_INTERVAL;

//    //    float squareStartPosY = coinStartPos.y;

//    //    for (int i = 0; i < squareSize * squareSize; i++)
//    //    {
//    //        Coin coin;

//    //        coin = coins[prevCoinIdx];
//    //        prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

//    //        coin.SetCoinGrade(coinGrade);

//    //        if (i % squareSize == 0)
//    //        {
//    //            coinStartPos.x += coin.GetWidth();
//    //        }

//    //        coinStartPos.y = squareStartPosY - (coin.GetHeight() * (i % squareSize));

//    //        coin.GetTransform.SetParent(floor.GetTransform);
//    //        coin.GetTransform.position = coinStartPos;

//    //        coin.SetActive(true);
//    //    }

//    //}
//    //#endregion

//}
