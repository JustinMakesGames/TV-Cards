using TMPro;
using UnityEngine;

public class PlacementSelectorHandler : MonoBehaviour
{

    [SerializeField] private TMP_Text movementText;
    public TMP_Text ReturnMovementText()
    {
        return movementText;
    }
}
