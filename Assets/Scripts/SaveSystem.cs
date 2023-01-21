using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using TMPro;

public class SaveSystem : MonoBehaviour
{
    const string WEBGL_SAVE_NAME = "WEBGL_SAVE";

    public TMP_InputField ImportField;
    public TMP_InputField ExportField;
    public WebGLNativeInputField ImportFieldWebGL;
    public WebGLNativeInputField ExportFieldWebGL;

    public GameObject ImportExportObject;
    public GameObject ImportExportObjectWebGL;

    const string FileType = ".sav";
    const string FilePath = "GameData";
    static string SavePath => Application.persistentDataPath + "/Saves/";
    static string BackUpSavePath => Application.persistentDataPath + "/BackUps/";

    private static int SaveCount;

    void Start()
    {
#if UNITY_WEBGL
        ImportExportObject.SetActive(false);
        ImportExportObjectWebGL.SetActive(true);
#else
        ImportExportObject.SetActive(true);
        ImportExportObjectWebGL.SetActive(false);
#endif
    }

    public static void SaveData<T>(T data, string fileName)
    {
#if UNITY_WEBGL
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        formatter.Serialize(memoryStream, data);
        string dataToSave = Convert.ToBase64String(memoryStream.ToArray());

        //WebGLFileSaver.SaveFile(dataToSave, fileName);
        PlayerPrefs.SetString(WEBGL_SAVE_NAME, dataToSave);
#else
        Directory.CreateDirectory(SavePath);
        Directory.CreateDirectory(BackUpSavePath);

        if (SaveCount % 5 == 0)
            Save(BackUpSavePath);
        Save(SavePath);

        void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path + fileName + FileType))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();
                formatter.Serialize(memoryStream, data);
                string dataToSave = Convert.ToBase64String(memoryStream.ToArray());
                writer.WriteLine(dataToSave);
                writer.Close();
            }
        }
#endif
    }

#if UNITY_WEBGL
    public static T LoadData<T>(string fileName)
    {
        string dataToLoad = PlayerPrefs.GetString(WEBGL_SAVE_NAME);

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(dataToLoad));

        return (T)formatter.Deserialize(memoryStream);

    }
#else
public static T LoadData<T>(string fileName)
    {
        Directory.CreateDirectory(SavePath);
        Directory.CreateDirectory(BackUpSavePath);

        bool isBackUpNeeded = false;
        T dataToReturn;

        Load(SavePath);
        if (isBackUpNeeded)
            Load(BackUpSavePath);

        return dataToReturn;

        void Load(string path)
        {
            using (StreamReader reader = new StreamReader(path + fileName + FileType))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                string dataToLoad = reader.ReadToEnd();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(dataToLoad));

                try
                {
                    dataToReturn = (T)formatter.Deserialize(memoryStream);
                }
                catch
                {
                    isBackUpNeeded = true;
                    dataToReturn = default;
                }

                reader.Close();
            }
        }
    }
#endif

    public static GameData LoadOrNewGame()
    {
        GameData data;
#if UNITY_WEBGL
        data = PlayerPrefs.HasKey(WEBGL_SAVE_NAME) ? SaveSystem.LoadData<GameData>(GameController.Instance.dataFileName) : new GameData();
#else
        data = SaveSystem.SaveExists(GameController.Instance.dataFileName) ? SaveSystem.LoadData<GameData>(GameController.Instance.dataFileName) : new GameData();
#endif
        return data;
    }

    public static bool SaveExists(string fileName)
    {
#if UNITY_WEBGL
        return PlayerPrefs.HasKey(WEBGL_SAVE_NAME);
#else
        return File.Exists(SavePath + fileName + FileType) || File.Exists(BackUpSavePath + fileName + FileType);
#endif
    }

    public void Import()
    {
#if UNITY_WEBGL
        PlayerPrefs.SetString(WEBGL_SAVE_NAME, ImportFieldWebGL.text.Trim());
#else
        Directory.CreateDirectory(SavePath);

        using (StreamWriter writer = new StreamWriter(SavePath + FilePath + FileType))
        {
            writer.WriteLine(ImportField.text);
            writer.Close();
        }
#endif

        GameController.Instance.Start();
    }

    public void Export()
    {
        GameController.Instance.Save();

#if UNITY_WEBGL
        ExportFieldWebGL.text = PlayerPrefs.GetString(WEBGL_SAVE_NAME);
#else
        Directory.CreateDirectory(SavePath);

        using (StreamReader reader = new StreamReader(SavePath + FilePath + FileType))
        {
            ExportField.text = reader.ReadToEnd();
            reader.Close();
        }
#endif
    }

    //     public void Import()
    //     {
    //         Directory.CreateDirectory(SavePath);

    //         using (StreamWriter writer = new StreamWriter(SavePath + FilePath + FileType))
    //         {
    // #if UNITY_WEBGL
    //             writer.WriteLine(ImportFieldWebGL.text);
    // #else
    //             writer.WriteLine(ImportField.text);
    // #endif
    //             writer.Close();
    //         }

    //         GameController.Instance.Start();
    //     }

    //     public void Export()
    //     {
    //         GameController.Instance.Save();
    //         Directory.CreateDirectory(SavePath);

    //         using (StreamReader reader = new StreamReader(SavePath + FilePath + FileType))
    //         {
    // #if UNITY_WEBGL
    //             ExportFieldWebGL.text = reader.ReadToEnd();
    // #else
    //             ExportField.text = reader.ReadToEnd();
    // #endif
    //             reader.Close();
    //         }
    //     }

    public void CopyToClipboard()
    {
        if (ExportField.text != "")
        {
#if UNITY_WEBGL
            GUIUtility.systemCopyBuffer = ExportFieldWebGL.text;
#else
            GUIUtility.systemCopyBuffer = ExportField.text;
#endif
        }
    }

    public void PasteFromClipboard()
    {
#if UNITY_WEBGL
        ImportFieldWebGL.text = GUIUtility.systemCopyBuffer;
#else
        ImportField.text = GUIUtility.systemCopyBuffer;
#endif
    }

    public void ClearImportField()
    {
#if UNITY_WEBGL
        ImportFieldWebGL.text = "";
#else
        ImportField.text = "";
#endif
    }

    public void ClearExportField()
    {
#if UNITY_WEBGL
        ExportFieldWebGL.text = "";
#else
        ExportField.text = "";
#endif
    }
}
