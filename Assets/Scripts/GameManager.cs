using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoolEvent startEvent;
    private void Start()
    {
        startEvent?.Invoke(true);
    }
}
