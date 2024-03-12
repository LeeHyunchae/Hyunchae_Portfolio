using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartItem : BaseItem
{
    public override void Init(GameObject _itemObj)
    {
        base.Init(_itemObj);

        itemType = EItemType.DINO;
    }

    public override void OnGetItem(PlayerController _player)
    {
        base.OnGetItem(_player);

        _player.IncreasePlayerHP();
    }
}
