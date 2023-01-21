using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;

public class UpgradeHandler : MonoBehaviour
{
    // Click Upgrades
    [Header("Click Upgrades")]
    public List<Upgrades> Upgrades;
    public Upgrades UpgradePrefab;

    public ScrollRect UpgradesScrollRect;
    public Transform UpgradesPanel;

    public string[] UpgradeNames;

    public BigDouble[] UpgradeBaseCost;
    public BigDouble[] UpgradeCostMult;
    public BigDouble[] UpgradeBasePower;
    public BigDouble[] UpgradeUnlock;
}
