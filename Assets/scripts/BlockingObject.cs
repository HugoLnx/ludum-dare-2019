using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingObject : MonoBehaviour
{
    private WalkingGrid grid;
    public Vector2Int Cell { get { return this.grid.GetCellToSnap(this.transform.position); } }

    void Awake()
    {
        this.grid = GetComponentInParent<WalkingGrid>();
        this.transform.position = this.grid.GetCellPosition(this.Cell);
    }

    void Update()
    {
        var p = this.transform.position;
        this.transform.position = new Vector3(p.x, p.y, 100 + this.Cell.y);
    }

    void OnEnable()
    {
        this.grid.AddBlockingObject(this);
    }

    void OnDisable()
    {
        //this.grid.RemoveBlockingObject(this);
    }
}
