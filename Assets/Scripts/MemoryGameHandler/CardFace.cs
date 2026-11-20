using UnityEngine;

[ExecuteAlways]
public class CardFace : MonoBehaviour
{
    [SerializeField] private GameObject front;
    [SerializeField] private GameObject back;

    private void LateUpdate()
    {
        if (front == null) return; // avoids errors while you're still assigning fields

        Camera cam = Camera.main;
        Vector3 viewDir = cam != null ? cam.transform.forward : Vector3.forward;
        bool showingFront = Vector3.Dot(transform.forward, viewDir) > 0f;

        if (front.activeSelf != showingFront) front.SetActive(showingFront);
        if (back != null && back.activeSelf == showingFront) back.SetActive(!showingFront);
    }
}
