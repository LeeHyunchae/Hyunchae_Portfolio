using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    private const float GOLD_CORRECTION = 0.5f;
    private const string HIGH_SCORE = "HighScore";
    private const string GOLD = "Gold";

    private int curScore;
    private int highScore;
    private bool isHighScore;
    private int gold;

    public int GetHighScore => highScore;

    public int GetScore => curScore;

    public bool GetIsHighScore => isHighScore;

    public int GetGold => gold;

    public override bool Initialize()
    {
        base.Initialize();

        LoadData();

        return true;
    }

    public void SetScore(int _score)
    {
        if(_score > highScore)
        {
            highScore = _score;
            isHighScore = true;

            SaveHighScore();
        }
        else
        {
            isHighScore = false;
        }

        curScore = _score;
        SetGold(_score);
    }

    public void SetGold(int _score)
    {
        gold += (int)(_score * GOLD_CORRECTION);

        SaveGold();
    }
    
    public int UseGold(int _cost)
    {
        gold -= _cost;

        SaveGold();

        return GetGold;
    }

    private void SaveGold()
    {
        PlayerPrefs.SetInt(GOLD, gold);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE, highScore);
    }

    private void LoadData()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE);
        gold = PlayerPrefs.GetInt(GOLD);
    }
}
