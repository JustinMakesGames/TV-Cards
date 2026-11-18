using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public string playerName;
    public int xPosition;
    public int yPosition;
    public int coinAmount;
    public int heartAmount;

    public PlayerSaveData(PlayerData playerData)
    {
        playerName = playerData.playerName;
        xPosition = playerData.currentPlayerPosition.x;
        yPosition = playerData.currentPlayerPosition.y;
        coinAmount = playerData.Coins;
        heartAmount = playerData.HeartPoints;
    }
}
public static class PlayerSaveManager
{
    public static void SavePlayerData(PlayerData playerData, string playerName)
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
