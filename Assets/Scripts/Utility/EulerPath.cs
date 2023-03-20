using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EulerPath : MonoBehaviour
{
    GameStates gameState;
    List<Dot> dots;
    List<Dot> evenDots;
    List<Dot> oddDots;

    List<Dot> correctPath;

    int pathIndex;

    private bool showSolution;

    private void Start()
    {
        dots = new List<Dot>();
        evenDots = new List<Dot>();
        oddDots = new List<Dot>();
        correctPath = new List<Dot>();
    }

    private void OnEnable()
    {
        EventManager.Undo += Undo;
        EventManager.Reset += Reset;
        EventManager.ChangeGameState += ChangeGameState;
        EventManager.MouseOverDot += SolutionControl;
        EventManager.SolutionButtonClicked += SolutionButtonClicked;
    }

    private void Undo()
    {
        if (showSolution)
        {
            DOTween.Kill(pathIndex);
            pathIndex -= 1;

            if (pathIndex<0)
            {
                pathIndex = 0;
            }

 
                correctPath[pathIndex].dotImage.transform.localScale = Vector3.one;
                correctPath[pathIndex].dotImage.transform.DOScale(1.5f, .2f).SetLoops(-1, LoopType.Yoyo)
                    .SetId(pathIndex);
            
        }
    }

    private void Reset()
    {
        if (showSolution)
        {
            if (pathIndex > 0)
            {
                DOTween.Kill(pathIndex);
                pathIndex = 0;
                showSolution = false;
            }
        }
    }


    private void ChangeGameState(GameStates obj)
    {
        gameState = obj;
    }

    // if current solution dot selected,continue
    private void SolutionControl(Dot dot, Vector3 pos)
    {
        if (gameState == GameStates.Drawing && showSolution)
        {
            if (dot == correctPath[pathIndex])
            {
                DOTween.Kill(pathIndex);
                pathIndex += 1;

                if (pathIndex < correctPath.Count)
                {
                    correctPath[pathIndex].dotImage.transform.localScale = Vector3.one;
                    correctPath[pathIndex].dotImage.transform.DOScale(1.5f, .2f).SetLoops(-1, LoopType.Yoyo)
                        .SetId(pathIndex);
                }
            }
        }
    }

    private void OnDisable()
    {
        EventManager.Undo -= Undo;
        EventManager.Reset -= Reset;
        EventManager.ChangeGameState -= ChangeGameState;
        EventManager.MouseOverDot -= SolutionControl;
        EventManager.SolutionButtonClicked -= SolutionButtonClicked;
    }

    // show solution
    private void SolutionButtonClicked()
    {
        DOTween.KillAll();
        pathIndex = 0;
        showSolution = true;
        SetLists();
        correctPath[pathIndex].dotImage.transform.localScale = Vector3.one;
        correctPath[pathIndex].dotImage.transform.DOScale(1.5f, .2f).SetLoops(-1, LoopType.Yoyo).SetId(pathIndex);
    }


    // evendots list is for dots that have even number of connections,
    //odddots list is for dots that have odd number of connections
    void SetLists()
    {
        dots.Clear();
        evenDots.Clear();
        oddDots.Clear();

        foreach (var dot in GetComponentsInChildren<Dot>())
        {
            dots.Add(dot);
        }

        foreach (var dot in dots)
        {
            dot.Reset();
            if (dot.connections.Count % 2 == 0)
            {
                evenDots.Add(dot);
            }
            else
            {
                oddDots.Add(dot);
            }
        }

        var startDot = oddDots[0];
        correctPath.Add(startDot);
        PathFinder(startDot);
    }

    /*
     * step 1 start dot is a odd dot, we can choose any of them
     * step 2 if this dot have neighbour that have even connection continue with it
     * step 3 repeat step 2 until dot dont have even dot connection
     * step 4 repeat this process with odd neighbour
     * step 5 if dot dont have a neighbour path over
     */
    void PathFinder(Dot startDot)
    {
        if (FindDotEvenNeighbour(startDot) != null)
        {
            var nextDot = FindDotEvenNeighbour(startDot);
            Debug.Log(nextDot);
            startDot.CheckForConnection(nextDot);
            nextDot.CheckForConnection(startDot);
            correctPath.Add(nextDot);

            PathFinder(nextDot);
        }
        else if (FindDotOddNeighbour(startDot) != null)
        {
            var nextDot = FindDotOddNeighbour(startDot);
            Debug.Log(nextDot);
            startDot.CheckForConnection(nextDot);
            nextDot.CheckForConnection(startDot);
            correctPath.Add(nextDot);

            PathFinder(nextDot);
        }
        else
        {
            EventManager.Reset();
            correctPath[pathIndex].Clicked();
            pathIndex++;
        }
    }

    // find dot's even neighbour
    Dot FindDotEvenNeighbour(Dot currentDot)
    {
        foreach (var conn in currentDot.connections)
        {
            if (conn.dot.connections.Count % 2 == 0 && !conn.isFilled)
            {
                return conn.dot;
            }
        }

        return null;
    }
    // find dot's odd neighbour

    Dot FindDotOddNeighbour(Dot currentDot)
    {
        foreach (var conn in currentDot.connections)
        {
            if (conn.dot.connections.Count % 2 != 0 && !conn.isFilled)
            {
                return conn.dot;
            }
        }

        return null;
    }
}