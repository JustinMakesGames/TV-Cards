using UnityEngine;

public class GridHandler : MonoBehaviour
{
    [SerializeField] private GameObject gridTile;
    [SerializeField] private Transform center;

    [SerializeField] private int gridSize;
    [SerializeField] private float spacing;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth;


    private void Start()
    {
        center = transform;
        SpawnGrid();
        DrawGridLines();
    }

    private void SpawnGrid()
    {
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

                Instantiate(gridTile, position, Quaternion.identity);
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
                center.position + new Vector3(offset, 0, -halfSize),
                center.position + new Vector3(offset, 0, halfSize)
            );

            CreateLine(
                center.position + new Vector3(-halfSize, 0, offset),
                center.position + new Vector3(halfSize, 0, offset)
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
}
