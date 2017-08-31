using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScreen : MonoBehaviour {

	// Use this for initialization
	void OnPreRender () {
        GL.Clear(true, true, Color.black, 0);
	}
}
