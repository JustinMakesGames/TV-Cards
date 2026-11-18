using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnHandling : MonoBehaviour
{
    [SerializeField] private Transform optionsFolder;

    public void StartPlayerTurn()
    {
        foreach (GameObject option in ListOfChildren.ReturnListOfChildren(optionsFolder))
        {
            option.SetActive(true);
        }

        PlayerChooseCard();
    }

    public void PlayerChooseCard()
    {
        if (TryGetComponent(out PlayerCardPickSystem cardPickSystem))
        {
            optionsFolder.gameObject.SetActive(true);
            cardPickSystem.SetCards(ReturnAvailableOptions());
        }
    }

    public void RestoreOption(GameObject cardFolder)
    {
        foreach (GameObject option in ListOfChildren.ReturnListOfChildren(optionsFolder))
        {
            if (option.TryGetComponent(out OptionsCardHandler optionsCard) && optionsCard.ReturnCardFolder() == cardFolder.transform)
            {
                option.SetActive(true);
            }
        }
    }

    private List<GameObject> ReturnAvailableOptions()
    {
        List<GameObject> available = new List<GameObject>();

        foreach (GameObject option in ListOfChildren.ReturnListOfChildren(optionsFolder))
        {
            if (option.activeSelf) available.Add(option);
        }

        return available;
    }
}
