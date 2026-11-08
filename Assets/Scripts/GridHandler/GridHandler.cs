using System.Collections.Generic;
using UnityEngine;


public class GridTileClass
{
    public GameObject gridTile;

    public GridTileClass(GameObject gridTile)
    {
        this.gridTile = gridTile;
    }
}

public class GridHandler : MonoBehaviour
{
    public static GridHandler Instance;

    [SerializeField] private GameObject gridTile;
    [SerializeField] private Transform center;

    [SerializeField] private int gridSize;
    [SerializeField] private float spacing;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth;

    private GameObject[,] _gridTiles;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        center = transform;
        SpawnGrid();
        DrawGridLines();
    }

    

    private void SpawnGrid()
    {
        _gridTiles = new GameObject[gridSize, gridSize];
        float halfSize = gridSize / 2f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = center.position + new Vector3(
                    (x - halfSize + 0.5f) * spacing,
                    0,
                    (z - halfSize + 0.5f) * spacing
                );

                GameObject gridTileClone = Instantiate(gridTile, position, Quaternion.identity);
                _gridTiles[x, z] = gridTileClone;
            }
        }
    }

    private void DrawGridLines()
    {
        float halfSize = gridSize * spacing / 2f;

        for (int i = 0; i <= gridSize; i++)
        {
            float offset = -halfSize + i * spacing;

            CreateLine(
                center.position + new Vector3(offset, ReturnYAxis(), -halfSize),
                center.position + new Vector3(offset, ReturnYAxis(), halfSize)
            );

            CreateLine(
                center.position + new Vector3(-halfSize, ReturnYAxis(), offset),
                center.position + new Vector3(halfSize, ReturnYAxis(), offset)
            );
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("Grid Line");
        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = lineMaterial;
        line.useWorldSpace = true;
    }

    private float ReturnYAxis()
    {
        if (_gridTiles[0,0].TryGetComponent(out Collider collider))
        {
            return collider.bounds.max.y;
        }

        return 0;
    }

    public GameObject[,] ReturnGrid()
    {
        return _gridTiles;
    }
    public Vector3 ReturnPosition(Vector2Int grid)
    {

        Transform tile = _gridTiles[grid.x, grid.y].transform;
        Vector3 position = new Vector3(tile.position.x, tile.position.y + 0.2f, tile.position.z);
        return position;
    }
}
