using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TurnOrderHandler : MonoBehaviour
{
    [SerializeField] private GameObject cardFolder;
    [SerializeField] private List<GameObject> cards;

    [SerializeField] private GameObject threeDimensionalCard;

    private List<Transform> _allPlayers = new List<Transform>();
    private List<Transform> _playersChoosingCards = new List<Transform>();
    private Dictionary<Transform, int> _chosenCardPlayers = new Dictionary<Transform, int>();

    private List<Transform> _allSpawnedCards = new List<Transform>();


    public void SetCards()
    {
        
        _allPlayers = PlayerHolder.ReturnPlayers().ToList();
        _playersChoosingCards = PlayerHolder.ReturnPlayers().ToList();
        cardFolder?.SetActive(true);
        GiveCardsNumber();
        MakePlayersChooseCard();
        
    }

    private void MakePlayersChooseCard()
    {
        for (int i = 0; i < _playersChoosingCards.Count; i++)
        {
            if (_playersChoosingCards[i].TryGetComponent(out PlayerCardPickSystem playerCardPickSystem))
            {
                playerCardPickSystem.SetCards(cards);
                print("epic");
            }
        }
    }

    private void GiveCardsNumber()
    {
        List<int> randomNumbers = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        for (int i = 0; i < cards.Count; i++)
        {
            int randomNumber = randomNumbers[Random.Range(0, randomNumbers.Count)];

            
            if (cards[i].TryGetComponent(out TurnOrderCard turnOrderCard))
            {
                turnOrderCard.SetCardNumber(this, randomNumber);
                randomNumbers.Remove(randomNumber);
            }
        }
    }

    public void PlayerChoosesCard(Transform player, GameObject card, int number)
    {

        DehoverAllCards();
        DestroyCardInfo(player, card, number);
        ContinueLettingRemainingPlayersChooseCard();
        SpawnThreeDimensionalCard(player, number);

        if (_playersChoosingCards.Count <= 0)
        {
            TurnUICardsOff();
            SortPlayers();
            StartCoroutine(ShowCards());
        }
    }

    private void TurnUICardsOff()
    {
        cardFolder?.SetActive(false);
    }

    private void DestroyCardInfo(Transform player, GameObject card, int number)
    {
        _chosenCardPlayers[player] = number;

        cards.Remove(card);
        Destroy(card);
        _playersChoosingCards.Remove(player);
    }

    private void ContinueLettingRemainingPlayersChooseCard()
    {
        for (int i = 0; i < _playersChoosingCards.Count; i++)
        {
            if (_playersChoosingCards[i].TryGetComponent(out PlayerCardPickSystem playerCardPickSystem))
            {
                playerCardPickSystem.ResetCards(cards);
            }
        }
    }

    private void SpawnThreeDimensionalCard(Transform player, int number)
    {
        Transform cardClone = Instantiate(threeDimensionalCard, player.position, Quaternion.identity, player).transform;

        _allSpawnedCards.Add(cardClone);

        if (cardClone.TryGetComponent(out ThreeDCardHandler cardHandler))
        {
            cardHandler.SetNumber(number);
        }



    }

    private void DehoverAllCards()
    {
        for (int i = 0; i < _playersChoosingCards.Count; i++)
        {
            if (_playersChoosingCards[i].TryGetComponent(out PlayerCardPickSystem playerCardPickSystem))
            {
                playerCardPickSystem.DehoverCards();
            }
        }
    }

    private void SortPlayers()
    {
        _allPlayers.Sort((a, b) => _chosenCardPlayers[b].CompareTo(_chosenCardPlayers[a]));
        for (int i = 0; i < _allPlayers.Count; i++)
        {
            print(_allPlayers[i]);
        }
    }

    private IEnumerator ShowCards()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < _allSpawnedCards.Count; i++)
        {
            if (_allSpawnedCards[i].TryGetComponent(out ThreeDCardHandler cardHandler))
            {
                cardHandler.RotateCard();
            }
        }

        yield return new WaitForSeconds(2f);
        ContinueDialogue();

    }

    private void ContinueDialogue()
    {
        StartHandler.Instance.SetPlayers(_allPlayers);
    }

    public void DestroyCards()
    {
        foreach (var card in _allSpawnedCards)
        {
            Destroy(card.gameObject);
        }
    }






}
