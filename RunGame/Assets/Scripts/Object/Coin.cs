using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECoinType
{
    BRONZE = 0,
    SILVER = 1,
    GOLD = 2,
    END = 3
}

public class Coin
{
    private Animator coinAnim;
    private SpriteRenderer sprite;
    private const string COINGRADE = "CoinGrade";

    private Transform _transform;
    private Transform parentTm;
    private ECoinType coinType;

    public Transform GetTransform => _transform;
    public bool GetActive => sprite.enabled;
    public bool GetIsInScreen => isInScreen;
    public ECoinType GetCoinType => coinType;

    public void SetIsInScreen(bool _isInScreen) => isInScreen = _isInScreen;
    public void SetParentTm(Transform _parent) => parentTm = _parent;

    private float boundsX;
    private float boundsY;

    private bool isInScreen;

    public virtual void Init(GameObject _coinObj)
    {
        coinAnim = _coinObj.GetComponentInChildren<Animator>();
        sprite = _coinObj.GetComponent<SpriteRenderer>();
        _transform = _coinObj.GetComponent<Transform>();

        boundsX = sprite.bounds.size.x;
        boundsY = sprite.bounds.size.y;

        SetCoinGrade((int)ECoinType.BRONZE);
    }

    public float GetWidth()
    {
        return boundsX;
    }

    public float GetHeight()
    {
        return boundsY;
    }

    public virtual void SetCoinGrade(int _grade)
    {
        coinType = (ECoinType)_grade;
        coinAnim.SetInteger(COINGRADE, (int)coinType);
    }

    public void SetActive(bool _active)
    {
        sprite.enabled = _active;

        if(!_active)
        {
            _transform.SetParent(parentTm);
        }
    }

}
