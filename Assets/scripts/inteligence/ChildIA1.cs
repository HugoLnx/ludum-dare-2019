using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildIA1 : MonoBehaviour
{
    private CharMovement child;
    void Awake()
    {
        this.child = GetComponent<CharMovement>();
        this.child.WalkSpeed = 3f;
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
                var direction = CharMovement.RandomDirection();
                var steps = this.child.Steps;
                this.child.Move(direction);
                yield return new WaitWhile(() => steps == this.child.Steps && !this.child.MovementBlocked);
            }
            this.child.Stop();
            yield return new WaitForSeconds(3f);
        }
    }
}
