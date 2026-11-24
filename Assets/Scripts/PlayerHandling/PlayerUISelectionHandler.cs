using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameObjectRow
{
    public List<GameObject> objects;
}

public class PlayerUISelectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject[,] possibleSelectedGameObjects;
    [SerializeField] private GameObject cursor;

    private Vector2Int _coordinates = new Vector2Int(0, 0);

    private GameObject _selectedObject;

    private bool _canSelect;
    public void SetPossibleGameObjects(GameObject[,] possibleSelectedGameObjects)
    {
        _coordinates = Vector2Int.zero;
        this.possibleSelectedGameObjects = possibleSelectedGameObjects;
        _canSelect = true;

        if (cursor != null)
        {
            // The cursor can't be seen if its Canvas is turned off, so make sure it's on
            Canvas canvas = cursor.GetComponentInParent<Canvas>(true);
            if (canvas != null && !canvas.gameObject.activeSelf) canvas.gameObject.SetActive(true);
            cursor.SetActive(true);
        }
        MoveToNearestAvailable();
        HandleUISelectionMovement();
    }

    public void UpdatePossibleGameObjects(GameObject[,] possibleSelectedGameObjects)
    {
        this.possibleSelectedGameObjects = possibleSelectedGameObjects;
        if (!_canSelect) return;

        MoveToNearestAvailable();
        HandleUISelectionMovement();
    }

    public void MoveOnTheAxis(InputAction.CallbackContext context)
    {
        if (!context.started || !_canSelect) return;

        Vector2 value = context.ReadValue<Vector2>();
        Vector2Int direction = Vector2Int.zero;

        if (value.x > 0f) direction = Vector2Int.right;
        else if (value.x < 0f) direction = Vector2Int.left;
        else if (value.y > 0f) direction = Vector2Int.down;
        else if (value.y < 0f) direction = Vector2Int.up;

        Move(direction);
    }

    public bool CanSelect => _canSelect;
    public Vector2Int Coordinates => _coordinates;
    public GameObject[,] PossibleGameObjects => possibleSelectedGameObjects;

    public void SelectCoordinates(Vector2Int coordinates)
    {
        if (!_canSelect || !IsInBounds(coordinates) || possibleSelectedGameObjects[coordinates.x, coordinates.y] == null) return;

        _coordinates = coordinates;
        HandleUISelectionMovement();
    }

    public void Move(Vector2Int direction)
    {
        if (!_canSelect) return;

        Vector2Int next = _coordinates + direction;
        while (direction != Vector2Int.zero && IsInBounds(next))
        {
            if (possibleSelectedGameObjects[next.x, next.y] != null)
            {
                _coordinates = next;
                break;
            }
            next += direction;
        }

        HandleUISelectionMovement();
    }

    private bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < possibleSelectedGameObjects.GetLength(0)
            && position.y >= 0 && position.y < possibleSelectedGameObjects.GetLength(1);
    }

    private void MoveToNearestAvailable()
    {
        _coordinates.x = Mathf.Clamp(_coordinates.x, 0, possibleSelectedGameObjects.GetLength(0) - 1);
        _coordinates.y = Mathf.Clamp(_coordinates.y, 0, possibleSelectedGameObjects.GetLength(1) - 1);
        if (possibleSelectedGameObjects[_coordinates.x, _coordinates.y] != null) return;

        float bestDistance = float.MaxValue;
        Vector2Int best = _coordinates;
        for (int y = 0; y < possibleSelectedGameObjects.GetLength(1); y++)
        {
            for (int x = 0; x < possibleSelectedGameObjects.GetLength(0); x++)
            {
                if (possibleSelectedGameObjects[x, y] == null) continue;
                float distance = Vector2Int.Distance(_coordinates, new Vector2Int(x, y));
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = new Vector2Int(x, y);
                }
            }
        }
        _coordinates = best;
    }

    public void PressButton(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Press();
    }

    public void Press()
    {
        if (!_canSelect || _selectedObject == null) return;

        if (_selectedObject.TryGetComponent(out ICardHandler cardHandler))
        {
            _canSelect = false;
            cursor?.SetActive(false);
            cardHandler.OnPressed(transform);
        }
        else if (_selectedObject.TryGetComponent(out Button button))
        {
            button.onClick?.Invoke();
            _canSelect = false;
            cursor?.SetActive(false);
        }
    }

    private void HandleUISelectionMovement()
    {
        _coordinates.x = Mathf.Clamp(_coordinates.x, 0, possibleSelectedGameObjects.GetLength(0) - 1);
        _coordinates.y = Mathf.Clamp(_coordinates.y, 0, possibleSelectedGameObjects.GetLength(1) - 1);

        _selectedObject = possibleSelectedGameObjects[_coordinates.x, _coordinates.y];
        if (_selectedObject == null) return;
        cursor.transform.position = ReturnCursorPosition();

    }

    private Vector3 ReturnCursorPosition()
    {
        Vector3[] corners = new Vector3[4];

        if (_selectedObject.TryGetComponent(out RectTransform rectTransform)) 
        {
            rectTransform.GetWorldCorners(corners);
            return corners[1];
        }

        return _selectedObject.transform.position;
    }


}
