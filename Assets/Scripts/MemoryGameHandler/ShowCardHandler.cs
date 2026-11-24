using UnityEngine;
using UnityEngine.UI;

public class ShowCardHandler : MonoBehaviour
{

    [SerializeField] private Image fruitObj;
    

    public void ShowCardToFind(FruitObject fruitObject)
    {
        fruitObj.sprite = fruitObject.fruitImage;

    }

    public void RemoveSprite()
    {
        fruitObj.sprite = null;
    }
}
