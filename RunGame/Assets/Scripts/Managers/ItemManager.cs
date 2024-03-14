using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private ItemModel[] items;

    public ItemModel[] GetItems => items;

    // Todo 저장 및 불러오기 기능

    public override bool Initialize()
    {
        base.Initialize();

        items = new ItemModel[(int)EItemType.END];

        LoadItemStatus();
        return true;
    }

    private void LoadItemStatus()
    {
        //Todo 불러오기로 세팅, 테이블 화

        int count = (int)EItemType.END;

        for(int i = 0; i<count; i++)
        {
            items[i] = new ItemModel();
        }

        int typeNum = (int)EItemType.HEART;

        items[typeNum].itemType = (EItemType)typeNum;
        items[typeNum].itemValueLevel = 0;
        items[typeNum].itemDurationLevel = 0;
        items[typeNum].baseItemValue = 1;
        items[typeNum].itemValue_IncreaseValue = 1;
        items[typeNum].valueLevelUp_CostInceaseValue = 500;
        items[typeNum].baseitemDuration = 0;
        items[typeNum].itemDuration_InceaseValue = 0;
        items[typeNum].durationLevelUp_CostInceaseValue = 0;
        items[typeNum].itemValueInfo = "체력 회복량 상승";
        items[typeNum].itemDurationInfo = null;
        items[typeNum].InitData();

        typeNum = (int)EItemType.DINO;

        items[typeNum].itemType = (EItemType)typeNum;
        items[typeNum].itemValueLevel = 0;
        items[typeNum].itemDurationLevel = 0;
        items[typeNum].baseItemValue = 0;
        items[typeNum].itemValue_IncreaseValue = 0;
        items[typeNum].valueLevelUp_CostInceaseValue = 0;
        items[typeNum].baseitemDuration = 5;
        items[typeNum].itemDuration_InceaseValue = 0.5f;
        items[typeNum].durationLevelUp_CostInceaseValue = 300;
        items[typeNum].itemValueInfo = null;
        items[typeNum].itemDurationInfo = "변신 지속시간 상승";
        items[typeNum].InitData();

        typeNum = (int)EItemType.MAGNET;

        items[typeNum].itemType = (EItemType)typeNum;
        items[typeNum].itemValueLevel = 0;
        items[typeNum].itemDurationLevel = 0;
        items[typeNum].baseItemValue = 5;
        items[typeNum].baseitemDuration = 5;
        items[typeNum].itemValue_IncreaseValue = 1;
        items[typeNum].itemDuration_InceaseValue = 0.5f;
        items[typeNum].valueLevelUp_CostInceaseValue = 500;
        items[typeNum].durationLevelUp_CostInceaseValue = 300;
        items[typeNum].itemValueInfo = "자석 범위 상승";
        items[typeNum].itemDurationInfo = "자석 지속시간 상승";
        items[typeNum].InitData();

        typeNum = (int)EItemType.ITEM_DROP_INTERVAL;

        items[typeNum].itemType = (EItemType)typeNum;
        items[typeNum].itemValueLevel = 0;
        items[typeNum].itemDurationLevel = 0;
        items[typeNum].baseItemValue = 0;
        items[typeNum].baseitemDuration = 15;
        items[typeNum].itemValue_IncreaseValue = 0;
        items[typeNum].itemDuration_InceaseValue = -0.5f;
        items[typeNum].valueLevelUp_CostInceaseValue = 0;
        items[typeNum].durationLevelUp_CostInceaseValue = 500;
        items[typeNum].itemValueInfo = null;
        items[typeNum].itemDurationInfo = "아이템 등장 시간 감소";
        items[typeNum].InitData();
    }

    public ItemModel GetItemModel(EItemType _itemType) => items[(int)_itemType];
    public ItemModel SetItemModel(ItemModel _itemModel) => items[(int)_itemModel.itemType] = _itemModel;
    
    public void SaveItemStatus()
    {
        //Todo 아이템 값 저장해주기
    }

    public void UpgradeItemValue(EItemType _itemType)
    {
        int type = (int)_itemType;

        items[type].UpgradeItemValue();
    }

    public void UpgradeItemDuration(EItemType _itemType)
    {
        int type = (int)_itemType;

        items[type].UpgradeItemDuration();
    }
}
