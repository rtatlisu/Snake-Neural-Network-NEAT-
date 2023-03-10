using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;


public class Network
{
    
    public List<List<Node>> layers;
    public List<int> nodesPerLayer;
    public List<Node> layers1D;
    public List<Node> genomeNodeGenes;
    public List<Synapse> genomeConnectionGenes;
    [HideInInspector]
    public int currentNumOfNodes = 1;
    float E = 2.71828f;
    [HideInInspector]
    public bool[] whichMove;
    Node node;
    [HideInInspector]
    public Synapse synapse;
    [HideInInspector]
    public NNVisualizer nnInstance;
  

    //creating a new network with either no hidden nodes or hidden nodes -> default or customized
    //this will instantiate nodes to form the network and the network will be instantiated
    //by the network manager

    //idea: network constructor will be called with the number of neccessary nodes
    //if network is default: 
    //only need input nodes and output nodes, no hidden nodes and connections 
    //between nodes
    //if network is exposed to mutation or crossover:
    //need a constructor and/or methods to create a new and modified network with new/altered connections
    //and/or new nodes



    //concept for a completely fresh(default) network
    public Network(int numberOfInputNodes, List<VisionInfo> list)
    {
         NetworkManager.instance.AddNetwork(this);
        layers = new List<List<Node>>();
        genomeNodeGenes = new List<Node>();
        genomeConnectionGenes = new List<Synapse>();
        layers.Add(new List<Node>()); // first layer i.e. input layer
        layers.Add(new List<Node>()); // last layer i.e. output layer
        whichMove = null;

        //input nodes
        for(int i = 0; i < numberOfInputNodes/3; i++)
        {
            AddNode(list[i].wallDist, "Input", 0);
            AddNode(list[i].tail, "Input", 0);
            AddNode(list[i].food, "Input", 0);
         
        }
        AddNode(true, "Bias", 0);

        //xavier weight initialization
        GameManager.instance.minWeight = -1 / Mathf.Sqrt(layers[0].Count);
        GameManager.instance.maxWeight = 1 / Mathf.Sqrt(layers[0].Count);
        //output nodes
        for(int i = 0; i < 4; i++)
        {
            AddNode(0, "Output", 1);
        }
        if(GameManager.instance.startinConnections != 0.0f && !GameManager.instance.loadSaveFile)
        {
            int connectionLimit = layers[0].Count * layers[1].Count;
            int numConnections = (int)(GameManager.instance.startinConnections * connectionLimit);
            for (int i = 0; i < numConnections; i++)
            {
                List<int> output = Evolution.FindValidConnection(genomeNodeGenes);
                int In = output[0];
                int Out = output[1];
                bool connectionViable = false;
                while (!connectionViable)
                {
                    if (genomeConnectionGenes.Count > 0)
                    {


                        for (int j = 0; j < genomeConnectionGenes.Count; j++)
                        {

                            //check if the exact connection exists
                            if (genomeConnectionGenes[j].GetIn().nodeNumber == (In + 1) && genomeConnectionGenes[j].GetOut().nodeNumber == (Out + 1))
                            {
                                connectionViable = false;

                                //infnite loop here should be fixed
                                output = Evolution.FindValidConnection(genomeNodeGenes);
                                In = output[0];
                                Out = output[1];
                                break;
                            }
                            else
                            {
                                connectionViable = true;
                            }
                        }
                    }
                    else
                    {
                        connectionViable = true;
                    }
                }

                NetworkManager.instance.CreateSynapse(this, genomeNodeGenes[In], genomeNodeGenes[Out],
                    RandomWeight(GameManager.instance.minWeight,GameManager.instance.maxWeight), true);
            }
        }
        




    }
    public Network(Network input)
    {
        NetworkManager.instance.AddNetwork(this);
        layers = new List<List<Node>>();
        genomeNodeGenes = new List<Node>();
        genomeConnectionGenes = new List<Synapse>();
        layers.Add(new List<Node>()); // first layer i.e. input layer
        layers.Add(new List<Node>()); // last layer i.e. output layer
        whichMove = null;

        //input nodes
        for (int i = 0; i < input.genomeNodeGenes.Count; i++)
        {
            if (input.genomeNodeGenes[i].type.Equals("Input"))
            {
                if (input.genomeNodeGenes[i].value == -99)
                {
                    AddNode(false, "Input", 0);
                }
                else
                {
                    AddNode(0, "Input", 0);
                }
            }
            else if (input.genomeNodeGenes[i].type.Equals("Bias"))
            {
                AddNode(true, "Bias", 0);
            }
            else if(input.genomeNodeGenes[i].type.Equals("Hidden"))
            {
                AddNode(0, "Hidden", input.genomeNodeGenes[i].layer);
            }
            else if (input.genomeNodeGenes[i].type.Equals("Output"))
            {
                AddNode(0, "Output", input.genomeNodeGenes[i].layer);
            }
            
        }

       for(int i = 0; i < input.genomeConnectionGenes.Count; i++)
        {
            NetworkManager.instance.CreateSynapse(this, input.genomeConnectionGenes[i].GetIn(), input.genomeConnectionGenes[i].GetOut(),
                input.genomeConnectionGenes[i].GetWeight(), input.genomeConnectionGenes[i].GetEnabled());
        }
    }

