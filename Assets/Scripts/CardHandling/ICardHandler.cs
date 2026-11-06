using UnityEngine;

public interface ICardHandler
{
    public void OnHovered();
    public void OnDehovered();
    public void OnPressed(Transform player);
}
