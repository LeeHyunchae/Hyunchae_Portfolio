using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartItem : BaseItem
{
    public override void Init(GameObject _itemObj)
    {
        base.Init(_itemObj);

        itemType = EItemType.HEART;
    }

    public override void OnGetItem(PlayerController _player)
    {
        base.OnGetItem(_player);

        int healCount = ItemManager.getInstance.GetItemModel(this.GetItemType).itemValue;

        for(int i = 0; i<healCount;i++)
        {
            _player.IncreasePlayerHP();
        }

    }
}
