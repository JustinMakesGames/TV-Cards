using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CPUCardHandling))]
public class PlayerCardPickSystem : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    private List<GameObject> _cards;

    private GameObject _selectedCard;
    private int _currentLocation;
    private bool _canChooseCard;

    public void SetCards(List<GameObject> cards)
    {
        _currentLocation = 0;
        _cards = cards;
        _canChooseCard = true;
        cursor?.SetActive(true);
        HandleCardSelectionMovement(true);

        if (TryGetComponent(out PlayerOrCPUChecker playerOrCPUChecker))
        {
            HandleCPU(playerOrCPUChecker);
        }

    }

    public void DehoverCards()
    {
        //HandleDehover();
    }

    public void ResetCards(List<GameObject> cards)
    {
        _cards = cards;
        _currentLocation = cards.IndexOf(_selectedCard);

        HandleCardSelectionMovement(false);
    }

    private void HandleCPU(PlayerOrCPUChecker playerOrCPUChecker)
    {
        if (playerOrCPUChecker.PlayerOrCPU() == PlayerOrCPU.CPU)
        {
            if (TryGetComponent(out CPUCardHandling cpuCardHandling))
            {
                cpuCardHandling.StartCardPicking();
            }
        }
    }

    public void MoveForCards(InputAction.CallbackContext context) 
    {

        if (!context.performed || !_canChooseCard) return;
        print(context.ReadValue<Vector2>().x);

        float movement = context.ReadValue<Vector2>().x;
        if (movement > 0)
        {
            HandleMovement(1);
        }

        else if (movement < 0)
        {
            HandleMovement(-1);
        }
        
    }

    public void HandleMovement(int direction)
    {
        HandleDehover();
        _currentLocation += direction;
        HandleCardSelectionMovement(true);
    }

    private void HandleDehover()
    {
        if (_selectedCard.TryGetComponent(out ICardHandler cardHandler))
        {
            cardHandler.OnDehovered();
        }
    }

    public void PressCard(InputAction.CallbackContext context)
    {
        if (!context.started || !_canChooseCard) return;

        SelectCard();
        
    }

    public void SelectCard()
    {
        if (_selectedCard.TryGetComponent(out ICardHandler cardHandler))
        {
            DisableCardPicking();
            cardHandler.OnPressed(transform);
        }
    }

    private void HandleCardSelectionMovement(bool onHover)
    {
        HandleCardOutOfIndex();


        _selectedCard = _cards[_currentLocation];
        cursor.transform.position = ReturnCursorPosition();

        if (onHover && _selectedCard.TryGetComponent(out ICardHandler cardHandler))
        {
            cardHandler.OnHovered();
        }

    }

    private void HandleCardOutOfIndex()
    {
        if (_currentLocation < 0)
        {
            _currentLocation = _cards.Count - 1;
        }

        else if (_currentLocation >= _cards.Count)
        {
            _currentLocation = 0;
        }
    }

    private Vector3 ReturnCursorPosition()
    {
        Vector3[] corners = new Vector3[4];

        if (_selectedCard.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.GetWorldCorners(corners);
            return corners[1];
        }

        return _selectedCard.transform.position;
    }

    private void DisableCardPicking()
    {
        _canChooseCard = false;
        cursor?.SetActive(false);

    }
    


}
