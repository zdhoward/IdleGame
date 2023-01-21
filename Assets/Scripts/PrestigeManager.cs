using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.MUIP;
using BreakInfinity;

public class PrestigeManager : MonoBehaviour
{
    public static PrestigeManager Instance;

    public TMP_Text PrestigeGainsLabel;
    public TMP_Text PrestigeCurrencyLabel;

    public ButtonManager PrestigeButton;
    public ModalWindowManager PrestigeConfirmationModal;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of PrestigeManager in this scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        PrestigeButton.SetText($"Prestige: {PrestigeGains().Notate()}");
        PrestigeCurrencyLabel.text = $"{GameController.Instance.data.prestigeCoinsAmount.Notate()} Prestige Coins";
    }

    public BigDouble PrestigeGains()
    {
        return BigDouble.Sqrt(GameController.Instance.data.coinsAmount / 1000);
    }

    public BigDouble PrestigeEffect()
    {
        return GameController.Instance.data.prestigeCoinsAmount / 100 + 1;
    }

    public void OpenPrestigeConfirmation()
    {
        PrestigeConfirmationModal.descriptionText = $"Reset all of your progress for {PrestigeGains().Notate()} Prestige Coins";
        PrestigeConfirmationModal.Open();
    }

    public void Prestige()
    {
        GameController.Instance.data.prestigeCoinsAmount += PrestigeGains();
        GameController.Instance.data.PrestigeReset();

        UpgradesManager.Instance.UpdateUpgradeUI(UpgradeTypes.Click);
        UpgradesManager.Instance.UpdateUpgradeUI(UpgradeTypes.Production);
        UpgradesManager.Instance.UpdateUpgradeUI(UpgradeTypes.Generators);

        GameController.Instance.Save();
    }

}
