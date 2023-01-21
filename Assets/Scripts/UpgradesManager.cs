using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;

public enum UpgradeTypes
{
    Click,
    Production,
    Generators,
}

public class UpgradesManager : MonoBehaviour
{
    public static UpgradesManager Instance;

    public UpgradeHandler[] UpgradeHandlers;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of UpgradesManager!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartUpgradeManager()
    {
        Methods.UpgradeCheck(GameController.Instance.data.clickUpgradeLevel, 4);
        Methods.UpgradeCheck(GameController.Instance.data.productionUpgradeLevel, 4);
        Methods.UpgradeCheck(GameController.Instance.data.productionUpgradeGenerated, 4);
        Methods.UpgradeCheck(GameController.Instance.data.generatorsUpgradeLevel, 4);

        // Click Upgrades
        UpgradeHandlers[0].UpgradeNames = new[] { "Click Power +1", "Click Power +5", "Click Power + 10", "Click Power + 25" };
        UpgradeHandlers[0].UpgradeBaseCost = new BigDouble[] { 10, 50, 100, 1000 };
        UpgradeHandlers[0].UpgradeCostMult = new BigDouble[] { 1.25, 1.35, 1.55, 2 };
        UpgradeHandlers[0].UpgradeBasePower = new BigDouble[] { 1, 5, 10, 25 };
        UpgradeHandlers[0].UpgradeUnlock = new BigDouble[] { 0, 25, 50, 500 };

        CreateUpgrades(GameController.Instance.data.clickUpgradeLevel, 0);

        // Production Upgrades
        UpgradeHandlers[1].UpgradeNames = new[] { "+1  <sprite name=\"coinIcon\">/s", "+5  <sprite name=\"coinIcon\">/s", "+10  <sprite name=\"coinIcon\">/s", "+25  <sprite name=\"coinIcon\">/s" };
        UpgradeHandlers[1].UpgradeBaseCost = new BigDouble[] { 25, 100, 1000, 25000 };
        UpgradeHandlers[1].UpgradeCostMult = new BigDouble[] { 1.5, 1.75, 2, 3 };
        UpgradeHandlers[1].UpgradeBasePower = new BigDouble[] { 1, 5, 10, 100 };
        UpgradeHandlers[1].UpgradeUnlock = new BigDouble[] { 0, 50, 500, 12500 };

        CreateUpgrades(GameController.Instance.data.productionUpgradeLevel, 1);

        // Generators Upgrades
        UpgradeHandlers[2].UpgradeNames = new[] {
            $"+0.1 \"{UpgradeHandlers[1].UpgradeNames[0]}\" Upgrades/s",
            $"+0.05 \"{UpgradeHandlers[1].UpgradeNames[1]}\" Upgrades/s",
            $"+0.02 \"{UpgradeHandlers[1].UpgradeNames[2]}\" Upgrades/s",
            $"+0.01 \"{UpgradeHandlers[1].UpgradeNames[3]}\" Upgrades/s",
            };
        UpgradeHandlers[2].UpgradeBaseCost = new BigDouble[] { 5000, 1e4, 1e5, 1e6 };
        UpgradeHandlers[2].UpgradeCostMult = new BigDouble[] { 1.25, 1.5, 2, 2.5 };
        UpgradeHandlers[2].UpgradeBasePower = new BigDouble[] { 0.1, 0.05, 0.02, 0.01 };
        UpgradeHandlers[2].UpgradeUnlock = new BigDouble[] { 2500, 5e3, 5e4, 5e5 };

        CreateUpgrades(GameController.Instance.data.generatorsUpgradeLevel, 2);

        UpdateUpgradeUI(UpgradeTypes.Click);
        UpdateUpgradeUI(UpgradeTypes.Production);
        UpdateUpgradeUI(UpgradeTypes.Generators);

        void CreateUpgrades<T>(List<T> level, int index)
        {
            foreach (Transform upgrade in UpgradeHandlers[index].UpgradesPanel.transform)
            {
                Destroy(upgrade.gameObject);
            }

            UpgradeHandlers[index].Upgrades.Clear();

            for (int i = 0; i < level.Count; i++)
            {
                Upgrades upgrade = Instantiate(UpgradeHandlers[index].UpgradePrefab, UpgradeHandlers[index].UpgradesPanel);
                upgrade.UpgradeID = i;
                upgrade.gameObject.SetActive(false);
                UpgradeHandlers[index].Upgrades.Add(upgrade);
            }

            UpgradeHandlers[index].UpgradesScrollRect.normalizedPosition = new Vector2(1, 1);
        }
    }

