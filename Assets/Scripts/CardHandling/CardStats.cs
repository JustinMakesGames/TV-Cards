using System;
using UnityEngine;

[Serializable]
public class CardStats
{
    public GameObject card;

    public CardStats(GameObject card)
    {
        this.card = card;
    }
}

[Serializable]
public class MovementCardStats : CardStats
{
    public int movement;

    public MovementCardStats(GameObject card, int movement) : base(card)
    {
        this.movement = movement;
    } 
}
