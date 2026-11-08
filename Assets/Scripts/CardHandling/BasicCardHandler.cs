using UnityEngine;

public class BasicCardHandler : MonoBehaviour, ICardHandler
{

    [SerializeField] private Animator animator;

    protected int onHoveredAmount;
    public virtual void OnHovered()
    {

        onHoveredAmount++;

        if (onHoveredAmount > 1) return;
        animator.SetTrigger("Up");
    }

    public virtual void OnDehovered()
    {

        onHoveredAmount--;

        if (onHoveredAmount > 0) return;
        animator.SetTrigger("Down");
    }

    public virtual void OnPressed(Transform player) 
    {
        OnDehovered();
    }
}