    void Update()
    {
        UnlockUpgrades(GameController.Instance.data.coinsAmount, UpgradeHandlers[0].UpgradeUnlock, 0);
        UnlockUpgrades(GameController.Instance.data.coinsAmount, UpgradeHandlers[1].UpgradeUnlock, 1);
        UnlockUpgrades(GameController.Instance.data.coinsAmount, UpgradeHandlers[2].UpgradeUnlock, 2);

        void UnlockUpgrades(BigDouble currency, BigDouble[] unlock, int index)
        {
            for (int i = 0; i < UpgradeHandlers[index].Upgrades.Count; i++)
            {
                if (!UpgradeHandlers[index].Upgrades[i].gameObject.activeSelf)
                {
                    switch (index)
                    {
                        case 0:
                            if (i == 0 || GameController.Instance.data.clickUpgradeLevel[i - 1] > 0 || GameController.Instance.data.clickUpgradeLevel[i] > 0)
                                UpgradeHandlers[index].Upgrades[i].gameObject.SetActive(GameController.Instance.data.coinsAmount >= unlock[i]);
                            break;
                        case 1:
                            if (i == 0 || GameController.Instance.data.productionUpgradeLevel[i - 1] > 0 || GameController.Instance.data.productionUpgradeLevel[i] > 0)
                                UpgradeHandlers[index].Upgrades[i].gameObject.SetActive(GameController.Instance.data.coinsAmount >= unlock[i]);
                            break;
                        case 2:
                            if (i == 0 || GameController.Instance.data.generatorsUpgradeLevel[i - 1] > 0 || GameController.Instance.data.generatorsUpgradeLevel[i] > 0)
                                UpgradeHandlers[index].Upgrades[i].gameObject.SetActive(GameController.Instance.data.coinsAmount >= unlock[i]);
                            break;

                    }
                }
            }
        }

        if (UpgradeHandlers[0].UpgradesScrollRect.transform.parent.gameObject.activeSelf)
        {
            UpgradeHandlers[0].UpgradeNames = new[] {
                $"Click Power +{(UpgradeHandlers[0].UpgradeBasePower[0] * PrestigeManager.Instance.PrestigeEffect()).Notate()}",
                $"Click Power +{(UpgradeHandlers[0].UpgradeBasePower[1] * PrestigeManager.Instance.PrestigeEffect()).Notate()}",
                $"Click Power +{(UpgradeHandlers[0].UpgradeBasePower[2] * PrestigeManager.Instance.PrestigeEffect()).Notate()}",
                $"Click Power +{(UpgradeHandlers[0].UpgradeBasePower[3] * PrestigeManager.Instance.PrestigeEffect()).Notate()}"
                };
            UpdateUpgradeUI(UpgradeTypes.Click);
        }
        if (UpgradeHandlers[1].UpgradesScrollRect.transform.parent.gameObject.activeSelf)
        {
            UpgradeHandlers[1].UpgradeNames = new[] {
                $"+{(UpgradeHandlers[1].UpgradeBasePower[0] * PrestigeManager.Instance.PrestigeEffect()).Notate()}  <sprite name=\"coinIcon\">/s",
                $"+{(UpgradeHandlers[1].UpgradeBasePower[1] * PrestigeManager.Instance.PrestigeEffect()).Notate()}  <sprite name=\"coinIcon\">/s",
                $"+{(UpgradeHandlers[1].UpgradeBasePower[2] * PrestigeManager.Instance.PrestigeEffect()).Notate()}  <sprite name=\"coinIcon\">/s",
                $"+{(UpgradeHandlers[1].UpgradeBasePower[3] * PrestigeManager.Instance.PrestigeEffect()).Notate()}  <sprite name=\"coinIcon\">/s"
                };

            UpdateUpgradeUI(UpgradeTypes.Production);
        }
        if (UpgradeHandlers[2].UpgradesScrollRect.transform.parent.gameObject.activeSelf)
        {
            UpgradeHandlers[2].UpgradeNames = new[] {
            $"+{(UpgradeHandlers[2].UpgradeBasePower[0] * PrestigeManager.Instance.PrestigeEffect()).Notate()} \"{UpgradeHandlers[1].UpgradeNames[0]}\" Upgrades/s",
            $"+{(UpgradeHandlers[2].UpgradeBasePower[1] * PrestigeManager.Instance.PrestigeEffect()).Notate()} \"{UpgradeHandlers[1].UpgradeNames[1]}\" Upgrades/s",
            $"+{(UpgradeHandlers[2].UpgradeBasePower[2] * PrestigeManager.Instance.PrestigeEffect()).Notate()} \"{UpgradeHandlers[1].UpgradeNames[2]}\" Upgrades/s",
            $"+{(UpgradeHandlers[2].UpgradeBasePower[3] * PrestigeManager.Instance.PrestigeEffect()).Notate()} \"{UpgradeHandlers[1].UpgradeNames[3]}\" Upgrades/s",
            };
            UpdateUpgradeUI(UpgradeTypes.Generators);
        }
    }

