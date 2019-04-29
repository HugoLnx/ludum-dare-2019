using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSprite : MonoBehaviour
{
    private const float DISTANCE = 0.15f;
    private const float DURATION = 0.1f;

    public PremadeAnimation RunKnockback(CharMovement.Direction charDirection)
    {
        var forward = CharMovement.GetDirectionVector(charDirection);
        var backwards = new Vector3(-forward.x, -forward.y, -forward.z);

        return new AnimationChain(this, new List<PremadeAnimation> {
            new TranslateAnimation(this, backwards * DISTANCE, duration: DURATION/2f),
            new TranslateAnimation(this, forward * DISTANCE, duration: DURATION/2f)
        }).Start();
    }
}
