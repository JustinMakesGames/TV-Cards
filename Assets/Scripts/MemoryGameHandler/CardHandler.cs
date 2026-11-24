using UnityEngine;
using UnityEngine.UI;

public class CardHandler : BasicCardHandler, ICardHandler 
{
    [SerializeField] private string fruitName;
    [SerializeField] private Sprite fruitImage;
    [SerializeField] private Image fruitDisplay;   // optional: the Image that shows the fruit (e.g. FrontCard)

    public FruitObject Fruit { get; private set; }

    public void SetPlayerStats(FruitObject fruitObject)
    {
        Fruit = fruitObject;
        fruitName = fruitObject.fruitName;
        fruitImage = fruitObject.fruitImage;

        if (fruitDisplay != null) fruitDisplay.sprite = fruitImage;
    }

    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);

        MemoryGameManager.Instance.HandlePlayerChoice(player, Fruit, gameObject);
        
    }
}
