using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PremadeAnimation
{
    bool HasFinished { get; }
    float Progress { get; }
    PremadeAnimation Start();
    PremadeAnimation SetDuration(float duration);
}
