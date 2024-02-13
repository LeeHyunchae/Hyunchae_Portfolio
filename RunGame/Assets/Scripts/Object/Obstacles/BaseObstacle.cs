using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObstacleType
{
    FIXED = 0,
    JUMP = 1,
    FLY = 2
}

public class BaseObstacle
{
    protected SpriteRenderer obstacleSprite = new SpriteRenderer();
    protected Transform _transform;
    protected EObstacleType obstacleType;
    protected float obstacleSpeedRate = 0;

    public Transform GetTransform => _transform;
    public bool GetActive => obstacleSprite.enabled;
    public EObstacleType GetObstacleType => obstacleType;
    public float GetSpeedRate => obstacleSpeedRate;

    public void SetActive(bool _active) => obstacleSprite.enabled = _active;
    public void SetSpeed(int _speed) => obstacleSpeedRate = _speed;

    protected float boundsX;
    protected float boundsY;

    protected float floorPosX;
    protected float floorPosY;

    public virtual void Init(GameObject _obstacleObj)
    {
        obstacleSprite = _obstacleObj.GetComponentInChildren<SpriteRenderer>();
        _transform = _obstacleObj.GetComponent<Transform>();

        boundsX = obstacleSprite.bounds.size.x;
        boundsY = obstacleSprite.bounds.size.y;
    }

    public float GetWidth()
    {
        return boundsX;
    }

    public float GetHeight()
    {
        return boundsY;
    }

    public void SetSprite(Sprite _sprite)
    {
        obstacleSprite.sprite = _sprite;
        boundsX = obstacleSprite.bounds.size.x;
        boundsY = obstacleSprite.bounds.size.y;
    }

    public virtual void Action() { }

    public virtual void SetFloorPosition(Vector2 _floorPos)
    {
        floorPosX = _transform.localPosition.x;
        floorPosY = _floorPos.y;
    }
}
