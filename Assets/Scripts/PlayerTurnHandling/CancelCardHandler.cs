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
        TryGetComponent(out PlayerTurnHandling playerTurnHandling);

        foreach (GameObject folder in cardFolders)
        {
            if (folder.activeSelf && playerTurnHandling) playerTurnHandling.RestoreOption(folder);

            folder.SetActive(false);
        }

        if (playerTurnHandling)
        {
            playerTurnHandling.PlayerChooseCard();
        }
    }
}
