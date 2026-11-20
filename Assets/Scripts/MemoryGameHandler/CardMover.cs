using UnityEngine;

// Moves this card to random spots inside its Canvas, one after another.
[RequireComponent(typeof(RectTransform))]
public class CardMover : MonoBehaviour
{
    [SerializeField] private float speed = 300f;          // canvas units per second
    [SerializeField] private float waitAtTarget = 0f;     // pause (seconds) before picking the next spot
    [SerializeField] private bool moveOnStart = false;     // the MemoryGameManager starts the movement while shuffling
    [SerializeField] private MemoryGameManager gameManager;   // found automatically if left empty

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Vector2 target;
    private float waitTimer;
    private bool isMoving;
    private bool movingToSlot;

    public bool IsMoving => isMoving;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null) canvasRect = canvas.rootCanvas.GetComponent<RectTransform>();
        if (gameManager == null) gameManager = FindFirstObjectByType<MemoryGameManager>();
    }

    // Moves the card to the slot it currently has in the MemoryGameManager's cards[,] grid
    public void MoveToGridPosition()
    {
        if (gameManager != null && gameManager.TryGetGridPosition(transform, out int column, out int row))
            MoveToGridPosition(column, row);
        else
            Debug.LogWarning($"{name} is not in the memory card grid.", this);
    }

    // Same as MoveToGridPosition(column, row), but with a custom speed
    public void MoveToGridPosition(int column, int row, float moveSpeed)
    {
        speed = moveSpeed;
        MoveToGridPosition(column, row);
    }

    // Moves the card to a specific cards[column, row] slot, then stops there
    public void MoveToGridPosition(int column, int row)
    {
        if (canvasRect == null || gameManager == null) return;

        target = canvasRect.InverseTransformPoint(gameManager.GetSlotPosition(column, row));
        waitTimer = 0f;
        movingToSlot = true;
        isMoving = true;
    }

    private void Start()
    {
        if (moveOnStart) StartMoving();
    }

    // Starts wandering around the canvas with a custom speed
    public void StartMoving(float moveSpeed)
    {
        speed = moveSpeed;
        StartMoving();
    }

    public void StartMoving()
    {
        if (canvasRect == null) return;
        movingToSlot = false;
        isMoving = true;
        PickNewTarget();
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (!isMoving || canvasRect == null) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        // Work in the canvas's local space so it doesn't matter what the card's parent is
        Vector2 current = canvasRect.InverseTransformPoint(rectTransform.position);
        Vector2 next = Vector2.MoveTowards(current, target, speed * Time.deltaTime);
        rectTransform.position = canvasRect.TransformPoint(next);

        if (next == target)
        {
            if (movingToSlot)
            {
                // Arrived at its grid slot: stay there
                movingToSlot = false;
                isMoving = false;
                return;
            }

            waitTimer = waitAtTarget;
            PickNewTarget();
        }
    }

    private void PickNewTarget()
    {
        Rect bounds = canvasRect.rect;
        Vector2 halfCard = rectTransform.rect.size * 0.5f;

        // Keep the whole card inside the canvas
        float minX = bounds.xMin + halfCard.x, maxX = bounds.xMax - halfCard.x;
        float minY = bounds.yMin + halfCard.y, maxY = bounds.yMax - halfCard.y;

        target = new Vector2(
            minX < maxX ? Random.Range(minX, maxX) : bounds.center.x,
            minY < maxY ? Random.Range(minY, maxY) : bounds.center.y);
    }
}
