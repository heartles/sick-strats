using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IVector2 {
    public int x, y;

    public IVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static bool operator==(IVector2 v1, IVector2 v2) {
        return v1.x == v2.x && v1.y == v2.y;
    }

    public static bool operator!=(IVector2 v1, IVector2 v2) {
        return !(v1 == v2);
    }

    public static IVector2 ToBlockPos(Vector3 pos) {
        var iPos = new IVector2();
        iPos.x = Mathf.RoundToInt(pos.x - .5f);
        iPos.y = Mathf.RoundToInt(pos.y - .5f);

        return iPos;
    }

    public IVector2 Add(int x, int y) {
        return new IVector2(this.x + x, this.y + y);
    }
}

public class Collisions {

    //TODO: Make this less shitty
    static readonly IVector2 _WorldMinBounds = new IVector2(-22, -10);
    static readonly IVector2 _WorldMaxBounds = new IVector2(22, 10);
    public static bool[,] Blocked;

    static Collisions() {
        Blocked = new bool[_WorldMaxBounds.x - _WorldMinBounds.x,
            _WorldMaxBounds.y - _WorldMinBounds.y];
    }

    public static void RegisterBlock(Vector3 pos) {
        var blockPos = IVector2.ToBlockPos(pos);

        Blocked[blockPos.x - _WorldMinBounds.x, blockPos.y - _WorldMinBounds.y] = true;
    }

    public static bool IsBlockFree(IVector2 pos) {
        if (Blocked[pos.x - _WorldMinBounds.x , pos.y - _WorldMinBounds.y]) return false;

        foreach (var unit in Unit.Units) {
            if (IVector2.ToBlockPos(unit.transform.position) == pos) {
                return false;
            }
        }

        return true;
    }
}
