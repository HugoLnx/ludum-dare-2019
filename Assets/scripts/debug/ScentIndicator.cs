using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentIndicator : MonoBehaviour
{
    private SpriteRenderer srenderer;
    public Vector2Int cell;
    public ObjectScent scent;
    private float currentScentPercent;
    private int currentScent;

    void Awake()
    {
        this.srenderer = GetComponent<SpriteRenderer>();   
    }

    public void UpdateScent()
    {
        this.currentScentPercent = scent.GetScentPercentOn(cell);
        this.currentScent = scent.GetScentOn(cell);
        this.srenderer.color = Color.Lerp(Color.green, Color.red, this.currentScentPercent);
    }
}
