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

        cursor?.SetActive(true);
        HandleUISelectionMovement();
    }

    public void MoveOnTheAxis(InputAction.CallbackContext context)
    {
        if (!context.started || !_canSelect) return;

        Vector2 value = context.ReadValue<Vector2>();

        if (value.x > 0)
        {
            _coordinates.x++;
        }

        else if (value.x < 0)
        {
            _coordinates.x--;
        }

        else if (value.y > 0)
        {
            _coordinates.y--;
        }

        else if (value.y < 0)
        {
            _coordinates.y++;
        }

        HandleUISelectionMovement();
    }

    public void PressButton(InputAction.CallbackContext context)
    {
        if (!context.started || !_canSelect) return;

        if (_selectedObject.TryGetComponent(out Button button)) 
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
        cursor.transform.position = _selectedObject.transform.position;

    }


}
