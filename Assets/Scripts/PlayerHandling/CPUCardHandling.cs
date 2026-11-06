using System.Collections;
using UnityEngine;

public enum CardPickingType
{
    Random
}
[RequireComponent(typeof(PlayerCardPickSystem))]
public class CPUCardHandling : MonoBehaviour
{
    private PlayerCardPickSystem _playerCardPickSystem;

    [Header("Random Card Handling")]
    [SerializeField] private int minimumCardMovement;
    [SerializeField] private int maximumCardMovement;
    [SerializeField] private float minimumThinkTime;
    [SerializeField] private float maximumThinkTime;

    private void Awake()
    {
        _playerCardPickSystem = GetComponent<PlayerCardPickSystem>();
    }
    public void StartCardPicking()
    {
        StartCoroutine(CalculateRandomCardPicking());
    }

    private IEnumerator CalculateRandomCardPicking()
    {
        int randomCardMovement = Random.Range(minimumCardMovement, maximumCardMovement + 1);

        for (int i = 0; i < randomCardMovement; i++) 
        {
            yield return new WaitForSeconds(GetRandomThinkTime());
            int randomDirection = (Random.Range(0, 2) == 0 ? 1 : -1);

            _playerCardPickSystem.HandleMovement(randomDirection);
        }
        yield return new WaitForSeconds(GetRandomThinkTime());

        _playerCardPickSystem.SelectCard();


    }

    private float GetRandomThinkTime()
    {
        return Random.Range(minimumThinkTime, maximumThinkTime);
    }
}
