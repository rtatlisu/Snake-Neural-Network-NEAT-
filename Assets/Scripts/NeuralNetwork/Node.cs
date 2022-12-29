using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public float value = -99;
    public string type; //input, output, hidden
     public int nodeNumber;
     public bool state;
     public int layer;
    public Node(int value, string type, int nodeNumber, int layer)
    {
        this.value = value;
        this.type = type;
        this.nodeNumber = nodeNumber;
        this.layer = layer;
    }

    public Node(bool state, string type, int nodeNumber, int layer)
    {
        this.state = state;
        this.type = type;
        this.nodeNumber = nodeNumber;
        this.layer = layer;
    }


}
