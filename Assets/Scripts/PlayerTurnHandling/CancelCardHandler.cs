using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CancelCardHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardFolders = new List<GameObject>();
    
    public void GoBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandleGoingBack();
        }
    }

    private void HandleGoingBack()
    {
        foreach (GameObject folder in cardFolders)
        {
            folder.SetActive(false);
        }

        if (TryGetComponent(out PlayerTurnHandling playerTurnHandling))
        {
            playerTurnHandling.PlayerChooseCard();
        }
    }
}
