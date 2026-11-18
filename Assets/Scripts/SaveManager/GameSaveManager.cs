using UnityEngine;
using System.IO;

public static class GameSaveManager
{
    public static void SaveGameData(PlayerData playerData, string playerName)
    {
        string path = Application.persistentDataPath + $"/{playerName}.json";
        PlayerSaveData data = new PlayerSaveData(playerData);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved Data");
    }

    public static PlayerSaveData LoadPlayerData(string playerName)
    {
        string path = Application.persistentDataPath + $"/{playerName}.json";
        if (!File.Exists(path))
        {
            Debug.Log("No save file found at: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        Debug.Log("Loaded Savedata");
        return data;
    }

    public static void DeletePlayerData(string playerName)
    {
        string path = Application.persistentDataPath + $"{playerName}.json";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
