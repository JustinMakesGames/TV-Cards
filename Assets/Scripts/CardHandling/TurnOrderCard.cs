using UnityEngine;

public class TurnOrderCard : MonoBehaviour
{
    public int CardNumber { get; private set; }

    public void SetCardNumber(int cardNumber)
    {
        CardNumber = cardNumber;
    }

}
