using Unity.VisualScripting;
using UnityEngine;

public enum Players
{
    RedPlayer = 0,
    BluePlayer = 1,
    GreenPlayer = 2,
    YellowPlayer = 3
}
public static class PlayerHolder
{
    public static Transform[] players;

    public static void SetPlayers(Transform[] newPlayers)
    {
        players = newPlayers;
    }

    public static Transform[] ReturnPlayers()
    {
        return players;
    }
    public static void SetPlayerDialogueOption(Players player)
    {

        if (players[(int)player] == null) return;

        if (players[(int)player].TryGetComponent(out PlayerDialogueHandler handler))
        {
            handler.LetPlayerContinueDialogue();
        }
    }

    public static void SetPlayerUISelection(Players player, GameObject[,] uiSelections)
    {

        if (players[(int)player] == null) return;

        if (players[(int)player].TryGetComponent(out PlayerUISelectionHandler handler))
        {
            handler.SetPossibleGameObjects(uiSelections);
        }
    }

    
}
