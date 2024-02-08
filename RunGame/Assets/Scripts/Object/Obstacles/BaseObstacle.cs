using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObstacleType
{
    FIXED = 0,
    JUMP = 1,
    FLY = 2
}

public abstract class BaseObstacle
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

    public virtual void Init(GameObject _obstacleObj)
    {
        obstacleSprite = _obstacleObj.GetComponentInChildren<SpriteRenderer>();
        _transform = _obstacleObj.GetComponent<Transform>();
    }

    public float GetWidth()
    {
        return obstacleSprite.bounds.size.x;
    }

    public float GetHeight()
    {
        return obstacleSprite.bounds.size.y;
    }

    public void SetSprite(Sprite _sprite)
    {
        obstacleSprite.sprite = _sprite;
    }

    public abstract void Action();

}
