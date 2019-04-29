using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharMovement.Direction;

public class BasicHunterAI : MonoBehaviour
{
    private HunterActions hunter;
    private CharMovement hunterMovement;
    private HunterSprite hunterSprite;
    private ObjectScent witchScent;
    private ObjectScent cauldronScent;
    private State state;
    private CharMovement.Direction witchShotDirection;
    private WalkingGrid grid;
    private HunterBang bang;

    enum State
    {
        CHASING_WITCH, CHASING_CAULDRON, SHOOTING,
        SEARCHING_RANDOMLY
    }

    void Awake()
    {
        this.bang = GetComponentInChildren<HunterBang>();
        this.grid = GetComponentInParent<WalkingGrid>();
        this.witchScent = this.grid.Witches[0].Scent;
        this.cauldronScent = this.grid.Cauldron.Scent;
        this.hunter = GetComponent<HunterActions>();
        this.hunterMovement = GetComponent<CharMovement>();
        this.hunterSprite = GetComponentInChildren<HunterSprite>();
    }

    void Start()
    {
        this.state = State.CHASING_CAULDRON;
        this.hunterMovement.WalkSpeed = 2.5f;
        StartCoroutine(StartIA());
    }

    private IEnumerator StartIA()
    {
        var jumpCell = this.grid.ToClosestEdgeVector(this.hunterMovement.Cell)*2;
        var jumpOrigin2d = this.grid.GetCellPosition(jumpCell);
        var jumpOrigin3d = new Vector3(jumpOrigin2d.x, jumpOrigin2d.y, this.transform.position.z);
        this.hunterMovement.TurnTo(jumpOrigin2d.x >= 0f ? CharMovement.Direction.LEFT : CharMovement.Direction.RIGHT);
        var jump = new JumpAnimation(this.hunterSprite, from: jumpOrigin3d, to: Vector2.zero).Start();
        yield return new WaitUntil(() => jump.HasFinished);
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
                    this.hunterMovement.Stop();
                    var anim = this.bang.Alert();
                    this.hunterMovement.TurnTo(this.witchShotDirection);
                    yield return new WaitForSeconds(1f);
                    yield return new WaitUntil(() => anim.HasFinished);
                    this.hunter.Shoot();
                    this.state = State.CHASING_CAULDRON;
                    yield return new WaitForSeconds(1f);
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

    private bool ChangeState()
    {
        var currentState = this.state;
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
        var changed = currentState != this.state;
        return changed;
    }
    
}