    public void UpdateUpgradeUI(UpgradeTypes type, int upgradeID = -1)
    {
        switch (type)
        {
            case UpgradeTypes.Click:
                UpdateAllUpgradeUIByType(UpgradeHandlers[0].Upgrades, GameController.Instance.data.clickUpgradeLevel, UpgradeHandlers[0].UpgradeNames, upgradeID);
                break;
            case UpgradeTypes.Production:
                UpdateAllUpgradeUIByType(UpgradeHandlers[1].Upgrades, GameController.Instance.data.productionUpgradeLevel, UpgradeHandlers[1].UpgradeNames, upgradeID, GameController.Instance.data.productionUpgradeGenerated);
                break;
            case UpgradeTypes.Generators:
                UpdateAllUpgradeUIByType(UpgradeHandlers[2].Upgrades, GameController.Instance.data.generatorsUpgradeLevel, UpgradeHandlers[2].UpgradeNames, upgradeID);
                break;
        }

        void UpdateAllUpgradeUIByType(List<Upgrades> upgrades, List<BigDouble> upgradeLevels, string[] upgradeNames, int ID, List<BigDouble> upgradeGenerated = null)
        {
            if (upgradeID == -1)
            {
                for (int i = 0; i < upgrades.Count; i++)
                {
                    UpdateUpgradeUIByID(upgrades, upgradeLevels, upgradeNames, i, upgradeGenerated);
                }
            }
            else
            {
                UpdateUpgradeUIByID(upgrades, upgradeLevels, upgradeNames, upgradeID, upgradeGenerated);
            }
        }

        void UpdateUpgradeUIByID(List<Upgrades> upgrades, List<BigDouble> upgradeLevels, string[] upgradeNames, int ID, List<BigDouble> upgradeGenerated = null)
        {
            BigDouble generated = upgradeGenerated == null ? 0 : upgradeGenerated[ID];

            BigDouble cost = GetUpgradeCost(type, ID);

            upgrades[ID].levelLabel.text = (upgradeLevels[ID] + generated).Notate();
            upgrades[ID].valueLabel.text = upgradeNames[ID];
            upgrades[ID].costLabel.text = $"Cost: <sprite name=\"coinIcon\"> {cost.Notate()}";
            upgrades[ID].progressBar.ChangeValue(Methods.Fill(GameController.Instance.data.coinsAmount, cost));

            if (GameController.Instance.data.coinsAmount < cost && upgrades[ID].button.isInteractable == true)
                upgrades[ID].button.Interactable(false);
            else if (GameController.Instance.data.coinsAmount >= cost && upgrades[ID].button.isInteractable == false)
                upgrades[ID].button.Interactable(true);
        }

    }

    public BigDouble GetUpgradeCost(UpgradeTypes type, int upgradeID)
    {
        switch (type)
        {
            case UpgradeTypes.Click:
                return GetCost(0, GameController.Instance.data.clickUpgradeLevel);
            case UpgradeTypes.Production:
                return GetCost(1, GameController.Instance.data.productionUpgradeLevel);
            case UpgradeTypes.Generators:
                return GetCost(2, GameController.Instance.data.generatorsUpgradeLevel);
        }

        return 0;

        BigDouble GetCost(int index, List<BigDouble> levels)
        {
            return UpgradeHandlers[index].UpgradeBaseCost[upgradeID] * BigDouble.Pow(UpgradeHandlers[index].UpgradeCostMult[upgradeID], levels[upgradeID]);
        }
    }

    public void BuyUpgrade(UpgradeTypes type, int upgradeID)
    {
        switch (type)
        {
            case UpgradeTypes.Click:
                Buy(GameController.Instance.data.clickUpgradeLevel);
                break;
            case UpgradeTypes.Production:
                Buy(GameController.Instance.data.productionUpgradeLevel);
                break;
            case UpgradeTypes.Generators:
                Buy(GameController.Instance.data.generatorsUpgradeLevel);
                break;
        }

        void Buy(List<BigDouble> upgradeLevels)
        {
            if (GameController.Instance.data.coinsAmount >= GetUpgradeCost(type, upgradeID))
            {
                GameController.Instance.data.SubtractCoins(GetUpgradeCost(type, upgradeID));
                upgradeLevels[upgradeID] += 1;

                UpdateUpgradeUI(type, upgradeID);
            }
        }
    }

    public void BuyMax()
    {
        GameData data = GameController.Instance.data;

        for (int i = 0; i < data.clickUpgradeLevel.Count; i++)
        {
            Methods.BuyMax(ref data.coinsAmount, ref data.clickUpgradeLevel, i, UpgradeHandlers[0].UpgradeBaseCost[i], (float)UpgradeHandlers[0].UpgradeCostMult[i].ToDouble());
        }

        for (int i = 0; i < data.productionUpgradeLevel.Count; i++)
        {
            Methods.BuyMax(ref data.coinsAmount, ref data.productionUpgradeLevel, i, UpgradeHandlers[1].UpgradeBaseCost[i], (float)UpgradeHandlers[1].UpgradeCostMult[i].ToDouble());
        }

        for (int i = 0; i < data.generatorsUpgradeLevel.Count; i++)
        {
            Methods.BuyMax(ref data.coinsAmount, ref data.generatorsUpgradeLevel, i, UpgradeHandlers[2].UpgradeBaseCost[i], (float)UpgradeHandlers[2].UpgradeCostMult[i].ToDouble());
        }
    }
}
