using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScentGrid : MonoBehaviour
{
    private List<ScentIndicator> indicators = new List<ScentIndicator>();
    public ObjectScent scent;
    public ScentIndicator indicatorPrefab;
    private Nullable<Vector2Int> scentOrigin = null;
    private WalkingGrid grid;

    void Start()
    {
        this.grid = GetComponentInParent<WalkingGrid>();
        for(var x = -scent.spread/2; x < scent.spread/2; x++)
        {
            for(var y = -scent.spread/2; y < scent.spread/2; y++)
            {
                var indicator = GameObject.Instantiate(indicatorPrefab);
                indicator.scent = scent;
                indicator.cell = new Vector2Int(x, y);
                indicator.transform.SetParent(this.transform);
                indicator.transform.position = grid.GetCellPosition(indicator.cell);
                indicators.Add(indicator);
            }
        }
    }
    
    void Update()
    {
        if (!scent.CurrentCell.HasValue) return;
        if (!scentOrigin.HasValue || !scentOrigin.Value.Equals(scent.CurrentCell.Value))
        {
            foreach(var indicator in indicators) indicator.UpdateScent();
            this.scentOrigin = scent.CurrentCell.Value;
        }

    }
}
