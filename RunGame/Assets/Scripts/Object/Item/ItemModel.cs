using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel
{
    public EItemType itemType;
    public int itemValueCost;
    public int itemDurationCost;
    public int baseItemValue;
    public float baseitemDuration;
    public int itemValueLevel;
    public int itemDurationLevel;
    public int itemValue;
    public float itemDuration;
    public int itemValue_IncreaseValue;
    public float itemDuration_InceaseValue;
    public int valueLevelUp_CostInceaseValue;
    public int durationLevelUp_CostInceaseValue;
    public string itemValueInfo;
    public string itemDurationInfo;

    public void InitData()
    {
        itemValueCost = valueLevelUp_CostInceaseValue;
        itemDurationCost = durationLevelUp_CostInceaseValue;
        SetValue();
        SetDuration();
    }

    public void UpgradeItemValue()
    {
        itemValueLevel++;
        itemValueCost = valueLevelUp_CostInceaseValue + itemValueLevel * valueLevelUp_CostInceaseValue;
        SetValue();
    }

    public void UpgradeItemDuration()
    {
        itemDurationLevel++;
        itemDurationCost = durationLevelUp_CostInceaseValue + itemDurationLevel * durationLevelUp_CostInceaseValue;
        SetDuration();
    }

    public void SetValue()
    {
        itemValue = baseItemValue + (itemValueLevel * itemValue_IncreaseValue);
    }

    public void SetDuration()
    {
        itemDuration = baseitemDuration + (itemDurationLevel * itemDuration_InceaseValue);
    }
}
