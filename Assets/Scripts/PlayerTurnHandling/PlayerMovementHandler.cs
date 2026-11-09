using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovementHandler : MonoBehaviour
{

    [SerializeField] private GameObject selectionObject;
    [SerializeField] private float playerMovementSpeed;

    private GameObject _currentSelectionObject;
    private bool _canMove;

    private GameObject[,] _grid;
    private Vector2Int _playerPosition;

    private List<GameObject> _walkedTiles = new List<GameObject>();
    private GameObject _currentTile;


    private int _walkedTileNumber;
    private TMP_Text _walkedTileNumberText;

    private GameObject _card;

    private Transform _playerObject;

    public void StartMovement(GameObject card, int movement)
    {
        _walkedTiles.Clear();
        _card = card;
        _walkedTileNumber = movement;
        _canMove = true;
        _currentSelectionObject = Instantiate(selectionObject);
        _grid = GridHandler.Instance.ReturnGrid();
        
        if (TryGetComponent(out PlayerData playerData))
        {
            _playerPosition = playerData.ReturnPlayerPosition();
            _playerObject = playerData.ReturnPlayerObject();
        }

        if (_currentSelectionObject.TryGetComponent(out PlacementSelectorHandler placementSelectorHandler))
        {
            _walkedTileNumberText = placementSelectorHandler.ReturnMovementText();
        } 

        SetSelectedObject(_playerPosition);
        _walkedTiles.Add(_currentTile);
        SetNumber();

        
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

        if (!IsValidPosition(newPosition)) return;
        SetSelectedObject(newPosition);
        CheckIfTileIsPreviousInList();
        SetNumber();
    }

    public void SelectSpaceInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !_canMove || _walkedTileNumber != 0) return;

        SelectSpace();
    }

    public void SelectSpace()
    {
        _canMove = false;
        DestroyCard();
        DestroySelectionObject();

        StartCoroutine(MovePlayerHandler());
    }

    private void DestroySelectionObject()
    {
        Destroy(_currentSelectionObject);
    }

    private void DestroyCard()
    {
        if (TryGetComponent(out PlayerData playerData))
        {
            if (_card.TryGetComponent(out BasicCardHandler cardHandler))
            {
                playerData.RemoveBasicCard(cardHandler);
            }
        }

        Destroy(_card);
    }

    private void SetSelectedObject(Vector2Int position)
    {
        _currentSelectionObject.transform.position = GridHandler.Instance.ReturnPosition(position);
        _playerPosition = position;
        _currentTile = _grid[position.x, position.y];

    }

    private void CheckIfTileIsPreviousInList()
    {
        if (_walkedTiles.Count >= 2 && _currentTile == _walkedTiles[_walkedTiles.Count - 2])
        {
            _walkedTiles.RemoveAt(_walkedTiles.Count - 1);
            _walkedTileNumber++;      
        }

        else
        {
            _walkedTiles.Add(_currentTile);
            _walkedTileNumber--;
        }
    }

    

    private bool IsValidPosition(Vector2Int position)
    {

        if (_walkedTileNumber == 0)
        {
            return IsBackwards(position);
            
            
        }
        return position.x >= 0 && position.x < _grid.GetLength(0) &&
        position.y >= 0 && position.y < _grid.GetLength(1);
    }

    private bool IsBackwards(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _grid.GetLength(0) || position.y < 0 || position.y >= _grid.GetLength(1))
        {
            return false;
        }
        GameObject nextTile = _grid[position.x, position.y];

        if (_walkedTiles.Count >= 2 && nextTile == _walkedTiles[_walkedTiles.Count - 2])
        {
            return true;
            
        }

        return false;
    }

    private void SetNumber()
    {
        _walkedTileNumberText.text = _walkedTileNumber.ToString();
    }

    private IEnumerator MovePlayerHandler()
    {
        _walkedTiles.RemoveAt(0);

        for (int i = 0; i < _walkedTiles.Count; i++)
        {
            Vector3 position = ReturnPosition(_walkedTiles[i]);

            yield return StartCoroutine(MoveToPosition(position));
        }

        if (TryGetComponent(out PlayerData playerData))
        {
            playerData.SetPosition(_playerPosition);
        }

        StartCoroutine(GameManager.Instance.StartPlayerTurn());

    }

    private Vector3 ReturnPosition(GameObject tile)
    {
        return tile.transform.position + Vector3.up * 0.2f;
    }
    private IEnumerator MoveToPosition(Vector3 position)
    {
        while (Vector3.Distance(_playerObject.position, position) > 0.01f)
        {
            _playerObject.position = Vector3.MoveTowards(_playerObject.position, position, playerMovementSpeed * Time.deltaTime);
            yield return null;
        }

        _playerObject.position = position;
    }


    

    

    
}
