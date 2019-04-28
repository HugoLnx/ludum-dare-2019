using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectScent : MonoBehaviour
{
    public int spread;
    private int[,] scent;
    private Nullable<Vector2Int> currentCell = null;
    private WalkingGrid grid;
    private Vector2Int center;
    private bool[,] alreadyEnqueded;
    private int countCalls;

    public Nullable<Vector2Int> CurrentCell { get { return currentCell; } }

    void Awake()
    {
        this.scent = new int[spread+10,spread+10];
        this.grid = GetComponentInParent<WalkingGrid>();
        this.center = new Vector2Int(spread / 2 + 5, spread / 2 + 5); var position = new Vector2(this.transform.position.x, this.transform.position.y);
        this.currentCell = this.grid.GetCellToSnap(position);
    }

    void Start()
    {
        UpdateScent();
    }

    void Update()
    {
        var position = new Vector2(this.transform.position.x, this.transform.position.y);
        var cell = this.grid.GetCellToSnap(position);
        if (!currentCell.HasValue || !currentCell.Value.Equals(cell))
        {
            this.currentCell = cell;
            UpdateScent();
        }
    }

    public int GetScentOn(Vector2Int cell)
    {
        Vector2Int coords = cell - this.currentCell.Value + this.center;
        if (coords.x < 0 || coords.x > spread || coords.y > spread || coords.y < 0) return 0;
        return this.scent[coords.x, coords.y];
    }
    
    public float GetScentPercentOn(Vector2Int cell)
    {
        return (GetScentOn(cell)-100f)/((float)spread);
    }

    public CharMovement.Direction GetDirectionToFollow(Vector2Int origin)
    {
        var right = GetScentOn(origin + Vector2Int.right);
        var left = GetScentOn(origin + Vector2Int.left);
        var up = GetScentOn(origin + Vector2Int.up);
        var down = GetScentOn(origin + Vector2Int.down);
        var bigger = Mathf.RoundToInt(Mathf.Max(right, left, up, down));
        if (right >= bigger) return CharMovement.Direction.RIGHT;
        if (left >= bigger) return CharMovement.Direction.LEFT;
        if (up >= bigger) return CharMovement.Direction.UP;
        return CharMovement.Direction.DOWN;

    }

    private void UpdateScent()
    {
        InitScent();
        Queue<(int, Vector2Int)> visitQueue = new Queue<(int, Vector2Int)>();
        visitQueue.Enqueue((this.spread, Vector2Int.zero));
        var callsCount = 0;
        this.alreadyEnqueded = new bool[spread + 10, spread + 10];
        while (visitQueue.Count > 0 && callsCount <= spread*spread+100)
        {
            //Debug.Log(callsCount++);
            var next = visitQueue.Dequeue();
            var strength = next.Item1;
            var cell = next.Item2;
            var coords = cell + this.center;
            this.scent[coords.x, coords.y] = strength + 100;
            EnqueueToVisit(visitQueue, strength - 1, cell + Vector2Int.right);
            EnqueueToVisit(visitQueue, strength - 1, cell + Vector2Int.left);
            EnqueueToVisit(visitQueue, strength - 1, cell + Vector2Int.up);
            EnqueueToVisit(visitQueue, strength - 1, cell + Vector2Int.down);
        }
    }

    private void EnqueueToVisit(Queue<(int, Vector2Int)> visitQueue, int steps, Vector2Int cell)
    {
        var coords = cell + this.center;
        
        if (steps > 0
            && Mathf.Abs(cell.x) <= spread / 2
            && Mathf.Abs(cell.y) <= spread / 2
            && this.scent[coords.x, coords.y] <= 0
            && !this.alreadyEnqueded[coords.x, coords.y]) {
            visitQueue.Enqueue((steps, cell));
            this.alreadyEnqueded[coords.x, coords.y] = true;
        }
    }

    private void InitScent()
    {
        this.scent = new int[spread + 10, spread + 10];
        for(var x = 0; x < spread+10; x++)
        {
            for (var y = 0; y < spread + 10; y++)
            {
                this.scent[x, y] = -1;
            }
        }
    }
}
