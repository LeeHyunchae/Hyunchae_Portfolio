using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObstacleType
{
    FIXED = 0,
    JUMP = 1,
    FLY = 2,
    END = 3
}

public class BaseObstacle
{
    protected SpriteRenderer obstacleSprite = new SpriteRenderer();
    protected Transform _transform;
    protected EObstacleType obstacleType;
    protected float obstacleSpeedRate = 8;
    protected bool isInScreen = false;

    public Transform GetTransform => _transform;
    public bool GetActive => obstacleSprite.enabled;
    public bool GetIsInScreen => isInScreen;
    public EObstacleType GetObstacleType => obstacleType;

    public void SetActive(bool _active) => obstacleSprite.enabled = _active;
    public void SetIsInScreen(bool _isInScrenn) => isInScreen = _isInScrenn;

    protected float boundsX;
    protected float boundsY;

    protected float floorPosX;
    protected float floorPosY;

    public virtual void Init(GameObject _obstacleObj)
    {
        obstacleSprite = _obstacleObj.GetComponentInChildren<SpriteRenderer>();
        _transform = _obstacleObj.GetComponent<Transform>();
    }

    public virtual void ResetData() { }

    public virtual void SetSpeed(int _speed){}

    public virtual float GetWidth()
    {
        return boundsX;
    }

    public virtual float GetHeight()
    {
        return boundsY;
    }

    public virtual void SetSprite(Sprite _sprite)
    {
        obstacleSprite.sprite = _sprite;
    }

    public virtual void Action() { }

    public virtual void SetFloorPosition(Vector2 _floorPos)
    {
        floorPosX = _transform.localPosition.x;
        floorPosY = _floorPos.y;
    }

    public virtual Vector2 GetPosition()
    {
        return _transform.position;
    }

}
