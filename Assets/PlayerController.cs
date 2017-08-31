using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ActionType {
    None,
    Attack,
    Move
}

public class Region {
    public IVector2 Center;
    public int Radius;

    private int roundToInfinities(float v) {
        return (v < 0) ? Mathf.RoundToInt(v - 0.5f) :
            (v > 0) ? Mathf.RoundToInt(v + 0.5f) :
            0;
    }

    public Vector3[] GetLine() {
        int r2 = Radius*Radius;

        int x = +Radius;
        int y = 0;
        var points = new List<Vector3>(r2/2);

        while (x >= y) {
            points.Add(new Vector3(x+Center.x, y+Center.y, 0));

            y++;
            while (x*x + y*y > r2) {
                x--; //TODO: do this

            }
        }

        return points.ToArray();
    }
}

public class PlayerController : MonoBehaviour {
    public Unit _selected;
    public Camera _cam;

    public Action _selectedAction;

    public Button[] _uiButtons = new Button[4];

    public GameObject _selectedOverlay;

    public Vector3 _prevMousePos;

    void SetCurrentAction(int actionIndex) {
        _selectedAction = null;

        for (int i = 0; i < _uiButtons.Length; i++) {
            if (i != actionIndex) {
                var c = _uiButtons[i].colors;
                c.normalColor = new Color(1, 1, 1);
                _uiButtons[i].colors = c;
            } else {
                var c = _uiButtons[i].colors;
                c.normalColor = new Color(1, 188 / 255f, 188 / 255f);
                _uiButtons[i].colors = c;
                _selectedAction = _selected.Actions[i];
            }
        }
    }

    // Use this for initialization
    void Start () {
        var line = new GameObject().AddComponent<LineRenderer>();
        line.SetPositions(new Region() { Radius = 5 }.GetLine());
        line.startColor = Color.green;
        line.endColor = Color.green;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        //TODO: Don't reuse vars for prefab and instance
        _selectedOverlay = Instantiate(_selectedOverlay);
        
        for (int i = 1; i <= 4; i++) {
            _uiButtons[i - 1] = GameObject.Find("UI Action " + i).
                GetComponent<Button>();

            _uiButtons[i - 1].GetComponent<Image>().enabled = false;
            _uiButtons[i - 1].GetComponentInChildren<Text>().enabled = false;
            _uiButtons[i - 1].enabled = false;
            int copy = i - 1;
            _uiButtons[i - 1].onClick.AddListener(() => {
                if (_selectedAction == _selected.Actions[copy]) {
                    SetCurrentAction(-1);
                } else {
                    SetCurrentAction(copy);
                }
            });
        }

        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        GameObject.Find("UI End Turn").GetComponent<Button>()
            .onClick.AddListener(() => {
                foreach (var unit in Unit.Units) {
                    unit.StdActionRemaining = 6;
                    unit.MoveActionRemaining = 6;
                }

                DeselectUnit();

                AIController.EndTurn();
            });
    }

    void SelectUnit (Unit u) {
        _selected = u;
        GameObject.Find("UI Unit Std").GetComponent<Text>()
            .text = "Std: " + u.StdActionRemaining;
        GameObject.Find("UI Unit Move").GetComponent<Text>()
            .text = "Move: " + u.MoveActionRemaining;
        GameObject.Find("UI Unit Stats").GetComponent<Text>()
            .text = "HP: " + u.HP;

        _selectedOverlay.GetComponent<SpriteRenderer>().enabled = true;
        _selectedOverlay.transform.position = u.transform.position;
        

        for (int i = 0; i < u.Actions.Length; i++) {
            var o = _uiButtons[i];

            o.enabled = true;
            o.GetComponent<Image>().enabled = true;

            var t = o.GetComponentInChildren<Text>();
            t.enabled = true;
            t.text = u.Actions[i].Name;
            // TODO: move this into button abstraction
        }
    }

