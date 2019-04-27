using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharMovement.Direction;

public class BasicHunterAI : MonoBehaviour
{
    private CharMovement hunter;
    public ObjectScent witchScent;

    void Awake()
    {
        this.hunter = GetComponent<CharMovement>();
    }

    void Start()
    {
        this.hunter.WalkSpeed = 2.5f;
        StartCoroutine(FollowScent(witchScent));
    }

    private IEnumerator FollowScent(ObjectScent scent)
    {

        while (true)
        {
            var scentDirection = scent.GetDirectionToFollow(hunter.Cell);
            if (!this.hunter.CurrentDirection.HasValue || scentDirection != this.hunter.CurrentDirection.Value)
            {
                this.hunter.Move(scentDirection);
            }
            var steps = this.hunter.Steps;
            yield return new WaitWhile(() => steps == this.hunter.Steps);
        }
    }
}
