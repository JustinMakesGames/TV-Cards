using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private BoolEvent testEvent;

    private void Awake()
    {
        testEvent?.Invoke(true);
    }
    public void EpicEvent()
    {
        print("Wowie zowie this event is working idiot");
    }
}
