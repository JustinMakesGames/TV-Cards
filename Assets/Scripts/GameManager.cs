using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    private void Start()
    {
        StartCoroutine(DialogueSystem.Instance.HandleDialogue("Introduction"));
    }
}
