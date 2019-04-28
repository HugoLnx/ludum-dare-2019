using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharMovement;

public class HunterShot : MonoBehaviour
{
    private const float SPEED = 5f;
    private Direction direction;
    private WalkingGrid grid;

    public void Setup(WalkingGrid grid, Vector2Int cell, Direction direction)
    {
        this.direction = direction;
        this.grid = grid;
        this.transform.SetParent(this.grid.transform);
        this.transform.position = this.grid.GetCellPosition(cell);
    }

    void Update()
    {
        if (this.grid == null) return;
        this.transform.position += CharMovement.GetDirectionVector(direction) * SPEED * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    private void CheckCollision(Collider2D collision)
    {
        if (collision.CompareTag(Witch.TAG))
        {
            var movement = collision.gameObject.GetComponent<CharMovement>();
            if (!movement.Cell.Equals(this.grid.GetCellToSnap(this.transform.position))) return;

            var witch = collision.gameObject.GetComponent<Witch>();
            witch.Dead();
            GameObject.Destroy(this.gameObject);
        }
    }
}
