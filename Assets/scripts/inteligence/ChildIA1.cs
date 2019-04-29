using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildIA1 : MonoBehaviour
{
    private WalkingGrid grid;
    private CharMovement child;
    void Awake()
    {
        this.grid = GetComponentInParent<WalkingGrid>();
        this.child = GetComponent<CharMovement>();
        this.child.WalkSpeed = 1f;
    }

    void Start()
    {
        StartCoroutine(StartAI());
    }

    public IEnumerator StartAI()
    {
        while (true)
        {
            for (var i = 0; i < 3; i++)
            {
                var blocked = this.grid.BlockedDirections(this.child.Cell);
                var direction = CharMovement.RandomDirection(except: blocked);
                var steps = this.child.Steps;
                this.child.Move(direction);
                yield return new WaitWhile(() => steps == this.child.Steps && !this.child.MovementBlocked);
            }
            this.child.Stop();

            for (var i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1f);
                this.child.TurnTo(CharMovement.RandomDirection(except: child.HeadedDirection));
            }
        }
    }
}
