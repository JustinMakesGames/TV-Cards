using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementHandler))]
public class CPUMovementHandler : MonoBehaviour
{
    [Tooltip("Pause after the selection marker appears, before the CPU takes its first step.")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float minimumStepTime = 0.3f;
    [SerializeField] private float maximumStepTime = 0.45f;
    [Tooltip("Pause on the final tile before the CPU confirms and walks there.")]
    [SerializeField] private float confirmTime = 0.6f;

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private const int NoDirection = 4;

    // Score for ending on an opponent's tile: you can't attack someone standing on your own tile.
    private const int OccupiedTilePenalty = 1000;

    private PlayerMovementHandler _movementHandler;

    private void Awake()
    {
        _movementHandler = GetComponent<PlayerMovementHandler>();
    }

    public void StartCPUMovement(Vector2Int startPosition, int steps)
    {
        StartCoroutine(HandleCPUMovement(startPosition, steps));
    }

    private IEnumerator HandleCPUMovement(Vector2Int startPosition, int steps)
    {
        List<Vector2Int> path = PlanPath(startPosition, steps, out _);

        if (path == null)
        {
            Debug.LogWarning($"{name} could not find a path of {steps} steps.");
            path = new List<Vector2Int>();
        }

        yield return new WaitForSeconds(startDelay);

        foreach (Vector2Int direction in path)
        {
            yield return new WaitForSeconds(Random.Range(minimumStepTime, maximumStepTime));
            _movementHandler.MoveDirection(direction);
        }

        yield return new WaitForSeconds(confirmTime);
        _movementHandler.SelectSpace();
    }

    // Plans a walk of exactly 'steps' moves. score = how many tiles short of attack range the CPU ends up
    // (0 = it can hit someone after the walk). Lower is better; CPUCardHandling uses it to pick a card.
    public List<Vector2Int> PlanPath(Vector2Int start, int steps, out int score)
    {
        GameObject[,] grid = GridHandler.Instance.ReturnGrid();
        List<Vector2Int> opponents = CPUBoardHelper.ReturnOpponentPositions(transform);
        int attackRange = ReturnPreferredRange();

        return FindPath(start, steps, grid.GetLength(0), grid.GetLength(1),
            position => ScoreEndPosition(position, opponents, attackRange), out score);
    }

    // The longest attack range the CPU holds, so it can stand back and still hit. 1 when it has no attack cards.
    private int ReturnPreferredRange()
    {
        int range = 1;

        if (TryGetComponent(out PlayerData playerData))
        {
            foreach (AttackCardHandler card in CPUBoardHelper.ReturnAttackCards(playerData))
            {
                range = Mathf.Max(range, card.ReturnRange());
            }
        }

        return range;
    }

    private int ScoreEndPosition(Vector2Int position, List<Vector2Int> opponents, int attackRange)
    {
        if (opponents.Count == 0) return 0;

        int best = int.MaxValue;

        foreach (Vector2Int opponent in opponents)
        {
            int distance = CPUBoardHelper.ReturnDistance(position, opponent);
            if (distance == 0) return OccupiedTilePenalty;

            best = Mathf.Min(best, Mathf.Max(0, distance - attackRange));
        }

        return best;
    }

    // Finds a walk of exactly 'steps' moves that never steps straight back (that would refund a step
    // in PlayerMovementHandler) and ends on the lowest-scoring tile. Ties are broken randomly so the CPU
    // doesn't walk the same route every time.
    private List<Vector2Int> FindPath(Vector2Int start, int steps, int width, int height,
        System.Func<Vector2Int, int> scoreEnd, out int bestScore)
    {
        bestScore = int.MaxValue;
        int stateCount = width * height * 5;

        // parents[k][state] = state at step k - 1 that led here, or -1 when unreached.
        int[][] parents = new int[steps + 1][];
        for (int k = 0; k <= steps; k++)
        {
            parents[k] = new int[stateCount];
            for (int s = 0; s < stateCount; s++) parents[k][s] = -1;
        }

        List<int> currentStates = new List<int> { EncodeState(start, NoDirection, width) };
        parents[0][currentStates[0]] = currentStates[0];

        int[] directionOrder = { 0, 1, 2, 3 };

        for (int k = 0; k < steps; k++)
        {
            List<int> nextStates = new List<int>();

            foreach (int state in currentStates)
            {
                DecodeState(state, width, out Vector2Int position, out int lastDirection);
                Shuffle(directionOrder);

                foreach (int direction in directionOrder)
                {
                    if (lastDirection != NoDirection && direction == (lastDirection + 2) % 4) continue;

                    Vector2Int next = position + Directions[direction];
                    if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height) continue;

                    int nextState = EncodeState(next, direction, width);
                    if (parents[k + 1][nextState] != -1) continue;

                    parents[k + 1][nextState] = state;
                    nextStates.Add(nextState);
                }
            }

            if (nextStates.Count == 0) return null;
            currentStates = nextStates;
        }

        List<int> bestStates = new List<int>();

        foreach (int state in currentStates)
        {
            DecodeState(state, width, out Vector2Int position, out _);
            int score = scoreEnd(position);

            if (score < bestScore)
            {
                bestScore = score;
                bestStates.Clear();
            }

            if (score == bestScore) bestStates.Add(state);
        }

        int endState = bestStates[Random.Range(0, bestStates.Count)];

        List<Vector2Int> path = new List<Vector2Int>();
        for (int k = steps; k > 0; k--)
        {
            DecodeState(endState, width, out _, out int direction);
            path.Add(Directions[direction]);
            endState = parents[k][endState];
        }

        path.Reverse();
        return path;
    }

    private int EncodeState(Vector2Int position, int lastDirection, int width)
    {
        return (position.y * width + position.x) * 5 + lastDirection;
    }

    private void DecodeState(int state, int width, out Vector2Int position, out int lastDirection)
    {
        lastDirection = state % 5;
        int cell = state / 5;
        position = new Vector2Int(cell % width, cell / width);
    }

    private void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
