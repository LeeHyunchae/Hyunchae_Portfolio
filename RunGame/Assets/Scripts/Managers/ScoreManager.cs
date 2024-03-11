using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    private int totalScore;
    private int highScore;

    public int GetTotalScore => totalScore;

    public void PlusScore(int _value) => totalScore += _value;

    public void MinusScore(int _value) => totalScore -= _value;

    public int GetHighScore => highScore;


    public void SetScore(int _score)
    {
        if(_score > highScore)
        {
            highScore = _score;
        }

        PlusScore(_score);
    }

    // Todo 저장 및 불러오기 기능 필요할듯
}
