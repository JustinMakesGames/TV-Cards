using System.Collections.Generic;
using UnityEngine;

public class AttackCardHandler : BasicCardHandler, ICardHandler
{
    [SerializeField] private int range;
    [SerializeField] private int damage;
    [SerializeField] private GameObject selectionObj;

    private List<GameObject> _tilesInRange = new List<GameObject>();
    private List<GameObject> _selectionObjClones = new List<GameObject>();

    private Transform _player;

    private GameObject[,] _grid;
    public override void OnHovered()
    {
        base.OnHovered();
        ShowRange();
    }

    public override void OnDehovered()
    {
        base.OnDehovered();
        DestroyShowObjs();
    }

    public void SetRange(Transform player, int range)
    {
        _player = player;
        this.range = range;
    }
    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);
        
        if (player.TryGetComponent(out PlayerAttackHandler attackHandler))
        {
            attackHandler.StartAttack(_tilesInRange, range, damage);
        }

        
    }

    private void ShowRange()
    {
        _grid = GridHandler.Instance.ReturnGrid();
        
        if (_player.TryGetComponent(out PlayerData playerData))
        {
            Vector2Int playerPosition = playerData.ReturnPlayerPosition();
            _tilesInRange = ReturnTilesInRange(playerPosition);

            ShowTileObjs(playerPosition);

        }

        
    }

    private void ShowTileObjs(Vector2Int center)
    {
        List<GameObject> highlightedObjs = ReturnTilesInRange(center);

        foreach (GameObject obj in highlightedObjs)
        {
            GameObject selectionObjClone = Instantiate(selectionObj, obj.transform.position + Vector3.up * 0.2f, obj.transform.rotation);

            _selectionObjClones.Add(selectionObjClone);

        }
    }

    private void DestroyShowObjs()
    {
        foreach (GameObject obj in _selectionObjClones)
        {
            Destroy(obj);
        }

        _selectionObjClones.Clear();
    }
    private List<GameObject> ReturnTilesInRange(Vector2Int center)
    {
        List<GameObject> tiles = new List<GameObject>();

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (x == 0 && y == 0) continue;
                if (Mathf.Abs(x) + Mathf.Abs(y) > range) continue;

                Vector2Int position = center + new Vector2Int(x, y);

                if (position.x < 0 || position.x >= _grid.GetLength(0) ||
                    position.y < 0 || position.y >= _grid.GetLength(1)) continue;

                tiles.Add(_grid[position.x, position.y]);
            }
        }

        return tiles;
    }

    


}