    public Network(int numberOfInputNodes)
    {
        NetworkManager.instance.AddNetwork(this);
        layers = new List<List<Node>>();
        genomeNodeGenes = new List<Node>();
        genomeConnectionGenes = new List<Synapse>();
        layers.Add(new List<Node>()); // first layer i.e. input layer
        layers.Add(new List<Node>()); // last layer i.e. output layer
        whichMove = null;

        //input nodes
        for (int i = 0; i < numberOfInputNodes/3; i++)
        {
            AddNode(0, "Input", 0);
            AddNode(false, "Input", 0);
            AddNode(false, "Input", 0);        
        }
        AddNode(true, "Bias", 0);
        //output nodes
        for (int i = 0; i < 4; i++)
        {
            AddNode(0, "Output", 1);
        }
   
    }




     float Normalize(int min, int max, int value)
    {
        return 1f - (float)(value-min)/(max-min);
    }

    public void UpdateInputNodes(List<VisionInfo> list)
    {   
        for(int i = 0; i < list.Count; i++)
        {
            for(int j = 0; j < 3; j+=3)
            {
                layers[0][j + i*3].value = Normalize(1,/*18*/GameManager.instance.boardSize - 2,list[i].wallDist);
                layers[0][j+1 + i*3].state = list[i].tail;
                layers[0][j+2 + i*3].state = list[i].food;


            }
        }
        FeedForward();
        DetermineOutput();
    }


    public void AddNode(int value, string type, int layer)
    {
        if (layer > layers.Count - 1)
        {
            Debug.Log("lol");
        }
        /*
                if (layer == layers.Count - 1 && type.Equals("Hidden"))
                {
                    AddLayer(layers.Count - 1);
                }
        */
        int catchLoop = 0;
        while (layer >= layers.Count - 1 && type.Equals("Hidden") && catchLoop < 9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
            {
                Debug.Log("layers error");
            }
            AddLayer(layers.Count - 1);
        }
      
        node = new Node(value, type, this.currentNumOfNodes, layer, (layers[layer].Count-1));
        genomeNodeGenes.Add(node);
       if(layer >= layers.Count)
        {
            Debug.Log(layers.Count + " " + layer);
        }
       
        
        layers[layer].Add(node); //thhat threw an error somehow
        node.layerIndex = layers[layer].Count - 1;
        this.currentNumOfNodes+=1;
    }

     public void AddNode(bool state, string type, int layer)
    {
        
        if(layer == layers.Count-1 && type.Equals("Hidden"))
        {
            AddLayer(layers.Count-1);
        }
     
        node = new Node(state, type, this.currentNumOfNodes, layer, (layers[layer].Count - 1));
        genomeNodeGenes.Add(node);
        layers[layer].Add(node);
        node.layerIndex = layers[layer].Count - 1;
        this.currentNumOfNodes+=1;
    }

