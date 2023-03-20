using System;
using UnityEngine;


public static class EventManager
{
    public static Action<Dot,Vector3> StartDrawing;
    public static Action<GameStates> ChangeGameState;
    public static Action<Dot,Vector3> MouseOverDot;
    public static Action CheckAllConnectionsDone;
    public static Action LevelWin;
    public static Action Undo;
    public static Action SolutionButtonClicked;
    public static Action Reset;
    public static Action<int> NextLevelButton;






}