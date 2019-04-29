using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WalkingGrid : MonoBehaviour
{
    private const int GRID_SIZE = 50;
    public float cellSize;
    public GridLine linePrefab;
    public bool debug;
    private List<BlockingObject> blockingObjects = new List<BlockingObject>();
    public Cauldron Cauldron {get; private set;}
    public List<Witch> Witches { get; private set; }

    void Awake()
    {
        this.Witches = new List<Witch>(this.GetComponentsInChildren<Witch>());
        this.Cauldron = this.GetComponentInChildren<Cauldron>();
    }

    void Start()
    {
        if (this.debug)
        {
            var offset = -(cellSize / 2f + GRID_SIZE / 2 * cellSize);
            for (var i = 0; i < GRID_SIZE; i++)
            {
                var verticalLine = GameObject.Instantiate(linePrefab);
                verticalLine.SetVertical(transform.position.y + offset + i * cellSize);
                var horizontalLine = GameObject.Instantiate(linePrefab);
                horizontalLine.SetHorizontal(transform.position.x + offset + i * cellSize);
            }
        }
    }

    public void AddBlockingObject(BlockingObject blockingObject)
    {
        this.blockingObjects.Add(blockingObject);
    }

    public void RemoveBlockingObject(BlockingObject blockingObject)
    {
        this.blockingObjects.Remove(blockingObject);
    }

    public bool IsBlocked(Vector2Int cell)
    {
        return this.blockingObjects.Any(o => o.Cell.Equals(cell));
    }

    public bool[] UnblockedDirections(Vector2Int cell)
    {
        return CharMovement.DIRECTIONS.Select(
            direction => IsBlocked(cell + CharMovement.GetDirectionVector2D(direction)))
            .ToArray();
    }

    public Vector2 GetCellPosition(Vector2Int cell)
    {
        return new Vector2(cell.x * cellSize, cell.y * cellSize);
    }

    public Vector2Int GetCellToSnap(Vector2 position)
    {
        var CELLS_OFFSET = 10000;
        var OFFSET = cellSize * CELLS_OFFSET;
        var cellX = Mathf.FloorToInt((position.x + OFFSET + cellSize / 2) / cellSize) - CELLS_OFFSET;
        var cellY = Mathf.FloorToInt((position.y + OFFSET + cellSize / 2) / cellSize) - CELLS_OFFSET;
        return new Vector2Int(cellX, cellY);
    }
}
