using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharMovement : MonoBehaviour
{
    private const float SNAP_SPEED = 5f;
    private const float WALK_SPEED = 5f;
    private const float MOVEMENT_COMMITMENT_PERCENT = 0.2f; // Commited to the movement after 20% of it being completed 
    private const float CELL_PROXIMITY_PERCENT = 0.05f; // If it is 5% further from the cell middle, it is on that middle

    private Animator animator;
    private Grid grid;
    private Nullable<Direction> movingDirection;
    private Vector2Int cell;
    private bool Walking { get { return this.movingDirection.HasValue; } }
    private Vector2 Position {
        get { return new Vector2(this.transform.position.x, this.transform.position.y); }
        set { this.transform.position = new Vector3(value.x, value.y); }
    }
    private Vector2 CellPosition
    {
        get { return this.grid.GetCellPosition(this.cell); }
    }
    private Motion CurrentMotion { get { return GetMotion(this.movingDirection); } }
    private float currentMoveDistance = 0;
    private bool commitedToMovement;
    private bool scheduledStop;

    public bool Snapping { get; private set; }

    public enum Direction { UP, DOWN, RIGHT, LEFT }
    private enum Motion { HORIZONTAL, VERTICAL, NONE }
    private readonly Dictionary<Direction, string> DIRECTION_NAMES = new Dictionary<Direction, string>
    {
        { Direction.UP, "up" },
        { Direction.DOWN, "down" },
        { Direction.RIGHT, "right" },
        { Direction.LEFT, "left" }
    };
    private readonly Dictionary<Direction, Vector3> DIRECTION_VECTORS = new Dictionary<Direction, Vector3>
    {
        { Direction.UP, Vector3.up },
        { Direction.DOWN, Vector3.down },
        { Direction.RIGHT, Vector3.right },
        { Direction.LEFT, Vector3.left }
    };

    void Awake()
    {
        this.animator = GetComponentInChildren<Animator>();
        this.grid = GetComponentInParent<Grid>();
    }

    void Start()
    {
        Snap(Motion.NONE);
    }

    void Update()
    {
        var closeToCellCenter = UpdateCell();
        if (closeToCellCenter && this.commitedToMovement)
        {
            this.commitedToMovement = false;

            if (this.scheduledStop)
            {
                this.scheduledStop = false;
                Stop();
                return;
            }
        }
        if (this.movingDirection.HasValue)
        {
            var vector = DIRECTION_VECTORS[this.movingDirection.Value];
            this.transform.position += vector * WALK_SPEED * Time.deltaTime;
            var normalizedInt = new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
            var nextCellPosition = this.grid.GetCellPosition(this.cell + normalizedInt);
            var missingToNextCell = nextCellPosition - Position;
            if (missingToNextCell.magnitude / this.grid.cellSize < 1f - MOVEMENT_COMMITMENT_PERCENT)
            {
                Debug.Log("commited!!!! " + missingToNextCell.magnitude.ToString());
                this.commitedToMovement = true;
            }
        }
    }

    public void Move(Direction direction)
    {
        if (this.commitedToMovement || this.Snapping || (this.movingDirection.HasValue && this.movingDirection.Value == direction)) return;
        if (GetMotion(direction) != CurrentMotion) Snap(CurrentMotion);
        this.scheduledStop = false;
        this.movingDirection = direction;
        this.animator.SetTrigger($"walk-{DIRECTION_NAMES[direction]}");
    }

    public void Stop()
    {
        if (!this.movingDirection.HasValue) return;
        if (this.commitedToMovement)
        {
            this.scheduledStop = true;
            return;
        }

        this.scheduledStop = false;
        var originalMotion = CurrentMotion;
        this.movingDirection = null;
        this.animator.SetTrigger("idle");
        Snap(originalMotion);
    }
    
    private void Snap(Motion originalMotion)
    {
        this.cell = this.grid.GetCellToSnap(this.transform.position);
        StartCoroutine(AnimatedSnap(this.cell, originalMotion));
    }

    private void InstantSnapTo(Vector2Int cell)
    {
        this.transform.position = this.grid.GetCellPosition(cell);
        this.cell = cell;
    }

    private IEnumerator AnimatedSnap(Vector2Int cell, Motion originalMotion)
    {
        this.Snapping = true;
        var delta = grid.GetCellPosition(cell) - Position;
        while (delta.magnitude > 0.001f)
        {
            var intendedMovement = delta.normalized * SNAP_SPEED * Time.deltaTime;
            var x = Mathf.Abs(intendedMovement.x) >= Mathf.Abs(delta.x) ? delta.x : intendedMovement.x;
            var y = Mathf.Abs(intendedMovement.y) >= Mathf.Abs(delta.y) ? delta.y : intendedMovement.y;
            var movement = new Vector2(x, y);
            this.Position += movement;
            delta -= movement;
            yield return new WaitForSeconds(0f);
            if (originalMotion == this.CurrentMotion) break;
        }
        this.Snapping = false;
    }

    private Motion GetMotion(Nullable<Direction> direction)
    {
        if (!direction.HasValue) return Motion.NONE;
        if (direction.Value == Direction.DOWN || direction.Value == Direction.UP) return Motion.VERTICAL;
        if (direction.Value == Direction.LEFT || direction.Value == Direction.RIGHT) return Motion.HORIZONTAL;

        return Motion.NONE;
    }

    private bool UpdateCell()
    {
        var OFFSET = this.grid.cellSize * 9999f;
        var halfCellSize = this.grid.cellSize / 2f;
        var diffX = (this.Position.x + OFFSET - halfCellSize) % this.grid.cellSize - halfCellSize;
        var diffY = (this.Position.y + OFFSET - halfCellSize) % this.grid.cellSize - halfCellSize;
        var distanceToCenter = Mathf.Abs(new Vector2(diffX, diffY).magnitude);
        
        if (distanceToCenter <= this.grid.cellSize * CELL_PROXIMITY_PERCENT)
        {
            this.cell = this.grid.GetCellToSnap(this.transform.position);
            return true;
        }
        return false;
    }
}
