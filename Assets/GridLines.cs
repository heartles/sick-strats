using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLines : MonoBehaviour {

    public Material _lineMaterial;
    public float _thickness;
    public GameObject _gridLine;
    public Color _color = new Color(.7f, .7f, .7f, .7f);

    // Use this for initialization
    void Start () {
	    for (int y = -10; y <= 10; y++) {
            var line = GameObject.Instantiate(_gridLine).GetComponent<LineRenderer>();
            line.transform.SetParent(this.transform);
            line.sharedMaterial = _lineMaterial;
            line.startWidth = _thickness;
            line.endWidth = _thickness;

            line.startColor = _color;
            line.endColor = _color;

            line.SetPosition(0, new Vector3(-22, y, 0));
            line.SetPosition(1, new Vector3(22, y, 0));
        }

        for (int x = -22; x <= 22; x++) {
            var line = GameObject.Instantiate(_gridLine).GetComponent<LineRenderer>();
            line.transform.SetParent(this.transform);
            line.sharedMaterial = _lineMaterial;
            line.startWidth = _thickness;
            line.endWidth = _thickness;

            line.startColor = _color;
            line.endColor = _color;

            line.SetPosition(0, new Vector3(x, -10, 0));
            line.SetPosition(1, new Vector3(x, 10, 0));
        }
    }
}
