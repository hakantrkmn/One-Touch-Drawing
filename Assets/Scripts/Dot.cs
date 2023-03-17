using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    GameStates gameState;
    public List<Connection> connections;
    public GameObject linePrefab;
    public List<Dot> connectedDots;
    public Image dotImage;
    private void Start()
    {
        SetNeighbours();
    }


    public bool SearchDotOnConnections(Dot dot)
    {
        foreach (var conn in connections)
        {
            if (conn.dot == dot)
            {
                return true;
            }
        }

        return false;
    }
    void SetNeighbours()
    {
        foreach (var connection in connections)
        {
            var goNext = false;
            foreach (var conn in connection.dot.connections)
            {
                if (conn.dot == this)
                {
                    goNext = true;
                    break;
                }
            }

            if (goNext)
            {
                continue;
            }

            var tempConn = new Connection();
            tempConn.dot = this;
            connection.dot.connections.Add(tempConn);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].dot != this && connections[i].dot != null)
            {
                if (i < transform.GetChild(1).childCount)
                {
                    if (transform.GetChild(1).GetChild(i).GetComponent<LineRenderer>())
                    {
                        var line = transform.GetChild(1).GetChild(i).GetComponent<LineRenderer>();
                        line.transform.localPosition = Vector3.zero;
                        var firstPos = GetComponent<RectTransform>().position;
                        var secondPos = connections[i].dot.GetComponent<RectTransform>().position;
                        firstPos.z = 0;
                        secondPos.z = 0;
                        line.SetPosition(0, firstPos);
                        line.SetPosition(1, secondPos);
                    }
                }
                else
                {
                    var line = Instantiate(linePrefab, transform.position, quaternion.identity, transform.GetChild(1))
                        .GetComponent<LineRenderer>();
                    line.transform.localPosition = Vector3.zero;
                    var firstPos = GetComponent<RectTransform>().position;
                    var secondPos = connections[i].dot.GetComponent<RectTransform>().position;
                    firstPos.z = 0;
                    secondPos.z = 0;
                    line.SetPosition(0, firstPos);
                    line.SetPosition(1, secondPos);
                }
            }
            else
            {
                //connections.RemoveAt(i);
            }
        }
    }

    public bool CheckConnections()
    {
        foreach (var connection in connections)
        {
            if (!connection.isFilled)
            {
                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        EventManager.Reset += Reset;
        EventManager.ChangeGameState += ChangeGameState;
    }

    public void Reset()
    {
        foreach (var connection in connections)
        {
            connection.isFilled = false;
        }

        connectedDots.Clear();
    }

    private void OnDisable()
    {
        EventManager.Reset -= Reset;
        EventManager.ChangeGameState -= ChangeGameState;
    }

    private void ChangeGameState(GameStates state)
    {
        gameState = state;
    }

    public void CheckForConnection(Dot nextDot)
    {
        foreach (var con in connections)
        {
            if (con.dot == nextDot)
            {
                con.isFilled = true;
            }
        }
    }
    

    public void DisconnectFromDot(Dot dot, bool removeLastConnection)
    {
        foreach (var con in connections)
        {
            if (con.dot == dot)
            {
                con.isFilled = false;
            }
        }

        if (removeLastConnection)
        {
            connectedDots.Remove(connectedDots.Last());
        }
    }

    private void OnMouseDown()
    {
        Clicked();
    }

    public void Clicked()
    {
        EventManager.StartDrawing(this, GetComponent<RectTransform>().position);
    }

    private void OnMouseOver()
    {
        if (gameState == GameStates.Drawing)
        {
            EventManager.MouseOverDot(this, GetComponent<RectTransform>().position);
        }
    }
}