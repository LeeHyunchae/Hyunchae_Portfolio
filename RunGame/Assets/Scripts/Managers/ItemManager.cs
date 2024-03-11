using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private const float BASE_DINO_DURATION = 5;
    private const int BASE_Heart_VALUE = 1;
    private const float BASE_MAGNET_DURATION = 5;
    private const int BASE_MAGNET_RANGE = 5;
    private const float BASE_ITEM_DROP_INTERVAL = 10;

    private float curDinoDuration = 0;
    private int curHeartValue = 0;
    private float curMagnetDuration = 0;
    private int curMagnetRange = 0;
    private float curItemDropInterval = 0;

    // Todo 저장 및 불러오기 기능

    public override bool Initialize()
    {
        base.Initialize();

        LoadItemStatus();
        return true;
    }

    private void LoadItemStatus()
    {
        //Todo 불러오기로 cur 값들 세팅해야함
        curDinoDuration = BASE_DINO_DURATION;
        curHeartValue = BASE_Heart_VALUE;
        curMagnetDuration = BASE_MAGNET_DURATION;
        curMagnetRange = BASE_MAGNET_RANGE;
        curItemDropInterval = BASE_ITEM_DROP_INTERVAL;
    }

    public float GetDinoDuration => curDinoDuration;
    public int GetHeartValue => curHeartValue;
    public float GetMagnetDuration => curMagnetDuration;
    public int GetMagnetRange => curMagnetRange;
    public float GetItemDropInterval => curItemDropInterval;

    public void SetDinoDuration(float _duration) => curDinoDuration = BASE_DINO_DURATION + _duration;
    public void SetHeartValue(int _value) => curHeartValue = BASE_Heart_VALUE + _value;
    public void SetMagnetDuration(float _duration) => curMagnetDuration = BASE_MAGNET_DURATION + _duration;
    public void SetMagnetRange(int _range) => curMagnetRange = BASE_MAGNET_RANGE + _range;
    public void SetItemDropInterval(float _interval) => curItemDropInterval = BASE_ITEM_DROP_INTERVAL - _interval;

    public void SaveItemStatus()
    {
        //Todo 아이템 값 저장해주기
    }
}
