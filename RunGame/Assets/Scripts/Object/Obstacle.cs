using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//.. TODO :: baseclass
public class Obstacle
{
    //.. TODO :: move to baseclass
    private SpriteRenderer obstacleSprite = new SpriteRenderer();
    private Transform _transform;

    public Transform GetTransform => _transform;
    public void SetActive(bool _active) => obstacleSprite.enabled = _active;
    public bool GetActive => obstacleSprite.enabled;

    public void Init(GameObject _obstacleObj)
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

}
