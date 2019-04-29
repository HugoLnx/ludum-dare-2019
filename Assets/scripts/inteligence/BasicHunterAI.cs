using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharMovement.Direction;

public class BasicHunterAI : MonoBehaviour
{
    private HunterActions hunter;
    private CharMovement hunterMovement;
    private ObjectScent witchScent;
    private ObjectScent cauldronScent;
    private State state;
    private CharMovement.Direction witchShotDirection;
    private WalkingGrid grid;

    enum State
    {
        CHASING_WITCH, CHASING_CAULDRON, SHOOTING,
        SEARCHING_RANDOMLY
    }

    void Awake()
    {
        this.grid = GetComponentInParent<WalkingGrid>();
        this.witchScent = this.grid.Witches[0].Scent;
        this.cauldronScent = this.grid.Cauldron.Scent;
        this.hunter = GetComponent<HunterActions>();
        this.hunterMovement = GetComponent<CharMovement>();
    }

    void Start()
    {
        this.state = State.CHASING_CAULDRON;
        this.hunterMovement.WalkSpeed = 2.5f;
        StartCoroutine(StartIA());
    }

    private IEnumerator StartIA()
    {
        while (true)
        {
            var steps = this.hunterMovement.Steps;
            switch (state)
            {
                case State.CHASING_WITCH:
                    StepFollowing(witchScent);
                    yield return new WaitWhile(() => steps == this.hunterMovement.Steps && !this.hunterMovement.MovementBlocked);
                    break;
                case State.CHASING_CAULDRON:
                    StepFollowing(cauldronScent);
                    yield return new WaitWhile(() => steps == this.hunterMovement.Steps && !this.hunterMovement.MovementBlocked);
                    break;
                case State.SEARCHING_RANDOMLY:
                    StepRandomly();
                    yield return new WaitWhile(() => steps == this.hunterMovement.Steps && !this.hunterMovement.MovementBlocked);
                    break;
                case State.SHOOTING:
                    this.hunterMovement.TurnTo(this.witchShotDirection);
                    this.hunter.Shoot();
                    this.state = State.CHASING_CAULDRON;
                    yield return new WaitForSeconds(1.5f);
                    break;
            }
            ChangeState();
        }
    }

    private void StepFollowing(ObjectScent scent)
    {
        var scentDirection = scent.GetDirectionToFollow(this.hunterMovement.Cell);
        if (!this.hunterMovement.CurrentDirection.HasValue || scentDirection != this.hunterMovement.CurrentDirection.Value)
        {
            this.hunterMovement.Move(scentDirection);
        }
    }

    private void StepRandomly()
    {
        var direction = CharMovement.RandomDirection(except: this.grid.BlockedDirections(this.hunterMovement.Cell));
        this.hunterMovement.Move(direction);
    }


    private void ChangeState()
    {
        if (this.state != State.SHOOTING)
        {
            var shotDirection = this.hunterMovement.Sees(witchScent.CurrentCell.Value, raid: 6);
            if (shotDirection.HasValue)
            {
                this.witchShotDirection = shotDirection.Value;
                this.state = State.SHOOTING;
            }
            else if (this.hunterMovement.Hear(witchScent.CurrentCell.Value, raid: 8))
            {
                this.state = State.CHASING_WITCH;
            }
            else if (this.hunterMovement.Sees(cauldronScent.CurrentCell.Value, raid: 2).HasValue)
            {
                this.state = State.SEARCHING_RANDOMLY;
            } else {
                this.state = State.CHASING_CAULDRON;
            }

        }
    }
    
}
