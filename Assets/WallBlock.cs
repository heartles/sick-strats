using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Collisions.RegisterBlock(transform.position);	
	}
}
