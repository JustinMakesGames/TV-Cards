using TMPro;
using UnityEngine;

public class ThreeDCardHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text _cardNumber;
    [SerializeField] private Animator cardRotateAnimator;

    public void SetNumber(int number)
    {
        _cardNumber.text = number.ToString();
    }

    public void RotateCard()
    {
        cardRotateAnimator?.SetTrigger("RotateCard");
    }
}
