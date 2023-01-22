using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NNVisualizer : MonoBehaviour
{
    Board boardScript;
    public GameObject node;
    public GameObject instantiateNode;
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
       // layers.Add(new List<GameObject>());
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
    

    public void vAddNode(int whichLayer, int layerIndex, string type, Vector2 node1Pos, Vector2 node2Pos)
    {/*
        if(layers.Count-1 < whichLayer)
        {
            
            AddjustLayers(whichLayer);
        }
       */
        //EXPERIMENTAL

           while(whichLayer > layers.Count-1)
           {
            layers.Add(new List<GameObject>());
           }

        //AddjustLayers(whichLayer);

        //0 1
        //whichlayer = 2
        //2 > 2-1
        //0 1 2
        //adjustlayers(2)
        //i = 2+1, i < la<ers.count=3

        //todo:
        //fix visuals
        //note: as vAddNode starts, the structual layers shhould already have the output layer be the last layer
        //gotta work on the case of adding a new layer in regard to if it makes sense to add the layer to the end or somewhere inbetween
        //board creates the nodes/layers in the order of input,hidden,output

        //hidden nodes and layers are added from left to right, so no insert operation is needed and since we always redraw thhe entire network,
        //we will never need to insert a list somewhere other than the end

        //ontop of that, this means, that we wont even need the addjustlayersmethod
       

        
        

        if (type.Equals("Input"))
        {
            
            instantiateNode = GameObject.Instantiate(node, transform.position + 
            new Vector3(whichLayer*5,-layers[whichLayer].Count*2/*-layerIndex*2*/,0), Quaternion.identity);
            instantiateNode.transform.parent = gameObject.transform;
            instantiateNode.name = "Node";
            layers[whichLayer].Add(instantiateNode);
            //layers[whichLayer][layerIndex] = instantiateNode;
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
            if(layers[whichLayer].Count != 0/*layers[whichLayer][0] != null*/)
            {
                for(int i = 0; i < layers[whichLayer].Count; i++)
                {/*
                    if (layers[whichLayer][i] != null)
                    {
                        layers[whichLayer][i].transform.position += new Vector3(0, 1, 0);
                        centerPosY = layers[whichLayer][i].transform.localPosition.y - 2;
                    }
                    */
                    layers[whichLayer][i].transform.position += new Vector3(0,1,0);
                }
                //insert the new node at the last nodes position -2;
                centerPosY = layers[whichLayer][layers[whichLayer].Count-1].transform.localPosition.y - 2;
            }
            else if(layers[layers.Count-1].Count == 0/*layers[whichLayer][0] == null*/)
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
            instantiateNode.name = "Node";
            layers[whichLayer].Add(instantiateNode);
            //layers[whichLayer][layerIndex] = instantiateNode;
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
                {/*
                    if (layers[whichLayer][i] != null)
                    {
                        layers[whichLayer][i].transform.position += new Vector3(0, 1, 0);
                        centerPosY = layers[whichLayer][i].transform.localPosition.y - 2;
                    }
                    */
                    layers[whichLayer][i].transform.position += new Vector3(0, 1, 0);
                }
                //insert the new node at the last nodes position -2;
                centerPosY = layers[whichLayer][layers[whichLayer].Count-1].transform.localPosition.y - 2;
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
            instantiateNode.name = "Node";
            layers[whichLayer].Add(instantiateNode);
            //layers[whichLayer][layerIndex] = instantiateNode;
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
                lr.startColor = Color.cyan;
                lr.endColor = Color.cyan;
             }
             else
             {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
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
        /*
        if(layers.Count == 1)
        {
            layers.Add(new List<GameObject>());
        }
        
        else
        {
            //used to be >
            while(insertIndex > layers.Count)
            {
                layers.Add(new List<GameObject>());
            }
            layers.Insert(insertIndex, new List<GameObject>());
            for(int i = insertIndex+1; i < layers.Count; i++)
            {
                for(int j = 0; j < layers[i].Count; j++)
                {
                    layers[i][j].transform.localPosition += Vector3.right*5;
                }
            }
            
        }
        */
        
        
            layers.Insert(insertIndex, new List<GameObject>());
            for (int i = insertIndex + 1; i < layers.Count; i++)
            {
                for (int j = 0; j < layers[i].Count; j++)
                {
                    layers[i][j].transform.localPosition += Vector3.right * 5;
                }
            }
        
        
    }

    


}



