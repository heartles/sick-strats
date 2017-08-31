using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtensions {
    public static bool ContainsWorldPoint(this Camera c, Vector2 world) {
        var vp = c.WorldToViewportPoint(world);

        return c.ContainsViewportPoint(vp);
    }

    public static bool ContainsViewportPoint(this Camera c, Vector2 vp) {
        if (vp.x > 1 || vp.x < 0 || vp.y > 1 || vp.y < 0)
            return false;
        return true;
    }
}

public class Action {
    public ActionType Type;
    public string Name;
}

public class Unit : MonoBehaviour {

    public static List<Unit> Units = new List<Unit>();

    public string Team;

    public float HP = 100;

    public Action[] Actions = new Action[] {
        new Action() {
            Type = ActionType.Move,
            Name = "Move"
        },
        new Action() {
            Type = ActionType.Attack,
            Name = "Attack"
        }
    };

	public int MoveActionRemaining = 6;
	public int StdActionRemaining = 6;
    // Use this for initialization
    void Start () {
        Units.Add(this);
	}
}
