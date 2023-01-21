using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.MUIP;

public enum NotationType
{
    Standard,
    Scientific,
    Engineering,
    Logarithmic,
    Letter,
}

public enum BuyAmount
{
    x1,
    x5,
    x10,
    x25,
    x100,
    MAX,
}

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public ButtonManager NotationTypeButton;
    public ButtonManager BuyAmountButton;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameSettings in this scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartSettings()
    {
        SyncSettings();
    }

    public void ChangeSetting(string settingName)
    {
        switch (settingName)
        {
            case "notation":
                RotateNotationType();
                break;
            case "buyAmount":
                RotateBuyAmount();
                break;
            default:
                Debug.LogError($"GameSettings.ChangeSettings: settingName does not exist: {settingName}");
                break;
        }
        SyncSettings(settingName);
    }

    public void SyncSettings(string settingName = "")
    {
        if (settingName == "notation" || settingName == "")
        {
            NotationTypeButton.SetText(GameController.Instance.data.NotationType.ToString());
        }

        if (settingName == "buyAmount" || settingName == "")
        {
            BuyAmountButton.SetText(GameController.Instance.data.BuyAmount.ToString());
        }
    }

    public void RotateBuyAmount()
    {
        int enumLength = System.Enum.GetValues(typeof(BuyAmount)).Length;
        GameController.Instance.data.BuyAmount++;

        if ((int)GameController.Instance.data.BuyAmount >= enumLength)
        {
            GameController.Instance.data.BuyAmount = 0;
        }
    }

    public void RotateNotationType()
    {
        int enumLength = System.Enum.GetValues(typeof(NotationType)).Length;
        GameController.Instance.data.NotationType++;

        if ((int)GameController.Instance.data.NotationType >= enumLength)
        {
            GameController.Instance.data.NotationType = 0;
        }
    }
}
