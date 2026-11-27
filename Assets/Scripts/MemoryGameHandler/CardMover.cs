using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CardMover : MonoBehaviour
{
    [SerializeField] private float speed = 300f;
    [SerializeField] private float waitAtTarget = 0f;
    [SerializeField] private bool moveOnStart = false;
    [SerializeField] private MemoryGameManager gameManager;

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

    public void MoveToGridPosition()
    {
        if (gameManager != null && gameManager.TryGetGridPosition(transform, out int column, out int row))
            MoveToGridPosition(column, row);
        else
            Debug.LogWarning($"{name} is not in the memory card grid.", this);
    }

    public void MoveToGridPosition(int column, int row, float moveSpeed)
    {
        speed = moveSpeed;
        MoveToGridPosition(column, row);
    }

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

        Vector2 current = canvasRect.InverseTransformPoint(rectTransform.position);
        Vector2 next = Vector2.MoveTowards(current, target, speed * Time.deltaTime);
        rectTransform.position = canvasRect.TransformPoint(next);

        if (next == target)
        {
            if (movingToSlot)
            {
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

        float minX = bounds.xMin + halfCard.x, maxX = bounds.xMax - halfCard.x;
        float minY = bounds.yMin + halfCard.y, maxY = bounds.yMax - halfCard.y;

        target = new Vector2(
            minX < maxX ? Random.Range(minX, maxX) : bounds.center.x,
            minY < maxY ? Random.Range(minY, maxY) : bounds.center.y);
    }
}
