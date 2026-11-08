using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [SerializeField] private BoolEvent startEvent;

    [SerializeField] private List<Transform> players;

    [SerializeField] private CameraEvent cameraEvent;
    [SerializeField] private TransformEvent randomCardEvent;
    private int _currentPlayer = -1;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        startEvent?.Invoke(true);
    }

    public void SetPlayers(List<Transform> players)
    {
        this.players = players;
        SetAllPlayerPlacements();
        SetRandomCards();
        StartCoroutine(StartPlayerTurn());
    }

    private void SetAllPlayerPlacements()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject[,] grid = GridHandler.Instance.ReturnGrid();
            Vector2Int position = Vector2Int.zero;

            if (players[i].TryGetComponent(out PlayerData playerData))
            {
                SetPlayerPlacement(grid, position, i, playerData);
            }

        }

        


    }

    private void SetRandomCards()
    {
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                randomCardEvent?.Invoke(players[i]);
            }
        }
    }

    private void SetPlayerPlacement(GameObject[,] grid, Vector2Int position, int player, PlayerData playerData)
    {
        switch (player)
        {
            case 0:
                position = Vector2Int.zero;
                break;
            case 1:
                position = new Vector2Int(grid.GetLength(0) - 1, 0);
                break;
            case 2:
                position = new Vector2Int(0, grid.GetLength(1) - 1);
                break;
            case 3:
                position = new Vector2Int(grid.GetLength(0) - 1, grid.GetLength(1) - 1);
                break;

        }

        playerData.SetPosition(position);

    }

    public IEnumerator StartPlayerTurn()
    {
        _currentPlayer++;
        cameraEvent?.Invoke(CameraKind.GridCamera);
        yield return new WaitForSeconds(2);
        
        if (players[_currentPlayer].TryGetComponent(out PlayerTurnHandling playerTurnHandling))
        {
            playerTurnHandling.StartPlayerTurn();
        }
        
        

    }
}
