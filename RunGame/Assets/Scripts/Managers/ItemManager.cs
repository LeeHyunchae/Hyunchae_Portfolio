using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private ItemModel[] items;

    public ItemModel[] GetItems => items;

    public override bool Initialize()
    {
        base.Initialize();

        items = new ItemModel[(int)EItemType.END];

        LoadItemStatus();
        return true;
    }

    private void InitItemData()
    {
        int count = (int)EItemType.END;

        for (int i = 0; i < count; i++)
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
        items[typeNum].itemValueInfo = "체력 회복량 상승 : ";
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
        items[typeNum].itemDurationInfo = "변신 지속시간 상승 : ";
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
        items[typeNum].itemValueInfo = "자석 범위 상승 : ";
        items[typeNum].itemDurationInfo = "자석 지속시간 상승 : ";
        items[typeNum].InitData();

        typeNum = (int)EItemType.ITEM_DROP_INTERVAL;

        items[typeNum].itemType = (EItemType)typeNum;
        items[typeNum].itemValueLevel = 0;
        items[typeNum].itemDurationLevel = 0;
        items[typeNum].baseItemValue = 0;
        items[typeNum].baseitemDuration = 20;
        items[typeNum].itemValue_IncreaseValue = 0;
        items[typeNum].itemDuration_InceaseValue = -0.5f;
        items[typeNum].valueLevelUp_CostInceaseValue = 0;
        items[typeNum].durationLevelUp_CostInceaseValue = 500;
        items[typeNum].itemValueInfo = null;
        items[typeNum].itemDurationInfo = "아이템 등장 시간 감소 : ";
        items[typeNum].InitData();

        SaveAllItemStatus();
    }

    private void LoadItemStatus()
    {
        int count = (int)EItemType.END;

        for (int i = 0; i < count; i++)
        {
            string json = PlayerPrefs.GetString(((EItemType)i).ToString());

            if (json == string.Empty)
            {
                InitItemData();
                return;
            }
            else
            {
                items[i] = new ItemModel();
                items[i] = JsonUtility.FromJson<ItemModel>(json);
            }
        }
    }

    public ItemModel GetItemModel(EItemType _itemType) => items[(int)_itemType];
    public ItemModel SetItemModel(ItemModel _itemModel) => items[(int)_itemModel.itemType] = _itemModel;
    
    public void SaveAllItemStatus()
    {
        int count = (int)EItemType.END;

        for(int i = 0; i < count; i++)
        {
            string jsonItem = JsonUtility.ToJson(items[i]);

            PlayerPrefs.SetString(items[i].itemType.ToString(), jsonItem);
        }
    }

    public void SaveItemStatus(EItemType _itemType)
    {
        string jsonItem = JsonUtility.ToJson(items[(int)_itemType]);
        PlayerPrefs.SetString(items[(int)_itemType].itemType.ToString(), jsonItem);

    }

    public void UpgradeItemValue(EItemType _itemType)
    {
        int type = (int)_itemType;

        items[type].UpgradeItemValue();
        SaveItemStatus(_itemType);
    }

    public void UpgradeItemDuration(EItemType _itemType)
    {
        int type = (int)_itemType;

        items[type].UpgradeItemDuration();
        SaveItemStatus(_itemType);
    }
}
