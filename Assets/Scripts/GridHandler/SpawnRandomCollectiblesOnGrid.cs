using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectibleSpawnSettings
{
    public GameObject prefab;

    [Tooltip("How much of the board this collectible should cover. 0.1 = 10% of all tiles.")]
    [Range(0f, 1f)] public float coverage = 0.1f;

    [Tooltip("Never have fewer than this many on the board (as long as there is room).")]
    public int minAmount = 1;

    [Tooltip("Never have more than this many on the board.")]
    public int maxAmount = 10;
}

public class SpawnRandomCollectiblesOnGrid : MonoBehaviour
{
    [SerializeField] private List<CollectibleSpawnSettings> objectsToSpawn = new List<CollectibleSpawnSettings>();

    [Tooltip("The board as a whole can never be covered by collectibles more than this. 0.5 = half the tiles.")]
    [SerializeField, Range(0f, 1f)] private float maxTotalCoverage = 0.5f;

    private GameObject[,] _grid;

    public void SpawnCollectibles()
    {
        _grid = GridHandler.Instance.ReturnGrid();
        if (_grid == null) return;

        List<GridTile> freeTiles = GetFreeTiles();
        int spaceLeftOnBoard = CalculateTotalSpaceLeft();

        foreach (CollectibleSpawnSettings settings in objectsToSpawn)
        {
            if (settings.prefab == null) continue;

            int amountToSpawn = CalculateAmountToSpawn(settings);
            amountToSpawn = Mathf.Min(amountToSpawn, spaceLeftOnBoard, freeTiles.Count);

            for (int i = 0; i < amountToSpawn; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, freeTiles.Count);
                freeTiles[randomIndex].GetCollectible(settings.prefab);
                freeTiles.RemoveAt(randomIndex);
            }

            spaceLeftOnBoard -= amountToSpawn;
        }
    }

    public int CalculateAmountToSpawn(CollectibleSpawnSettings settings)
    {
        int targetAmount = CalculateTargetAmount(settings);
        int amountOnBoard = CountOnBoard(settings.prefab);

        return Mathf.Max(0, targetAmount - amountOnBoard);
    }

    public int CalculateTargetAmount(CollectibleSpawnSettings settings)
    {
        int totalTiles = _grid.GetLength(0) * _grid.GetLength(1);
        int amount = Mathf.RoundToInt(totalTiles * settings.coverage);

        int max = Mathf.Max(settings.minAmount, settings.maxAmount);
        return Mathf.Clamp(amount, settings.minAmount, max);
    }

    private int CalculateTotalSpaceLeft()
    {
        int totalTiles = _grid.GetLength(0) * _grid.GetLength(1);
        int maxCollectibles = Mathf.FloorToInt(totalTiles * maxTotalCoverage);

        int collectiblesOnBoard = 0;
        foreach (GameObject tileObject in _grid)
        {
            if (tileObject.TryGetComponent(out GridTile tile) && tile.HasCollectible())
            {
                collectiblesOnBoard++;
            }
        }

        return Mathf.Max(0, maxCollectibles - collectiblesOnBoard);
    }

    private int CountOnBoard(GameObject prefab)
    {
        int count = 0;
        foreach (GameObject tileObject in _grid)
        {
            if (tileObject.TryGetComponent(out GridTile tile) && tile.HasCollectible() && tile.ReturnCollectiblePrefab() == prefab)
            {
                count++;
            }
        }

        return count;
    }

    private List<GridTile> GetFreeTiles()
    {
        HashSet<Vector2Int> playerPositions = GetPlayerPositions();
        List<GridTile> freeTiles = new List<GridTile>();

        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int z = 0; z < _grid.GetLength(1); z++)
            {
                if (playerPositions.Contains(new Vector2Int(x, z))) continue;

                if (_grid[x, z].TryGetComponent(out GridTile tile) && !tile.HasCollectible() && tile.ReturnPlayer() == null)
                {
                    freeTiles.Add(tile);
                }
            }
        }

        return freeTiles;
    }

    private HashSet<Vector2Int> GetPlayerPositions()
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
        if (GameManager.Instance == null || GameManager.Instance.ReturnPlayers() == null) return positions;

        foreach (Transform player in GameManager.Instance.ReturnPlayers())
        {
            if (player != null && player.TryGetComponent(out PlayerData playerData))
            {
                positions.Add(playerData.ReturnPlayerPosition());
            }
        }

        return positions;
    }
}
