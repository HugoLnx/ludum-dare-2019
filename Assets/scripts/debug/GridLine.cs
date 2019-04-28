using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLine : MonoBehaviour
{
    private const float LINE_WEIGHT = 0.05f;
    private LineRenderer line;

    void Awake()
    {
        this.line = GetComponent<LineRenderer>();
    }

    public void SetHorizontal(float y)
    {
        this.line.SetPositions(new Vector3[]
        {
            new Vector3(0f, y-LINE_WEIGHT/2f, 1f),
            new Vector3(0f, y+LINE_WEIGHT/2f, 1f)
        });
    }

    public void SetVertical(float x)
    {
        this.line.SetPositions(new Vector3[]
        {
            new Vector3(x-LINE_WEIGHT/2f, 0f, 1f),
            new Vector3(x+LINE_WEIGHT/2f, 0f, 1f)
        });
    }
}