    public void AddLayer(int insertIndex)
    {
        //important for the visualization of the network:
        //if layers are moved, i also need to adjust that in the nnvisualizer class
         layers.Insert(insertIndex, new List<Node>());

         for(int i = insertIndex+1; i < layers.Count; i++)
         {
            for(int j = 0; j < layers[i].Count; j++)
            {
                layers[i][j].layer+=1;

            }
         }

    }
    public void ShiftNode(Node node, int fromLayer, int fromLayerIndex, int toLayer)
    {
        if (layers.Count - 1 <= toLayer)
        {
            AddLayer(toLayer);
        }

        layers[toLayer].Add(node);
        node.layer = toLayer;
        node.layerIndex = layers[toLayer].Count - 1;
        layers[fromLayer].RemoveAt(fromLayerIndex);
        for (int i = 0; i < layers[fromLayer].Count; i++)
        {
            layers[fromLayer][i].layerIndex = i;
        }
        

    }


   

   public float RandomWeight(float min, float max)
   {
        return (float)System.Math.Round((Random.Range(min,max)),2);
   }




   public void FeedForward()
   {
        //find first node that is no input node
        //find all connections in which this node is marked as out
        //having the connections, calculate the values of the In's*weights and put the sum
        //in an activation function

        float sum = 0;
     

        //going through all layers but the first
        for(int i = 1; i < layers.Count; i++)
        {
            bool sumAltered;
            //going through each node in a layer
            for(int j = 0; j < layers[i].Count; j++)
            {
                sum = 0;
                sumAltered = false;
                float biasWeight = 0f;
                //for each node, go through each connection to find the ones where this node is marked
                //as out
                for(int f = 0; f < genomeConnectionGenes.Count; f++)
                { 
                    if(genomeConnectionGenes[f].GetOut().nodeNumber == layers[i][j].nodeNumber &&
                    genomeConnectionGenes[f].GetEnabled())
                    {
                       
                        //check if node is of type bool (value=-99) or not
                        //if type==bool ->  true:sum+=weight*1, false:sum+=weight*0
                        if(genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber-1].value == -99 &&
                            !genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber-1].type.Equals("Bias"))
                        {
                            /*
                            if(genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber-1].state == true)
                            {
                                sum+=genomeConnectionGenes[f].GetWeight();
                                sumAltered=true;
                            }
                            */
                            
                            sum += genomeConnectionGenes[f].GetWeight() * (genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber - 1].state == true ? 1 : 0);
                            sumAltered = true;
                        }
                        else if(genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber - 1].value == -99 &&
                            genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber - 1].type.Equals("Bias"))
                        {
                            biasWeight = genomeConnectionGenes[f].GetWeight();
                            sumAltered = true;
                        }
                        else
                        {                    
                            sum+=genomeConnectionGenes[f].GetWeight()*
                            genomeNodeGenes[genomeConnectionGenes[f].GetIn().nodeNumber-1].value;
                            sumAltered=true;
                        }                    
                    }
                }
                if(sumAltered)
                {      
                    layers[i][j].value = Sigmoid(sum, biasWeight);
                }
                else
                {
                    layers[i][j].value = 0.0f;
                }
            }
        }
   }

   public void DetermineOutput()
   {
       
        //4 output nodes(up,down,right,left)
        //the node with the highest number will be activated
        //when this function is called, the output nodes' values are already set from the feeforward function
        // whichMove = new bool[] {false, false, false, false};
        whichMove = new bool[] { false, false, false, false};
        int curHighestIndex = 0;
        bool indexAssigned = false;
        for(int i = 0; i < layers[layers.Count-1].Count; i++)
        {
            if(layers[layers.Count-1][i].value >= layers[layers.Count-1][curHighestIndex].value && layers[layers.Count-1][i].value != 0.0f)
            {
                curHighestIndex = i;
                indexAssigned = true;               
            }
        }
        
        //order: [up, down, right, left]
        if(indexAssigned)
        {
            whichMove[curHighestIndex] = true;
        }
    
        //now the highest value among the 4 nodes is determined and we know which node it is
        //next step is to fire either up,left,right or down to the snake so it does the move

   } 

   float Sigmoid(float weightedSum, float biasWeight)
   {
        return 1/(1+Mathf.Pow(E,-(weightedSum + biasWeight)));
   }

        //overview
        //neural network is set up, but functionallity is not tested yet
        //the network manager is not really used besides a reference to some values
        //speciation was nnot yet addressed but should be possible
           //meaning, it can happen but i am not yet actively classifying networks as
           //being similar or not, so each snake with its own network can have a unique network
           //but i am not yet taking advantage of mutational advantages which could right now
           //be drowned when the next generation is created
        //evolution is not yet implemented at all, so right now we can only see a snake with input
        //and output nodes and therefore, since we have no connections, the snake wont move


        //todo
        //evolution
            //selection
            //fitness function with parameters like [fruits eaten, distance/time survived,...]
                //need to decide if i rewards fruits eaten or distance travelled more
            //crossover
            //mutation kinda done
        //visualization of the snakes networks and furthermore:
            //attached to the camera: generation number, num of species, and maybe some avg. data
            //per board: fruits eaten, fitness, steps left,..

        //problem
        //all population should be displayed at all times, but how am i dealing with speciation
        //how to do evolution if there evolves a second species but it only consists of one network 
        //if the snake dies, the brain dies with it. maybe its better to attach the brain to the snake
        //via the board or via the gamemanager

        //possible solution for 1 member species'
        //not only for one member, but several member species', if the maximum reached fitness does not
        //increase within a given nummer of generation, e.g. 15, the members of that species are not allowed
        //to reproduce
        //what does that mean for us, if we want each generation to have equally many snakes, e.g. 50 or 100
        //case 1: species is fine and allowed to reproduce
        //according to a set rule, in my case probably the x% best performing snakes, the snakes will mate
        //until the number of offspring equals the current members of the species -> easy solution
        //case 2: species is small, e.g. 2-4 members
        //should still be no problem, yet we gotta make sure that our x% best rule does not exclude
        //every snake but 1, since there is no crossover then.
        //case 3: 1 member species
        //we let the snake's offspring inherit its genes completely and rely on mutation for improvement
        //just as for many member species, if this 1 member species does not improve within a given
        //number of generations, we basically prevent them from creating offspring and therefore
        //we are eliminating them and thats where the following problem occurs
        //if a species is not allowed to reproduce, there will be less offspring than wished
        //therefore, we gotta make other species reproduce more to equalize the missing offspring
        //tracking that with a bool or a for-loop that goes on until offspring.count == current.count
        //should work


   
   

   





}
[System.Serializable]
public class Synapse
{
   // private int In;
   // private int Out;
   [SerializeField]private Node In;
    [SerializeField] private Node Out;
    [SerializeField] private float weight;
    [SerializeField] private bool enabled;
    [SerializeField] private int innovationNumber;

    public Synapse(Node In, Node Out, float weight, bool enabled, int innovationNumber)
    {
        this.In = In;
        this.Out = Out;
        this.weight = weight;
        this.enabled = enabled;
        this.innovationNumber = innovationNumber;
    }


    public void SetIn(Node node)
    {
        In = node;
    }

    public Node GetIn()
    {
        return In;
    } 


    public void SetOut(Node node)
    {
        Out = node;
    }

    public Node GetOut()
    {
        return Out;
    } 

    public void SetWeight(float value)
    {
        weight = value;
    }

    public float GetWeight()
    {
        return weight;
    } 

    public void SetEnabled(bool state)
    {
        enabled = state;
    }

    public bool GetEnabled()
    {
        return enabled;
    } 

    public void SetInnovationnumber(int value)
    {
        innovationNumber = value;
    }

    public int GetInnovationnumber()
    {
        return innovationNumber;
    } 
   
}




