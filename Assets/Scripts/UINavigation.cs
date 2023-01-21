using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINavigation : MonoBehaviour
{
    public GameObject HomeScreen;
    public GameObject SettingsScreen;

    void Start()
    {
        SwitchUpgradesTab("click");
    }

    public void SwitchUpgradesTab(string tab)
    {
        UpgradesManager.Instance.UpgradeHandlers[0].UpgradesScrollRect.transform.parent.gameObject.SetActive(false);
        UpgradesManager.Instance.UpgradeHandlers[1].UpgradesScrollRect.transform.parent.gameObject.SetActive(false);
        UpgradesManager.Instance.UpgradeHandlers[2].UpgradesScrollRect.transform.parent.gameObject.SetActive(false);

        switch (tab)
        {
            case "click":
                UpgradesManager.Instance.UpgradeHandlers[0].UpgradesScrollRect.transform.parent.gameObject.SetActive(true);
                break;
            case "production":
                UpgradesManager.Instance.UpgradeHandlers[1].UpgradesScrollRect.transform.parent.gameObject.SetActive(true);
                break;
            case "generator":
                UpgradesManager.Instance.UpgradeHandlers[2].UpgradesScrollRect.transform.parent.gameObject.SetActive(true);
                break;
        }
    }

    public void SwitchScreen(string screen)
    {
        HomeScreen.SetActive(false);
        SettingsScreen.SetActive(false);

        switch (screen)
        {
            case "home":
                HomeScreen.SetActive(true);
                break;
            case "settings":
                SettingsScreen.SetActive(true);
                break;
        }
    }
}
