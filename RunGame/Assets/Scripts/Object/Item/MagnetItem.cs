using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItem : BaseItem
{
    public override void Init(GameObject _itemObj)
    {
        base.Init(_itemObj);

        itemType = EItemType.DINO;
    }
}
