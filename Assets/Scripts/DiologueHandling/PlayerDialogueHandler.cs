using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDialogueHandler : MonoBehaviour
{
    [SerializeField] private BoolEvent continueDialogEvent;
    private bool _canContinueDialogue;

    public void LetPlayerContinueDialogue()
    {
        _canContinueDialogue = true;
    }

    public void PlayerContinueDialogueInput(InputAction.CallbackContext context)
    {
        if (context.started && _canContinueDialogue)
        {
            continueDialogEvent?.Invoke(true);
        }
    }
}
