using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;
using System;


public class Board : MonoBehaviour
{
    public int boardNumber;
    public GameObject boarderTiles;
    public GameObject fruitPrefab;
    public GameObject fruitInstance;
    public Fruit fruitScript = null;
    public GameObject snakePrefab;
    public GameObject snakeInstance;
    public Snake snakeScript = null;
    public  static int boarderSizeX = 20;
    Vector2 boardVector;
    public bool gameRunning;
    public bool gameOver;
     GameObject northWall;
     GameObject eastWall;
     GameObject southWall;
     GameObject westWall;
    public  GameObject nnVisualizer;
    public GameObject nnvInstance;
    bool newGen;
    TextMeshProUGUI fitnessRef, movesLeftRef, distanceRef, fruitsRef, speciesRef;
    public int fitness = 0;
     public int fruitsEaten = 0;
     public int distTravelled = 0;
     public int movesLeft = 50;
     public int species = 0;
     public Network childBrain = null;
    public bool start_drawing_nn = false;
    double distance = 0;
    int moveToFruitCounter = 0;
     
    

    
   void Awake()
   {   
    speciesRef = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    fitnessRef = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
    fruitsRef = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
    distanceRef = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
    movesLeftRef = transform.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>();
    boardNumber = GameManager.instance.boardNumber;
   }

   
    void Start()
    { 
   
       
        nnvInstance = Instantiate(nnVisualizer,Vector3.zero,Quaternion.identity);
        nnvInstance.transform.parent = this.gameObject.transform;
        nnvInstance.name = "NNVisualizer";
        

        northWall = new GameObject("NorthWall");
        northWall.transform.parent = this.gameObject.transform;
        northWall.transform.position = new Vector2((float)(Board.boarderSizeX-1)/2,Board.boarderSizeX-1);
        eastWall = new GameObject("EastWall");
        eastWall.transform.parent = this.gameObject.transform;
         eastWall.transform.position = new Vector2(Board.boarderSizeX-1,(float)(Board.boarderSizeX-1)/2);
        southWall = new GameObject("SouthWall");
        southWall.transform.parent = this.gameObject.transform;
         southWall.transform.position = new Vector2((float)(Board.boarderSizeX-1)/2,0);
        westWall = new GameObject("WestWall");
        westWall.transform.parent = this.gameObject.transform;
         westWall.transform.position = new Vector2(0,(float)(Board.boarderSizeX-1)/2);
       
         boardVector = this.gameObject.transform.position;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < boarderSizeX; j++)
            {
                if(i==0)
                {
                    Instantiate(boarderTiles, boardVector, Quaternion.identity).transform.parent=
                    southWall.transform;
                    if(j < boarderSizeX-1)
                    {
                        boardVector+=Vector2.right;
                    } 
                }
                else if(i==1)
                {
                    Instantiate(boarderTiles, boardVector, Quaternion.identity).transform.parent=
                    eastWall.transform;
                    if(j < boarderSizeX-1)
                    {
                        boardVector+=Vector2.up;
                    }
                }
                else if(i==2)
                {
                    Instantiate(boarderTiles, boardVector, Quaternion.identity).transform.parent=
                    northWall.transform;
                    if(j < boarderSizeX-1)
                    {
                        boardVector+=Vector2.left;
                    }        
                }
                else if(i==3)
                {
                    Instantiate(boarderTiles, boardVector, Quaternion.identity).transform.parent=
                    westWall.transform;
                    if(j < boarderSizeX-1)
                    {
                        boardVector+=Vector2.down;
                    }  
                }
            }
        }
 

        GameManager.instance.setCurBoard();
    }

    // Update is called once per frame
    void Update()
    {
        speciesRef.text = "Species: " + species;
        fitnessRef.text = "Fitness: "+fitness;
        fruitsRef.text = "Fruits: " + fruitsEaten;
        distanceRef.text = "Distance: " + distTravelled;
        movesLeftRef.text = "Moves left: " + movesLeft;

        if(movesLeft <= 0)
        {
            gameRunning = false;
            gameOver = true;
        }

      
        if (snakeScript == null)
        {
             snakeInstance = Instantiate(snakePrefab, Vector2.zero, Quaternion.identity);
             snakeInstance.name = "Snake";
             snakeInstance.transform.parent = this.transform;
             snakeScript = snakeInstance.GetComponent<Snake>();
             gameOver = false;
             movesLeft = 100;
        
        }


        
          if(snakeScript.brain != null && /*!newGen*/ start_drawing_nn || snakeScript.brain != null && GameManager.instance.gen == 1 && !newGen)
            {
                start_drawing_nn = false;

            /*
                for(int i = 0; i < snakeScript.brain.layers.Count; i++)
                {
                    nnvInstance.GetComponent<NNVisualizer>().layers.Add(new List<GameObject>());
                    for(int j = 0; j < snakeScript.brain.layers[i].Count; j++)
                    {
                        nnvInstance.GetComponent<NNVisualizer>().layers[i].Add(null);
                    }
                }
            */
                NNVisualizer nnvScript = nnvInstance.GetComponent<NNVisualizer>();
                newGen = true;
                //draw nodes
                //input
                for(int i = 0; i < snakeScript.brain.layers[0].Count; i++)
                {
                    nnvInstance.GetComponent<NNVisualizer>().vAddNode(
                        /*snakeScript.brain.layers[0][i].layer*/0, i, "Input" ,Vector2.zero, Vector2.zero);
                    nnvScript.instantiateNode.GetComponent<vNode>().state = snakeScript.brain.layers[0][i].state;
                    nnvScript.instantiateNode.GetComponent<vNode>().value = snakeScript.brain.layers[0][i].value;
                    nnvScript.instantiateNode.GetComponent<vNode>().type = snakeScript.brain.layers[0][i].type;
                    nnvScript.instantiateNode.GetComponent<vNode>().nodeNumber = snakeScript.brain.layers[0][i].nodeNumber;
                    nnvScript.instantiateNode.GetComponent<vNode>().layer = snakeScript.brain.layers[0][i].layer;
                    nnvScript.instantiateNode.GetComponent<vNode>().layerIndex = snakeScript.brain.layers[0][i].layerIndex;
                }
                //Hidden
                if(snakeScript.brain.layers.Count > 2)
                {
                    //-2 excludes input and output layer
                    for(int i = 0; i < snakeScript.brain.layers.Count - 2; i++)
                    {
                    //i = 0; i < 8 - 2 = 6
                    //j = 0; j < layer[1].count = 3
                    //layer[1][0].layer...layer[1][2]
                        //i+1 -> starting at the first hidden layer 
                        for(int j = 0; j < snakeScript.brain.layers[i+1].Count; j++)
                        {
                            nnvInstance.GetComponent<NNVisualizer>().vAddNode(
                           /* snakeScript.brain.layers[i+1][j].layer*/(i+1),j, "Hidden" ,Vector2.zero, Vector2.zero);
                            nnvScript.instantiateNode.GetComponent<vNode>().state = snakeScript.brain.layers[i + 1][j].state;
                            nnvScript.instantiateNode.GetComponent<vNode>().value = snakeScript.brain.layers[i + 1][j].value;
                            nnvScript.instantiateNode.GetComponent<vNode>().type = snakeScript.brain.layers[i + 1][j].type;
                            nnvScript.instantiateNode.GetComponent<vNode>().nodeNumber = snakeScript.brain.layers[i + 1][j].nodeNumber;
                            nnvScript.instantiateNode.GetComponent<vNode>().layer = snakeScript.brain.layers[i + 1][j].layer;
                            nnvScript.instantiateNode.GetComponent<vNode>().layerIndex = snakeScript.brain.layers[i + 1][j].layerIndex;
                            //[1][x] -> [0,hidden1,2]
                            //[2][x] -> [0,hidden1,hidden2,2]
                        }
                    }
                }
                //output
                for(int i = 0; i < snakeScript.brain.layers[snakeScript.brain.layers.Count-1].Count; i++)
                {
                    nnvInstance.GetComponent<NNVisualizer>().vAddNode(
                        snakeScript.brain.layers[snakeScript.brain.layers.Count-1][i].layer,i, "Output",Vector2.zero, Vector2.zero);
                    nnvScript.instantiateNode.GetComponent<vNode>().state = snakeScript.brain.layers[snakeScript.brain.layers.Count - 1][i].state;
                    nnvScript.instantiateNode.GetComponent<vNode>().value = snakeScript.brain.layers[snakeScript.brain.layers.Count - 1][i].value;
                    nnvScript.instantiateNode.GetComponent<vNode>().type = snakeScript.brain.layers[snakeScript.brain.layers.Count - 1][i].type;
                    nnvScript.instantiateNode.GetComponent<vNode>().nodeNumber = snakeScript.brain.layers[snakeScript.brain.layers.Count - 1][i].nodeNumber;
                    nnvScript.instantiateNode.GetComponent<vNode>().layer = snakeScript.brain.layers[snakeScript.brain.layers.Count - 1][i].layer;
                    nnvScript.instantiateNode.GetComponent<vNode>().layerIndex = snakeScript.brain.layers[snakeScript.brain.layers.Count - 1][i].layerIndex;
                }


            //when drawing the net, the net already exists.
            //meaning, with the shifts of the last layer, there will be an ordered list
            //with 1 hidden layer it will be [input, hidden, output].
            //the shhift that only happens in a list now must happen visually
            //if input and output are drawn, we can start from the last hidden layer
            //and always insert at index 1, moving all other nodes to the right

            //hidden
            //need the two nodes to place the new node inbetween
            //additionally: remove the existng connection between A and B, place node C
            //on position (A+B)/2, connect A with C and C with B

            //if no connections exit, set gameover true and gamerunning false, since the snake wont move and the game wouldnt end for it
            //IMPORTANT: if all connections are disabled, the snake wouldnt move as well
            //if snake cant move because the output doesnt suit the requirements, game needs to end as well (snake should move up
            //but is facing down)


     
                //draw connections
               for(int i = 0; i < snakeScript.brain.genomeConnectionGenes.Count; i++)
               {    
                    NNVisualizer nnv = nnvInstance.GetComponent<NNVisualizer>();
                    List<Synapse> connections = snakeScript.brain.genomeConnectionGenes;
                    List<Node> nodes = snakeScript.brain.genomeNodeGenes;
                /*
                    for(int j = 0; j < connections.Count; j++)
                    {
                         connections[j].SetIn(nodes[connections[j].GetIn().nodeNumber - 1]);
                         connections[j].SetOut(nodes[connections[j].GetOut().nodeNumber - 1]);

                    }
                   
                
                    Vector2 start = nnv.layers[connections[i].GetIn().layer][connections[i].GetIn().layerIndex].transform.position;

                    Debug.Log("problem: " + nnv.layers.Count + " " + connections[i].GetOut().layer + " " +
                        connections[i].GetOut().layerIndex + " nodeOut: " + connections[i].GetOut().nodeNumber + " board: " + boardNumber);
               
                    if(nnv.layers.Count <= connections[i].GetOut().layer)
                    {
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        Debug.Log("i hate it: " + nodes[j].nodeNumber + " " + nodes[j].layer);
                    }
                }
                    Vector2 end = nnv.layers[connections[i].GetOut().layer][connections[i].GetOut().layerIndex].transform.position;
                    int nodeIn = connections[i].GetIn().nodeNumber;
                    int nodeOut = connections[i].GetOut().nodeNumber;
                    int innoNum = connections[i].GetInnovationnumber();
                    float weight = connections[i].GetWeight();
                    bool enabled = connections[i].GetEnabled();

                    nnv.vAddConnection(start,end,nodeIn,nodeOut, innoNum, weight, enabled);
                */
                    
                            int InIndex=0;
                            int OutIndex=0;
                            int InLayer=0;
                            int OutLayer=0;
                             for(int j = 0; j < snakeScript.brain.layers.Count; j++)
                            {
                                for(int f = 0; f < snakeScript.brain.layers[j].Count; f++)
                                {
                                    if(snakeScript.brain.layers[j][f].nodeNumber ==
                                    connections[i].GetOut().nodeNumber)
                                    {
                                        OutLayer = j;
                                        OutIndex = f;
                                        break;
                                    }
                                    else if(snakeScript.brain.layers[j][f].nodeNumber ==
                                    connections[i].GetIn().nodeNumber)
                                    {
                                        InLayer = j;
                                        InIndex = f;
                                        break;

                                    }
                                }
                            }
                            int innoNum=-1;
                            float weight = -100f;
                            bool enabled = false;
                            for(int b = 0; b < connections.Count; b++)
                            {
                                if(connections[i].GetIn().nodeNumber == snakeScript.brain.layers[InLayer][InIndex].nodeNumber &&
                                connections[i].GetOut().nodeNumber == snakeScript.brain.layers[OutLayer][OutIndex].nodeNumber)
                                {
                                    innoNum =snakeScript.brain.genomeConnectionGenes[i].GetInnovationnumber();
                                    weight = snakeScript.brain.genomeConnectionGenes[i].GetWeight();
                                    enabled = snakeScript.brain.genomeConnectionGenes[i].GetEnabled();
                                    break;
                                }
                            }

                            nnv.vAddConnection(nnv.layers[InLayer][InIndex].transform.position, 
                            nnv.layers[OutLayer][OutIndex].transform.position,
                            snakeScript.brain.layers[InLayer][InIndex].nodeNumber,
                            snakeScript.brain.layers[OutLayer][OutIndex].nodeNumber,innoNum, weight, enabled);

                            
               }
                
          }
             
       //todo
       //make the nnvisualizer work for new layers being added
        if(fruitScript == null && snakeScript.snakeComposites.Count != 0)
        {
            fruitInstance = Instantiate(fruitPrefab,Vector2.zero, Quaternion.identity);
            fruitInstance.name = "Fruit";
            fruitInstance.transform.parent = this.gameObject.transform;
            fruitScript = fruitInstance.GetComponent<Fruit>();
            fruitInstance.transform.position = fruitScript.GetComponent<Fruit>().
            spawnLocation(snakeScript.snakeComposites, snakeScript.whichSnakeAmI);
         
        }
        //alternative:
        //make the snake call a function with thhe following if-code at the correct point
        //main idea of this: make the board attach the brain to the snake and not let the
        //snake attach its brain to itself
        //give snake brain and vision
        //if gen 1 -> default creation, else -> attach crossed brain
  
        if(snakeScript.brain == null && snakeScript.snakeComposites != null &&
        snakeScript.snakeComposites.Count > 0 && GameManager.instance.gen == 1) 
        {
            snakeScript.Vision(snakeScript.snakeComposites[0].transform.position);
            
            snakeScript.inputs = new List<VisionInfo>{snakeScript.north, snakeScript.east, 
            snakeScript.south, snakeScript.west};

            snakeScript.brain = new Network(12, snakeScript.inputs);
            snakeScript.brain.UpdateInputNodes(snakeScript.inputs);       
        }

        //thhis is for all children 
          
        else if(snakeScript.brain == null && snakeScript.snakeComposites != null &&
        snakeScript.snakeComposites.Count > 0 && GameManager.instance.gen > 1 && childBrain != null)
        {
            snakeScript.Vision(snakeScript.snakeComposites[0].transform.position);
            
            snakeScript.inputs = new List<VisionInfo>{snakeScript.north, snakeScript.east, 
            snakeScript.south, snakeScript.west};

            snakeScript.brain =  childBrain;
            snakeScript.brain.UpdateInputNodes(snakeScript.inputs);

        } 

        if(snakeScript.snakeComposites.Count != 0 )
        {
            OnFoodPickUp();
        }   
    }

   public void HandleCollision()
    { 
       gameRunning = false;
       gameOver = true;
    }

    void OnFoodPickUp()
    {          
        if((Vector2)snakeScript.snakeComposites[0].transform.position == fruitScript.fruitSpot)
        {
            Destroy(fruitInstance);
            fruitScript = null;
            snakeScript.snakeComposites.Add(Instantiate(snakeScript.singeTail, 
            snakeScript.formerPosition[snakeScript.formerPosition.Count-1],Quaternion.identity));

            snakeScript.formerPosition.Add(snakeScript.snakeComposites[snakeScript.snakeComposites.Count-1].transform.position);

            snakeScript.snakeComposites[snakeScript.snakeComposites.Count-1].transform.parent 
            = snakeScript.gameObject.transform;

            fruitsEaten++;
            movesLeft = 100;
        }
    }


    public void CalcFitness()
    {
        /*fitness = (int) (((fruitsEaten*5) + distTravelled) - ((1/(2+fruitsEaten/distTravelled))*
         ((fruitsEaten*5) + distTravelled)));
        */
        /* fitness = (int)(((fruitsEaten * 15) + distTravelled) - ((1 / (2 + fruitsEaten / distTravelled)) *
          ((fruitsEaten * 15) + distTravelled)));
        */
        Vector2 fruitPos = fruitInstance.transform.position;
        Vector2 snakePos = snakeInstance.GetComponent<Snake>().snakeComposites[0].transform.position;
        
        if (Math.Sqrt((Math.Pow((fruitPos.x - snakePos.x), 2) + Math.Pow((fruitPos.y - snakePos.y), 2))) > distance && distance != 0)
        {
            moveToFruitCounter -= 2;
          
        }
        else
        {
            moveToFruitCounter += 1;
            
        }
      
        distance = Math.Sqrt((Math.Pow((fruitPos.x - snakePos.x), 2) + Math.Pow((fruitPos.y - snakePos.y), 2)));

        //fitness = (int) (fruitsEaten * 15 + distTravelled - ((1 / (1 + fruitsEaten)) * (distTravelled * 0.5))) + moveToFruitCounter;
        fitness = moveToFruitCounter + fruitsEaten*10;

        if(fitness < 0)
        {
            fitness = 0;
        }

    }


    public void ColorizeNetwork()
    {
        NNVisualizer nnvScript = nnvInstance.GetComponent<NNVisualizer>();
        //litting up bool input nodes
        for(int i = 0; i < snakeScript.brain.layers[0].Count; i++)
        {
            if(snakeScript.brain.layers[0][i].value == -99)
            {
                if(snakeScript.brain.layers[0][i].state == true)
                {
                    nnvScript.layers[0][i].
                    GetComponent<SpriteRenderer>().color = Color.green;
                }
                else
                {
                    nnvScript.layers[0][i].
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }

        //if snake made a move that changed its walldist, let the node having the corresponding
        //walldist value lit up in green
        for(int i = 0; i < snakeScript.visionList[0].Count; i++)
        {
                if(snakeScript.visionList[0] != null && snakeScript.visionList[1] != null && 
                snakeScript.visionList[0][i].wallDist != snakeScript.visionList[1][i].wallDist)
            {
                //0=input layer, i*3 = every third vision is a walldist
                nnvScript.layers[0][i*3].
                GetComponent<SpriteRenderer>().color = Color.green;

            }
            else
            {
                nnvScript.layers[0][i*3].
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        //output layer: the node being active is green, else white
        for(int i = 0; i < snakeScript.brain.layers[snakeScript.brain.layers.Count-1].Count; i++)
        {
            if(snakeScript.brain.whichMove[i])
            {
                nnvScript.layers[nnvScript.layers.Count-1][i].
                GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                nnvScript.layers[nnvScript.layers.Count-1][i].
                GetComponent<SpriteRenderer>().color = Color.white;

            }
        }


        //connections: white -> enabled, grey -> disabled         
        for(int i = 0; i < nnvScript.vConnections.Count; i++)
        {
            if(nnvScript.vConnections[i].GetComponent<vConnection>().enabled)
            {
                nnvScript.vConnections[i].GetComponent<LineRenderer>().startColor = Color.cyan;
                nnvScript.vConnections[i].GetComponent<LineRenderer>().endColor = Color.cyan;
            }
            else
            {
                nnvScript.vConnections[i].GetComponent<LineRenderer>().startColor = Color.red;
                nnvScript.vConnections[i].GetComponent<LineRenderer>().endColor = Color.red;
            }
        }
    }
}



