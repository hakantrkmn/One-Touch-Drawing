using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    protected abstract  void SetNeighbours();
    protected abstract  void CreateLine();
    public abstract bool CheckConnections();
    public abstract void CheckForConnection(ItemBase nextDot);

    public abstract void DisconnectFromDot(ItemBase dot, bool removeLastConnection);


}
