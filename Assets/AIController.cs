using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public static void EndTurn() {
        foreach (var unit in Unit.Units) {
            if (unit.Team == "AI") {
                var pos = unit.transform.position;
                var blockPos = IVector2.ToBlockPos(pos);

                if (Collisions.IsBlockFree(blockPos.Add(1, 0))) {
                    pos.x += 1;
                } else if (Collisions.IsBlockFree(blockPos.Add(-1, 0))) {
                    pos.x -= 1;
                }

                unit.transform.position = pos;
            }
        }
    }
}
