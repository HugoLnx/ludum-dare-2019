using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharMovement.Direction;

public class BasicHunterAI : MonoBehaviour
{
    private CharMovement hunter;

    void Awake()
    {
        this.hunter = GetComponent<CharMovement>();
    }

    void Start()
    {
        StartCoroutine(StartAI());
    }

    private IEnumerator StartAI()
    {

        while (true)
        {
            this.hunter.Move(RIGHT);
            yield return new WaitUntil(() => this.hunter.Steps >= 5);
            this.hunter.Move(LEFT);
            yield return new WaitUntil(() => this.hunter.Steps >= 5);
            //this.hunter.Step(RIGHT);
            //yield return new WaitWhile(() => this.hunter.Walking);
            //this.hunter.Step(LEFT);
            //yield return new WaitWhile(() => this.hunter.Walking);
            //this.hunter.Step(LEFT);
            //yield return new WaitWhile(() => this.hunter.Walking);
        }
    }
}
