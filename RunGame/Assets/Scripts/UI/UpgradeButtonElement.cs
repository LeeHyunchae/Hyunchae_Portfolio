using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonData
{
    public EItemType itemType;
    public int btnIdx;
    public bool isValueUpgradeBtn;
}


public class UpgradeButtonElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeInfoText;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;
    [SerializeField] private Button upgradeButton;

    private ButtonData buttonData;

    public ButtonData GetButtonData => buttonData;
    public void SetButtonData(ButtonData _buttonData) => buttonData = _buttonData;

    public Button GetButton()
    {
        return upgradeButton;
    }

    public void SetInfoText(string _text)
    {
        upgradeInfoText.text = _text;
    }

    public void SetCostText(string _text)
    {
        upgradeButtonText.text = _text;
    }
}
