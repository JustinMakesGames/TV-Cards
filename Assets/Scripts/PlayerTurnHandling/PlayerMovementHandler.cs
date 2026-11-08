using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovementHandler : MonoBehaviour
{

    [SerializeField] private GameObject selectionObject;

    private GameObject _currentSelectionObject;
    private bool _canMove;

    private GameObject[,] _grid;
    private Vector2Int _playerPosition;

    private List<GameObject> _walkedTiles = new List<GameObject>();
    private GameObject _currentTile;

    public void StartMovement(int movement)
    {
        _canMove = true;
        _currentSelectionObject = Instantiate(selectionObject);
        _grid = GridHandler.Instance.ReturnGrid();
        
        if (TryGetComponent(out PlayerData playerData))
        {
            _playerPosition = playerData.ReturnPlayerPosition();
        }

        SetSelectedObject(_playerPosition);

        
    }



    public void MoveSelectionObject(InputAction.CallbackContext context)
    {
        if (!context.performed || !_canMove) return;

        Vector2 direction = context.ReadValue<Vector2>();

        MoveDirection(direction);

    }

    public void MoveDirection(Vector2 direction)
    {

        Vector2Int roundedUpDirection = new Vector2Int(Mathf.CeilToInt(direction.x), Mathf.CeilToInt(direction.y));
        Vector2Int newPosition = _playerPosition + roundedUpDirection;
        SetSelectedObject(newPosition);
        CheckIfTileIsPreviousInList();
    }

    private void SetSelectedObject(Vector2Int position)
    {

        if (!IsValidPosition(position)) return;
        _currentSelectionObject.transform.position = GridHandler.Instance.ReturnPosition(position);
        _playerPosition = position;
        _currentTile = _grid[position.x, position.y];

    }

    private void CheckIfTileIsPreviousInList()
    {
        if (_walkedTiles.Count <= 0 || _currentTile != _walkedTiles[_walkedTiles.Count - 1])
        {
            _walkedTiles.Add(_currentTile);
        }

        else
        {

        }
    }

    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < _grid.GetLength(0) &&
        position.y >= 0 && position.y < _grid.GetLength(1);
    }
    

    

    

    
}