    void DeselectUnit () {
        if (_selected == null)
            return;
        GameObject.Find("UI Unit Std").GetComponent<Text>()
            .text = "";
        GameObject.Find("UI Unit Move").GetComponent<Text>()
            .text = "";
        GameObject.Find("UI Unit Stats").GetComponent<Text>()
            .text = "";

        _selectedOverlay.GetComponent<SpriteRenderer>().enabled = false;

        for (int i = 0; i < _selected.Actions.Length; i++) {
            var o = _uiButtons[i];

            o.enabled = true;
            o.GetComponent<Image>().enabled = false;
            o.GetComponentInChildren<Text>().enabled = false;
            // TODO: move this into button abstraction
        }

        _selected = null;
        _selectedAction = null;
    }
	
	// Update is called once per frame
	void Update () {
        var mPos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(2)) {
            _cam.transform.position += -(mPos - _prevMousePos);
        }
        _prevMousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        var dZoom = Input.GetAxis("Mouse ScrollWheel");
        _cam.orthographicSize -= dZoom * 4 * (_cam.orthographicSize / 2);

        if (!_cam.ContainsWorldPoint(mPos)) {
            return;
        }

        mPos.z = 0;

        var mBlockPos = new IVector2(
            Mathf.RoundToInt(mPos.x - 0.5f),
            Mathf.RoundToInt(mPos.y - 0.5f));

        mPos.x = mBlockPos.x + .5f;
        mPos.y = mBlockPos.y + .5f;

        System.Action executeMoveAction = () => {
            int dist = Mathf.RoundToInt(Mathf.Abs(mPos.x - _selected.transform.position.x) + Mathf.Abs(mPos.y - _selected.transform.position.y));
            if (dist > 0 && Collisions.IsBlockFree(mBlockPos)) {
                if (dist <= _selected.MoveActionRemaining) {
                    _selected.MoveActionRemaining -= dist;
                    _selected.transform.position = mPos;

                } else if (dist <= _selected.MoveActionRemaining + _selected.StdActionRemaining) {
                    dist -= _selected.MoveActionRemaining;
                    _selected.MoveActionRemaining = 0;
                    _selected.StdActionRemaining -= dist;

                    _selected.transform.position = mPos;
                }

                SelectUnit(_selected);
                // TODO: make UI follow unit's stats w/o having to track them
            }
        };

        if (Input.GetMouseButtonUp(0)) {
            if (_selectedAction == null) {
                bool found = false;
                foreach (var unit in Unit.Units) {
                    if (unit.Team == "Player" && unit.GetComponent<Collider2D>().bounds.Contains(mPos)) {
                        if (unit == _selected) {
                            DeselectUnit();
                        } else {
                            SelectUnit(unit);
                        }
                        found = true;
                    }
                }

                if (!found) {
                    DeselectUnit();
                }
            } else switch (_selectedAction.Type) {
                case ActionType.None:
                    throw new InvalidOperationException("ActionType was None");

                case ActionType.Move:
                    executeMoveAction();
                    break;

                case ActionType.Attack: {
                    if (_selected.StdActionRemaining < 6) break;

                    Unit toAttack = null;
                    bool found = false;
                    foreach (var unit in Unit.Units) {
                        if (unit.GetComponent<Collider2D>().bounds.Contains(mPos)) {
                            toAttack = unit;
                            found = true;
                        }
                    }

                    if (!found) break;

                    toAttack.HP -= 10;
                    if (toAttack.HP <= 0) {
                        Destroy(toAttack.gameObject);
                    }
                    _selected.StdActionRemaining -= 6;

                    SelectUnit(_selected); //TODO: FIX THIS SHITTY HACK
                } break;                    
            }
        }

        var mb2 = Input.GetMouseButtonUp(1);
        if (mb2 && _selected != null) {
            executeMoveAction();
        }

        var cameraVel = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) cameraVel.x -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) cameraVel.x += 1;
        if (Input.GetKey(KeyCode.DownArrow)) cameraVel.y -= 1;
        if (Input.GetKey(KeyCode.UpArrow)) cameraVel.y += 1;

        cameraVel *= Time.deltaTime * 10f;
        _cam.transform.position += new Vector3(cameraVel.x, cameraVel.y, 0);
    }
}
