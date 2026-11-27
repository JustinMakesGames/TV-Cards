using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [SerializeField] private BoolEvent startEvent;

    [SerializeField] private List<Transform> players;

    [SerializeField] private CameraEvent cameraEvent;
    [SerializeField] private TransformEvent randomMovementCardEvent;
    [SerializeField] private TransformEvent randomAttackCardEvent;

    [Header("Minigames")]
    [SerializeField] private string[] minigameScenes = { "MemoryMinigameScene" };
    [SerializeField] private int[] coinsPerPlacement = { 10, 5, 2, 0 };
    [SerializeField] private float fadeDuration = SceneFader.DefaultDuration;

    [Header("Collectibles")]
    [Tooltip("Tops up coins and hearts on the board after every minigame. Found automatically if left empty.")]
    [SerializeField] private SpawnRandomCollectiblesOnGrid collectibleSpawner;

    private int _currentPlayer = -1;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
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
        SetCoinsAndHearts();
        SpawnNewCollectibles();
        StartCoroutine(StartPlayerTurn());
    }

    public List<Transform> ReturnPlayers()
    {
        return players;
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

    private void SetCoinsAndHearts()
    {
        foreach (Transform player in players)
        {
            if (player.TryGetComponent(out PlayerData playerData))
            {
                playerData.ChangeCoinAmount(10);
                playerData.ChangeHeartPointsAmount(10);
            }

            PlayerMemoryScoreAmount memoryScore = player.GetComponentInChildren<PlayerMemoryScoreAmount>(true);
            if (memoryScore) memoryScore.ShowMemoryScore(false);
        }
    }

    private void SetRandomCards()
    {
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                randomMovementCardEvent?.Invoke(players[i]);
                randomAttackCardEvent?.Invoke(players[i]);
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
        if (_currentPlayer >= players.Count - 1 && minigameScenes.Length > 0)
        {
            yield return PlayMinigame();
        }
        _currentPlayer = (_currentPlayer >= players.Count - 1) ? 0 : _currentPlayer + 1;
        cameraEvent?.Invoke(CameraKind.GridCamera);
        yield return new WaitForSeconds(2);
        
        if (players[_currentPlayer].TryGetComponent(out PlayerTurnHandling playerTurnHandling))
        {
            playerTurnHandling.StartPlayerTurn();
        }
    }

    private IEnumerator PlayMinigame()
    {
        string sceneName = minigameScenes[Random.Range(0, minigameScenes.Length)];
        Scene mainScene = SceneManager.GetActiveScene();
        Transform[] mainPlayerHolder = PlayerHolder.ReturnPlayers();

        MinigameSession.Begin(players);

        yield return SceneFader.FadeToBlack(fadeDuration);

        List<GameObject> hiddenObjects = HideMainGame(mainScene);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene minigameScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(minigameScene);

        yield return SceneFader.FadeFromBlack(fadeDuration);

        yield return new WaitUntil(() => MinigameSession.IsFinished);

        List<MinigameResult> results = MinigameSession.Results;

        yield return SceneFader.FadeToBlack(fadeDuration);

        yield return SceneManager.UnloadSceneAsync(minigameScene);
        SceneManager.SetActiveScene(mainScene);

        foreach (GameObject hiddenObject in hiddenObjects)
        {
            if (hiddenObject) hiddenObject.SetActive(true);
        }

        PlayerHolder.SetPlayers(mainPlayerHolder);
        MinigameSession.End();

        cameraEvent?.Invoke(CameraKind.GridCamera);
        yield return SceneFader.FadeFromBlack(fadeDuration);

        yield return GiveMinigameRewards(results);

        SpawnNewCollectibles();
    }

    private void SpawnNewCollectibles()
    {
        if (collectibleSpawner == null)
        {
            collectibleSpawner = FindFirstObjectByType<SpawnRandomCollectiblesOnGrid>();
        }

        if (collectibleSpawner == null)
        {
            Debug.LogWarning("No SpawnRandomCollectiblesOnGrid found in the scene, so no new collectibles were spawned.");
            return;
        }

        collectibleSpawner.SpawnCollectibles();
    }

    private List<GameObject> HideMainGame(Scene mainScene)
    {
        List<GameObject> rootObjects = new List<GameObject>(mainScene.GetRootGameObjects());
        if (gameObject.scene != mainScene) rootObjects.AddRange(gameObject.scene.GetRootGameObjects());

        List<GameObject> hiddenObjects = new List<GameObject>();
        foreach (GameObject rootObject in rootObjects)
        {
            if (rootObject == transform.root.gameObject || !rootObject.activeSelf) continue;
            if (SceneFader.IsFader(rootObject)) continue;

            rootObject.SetActive(false);
            hiddenObjects.Add(rootObject);
        }

        return hiddenObjects;
    }

    private IEnumerator GiveMinigameRewards(List<MinigameResult> results)
    {
        yield return new WaitForSeconds(1);

        string[] placementNames = { "first", "second", "third", "fourth" };

        yield return DialogueSystem.Instance.HandleSingularDialogue("Minigame results!");

        foreach (MinigameResult result in results)
        {
            int coins = result.placement - 1 < coinsPerPlacement.Length ? coinsPerPlacement[result.placement - 1] : 0;
            string placementName = result.placement - 1 < placementNames.Length ? placementNames[result.placement - 1] : $"#{result.placement}";

            Transform player = players.Find(p => p.name == result.playerName);
            if (player != null && player.TryGetComponent(out PlayerData playerData))
            {
                playerData.ChangeCoinAmount(coins);
            }
            else
            {
                Debug.LogWarning($"No main game player found named {result.playerName}.");
            }

            yield return DialogueSystem.Instance.HandleSingularDialogue($"{result.playerName} finished {placementName} and gets {coins} coins!");
        }
    }
}
