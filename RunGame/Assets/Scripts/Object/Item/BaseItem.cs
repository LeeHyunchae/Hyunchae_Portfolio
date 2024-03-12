using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    HEART = 0,
    DINO = 1,
    MAGNET = 2,
    END = 3
}

public class BaseItem
{
    protected SpriteRenderer sprite;

    protected Transform _transform;
    protected Transform parentTm;
    protected EItemType itemType;

    public Transform GetTransform => _transform;
    public bool GetActive => sprite.enabled;
    public bool GetIsInScreen => isInScreen;
    public EItemType GetItemType => itemType;

    public void SetParentTm(Transform _parent) => parentTm = _parent;
    public void SetIsInScreen(bool _isInScreen) => isInScreen = _isInScreen;

    protected float boundsX;
    protected float boundsY;
    protected bool isInScreen;

    public virtual void Init(GameObject _itemObj)
    {
        sprite = _itemObj.GetComponent<SpriteRenderer>();
        _transform = _itemObj.GetComponent<Transform>();

        boundsX = sprite.bounds.size.x;
        boundsY = sprite.bounds.size.y;

    }

    public float GetWidth()
    {
        return boundsX;
    }

    public float GetHeight()
    {
        return boundsY;
    }

    public virtual void OnGetItem(PlayerController _player) { }

    public void SetActive(bool _active)
    {
        sprite.enabled = _active;

        if(!_active)
        {
            _transform.SetParent(parentTm);
        }
    }
}
