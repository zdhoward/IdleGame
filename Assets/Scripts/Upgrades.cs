using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.MUIP;

public class Upgrades : MonoBehaviour
{
    public int UpgradeID;

    public ButtonManager button;

    public TMP_Text levelLabel;
    public TMP_Text valueLabel;
    public TMP_Text costLabel;

    public ProgressBar progressBar;

    public void BuyClickUpgrade()
    {
        UpgradesManager.Instance.BuyUpgrade(UpgradeTypes.Click, UpgradeID);
    }

    public void BuyProductionUpgrade()
    {
        UpgradesManager.Instance.BuyUpgrade(UpgradeTypes.Production, UpgradeID);
    }

    public void BuyGeneratorUpgrade()
    {
        UpgradesManager.Instance.BuyUpgrade(UpgradeTypes.Generators, UpgradeID);
    }
}
