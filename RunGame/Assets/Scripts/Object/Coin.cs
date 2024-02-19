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
    private ECoinType coinType;

    public Transform GetTransform => _transform;
    public bool GetActive => sprite.enabled;
    public ECoinType GetObstacleType => coinType;

    public void SetActive(bool _active) => sprite.enabled = _active;

    private float boundsX;
    private float boundsY;

    private float floorPosX;
    private float floorPosY;

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

    public virtual void Action() { }

    public virtual void SetFloorPosition(Vector2 _floorPos)
    {
        floorPosX = _transform.localPosition.x;
        floorPosY = _floorPos.y;
    }
}
