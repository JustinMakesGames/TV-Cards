using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OptionsCardHandler : BasicCardHandler, ICardHandler
{

    [SerializeField] private Transform cardFolder;

    public Transform ReturnCardFolder()
    {
        return cardFolder;
    }

    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);

        cardFolder.gameObject?.SetActive(true);
        List<GameObject> cards = ListOfChildren.ReturnListOfChildren(cardFolder);

        if (player.TryGetComponent(out PlayerCardPickSystem cardPickSystem))
        {
            cardPickSystem.SetCards(cards);
        }


        gameObject.SetActive(false);
        transform.parent.gameObject?.SetActive(false);
        
    }

    
}
