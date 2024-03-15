using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopPanelController : UIBaseController
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private UpgradeButtonElement originBtn;
    [SerializeField] private Transform upgradeBtnParent;
    [SerializeField] private Button CloseBtn;

    private const string GOLD = "GOLD : ";

    private List<UpgradeButtonElement> buttonElements;
    private Button[] upgradeBtns;
    private ItemManager itemManager;
    private int buttonCount = 0;
    private ScoreManager scoreManager;

    public override void Init()
    {
        base.Init();

        itemManager = ItemManager.getInstance;
        scoreManager = ScoreManager.getInstance;

        buttonElements = new List<UpgradeButtonElement>();

        RefreshGoldText();

        InitBtnData();

        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    private void InitBtnData()
    {
        ItemModel[] items = itemManager.GetItems;

        List<ButtonData> buttonDatas = new List<ButtonData>();

        int count = items.Length;

        for(int i = 0; i<count; i++)
        {
            if (items[i].baseItemValue > 0)
            {
                ButtonData buttonData = CreateButtonData(items[i], true);

                buttonDatas.Add(buttonData);

                buttonCount++;
            }

            if (items[i].baseitemDuration > 0)
            {
                ButtonData buttonData = CreateButtonData(items[i], false);

                buttonDatas.Add(buttonData);
                
                buttonCount++;
            }

        }

        upgradeBtns = new Button[buttonCount];
        CreateUpgradeBtns(buttonDatas);

    }

    private ButtonData CreateButtonData(ItemModel _itemModel,bool _isValueType)
    {
        ButtonData buttonData = new ButtonData();

        buttonData.btnIdx = buttonCount;

        buttonData.itemType = _itemModel.itemType;

        buttonData.isValueUpgradeBtn = _isValueType;

        return buttonData;
    }

    private void CreateUpgradeBtns(List<ButtonData> _buttonDatas)
    {
        ItemModel[] items = itemManager.GetItems;

        for(int i = 0; i < buttonCount; i++)
        {
            UpgradeButtonElement buttonElement = GameObject.Instantiate<UpgradeButtonElement>(originBtn, upgradeBtnParent);
            Button button = buttonElement.GetButton();
            ButtonData buttonData = _buttonDatas[i];
            buttonElement.SetButtonData(buttonData);
            ItemModel item = items[(int)buttonData.itemType];

            upgradeBtns[i] = button;

            upgradeBtns[i].onClick.AddListener(() => OnClickUpgradeButton(buttonData.btnIdx));

            if(buttonData.isValueUpgradeBtn)
            {
                buttonElement.SetInfoText(item.itemValueInfo + item.itemValue);
                buttonElement.SetCostText(item.itemValueCost.ToString());
            }
            else
            {
                buttonElement.SetInfoText(item.itemDurationInfo + item.itemDuration);
                buttonElement.SetCostText(item.itemDurationCost.ToString());
            }

            buttonElements.Add(buttonElement);
        }
    }

    private void OnClickUpgradeButton(int _btnIdx)
    {
        ButtonData buttonData = buttonElements[_btnIdx].GetButtonData;
        ItemModel item = itemManager.GetItemModel(buttonData.itemType);

        int curGold = scoreManager.GetGold;


        if (buttonData.isValueUpgradeBtn)
        {
            if(curGold >= item.itemValueCost)
            {
                scoreManager.UseGold(item.itemValueCost);
                RefreshGoldText();

                itemManager.UpgradeItemValue(buttonData.itemType);
                buttonElements[_btnIdx].SetInfoText(item.itemValueInfo + item.itemValue);
                buttonElements[_btnIdx].SetCostText(item.itemValueCost.ToString());
            }
        }
        else
        {
            if(curGold >= item.itemDurationCost)
            {
                scoreManager.UseGold(item.itemDurationCost);
                RefreshGoldText();

                itemManager.UpgradeItemDuration(buttonData.itemType);
                buttonElements[_btnIdx].SetInfoText(item.itemDurationInfo + item.itemDuration);
                buttonElements[_btnIdx].SetCostText(item.itemDurationCost.ToString());
            }
        }
    }

    private void OnClickCloseBtn()
    {
        UIManager.getInstance.Hide();
    }

    private void RefreshGoldText()
    {
        goldText.text = GOLD + scoreManager.GetGold;
    }
}
