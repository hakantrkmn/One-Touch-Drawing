using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
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
      EventManager.ChangeGameState += ChangeGameState;
      EventManager.MouseOverDot += SolutionControl;
      EventManager.SolutionButtonClicked += SolutionButtonClicked;
   }

   private void ChangeGameState(GameStates obj)
   {
      gameState = obj;
   }

   private void SolutionControl(Dot dot, Vector3 pos)
   {
      if (gameState==GameStates.Drawing && showSolution)
      {
         if (dot == correctPath[pathIndex])
         {
            DOTween.Kill(pathIndex);
            pathIndex += 1;

            if (pathIndex<correctPath.Count)
            {
               correctPath[pathIndex].dotImage.transform.DOScale(1.5f, .2f).SetLoops(-1, LoopType.Yoyo).SetId(pathIndex);

            }
         }
         
      }
     
   }

   private void OnDisable()
   {
      EventManager.ChangeGameState -= ChangeGameState;
      EventManager.MouseOverDot -= SolutionControl;
      EventManager.SolutionButtonClicked -= SolutionButtonClicked;
   }

   private void SolutionButtonClicked()
   {
      showSolution = true;
      SetLists();
      correctPath[pathIndex].dotImage.transform.DOScale(1.2f, .2f).SetLoops(-1, LoopType.Yoyo).SetId(pathIndex);
   }

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
         if (dot.connections.Count%2==0)
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

   void PathFinder(Dot startDot)
   {
      if (FindDotEvenNeighbour(startDot)!=null)
      {
         var nextDot = FindDotEvenNeighbour(startDot);
         Debug.Log(nextDot);
         startDot.CheckForConnection(nextDot);
         nextDot.CheckForConnection(startDot);
         correctPath.Add(nextDot);

         PathFinder(nextDot);
      }
      else if(FindDotOddNeighbour(startDot)!=null)
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
   Dot FindDotEvenNeighbour(Dot currentDot)
   {
      foreach (var conn in currentDot.connections)
      {
         if (conn.dot.connections.Count%2==0 && !conn.isFilled)
         {
            return conn.dot;
         }
      }

      return null;
   }
   Dot FindDotOddNeighbour(Dot currentDot)
   {
      foreach (var conn in currentDot.connections)
      {
         if (conn.dot.connections.Count%2!=0 && !conn.isFilled)
         {
            return conn.dot;
         }
      }

      return null;
   }
}
