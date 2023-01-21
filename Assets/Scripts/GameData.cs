using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BreakInfinity;

[Serializable]
public class GameData
{
    public BigDouble coinsAmount;
    public BigDouble prestigeCoinsAmount;

    public List<BigDouble> clickUpgradeLevel;
    public List<BigDouble> productionUpgradeLevel;
    public List<BigDouble> productionUpgradeGenerated;
    public List<BigDouble> generatorsUpgradeLevel;

    public NotationType NotationType;
    public BuyAmount BuyAmount;

    public GameData()
    {
        coinsAmount = 0;
        prestigeCoinsAmount = 0;

        clickUpgradeLevel = new BigDouble[4].ToList();
        productionUpgradeLevel = new BigDouble[4].ToList();
        productionUpgradeGenerated = new BigDouble[4].ToList();
        generatorsUpgradeLevel = new BigDouble[4].ToList();

        // Settings
        NotationType = NotationType.Letter;
    }

    public void PrestigeReset()
    {
        coinsAmount = 0;

        clickUpgradeLevel = new BigDouble[4].ToList();
        productionUpgradeLevel = new BigDouble[4].ToList();
        productionUpgradeGenerated = new BigDouble[4].ToList();
        generatorsUpgradeLevel = new BigDouble[4].ToList();
    }

    public void AddCoins(BigDouble numberToAdd)
    {
        coinsAmount += numberToAdd;
    }

    public void SubtractCoins(BigDouble numberToSubtract)
    {
        coinsAmount -= numberToSubtract;
    }
}
