using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNVisualizer : MonoBehaviour
{
    Board boardScript;
    public GameObject node;
    GameObject instantiateNode;
    int numOfNodes = 0;
    bool createNode;
    public List<List<GameObject>> layers;
    int numOfConnections = 1;
    int centerNode1;
    int centerNode2;
    float centerPosY;
    public List<GameObject> vConnections;


    // Start is called before the first frame update
    void Start()
    {
        boardScript = transform.parent.gameObject.GetComponent<Board>();
        transform.position = transform.parent.position + new Vector3(Board.boarderSizeX-1,Board.boarderSizeX-1,0)
        + Vector3.right*2;
        layers = new List<List<GameObject>>();
        layers.Add(new List<GameObject>());
        vConnections = new List<GameObject>();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //major idea:
        //a network doesnt change within a generation.
        //if a network doesnt undergo mutation, it will remain the same in the next gen.
        //hence, instead of manually drawing the network node for node and connection for connection,
        //i might just safe the visual representation of the network somehow and just "paste" it in
        
    }
    

    public void vAddNode(int whichLayer, string type, Vector2 node1Pos, Vector2 node2Pos)
    {
        if(layers.Count-1 < whichLayer)
        {
            
            AddjustLayers(whichLayer);
        }
      

        if(type.Equals("Input"))
        {
            
            instantiateNode = GameObject.Instantiate(node, transform.position + 
            new Vector3(whichLayer*5,-layers[whichLayer].Count*2,0), Quaternion.identity);
            instantiateNode.transform.parent = gameObject.transform;
            layers[whichLayer].Add(instantiateNode);
            numOfNodes++;
            if(numOfNodes == 12)
            {
                if(layers[0].Count%2 == 0)
                {
                    centerNode1 = layers[0].Count/2;
                    centerNode2 = (layers[0].Count/2) - 1;
                    centerPosY =  (layers[0][centerNode1].transform.localPosition.y + 
                                    layers[0][centerNode2].transform.localPosition.y)/2;
                                
                }
                else
                {
                    centerNode1 = (layers[0].Count-1)/2;
                    centerNode2 = 0;
                    centerPosY = layers[0][centerNode1].transform.localPosition.y;
                }
                
            }
            
        }
       
        else if(type.Equals("Output"))
        {
            //if an output node exist -> move all existing nodes up by 1
            //layers.count-1 could be replaced by whichLayer
            if(layers[layers.Count-1].Count != 0)
            {
                for(int i = 0; i < layers[layers.Count-1].Count; i++)
                {
                    layers[layers.Count-1][i].transform.position += new Vector3(0,1,0);
                }
                //insert the new node at the last nodes position -2;
                centerPosY = layers[layers.Count-1][layers[layers.Count-1].Count-1].
                transform.localPosition.y - 2;
            }
            else if(layers[layers.Count-1].Count == 0)
            {
                if(layers[0].Count%2 == 0)
                {
                    centerNode1 = layers[0].Count/2;
                    centerNode2 = (layers[0].Count/2) - 1;
                    centerPosY =  (layers[0][centerNode1].transform.localPosition.y + 
                                    layers[0][centerNode2].transform.localPosition.y)/2;
                                
                }
                else
                {
                    centerNode1 = (layers[0].Count-1)/2;
                    centerNode2 = 0;
                    centerPosY = layers[0][centerNode1].transform.localPosition.y;
                }

            }
            
             instantiateNode = GameObject.Instantiate(node, transform.position + 
            new Vector3(whichLayer*5,centerPosY,0), Quaternion.identity);

            instantiateNode.transform.parent = gameObject.transform;
            layers[whichLayer].Add(instantiateNode);
            numOfNodes++;

        }
        else if(type.Equals("Hidden"))
        {
            //needs to be inserted in the middle of the former connection
            //no need for readjustment of other nodes in the same layer should be neccessary
            //BUT!
            //when we create a new layer, a hidden layer, we need to adjust the x values of all layers
            //to the right of the new layer, could already work though.
            //plan for now:
            //add node1 x pos and node2 x pos as well as their y pos's and divide by 2
            //need to take care of rounding due to odd numbers though, or not?
            if(layers[whichLayer].Count != 0)
            {
                for(int i = 0; i < layers[whichLayer].Count; i++)
                {
                    layers[whichLayer][i].transform.position += new Vector3(0,1,0);
                }
                //insert the new node at the last nodes position -2;
                centerPosY = layers[whichLayer][layers[whichLayer].Count-1].
                transform.localPosition.y - 2;
            }
            else if(layers[whichLayer].Count == 0)
            {
                if(layers[0].Count%2 == 0)
                {
                    centerNode1 = layers[0].Count/2;
                    centerNode2 = (layers[0].Count/2) - 1;
                    centerPosY =  (layers[0][centerNode1].transform.localPosition.y + 
                                    layers[0][centerNode2].transform.localPosition.y)/2;
                                
                }
                else
                {
                    centerNode1 = (layers[0].Count-1)/2;
                    centerNode2 = 0;
                    centerPosY = layers[0][centerNode1].transform.localPosition.y;
                }

            }
            

              instantiateNode = GameObject.Instantiate(node, transform.position + 
            new Vector3(whichLayer*5,centerPosY,0), Quaternion.identity);

            instantiateNode.transform.parent = gameObject.transform;
            layers[whichLayer].Add(instantiateNode);
            numOfNodes++;
            //probably not working as of now
        }
        
    }

    public void vAddConnection(Vector2 start, Vector2 end, int nodeIn, int nodeOut, int innoNum, float weight, bool enabled)
    {

            GameObject myLine = new GameObject();
            myLine.AddComponent<vConnection>();
            myLine.GetComponent<vConnection>().nodeIn = nodeIn;
            myLine.GetComponent<vConnection>().nodeOut = nodeOut;
            myLine.GetComponent<vConnection>().innovationNumber = innoNum;
            myLine.GetComponent<vConnection>().weight = weight;
            myLine.GetComponent<vConnection>().enabled = enabled;
            myLine.transform.parent = this.gameObject.transform;
            myLine.name = "Connection " + numOfConnections;
            numOfConnections++;
             myLine.transform.position = start;
             myLine.AddComponent<LineRenderer>();
             LineRenderer lr = myLine.GetComponent<LineRenderer>();
             lr.material = new Material(Shader.Find("Sprites/Default"));
             if(enabled)
             {
                lr.startColor = Color.white;
                lr.endColor = Color.white;
             }
             else
             {
                lr.startColor = Color.gray;
                lr.endColor = Color.gray;
             }
             
             lr.startWidth = 0.1f;
             lr.endWidth = 0.1f;
             lr.SetPosition(0, start);
             lr.SetPosition(1, end);
            vConnections.Add(myLine);
             
           //  GameObject.Destroy(myLine, duration);
    }

    public void AddjustLayers(int insertIndex)
    {
        if(layers.Count == 1)
        {
            layers.Add(new List<GameObject>());
        }
        else
        {
            layers.Insert(insertIndex, new List<GameObject>());
            for(int i = insertIndex+1; i < layers.Count; i++)
            {
                for(int j = 0; j < layers[i].Count; j++)
                {
                    layers[i][j].transform.localPosition += Vector3.right*5;
                }
            }
            
        }    
    }

    


}



