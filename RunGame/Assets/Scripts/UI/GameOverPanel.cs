using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIBaseController
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] Button restartBtn;

    private const string SCORE = "SCORE : ";
    private const string MAINSCENE = "MainScene";

    public override void Init()
    {
        base.Init();

        restartBtn.onClick.AddListener(OnClickRestartButton);
        highScoreText.enabled = false;

        SetScoreText();
    }

    private void SetScoreText()
    {
        scoreText.text = SCORE + ScoreManager.getInstance.GetScore;

        if(ScoreManager.getInstance.GetIsHighScore)
        {
            highScoreText.enabled = true;
        }
    }

    private void OnClickRestartButton()
    {
        UIManager.getInstance.UnloadScene();
        SceneController.getInstance.ChangeScene(MAINSCENE);
    }
}
