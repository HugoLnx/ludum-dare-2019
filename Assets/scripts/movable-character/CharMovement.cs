using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharMovement : MonoBehaviour
{
    private const float SNAP_SPEED = 5f;
    public float WalkSpeed { get; set; }
    private const float MOVEMENT_COMMITMENT_PERCENT = 0.2f; // Commited to the movement after 20% of it being completed 
    private const float CELL_PROXIMITY_PERCENT = 0.25f; // If it is 5% further from the cell middle, it is on that middle

    private Animator animator;
    private Grid grid;
    private Nullable<Direction> movingDirection;
    public Nullable<Direction> CurrentDirection { get { return movingDirection; } }
    public Direction HeadedDirection { get; private set; }
    private Vector2Int cell;
    public bool Walking { get { return this.movingDirection.HasValue; } }
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
    public Vector2Int Cell { get { return this.cell; } }
    public bool instantSnap = false;

    public enum Direction { UP, DOWN, RIGHT, LEFT }
    private static readonly Direction[] DIRECTIONS = new Direction[] { Direction.UP, Direction.DOWN, Direction.RIGHT, Direction.LEFT };
    private enum Motion { HORIZONTAL, VERTICAL, NONE }
    private readonly Dictionary<Direction, string> DIRECTION_NAMES = new Dictionary<Direction, string>
    {
        { Direction.UP, "up" },
        { Direction.DOWN, "down" },
        { Direction.RIGHT, "right" },
        { Direction.LEFT, "left" }
    };
    private static readonly Dictionary<Direction, Vector3> DIRECTION_VECTORS = new Dictionary<Direction, Vector3>
    {
        { Direction.UP, Vector3.up },
        { Direction.DOWN, Vector3.down },
        { Direction.RIGHT, Vector3.right },
        { Direction.LEFT, Vector3.left }
    };

    public Direction? Sees(Vector2Int cell, int raid)
    {
        if ((cell - this.cell).magnitude > raid) return null;
        if (cell.x == this.cell.x)
        {
            return cell.y > this.cell.y ? Direction.UP : Direction.DOWN;
        }
        if (cell.y == this.cell.y)
        {
            return cell.x > this.cell.x ? Direction.RIGHT : Direction.LEFT;
        }
        return null;
    }

    public bool Hear(Vector2Int cell, int raid)
    {
        return (cell - this.cell).magnitude <= raid;
    }


    public static Direction RandomDirection()
    {
        var inx = UnityEngine.Random.Range(0, DIRECTIONS.Length - 1);
        return DIRECTIONS[inx];
    }

    public int Steps { get; private set; }

    void Awake()
    {
        this.HeadedDirection = Direction.DOWN;
        this.WalkSpeed = 5f;
        this.animator = GetComponentInChildren<Animator>();
        this.grid = GetComponentInParent<Grid>();
        this.cell = this.grid.GetCellToSnap(this.transform.position);
        InstantSnapTo(this.cell);
    }

    void Update()
    {
        var closeToCellCenter = UpdateCell();
        if (!Snapping && closeToCellCenter && this.commitedToMovement)
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
            var vector = CharMovement.GetDirectionVector(this.movingDirection.Value);
            this.transform.position += vector * WalkSpeed * Time.deltaTime;
            var normalizedInt = new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
            var nextCellPosition = this.grid.GetCellPosition(this.cell + normalizedInt);
            var missingToNextCell = nextCellPosition - Position;
            if (missingToNextCell.magnitude / this.grid.cellSize < 1f - MOVEMENT_COMMITMENT_PERCENT)
            {
                this.commitedToMovement = true;
            }
        }
    }

    public static Vector3 GetDirectionVector(Direction direction)
    {
        return DIRECTION_VECTORS[direction];
    }

    public void Move(Direction direction)
    {
        if (this.commitedToMovement || this.Snapping || (this.movingDirection.HasValue && this.movingDirection.Value == direction)) return;
        if (GetMotion(direction) != CurrentMotion) Snap(CurrentMotion);
        this.animator.SetBool("is-idle", false);
        this.Steps = 0;
        this.scheduledStop = false;
        this.movingDirection = direction;
        this.HeadedDirection = direction;
        this.animator.SetTrigger($"walk-{DIRECTION_NAMES[direction]}");
    }

    public void TurnTo(Direction direction)
    {
        Stop();
        this.HeadedDirection = direction;
        StartCoroutine(PostponeTurnTo(direction));

    }
    private IEnumerator PostponeTurnTo(Direction direction)
    {
        yield return new WaitForFixedUpdate();
        this.animator.SetTrigger($"turn-to-{DIRECTION_NAMES[direction]}");
    }

    public void Stop()
    {
        if (!this.movingDirection.HasValue) return;
        if (this.commitedToMovement)
        {
            this.scheduledStop = true;
            return;
        }
        this.Steps = 0;
        this.animator.SetBool("is-idle", true);

        this.scheduledStop = false;
        var originalMotion = CurrentMotion;
        this.movingDirection = null;
        this.animator.SetTrigger("idle");
        Snap(originalMotion);
    }
    
    private void Snap(Motion originalMotion)
    {
        if (instantSnap)
        {
            InstantSnapTo(this.cell);
        }
        else
        {
            StartCoroutine(AnimatedSnap(this.cell, originalMotion));
        }
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
        this.cell = this.grid.GetCellToSnap(this.transform.position);
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
            var cell = this.grid.GetCellToSnap(this.transform.position);
            if (!cell.Equals(this.cell))
            {
                this.Steps++;
                this.cell = cell;
                return true;
            }
        }
        return false;
    }
}
