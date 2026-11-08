using UnityEngine;

public class TimeScaler : MonoBehaviour
{

    [SerializeField] private float timeScale = 1;
    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
