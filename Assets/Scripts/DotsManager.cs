using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DotsManager : MonoBehaviour
{
    public GameStates gameState;
    LineRenderer _line;
    private int _pointIndex;
    private Vector3 currentPos;
    private Dot currentDot;
    private Dot previousDot;

    public List<Dot> allDots;


    private void Start()
    {
        _line = GetComponent<LineRenderer>();
    }


    private void OnValidate()
    {
        allDots.Clear();
        foreach (Dot dot in GetComponentsInChildren<Dot>())
        {
            allDots.Add(dot);
        }
    }

    private void OnEnable()
    {
        EventManager.Reset += Reset;
        EventManager.Undo += Undo;
        EventManager.MouseOverDot += MouseOverDot;
        EventManager.ChangeGameState += ChangeGameState;
        EventManager.StartDrawing += StartDrawing;
        EventManager.CheckAllConnectionsDone += CheckAllConnectionsDone;
    }
    

    private void Reset()
    {
        _pointIndex = 0;
        _line.positionCount = 2;
        _line.enabled = false;
        EventManager.ChangeGameState(GameStates.Start);
    }

    private void Undo()
    {
        if (currentDot == previousDot)
        {
            EventManager.ChangeGameState(GameStates.Start);
            _line.enabled = false;
        }
        else
        {
            if (previousDot.connectedDots.Count != 0)
            {
                _pointIndex--;
                _line.positionCount = _pointIndex + 2;
                var tempPrevious = previousDot.connectedDots.Last();
                var tempCurrent = previousDot;
                currentDot.DisconnectFromDot(previousDot, true);
                previousDot.DisconnectFromDot(currentDot, false);

                currentDot = tempCurrent;
                previousDot = tempPrevious;
            }
            else
            {
                _pointIndex--;
                _line.positionCount = _pointIndex + 2;
                currentDot.DisconnectFromDot(previousDot, true);
                previousDot.DisconnectFromDot(currentDot, false);

                currentDot = previousDot;
            }
        }
    }

    private void CheckAllConnectionsDone()
    {
        foreach (var dot in allDots)
        {
            if (!dot.CheckConnections())
            {
                return;
            }
        }

        EventManager.ChangeGameState(GameStates.LevelWin);
        _line.positionCount = _line.positionCount - 1;
        EventManager.LevelWin();
    }

    private void MouseOverDot(Dot dot, Vector3 pos)
    {
        if (dot != currentDot && !dot.connectedDots.Contains(currentDot) && !currentDot.connectedDots.Contains(dot) && dot.SearchDotOnConnections(currentDot))
        {
            dot.connectedDots.Add(currentDot);
            dot.CheckForConnection(currentDot);
            currentDot.CheckForConnection(dot);
            previousDot = currentDot;
            currentDot = dot;
            pos.z = 0;
            _pointIndex++;
            _line.SetPosition(_pointIndex, pos);
            _line.positionCount = _pointIndex + 2;
            EventManager.CheckAllConnectionsDone();
        }
    }

    private void ChangeGameState(GameStates state)
    {
        gameState = state;
    }

    private void OnDisable()
    {
        EventManager.Undo -= Undo;
        EventManager.Reset -= Reset;
        EventManager.MouseOverDot -= MouseOverDot;
        EventManager.ChangeGameState -= ChangeGameState;
        EventManager.StartDrawing -= StartDrawing;
        EventManager.CheckAllConnectionsDone -= CheckAllConnectionsDone;
    }

    private void StartDrawing(Dot dot, Vector3 startPos)
    {
        currentDot = dot;
        previousDot = dot;
        startPos.z = 0;
        _line.enabled = true;
        _line.SetPosition(_pointIndex, startPos);
        EventManager.ChangeGameState(GameStates.Drawing);
    }

    private void Update()
    {
        if (gameState == GameStates.Drawing)
        {
            currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0;
            _line.SetPosition(_pointIndex + 1, currentPos);
        }
    }
}