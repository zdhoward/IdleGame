using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BreakInfinity;
using Michsky.MUIP;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public string dataFileName = "GameData";
    public GameData data;

    [SerializeField] TMP_Text coinsLabel;
    [SerializeField] ButtonManager coinClickPowerLabel;
    [SerializeField] TMP_Text coinsPerSecondLabel;

    public float SaveTime;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of GameController!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        data = SaveSystem.LoadOrNewGame();

        UpgradesManager.Instance.StartUpgradeManager();
        GameSettings.Instance.StartSettings();
    }

    void Update()
    {
        coinsLabel.text = $"<sprite name=\"coinIcon\">{data.coinsAmount.Notate()}";
        coinClickPowerLabel.SetText($"+{GetClickPower().Notate()}  <sprite name=\"coinIcon\">");
        coinsPerSecondLabel.text = $"{GetCoinsPerSecond().Notate()}  <sprite name=\"coinIcon\">/s";

        data.AddCoins(GetCoinsPerSecond() * Time.deltaTime);

        for (int i = 0; i < data.productionUpgradeLevel.Count; i++)
        {
            data.productionUpgradeGenerated[i] += GetUpgradesPerSecond(i) * Time.deltaTime;
        }

        SaveTime += Time.deltaTime * (1 / Time.timeScale);
        if (SaveTime >= 15)
        {
            Save();
            SaveTime = 0;
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public void GenerateCoins()
    {
        data.AddCoins(GetClickPower());
    }

    public BigDouble GetClickPower()
    {
        BigDouble total = 1;

        for (int i = 0; i < data.clickUpgradeLevel.Count; i++)
        {
            total += UpgradesManager.Instance.UpgradeHandlers[0].UpgradeBasePower[i] * data.clickUpgradeLevel[i];
        }

        return total * PrestigeManager.Instance.PrestigeEffect();
    }

    public BigDouble GetCoinsPerSecond()
    {
        BigDouble total = 0;

        for (int i = 0; i < data.productionUpgradeLevel.Count; i++)
        {
            total += UpgradesManager.Instance.UpgradeHandlers[0].UpgradeBasePower[i] * (data.productionUpgradeLevel[i] + data.productionUpgradeGenerated[i]);
        }

        return total * PrestigeManager.Instance.PrestigeEffect();
    }

    public BigDouble GetUpgradesPerSecond(int index)
    {
        return UpgradesManager.Instance.UpgradeHandlers[2].UpgradeBasePower[index] * data.generatorsUpgradeLevel[index] * PrestigeManager.Instance.PrestigeEffect();
    }

    public void Save()
    {
        SaveSystem.SaveData(data, dataFileName);
    }


    public void ResetGameData()
    {
        data = new GameData();
        Start();
    }
}
