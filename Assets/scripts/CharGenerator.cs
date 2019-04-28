using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharGenerator : MonoBehaviour
{
    public CharMovement prefab;
    public float delay;
    private WalkingGrid grid;
    private int WIDTH = 20;
    private int HEIGHT = 8;

    void Start()
    {
        this.grid = GetComponentInParent<WalkingGrid>();
        StartCoroutine(TimeSpawn());    
    }

    private IEnumerator TimeSpawn()
    {
        while (true)
        {
            var x = Random.Range(-((WIDTH - 3) / 2)-1, ((WIDTH - 3)/2)-1);
            var y = Random.Range(-(HEIGHT / 2 - 1), HEIGHT/2 - 1);
            if (x >= -1) x += 3;
            var cell = new Vector2Int(x, y);
            var obj = GameObject.Instantiate(prefab, this.grid.transform);
            obj.Cell = cell;
            obj.transform.position = this.grid.GetCellPosition(cell);
            yield return new WaitForSeconds(this.delay);
        }
    }
}
