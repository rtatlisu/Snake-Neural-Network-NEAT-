using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public float value = -99;
    public string type; //input, output, hidden
     public int nodeNumber;
     public bool state;
     public int layer;
    public int layerIndex;
    public Node(int value, string type, int nodeNumber, int layer, int layerIndex)
    {
        this.value = value;
        this.type = type;
        this.nodeNumber = nodeNumber;
        this.layer = layer;
        this.layerIndex = layerIndex;
    }

    public Node(bool state, string type, int nodeNumber, int layer, int layerIndex)
    {
        this.state = state;
        this.type = type;
        this.nodeNumber = nodeNumber;
        this.layer = layer;
        this.layerIndex = layerIndex;
    }


}
