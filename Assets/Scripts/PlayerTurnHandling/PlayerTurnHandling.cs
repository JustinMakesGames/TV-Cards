using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnHandling : MonoBehaviour
{

    [SerializeField] private Transform optionsFolder;

    [SerializeField] private List<GameObject> options = new List<GameObject>();
    public void StartPlayerTurn()
    {
        if (TryGetComponent(out PlayerCardPickSystem cardPickSystem))
        {
            optionsFolder.gameObject?.SetActive(true);
            cardPickSystem.SetCards(options);
        }
    }

    

    
}
