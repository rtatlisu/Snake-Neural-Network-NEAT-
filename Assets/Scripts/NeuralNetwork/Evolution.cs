using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking.Types;

public static class Evolution
{
    //todo:
    //selection
        //determine a rule to make individuals mate with eachother
            //idea now: all individuals have a chance to mate, but their chance to mate is based
            //on their fitness in the current run
            //so for each species we will have a pool of individuals and 2 non-identical individuals
            //will mate until the species' population is equal to the offspring population
            //if species contains only one individual, this individual will pass on its own genes to
            //one offspring, meaning improvement can only occur with mutation
        //need a fitness function with good paramterers and weights on those paramters to reward or punish
        //certain behavior
        
    //crossover
        //the way of crossover is specified by neat
        //crossover will affect the node and connection list
    //mutation
        //mutations are already written, but not tested
        //we have weight mutation, add node mutation and add connection mutation

    
   public static List<GameObject> Selection(List<GameObject> snakes) //needed paramters: entire species population
   {
    //idea: chance to mate = snake.fitness / highestFitness
    //but if a snake in this gen has the highhest fitness, it would always be selected for mating
    //maybe i can modify the sigmoid function to give outputs between 0.00 and 0.90

        


        //sum fitness of all snakes
        int sumOfFitness = 0;
        if(snakes.Count > 1)
        {
            for(int i = 0; i < snakes.Count; i++)
            {
                sumOfFitness+= snakes[i].GetComponent<Snake>().boardScript.fitness;
            }
        }
        else
        {
            //sometimes gives an argument out of range exception
            if(snakes.Count-1 < 0)
            {
                Debug.Log("lol");
            }
            sumOfFitness = snakes[0].GetComponent<Snake>().boardScript.fitness;
        }
        List<GameObject> orderedFitness = new List<GameObject>();
       
        if(snakes.Count > 1)
        {
            for(int i = 0; i < snakes.Count; i++)
            {
                orderedFitness.Add(snakes[i]);
            }
        }
        else
        {
            orderedFitness.Add(snakes[0]);
        }
        if(orderedFitness.Count > 1)
        {
          orderedFitness = SelectionSort(orderedFitness, "fitness"); 
        }
       
        
        //accumulate probabilities
        double previousProb = 0.0d;
        if(orderedFitness.Count > 1)
        {
            
            for(int i = 0; i < orderedFitness.Count; i++)
            {
                if (sumOfFitness == 0)
                {
                    orderedFitness[i].GetComponent<Snake>().probability = 0;
                }
                else
                {
                    double relativeProb = (double)orderedFitness[i].GetComponent<Snake>().boardScript.fitness / sumOfFitness;
                    orderedFitness[i].GetComponent<Snake>().probability = previousProb + relativeProb;
                    previousProb = orderedFitness[i].GetComponent<Snake>().probability;

                }
               
            }
        }
        else
        {
            orderedFitness[0].GetComponent<Snake>().probability = 1;
        }
       

        //this process gotta happen 2*snakePop times, 2 because 2 parents
        //rnd will be random floats with 2 decimal places, e.g. 0.32, 0.6, 0.99,...
        List<GameObject> parents = new List<GameObject>();
       
        if(orderedFitness.Count > 1)
        {
            while(parents.Count < 2)
            {

                float rnd = (float) System.Math.Round(Random.Range(0.0f,1.0f),2);
                int x = 0;
                int winnerIndex = 0;

           
                   
                
                while ((x+1) < orderedFitness.Count && rnd > orderedFitness[x].GetComponent<Snake>().probability)
                {
                    x++;
        
                }    
                
                        
                    
                
               
                winnerIndex = x;

                //could be changed, but for now: one snake cant breed with itself  
                parents.Add(orderedFitness[x]);
                orderedFitness.RemoveAt(x);
            }
        }
        else
        {
            parents.Add(orderedFitness[0]);
        }
        
        //one call of selection returns a list of 2 snakes, parent 1 and 2

        //further important thinngs:
        //selection gotta work for 1 snake, therefore reproducing with one parent gotta work
        //is it problematic if 2 parents are the same snake?
            
    return parents;
   }


    //maybe crossover could bbe reworked considering efficiency and length
   public static Network Crossover(List<GameObject> parents) //needed parameters: taking in 2 snakes 
   {
        Network childBrain = new Network(12);
    if (parents.Count > 1)
    {
            int parent1Size = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes.Count;
            int parent2Size = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes.Count;
            List<Synapse> p1Connections = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes;
            List<Synapse> p2Connections = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes;
           


            List<Synapse> parent1Genes = new List<Synapse>();
        List<Synapse> parent2Genes = new List<Synapse>();
        
        bool matchingFound = false;
        int matchingIndexP1 = -1;
        int matchingIndexP2 = -1;

        //parent 1 genome first
        //fill the child's genomeConnectionList with based on the connections the parents share
        for(int i = 0; i < parent1Size; i++)
        {
            matchingFound = false;
            
            matchingIndexP1 = -1;
            matchingIndexP2 = -1;
            for(int j = 0; j < parent2Size; j++)
            {
                    //both hhave the gene, 50/50
                if (p1Connections[i]/*parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[i]*/.GetInnovationnumber() ==
                p2Connections[j]/*parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[j]*/.GetInnovationnumber())
                {
                    matchingFound = true;
                    matchingIndexP1 = i;
                    matchingIndexP2 = j;
                    break;
       
                }
            }

            if(matchingFound)
            {
                int rnd = Random.Range(0,2);
                if(rnd == 0)
                {
                    //Node In = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetIn();
                    //Node Out = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetOut();
                    //float weight = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetWeight();
                    //bool enabled = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetEnabled();
                    Node In = p1Connections[matchingIndexP1].GetIn();
                    Node Out = p1Connections[matchingIndexP1].GetOut();
                    float weight = p1Connections[matchingIndexP1].GetWeight();
                    bool enabled = p1Connections[matchingIndexP1].GetEnabled();

                    NetworkManager.instance.CreateSynapse(childBrain,In,Out,weight,enabled);
                }
                else
                {
                    // Node In = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetIn();
                    // Node Out = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetOut();
                    // float weight = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetWeight();
                    // bool enabled = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetEnabled();
                    Node In = p2Connections[matchingIndexP2].GetIn();
                    Node Out = p2Connections[matchingIndexP2].GetOut();
                    float weight = p2Connections[matchingIndexP2].GetWeight();
                    bool enabled = p2Connections[matchingIndexP2].GetEnabled();

                    NetworkManager.instance.CreateSynapse(childBrain,In,Out,weight,enabled);
                }
            } 
            else
            {
                    //disjoint/excess genes
                    parent1Genes.Add(/*parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[i]*/p1Connections[i]);
            }

        }

        //parent 2 genome now, bbut only disjoint/excess genes, since those that are equal to the other parent, are already in the chhild
        for(int i = 0; i < parent2Size; i++)
        {
            matchingFound = false;
            matchingIndexP1 = -1;
            matchingIndexP2 = -1;
            for(int j = 0; j < parent1Size; j++)
            {
                    //both hhave the gene, 50/50
                if (p2Connections[i]/*parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[i]*/.GetInnovationnumber() ==
                    p1Connections[j]/*parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[j]*/.GetInnovationnumber())
                {
                    matchingFound = true;
                    break;
                }
                else
                {
                    matchingFound = false;
                    matchingIndexP1 = i;
                    matchingIndexP2 = j;             
                }
            }  
            //only looking for the excess/disjoint genes
            if(!matchingFound)
            {
                parent2Genes.Add(p2Connections[i]/*parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[i]*/);
            }

        }
        //from here on, thhe childs brain should have all connections that both parents share, the ones that the parents
        //dont share will be distribbuted to the child from the fitter parent
        //important: the gene should be placed in order according to the innovationnumber

        if(parents[0].GetComponent<Snake>().boardScript.fitness > parents[1].GetComponent<Snake>().boardScript.fitness)
        {
            //going through every disjoint/excess gene
            for(int i = 0; i < parent1Genes.Count; i++)
            {
                //going through every gene in the child to insert the excess/disjoint genes in the right spot
               // int connectionLength = childBrain.genomeConnectionGenes.Count;
               if(childBrain.genomeConnectionGenes.Count > 0)
                {
                    for (int j = 0; j < childBrain.genomeConnectionGenes.Count; j++)
                    {
                        if (parent1Genes[i].GetInnovationnumber() < childBrain.genomeConnectionGenes[j].GetInnovationnumber())
                        {
                            //childBrain.genomeConnectionGenes.Insert(j, parent1Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent1Genes[i].GetIn(), parent1Genes[i].GetOut(),
                            parent1Genes[i].GetWeight(), parent1Genes[i].GetEnabled());
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            // childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent1Genes[i].GetIn(), parent1Genes[i].GetOut(),
                            parent1Genes[i].GetWeight(), parent1Genes[i].GetEnabled());
                                break;
                        }
                    }

                }
               else
                {
                    //childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
                    NetworkManager.instance.CreateSynapse(childBrain, parent1Genes[i].GetIn(), parent1Genes[i].GetOut(),
                    parent1Genes[i].GetWeight(), parent1Genes[i].GetEnabled());
                }
               
            }
        }
        else if(parents[0].GetComponent<Snake>().boardScript.fitness < parents[1].GetComponent<Snake>().boardScript.fitness)
        {
            //going through every disjoint/excess gene
            for(int i = 0; i < parent2Genes.Count; i++)
            {
                //going through every gene in the child to insert the excess/disjoint genes in the right spot
                if(childBrain.genomeConnectionGenes.Count > 0)
                {
                    for (int j = 0; j < childBrain.genomeConnectionGenes.Count; j++)
                    {
                        if (parent2Genes[i].GetInnovationnumber() < childBrain.genomeConnectionGenes[j].GetInnovationnumber())
                        {
                            //childBrain.genomeConnectionGenes.Insert(j, parent2Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent2Genes[i].GetIn(), parent2Genes[i].GetOut(),
                            parent2Genes[i].GetWeight(), parent2Genes[i].GetEnabled());
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            //childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent2Genes[i].GetIn(), parent2Genes[i].GetOut(),
                            parent2Genes[i].GetWeight(), parent2Genes[i].GetEnabled());
                            break;
                        }
                    }

                }
                else
                {
                    //childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                    NetworkManager.instance.CreateSynapse(childBrain, parent2Genes[i].GetIn(), parent2Genes[i].GetOut(),
                    parent2Genes[i].GetWeight(), parent2Genes[i].GetEnabled());
                }
                
            }

        }
        //parents have same fitness -> child gets disjoint/excess genes from both parents
        else
        {
            
            //going through every disjoint/excess gene
            for(int i = 0; i < parent1Genes.Count; i++)
            {
                //going through every gene in the child to insert the excess/disjoint genes in the right spot
                if(childBrain.genomeConnectionGenes.Count > 0)
                {
                    for (int j = 0; j < childBrain.genomeConnectionGenes.Count; j++)
                    {
                        if (parent1Genes[i].GetInnovationnumber() < childBrain.genomeConnectionGenes[j].GetInnovationnumber())
                        {
                            //childBrain.genomeConnectionGenes.Insert(j, parent1Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent1Genes[i].GetIn(), parent1Genes[i].GetOut(),
                            parent1Genes[i].GetWeight(), parent1Genes[i].GetEnabled());
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            // childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent1Genes[i].GetIn(), parent1Genes[i].GetOut(),
                            parent1Genes[i].GetWeight(), parent1Genes[i].GetEnabled());
                            break;
                        }
                    }

                }
                else
                {
                    //childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
                    NetworkManager.instance.CreateSynapse(childBrain, parent1Genes[i].GetIn(), parent1Genes[i].GetOut(),
                    parent1Genes[i].GetWeight(), parent1Genes[i].GetEnabled());
                }
                
            }


            //going through every disjoint/excess gene
            for(int i = 0; i < parent2Genes.Count; i++)
            {
                //going through every gene in the child to insert the excess/disjoint genes in the right spot
                if(childBrain.genomeConnectionGenes.Count > 0)
                {
                    for (int j = 0; j < childBrain.genomeConnectionGenes.Count; j++)
                    {
                        if (parent2Genes[i].GetInnovationnumber() < childBrain.genomeConnectionGenes[j].GetInnovationnumber())
                        {
                            //childBrain.genomeConnectionGenes.Insert(j, parent2Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent2Genes[i].GetIn(), parent2Genes[i].GetOut(),
                            parent2Genes[i].GetWeight(), parent2Genes[i].GetEnabled());
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            //childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                            NetworkManager.instance.CreateSynapse(childBrain, parent2Genes[i].GetIn(), parent2Genes[i].GetOut(),
                            parent2Genes[i].GetWeight(), parent2Genes[i].GetEnabled());
                            break;
                        }
                    }

                }
                else
                {
                    //childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                    NetworkManager.instance.CreateSynapse(childBrain, parent2Genes[i].GetIn(), parent2Genes[i].GetOut(),
                    parent2Genes[i].GetWeight(), parent2Genes[i].GetEnabled());

                }
                
            }
        }

        //EXPERIMENTAL
        //now nodes have to be added and inserted into the right layer according to the connection genes the child now has

        //determining all nodes to be added
        List<Node> insertedNodes = new List<Node>();
        for(int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
        {
            bool add = false;
            if(insertedNodes.Count > 0 && childBrain.genomeConnectionGenes[i].GetOut().type.Equals("Hidden"))
            {
                for (int j = 0; j < insertedNodes.Count; j++)
                {
                    if (childBrain.genomeConnectionGenes[i].GetOut().nodeNumber == insertedNodes[j].nodeNumber)
                    {
                        add = false;
                        break;
                    }
                    else
                    {
                        add = true;
                    }
                }
            }
            else if(childBrain.genomeConnectionGenes[i].GetOut().type.Equals("Hidden"))
            {
                add = true;
            }

            if(add)
            {    //initially insert all hidden nodes in the first layer, i.e. input ->hidden->output
                 childBrain.AddNode(0, "Hidden", 1);
                 insertedNodes.Add(childBrain.genomeNodeGenes[childBrain.genomeNodeGenes.Count - 1]);
            }
           
        }
        //now we shift the hidden nodes to the righht if a connection between 2 hidden nodes demands thhat
        bool restart = true;
        int catchLoop = 0;
        while(restart && catchLoop < 9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
                {
                    Debug.Log("node shift erorr");
                }
            if(childBrain.genomeConnectionGenes.Count > 0)
            {
                for (int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
                {
                    //shiftnode needed
                    //index out of bounds appeared below
                    if (childBrain.genomeConnectionGenes[i].GetIn().type.Equals("Hidden")
                        && childBrain.genomeConnectionGenes[i].GetOut().type.Equals("Hidden")
                        && childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetIn().nodeNumber - 1].layer >=
                        childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layer)
                    {
                       // childBrain.ShiftNode(childBrain.genomeConnectionGenes[i].GetOut(), childBrain.genomeConnectionGenes[i].GetOut().layer,
                           // childBrain.genomeConnectionGenes[i].GetOut().layerIndex, (childBrain.genomeConnectionGenes[i].GetOut().layer + 1));
                           childBrain.ShiftNode(childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1],
                               childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layer,
                            childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layerIndex,
                            childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layer+1);
//todo:
//need to work with genomenodegenes because genomeconnectiongenes holds the layers from the parents in which
//the nodes have already been positioned in their layers
//we need to start off hhere with all hidden nodes in layer 1 though
                        restart = true;
                        break;
                    }
                    else
                    {
                        restart = false;
                    }
                }
            }
            else
            {
                    restart = false;
            }
            

        }
        
     
      




/*
        //now nodes have to be added and inserted into the right layer according to the connection genes the child now has
        for(int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
        {
            bool add = false;
            //making sure to not add the same geneNode twice
            //problem:
            //chhild is not inheriting new nodes based on the connections

            //in the following: check the connections IN and OUT nodes separtely, if either is not correlatable to an existing node, create and add one
            if (childBrain.genomeNodeGenes.Count > 0)
            {


                for (int j = 0; j < childBrain.genomeNodeGenes.Count; j++)
                {
                    if (childBrain.genomeConnectionGenes[i].GetIn().nodeNumber == childBrain.genomeNodeGenes[j].nodeNumber)
                    {
                        add = false;
                        break;
                    }
                    else
                    {
                        add = true;
                    }
                }
            }
            else
            {
                add = true;
            }

            if(add)
            {     
                if(childBrain.genomeConnectionGenes[i].GetIn().value == -99)
                {
                        Debug.Log("bugged here 3");
                        childBrain.AddNode(childBrain.genomeConnectionGenes[i].GetIn().state, "Hidden", childBrain.genomeConnectionGenes[i].GetIn().layer);
                }
                else
                {
                        Debug.Log("bugged here 4");
                        childBrain.AddNode((int)childBrain.genomeConnectionGenes[i].GetIn().value, "Hidden", childBrain.genomeConnectionGenes[i].GetIn().layer);
                }
            }

            add = false;

            if(childBrain.genomeNodeGenes.Count > 0)
            {
                for (int j = 0; j < childBrain.genomeNodeGenes.Count; j++)
                {
                    if (childBrain.genomeConnectionGenes[i].GetOut().nodeNumber == childBrain.genomeNodeGenes[j].nodeNumber)
                    {
                        add = false;
                        break;
                    }
                    else
                    {
                        add = true;

                    }
                }

            }
            else
            {
                add = true;
            }
            
            
            if(add)
            {       
                if(childBrain.genomeConnectionGenes[i].GetOut().value == -99)
                {
                        Debug.Log("bugged here 1");
                    childBrain.AddNode(childBrain.genomeConnectionGenes[i].GetOut().state, "Hidden", childBrain.genomeConnectionGenes[i].GetOut().layer);
                }
                else
                {
                        Debug.Log("bugged here 2");
                        childBrain.AddNode((int)childBrain.genomeConnectionGenes[i].GetOut().value, "Hidden", childBrain.genomeConnectionGenes[i].GetOut().layer);
                }
            }
            
         
        }
*/
        //now that nodes and connections are present in the child, we need to reassign the In and Out properties of each connection to thhose of
        //the nodes in genomenodelist, since otherwise the connections are still pointing to the nodes of parentA/parentB
        for(int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
        {
            childBrain.genomeConnectionGenes[i].SetIn(childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetIn().nodeNumber-1]);
      

            childBrain.genomeConnectionGenes[i].SetOut(childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber-1]);
            }




        //EXPERIMENTAL
        //there is a chance to reenable disabled connections, righht now that is 25% for each disabled gene
        for(int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
        {
                if (!childBrain.genomeConnectionGenes[i].GetEnabled())
                {
                    int rnd = Random.Range(1, 101);
                    if (rnd <= (int)Mathf.Ceil(GameManager.instance.geneReenableProb * 100))
                    {
                        childBrain.genomeConnectionGenes[i].SetEnabled(true);
                    
                    }

                }
               
        }


        return childBrain;
    }

    //for 1-member species
    else
    {
        Network network = parents[0].GetComponent<Snake>().brain;
        for(int i = 0; i < network.genomeConnectionGenes.Count; i++)
        {
                NetworkManager.instance.CreateSynapse(childBrain, network.genomeConnectionGenes[i].GetIn(), network.genomeConnectionGenes[i].GetOut(),
                network.genomeConnectionGenes[i].GetWeight(), network.genomeConnectionGenes[i].GetEnabled());
        }
        List<Node> insertedNodes = new List<Node>();
        for (int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
        {
            bool add = false;
            if (insertedNodes.Count > 0 && childBrain.genomeConnectionGenes[i].GetOut().type.Equals("Hidden"))
            {
                for (int j = 0; j < insertedNodes.Count; j++)
                {
                    if (childBrain.genomeConnectionGenes[i].GetOut().nodeNumber == insertedNodes[j].nodeNumber)
                    {
                        add = false;
                        break;
                    }
                    else
                    {
                        add = true;
                    }
                }
            }
            else if (childBrain.genomeConnectionGenes[i].GetOut().type.Equals("Hidden"))
            {
                add = true;
            }

            if (add)
            {    //initially insert all hidden nodes in the first layer, i.e. input ->hidden->output
                childBrain.AddNode(0, "Hidden", 1);
                insertedNodes.Add(childBrain.genomeNodeGenes[childBrain.genomeNodeGenes.Count - 1]);
            }

        }
        bool restart = true;
        int catchLoop = 0;
        while (restart && catchLoop < 9999)
        {
            ++catchLoop;
                if(catchLoop == 9999)
                {
                    Debug.Log("nodeshift error 2");
                }
            if (childBrain.genomeConnectionGenes.Count > 0)
            {
                for (int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
                {
                    //shiftnode needed
                    //index out of bounds appeared below
                    if (childBrain.genomeConnectionGenes[i].GetIn().type.Equals("Hidden")
                        && childBrain.genomeConnectionGenes[i].GetOut().type.Equals("Hidden")
                        && childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetIn().nodeNumber - 1].layer >=
                        childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layer)
                    {
                        // childBrain.ShiftNode(childBrain.genomeConnectionGenes[i].GetOut(), childBrain.genomeConnectionGenes[i].GetOut().layer,
                        // childBrain.genomeConnectionGenes[i].GetOut().layerIndex, (childBrain.genomeConnectionGenes[i].GetOut().layer + 1));
                        childBrain.ShiftNode(childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1],
                            childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layer,
                            childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layerIndex,
                            childBrain.genomeNodeGenes[childBrain.genomeConnectionGenes[i].GetOut().nodeNumber - 1].layer + 1);
                        //todo:
                        //need to work with genomenodegenes because genomeconnectiongenes holds the layers from the parents in which
                        //the nodes have already been positioned in their layers
                        //we need to start off hhere with all hidden nodes in layer 1 though
                        restart = true;
                        break;
                    }
                    else
                    {
                        restart = false;
                    }
                }
            }
            else
            {
                restart = false;
            }


        }
            //return parents[0].GetComponent<Snake>().brain;
            return childBrain;
    }
   }


    public static Network AddNodeMutation(Network network)
    {
        if(network.genomeConnectionGenes.Count > 0)
        {
            int rnd2 = Random.Range(0,network.genomeConnectionGenes.Count);
            
            
            bool viable = false;
            //checking if there is at least 1 enabled gene
            for(int i = 0; i < network.genomeConnectionGenes.Count; i++)
            {
                if (network.genomeConnectionGenes[i].GetEnabled() == false)
                {
                    viable = false;
                }
                else
                {
                    viable = true;
                    break;
                }
            }
            
            if (viable)
            {
                // this prevents disabling a disabled connection, which makes no sense of course
                int catchLoop = 0;
                while (network.genomeConnectionGenes[rnd2].GetEnabled() == false && catchLoop < 9999 )
                {
                    ++catchLoop;
                    if(catchLoop == 9999)
                    {
                        Debug.Log("viable error");
                    }
                    rnd2 = Random.Range(0, network.genomeConnectionGenes.Count);
                }

                //disable existing connection
                network.genomeConnectionGenes[rnd2].SetEnabled(false);
                //create new node
                //nodenumer-1 is experimental
                // int nodeInLayer = network.genomeNodeGenes[network.genomeConnectionGenes[rnd2].GetIn().nodeNumber-1].layer;
                int nodeInLayer = network.genomeConnectionGenes[rnd2].GetIn().layer;
                // int nodeOutLayer = network.genomeNodeGenes[network.genomeConnectionGenes[rnd2].GetOut().nodeNumber-1].layer;
                int nodeOutLayer = network.genomeConnectionGenes[rnd2].GetOut().layer;

                //i.e one or more layer(s) inbetween
                // Debug.Log(nodeOutLayer + " - " + nodeInLayer + " > " + " 1");


                if (nodeOutLayer - nodeInLayer > 1)
                {
                    network.AddNode(0, "Hidden", (nodeInLayer + 1));
                }
                //i.e. no layer inbetween -> we need a new layer
                else if (nodeOutLayer - nodeInLayer == 1)
                {

                    network.AddLayer(nodeOutLayer);
                    network.AddNode(0, "Hidden", nodeOutLayer);
                }

                //synapse leading into the new node
                NetworkManager.instance.CreateSynapse(network, network.genomeConnectionGenes[rnd2].GetIn(), network.genomeNodeGenes[network.genomeNodeGenes.Count - 1]
                , 1, true);
                //synapse leading out of the new node
                NetworkManager.instance.CreateSynapse(network, network.genomeNodeGenes[network.genomeNodeGenes.Count - 1], network.genomeConnectionGenes[rnd2].GetOut(),
                network.genomeConnectionGenes[rnd2].GetWeight(), true);
            }
        }
        return network;
    }
    public static Network AddConnectionMutation(Network network)
    {

        List<int> output = FindValidConnection(network.genomeNodeGenes);
        int In = output[0];
        int Out = output[1];

        if (network.genomeNodeGenes[In].layer == network.genomeNodeGenes[Out].layer && network.genomeNodeGenes[In].type.Equals("Hidden")
            && network.genomeNodeGenes[Out].type.Equals("Hidden"))
        {
            Debug.Log("shouldnt happen1");
        }


        //check if connection already exists or is not viable
        bool connectionViable = false;
        int catchLoop = 0;
        while(!connectionViable && catchLoop < 9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
            {
                Debug.Log("connectionViable error");
            }
            if(network.genomeConnectionGenes.Count > 0)
            {
                for (int i = 0; i < network.genomeConnectionGenes.Count; i++)
                {
                    //check if the exact connection exists
                    if (network.genomeConnectionGenes[i].GetIn().nodeNumber == (In + 1) && network.genomeConnectionGenes[i].GetOut().nodeNumber == (Out + 1))
                    {
                        connectionViable = false;
   
                        //infnite loop here should be fixed
                        output = FindValidConnection(network.genomeNodeGenes);
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

        if (network.genomeNodeGenes[In].layer == network.genomeNodeGenes[Out].layer && network.genomeNodeGenes[In].type.Equals("Hidden")
            && network.genomeNodeGenes[Out].type.Equals("Hidden"))
        {
            Debug.Log("shouldnt happen2");
        }

        NetworkManager.instance.CreateSynapse(network,network.genomeNodeGenes[In], network.genomeNodeGenes[Out], network.RandomWeight(NetworkManager.instance.minWeight,
        NetworkManager.instance.maxWeight), true);

      
            
        return network;
        
    }
    public static Network AlterWeightMutation(Network network)
    {
        if(network.genomeConnectionGenes.Count != 0)
        {
            //alter weight by -0.1 - 0.1
            int randomConnection = Random.Range(0, network.genomeConnectionGenes.Count);
            int rnd = Random.Range(0, 101);
            int prob = (int)Mathf.Ceil(GameManager.instance.randomWeightProb * 100);

            //small percentage to assign a random value (-10,10)
            if(rnd <= prob)
            {
                network.genomeConnectionGenes[randomConnection].SetWeight(network.RandomWeight(NetworkManager.instance.minWeight, NetworkManager.instance.maxWeight));
            }
            //high probability to alter the weight a little (-2.5,2.5)
            else
            {
                float weightChange = (float)System.Math.Round(Random.Range(-GameManager.instance.mutationPower, GameManager.instance.mutationPower), 2);
                network.genomeConnectionGenes[randomConnection].SetWeight(network.genomeConnectionGenes[randomConnection].GetWeight() + weightChange);
            }
            
        }
        return network;
    }


    //this method makes sure thhat
    //1) In cant be an output node and Out cant be an input node
    //2) a hidden node being in a layer after another hidden node cant connect from right(In) to left(Out)
    //little optimization:
    //we could limit the random.range to only the output nodes, since they never change their node number
    static List<int> FindValidConnection(List<Node> nodes)
    {
        List<int> output = new List<int>();
        int In = Random.Range(0, nodes.Count);
        int catchLoop = 0;
        while (nodes[In].type.Equals("Output") && catchLoop < 9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
            {
                Debug.Log("findvalidconnection1 error");
            }
            In = Random.Range(0, nodes.Count);
        }
        int Out = Random.Range(0, nodes.Count);
        catchLoop = 0;
        while (nodes[Out].type.Equals("Input") && catchLoop < 9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
            {
                Debug.Log("findvalidconnection2 error");
            }
            Out = Random.Range(0, nodes.Count);
        }
       

        if (nodes[In].type.Equals("Hidden") && nodes[Out].type.Equals("Hidden"))
        {
            if (nodes[In].layer >= nodes[Out].layer)
            {
                output = FindValidConnection(nodes);
                In = output[0];
                Out = output[1];
            }
        }

        if (nodes[In].type.Equals("Hidden") && nodes[Out].type.Equals("Hidden"))
        {
            Debug.Log("hhere");
        }

        output = new List<int>();
        output.Add(In);
        output.Add(Out);

        return output;

    }


   public static List<GameObject> SelectionSort(List<GameObject> list, string type)
   {
        if(!type.Equals("fitness") && !type.Equals("species") && !type.Equals("avgFitness") && !type.Equals("snakeInstance"))
        {
            Debug.Log("this is not viable as type: " + type);
        }
        int insertIndex = 0;
        int maxIndex = list.Count-1;
        int minElement = 0;
        for(int i = 0; i < list.Count-1; i++)
        {
            minElement = insertIndex;

            for(int j = insertIndex+1; j < maxIndex+1; j++)
            {
                if(type.Equals("fitness") && list[j].GetComponent<Snake>().boardScript.fitness < list[minElement].GetComponent<Snake>().boardScript.fitness)
                {
                    minElement = j;
    
                }
                else if(type.Equals("species") && list[j].GetComponent<Board>().species < list[minElement].GetComponent<Board>().species)
                {
                    minElement = j;
                }
                else if (type.Equals("avgFitness") && list[j].GetComponent<Snake>().boardScript.avgFitness <
                    list[minElement].GetComponent<Snake>().boardScript.avgFitness)
                {
                    minElement = j;
                }
   
            }
            GameObject temp = list[insertIndex];
            list[insertIndex] = list[minElement];
            list[minElement] = temp;
            insertIndex++;
        }

        return list;
   }

    public static List<GameObject> SelectionSort(List<GameObject> list)
    {
        
        int insertIndex = 0;
        int maxIndex = list.Count - 1;
        int minElement = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            minElement = insertIndex;

            for (int j = insertIndex + 1; j < maxIndex + 1; j++)
            {
                
                if (list[j].GetComponent<Board>().snakeInstance.GetComponent<Snake>().species <
                    list[minElement].GetComponent<Board>().snakeInstance.GetComponent<Snake>().species)
                {
                    minElement = j;
                }
            }
            GameObject temp = list[insertIndex].GetComponent<Board>().snakeInstance;
            list[insertIndex].GetComponent<Board>().snakeInstance = list[minElement].GetComponent<Board>().snakeInstance;
            list[minElement].GetComponent<Board>().snakeInstance = temp;
            insertIndex++;
        }

        return list;
    }




    public static void UpdateSpeciesMetrics()
    {
        //remove any species containing 0 snakes
        int it = 0;
        int catchLoop = 0;
        while (it < GameManager.instance.species.Count && catchLoop < 9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
            {
                Debug.Log("updatespeciesmetrics error");
            }
            if (GameManager.instance.species[it].snakes.Count == 0)
            {
                GameManager.instance.species.RemoveAt(it);
                it = 0;
            }
            else
            {
                it++;
            }
        }
        //update species parameters
        if(!GameManager.instance.multiRunsPerGen)
        {
            for (int i = 0; i < GameManager.instance.species.Count; i++)
            {
                Species species = GameManager.instance.species[i];
                species.setAge(species.getAge() + 1);
                species.setLastImprovement(species.getLastImprovement() + 1);
                int avgFitness = 0;
                for (int j = 0; j < species.snakes.Count; j++)
                {
                    if (species.snakes[j].GetComponent<Snake>().boardScript.fitness > species.getMaxFitness())
                    {
                        species.setMaxFitness(species.snakes[j].GetComponent<Snake>().boardScript.fitness);
                        species.setLastImprovement(0);
                    }
                    avgFitness += species.snakes[j].GetComponent<Snake>().boardScript.fitness;
                }
                if (avgFitness != 0)
                {
                    avgFitness /= species.snakes.Count;
                }

                species.setAvgFitness(avgFitness);

                if (species.getAge() >= GameManager.instance.speciesMaturationTime)
                {
                    species.setYoung(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < GameManager.instance.species.Count; i++)
            {
                Species species = GameManager.instance.species[i];

                if(GameManager.instance.nextGen)
                {
                    species.setAge(species.getAge() + 1);
                    species.setLastImprovement(species.getLastImprovement() + 1);
                }
                
                int avgFitness = 0;
                for (int j = 0; j < species.snakes.Count; j++)
                {
                    if (species.snakes[j].GetComponent<Snake>().boardScript.fitness > species.getMaxFitness())
                    {
                        species.setMaxFitness(species.snakes[j].GetComponent<Snake>().boardScript.fitness);

                        species.setLastImprovement(0);
                       
                    }

                    avgFitness += species.snakes[j].GetComponent<Snake>().boardScript.fitness;
                }
                if (avgFitness != 0)
                {
                    avgFitness /= species.snakes.Count;
                }

                species.setAvgFitness(avgFitness);

            }
        }
        

    }







    public static void Evaluate(List<GameObject> snakes)
    {
        //if there are no species yet, create one and assign all snakes to it
        if (GameManager.instance.species.Count == 0)
        {
            Species newSpecies = new Species(1);
            GameManager.instance.species.Add(newSpecies);
            for (int i = 0; i < snakes.Count; i++)
            {
                GameManager.instance.species[0].AddSnake(snakes[i]);
            }
        }

       
        //calculate adjustedfitness for each snake in each species and sum up the adjusted fitness of each snake per species
        float sumOfAdjFitnesses = 0.0f;
        List<float> adjFitnessesList = new List<float>();
        for (int i = 0; i < GameManager.instance.species.Count; i++)
        {

            for (int j = 0; j < GameManager.instance.species[i].snakes.Count; j++)
            {
                sumOfAdjFitnesses += AdjustedFitness(GameManager.instance.species[i].snakes[j].GetComponent<Snake>(),
                    GameManager.instance.species[i].snakes);
                    GameManager.instance.species[i].snakes[j].GetComponent<Snake>().boardScript.avgFitness +=
                    GameManager.instance.species[i].snakes[j].GetComponent<Snake>().boardScript.fitness;
            }
            //adjFitnessesList.Add(sumOfAdjFitnesses);
            GameManager.instance.species[i].setAdjFitness(sumOfAdjFitnesses);
            sumOfAdjFitnesses = 0;


        }
        
        
        

      

    }



    static float AdjustedFitness(Snake snake, List<GameObject> speciesPop)
    {
        return (float)snake.boardScript.fitness / speciesPop.Count;
    }




    public static void Reproduce()
    {
        GameManager.instance.networks = new List<Network>();
        if(!GameManager.instance.multiRunsPerGen || GameManager.instance.multiRunsPerGen && GameManager.instance.nextGen)
        {
            for (int i = 0; i < GameManager.instance.species.Count; i++)
            {


                for (int j = 0; j < GameManager.instance.species[i].getOffspring(); j++)
                {
                    Network network = Evolution.Crossover(Evolution.Selection(GameManager.instance.species[i].snakes));

                    //EXPERIMENTAL
                    //the for loops are experimental and give each node/connection of a network the chance to mutate instead of only
                    //having the chance of mutating 3 times at max (weight, add node, add connection)
                    int rnd;
                    int prob = (int)Mathf.Ceil(GameManager.instance.addNodeProb * 100);
                    if (GameManager.instance.multipleStructuralMutations)
                    {
                        for (int x = 0; x < network.genomeConnectionGenes.Count; x++)
                        {
                            rnd = Random.Range(1, 101);
                            if (rnd <= prob)
                            {
                                Evolution.AddNodeMutation(network);
                            }
                        }

                        prob = (int)Mathf.Ceil(GameManager.instance.addConnectionProb * 100);
                        for (int x = 0; x < network.genomeNodeGenes.Count - 4; x++) //-4 because i exclude the output nodes
                        {
                            rnd = Random.Range(1, 101);
                            if (rnd <= prob)
                            {
                                Evolution.AddConnectionMutation(network);
                            }
                        }

                        prob = (int)Mathf.Ceil(GameManager.instance.alterWeightProb * 100);
                        for (int x = 0; x < network.genomeConnectionGenes.Count; x++)
                        {
                            rnd = Random.Range(1, 101);
                            if (rnd <= prob)
                            {
                                Evolution.AlterWeightMutation(network);
                            }
                        }
                    }
                    else
                    {
                        rnd = Random.Range(1, 101);
                        if (rnd <= prob)
                        {
                            Evolution.AddNodeMutation(network);

                        }

                        prob = (int)Mathf.Ceil(GameManager.instance.addConnectionProb * 100);
                        rnd = Random.Range(1, 101);
                        if (rnd <= prob)
                        {
                            Evolution.AddConnectionMutation(network);
                        }

                        prob = (int)Mathf.Ceil(GameManager.instance.alterWeightProb * 100);
                        for (int x = 0; x < network.genomeConnectionGenes.Count; x++)
                        {
                            rnd = Random.Range(1, 101);
                            if (rnd <= prob)
                            {
                                Evolution.AlterWeightMutation(network);
                            }
                        }
                    }

                    GameManager.instance.networks.Add(network);
                    GameManager.instance.speciesNumsCopy.Add(GameManager.instance.species[i].getId());
                }
            }
        }
        else
        {
            for(int i = 0; i < GameManager.instance.species.Count; i++)
            {
                for(int j = 0; j < GameManager.instance.species[i].snakes.Count; j++)
                {
                    GameManager.instance.networks.Add(GameManager.instance.species[i].snakes[j].GetComponent<Snake>().brain);
                    GameManager.instance.speciesNumsCopy.Add(GameManager.instance.species[i].getId());
                }
            }
        }
        
    }




    public static void Eliminate()
    {
        int punishedSpecies = 0;
        for(int i = 0; i < GameManager.instance.species.Count; i++)
        {
            bool punishSpecies = false;
            if (GameManager.instance.species[i].getLastImprovement() >= GameManager.instance.noImprovementDropoff)
            {
                punishedSpecies = i;
                punishSpecies = true;
            }

            
            if (punishSpecies)
            {
                GameManager.instance.species[punishedSpecies].setAdjFitness(0);
                punishSpecies = false;
                break;
            }
            
        }
   
    }





    public static void CalculateOffspring()
    {
        float divisor = 0.0f;

        //the divisor to specify how many chhildren a species can hhave is calculated here in form of the sum of all adj fitnesses
    
        for (int i = 0; i < GameManager.instance.species.Count; i++)
        {
            divisor += GameManager.instance.species[i].getAdjFitness();
        }
        if (divisor == 0)
        {
            divisor = 1.0f;
        }

        //list of decimal values representing the percentage of offspring each species gets
        List<float> offspringStake = new List<float>();

        for (int i = 0; i < GameManager.instance.species.Count; i++)
        {
            offspringStake.Add((float)GameManager.instance.species[i].getAdjFitness() / divisor);
        }
        //if all adjusted fitnesses of a species are 0, we assign 1 to the offspringstake since othherwise the species wont reproduce
        //this could be desirable, but especially when starting the game and having only 1 species, we need to ensure them to reproduce
        //adjustment to above:
        //case 1: if we only have 1 species and the sum of adjusted fitnesses is 0, below applies
        //case 2: if we have multiple species and every species has a sum of adjusted fitnesses of 0, below applies for one species

        bool allZero = false;
        if (offspringStake.Count == 1 && offspringStake[0] == 0)
        {
            offspringStake[0] = 1;
        }
        else
        {
            for (int i = 0; i < offspringStake.Count; i++)
            {
                if (offspringStake[i] == 0)
                {
                    allZero = true;
                }
                else
                {
                    allZero = false;
                    break;
                }
            }
            //random species will be able to reproduce
            if (allZero)
            {
                int rnd = Random.Range(0, offspringStake.Count);
                offspringStake[rnd] = 1;
            }

        }



        //turning the percentages into integers, i.e. actual number of offspring
        List <int> offspringNum = new List<int>();
        offspringNum = OffspringDistribution(offspringStake);
        for(int i = 0; i < offspringNum.Count; i++)
        {
            GameManager.instance.species[i].setOffspring(offspringNum[i]);
        }

        //after offspring numbers hhave been assigned fairly throughh adjusted fitness calculation, we grant new species (young tag) that have
        //would not receive any offspring one chhild
        //by doing that we need to take away a child from anothher species to fit the preset populationsize (30 for now)
        //it seems to be somewhat fair to take thhis child from thhe species that would receive thhe most children

    }







    static List<int> OffspringDistribution(List<float> stake)
    {
        List<float> floatDist = new List<float>();
        List<int> offspringDist = new List<int>();
        float excess = 0.0f;

        for (int i = 0; i < stake.Count; i++)
        {
            floatDist.Add(stake[i] * GameManager.instance.numOfBoards);
        }

        if (floatDist.Count > 1)
        {
            for (int i = 0; i < floatDist.Count - 1; i++)
            {
                int rawNumber = Mathf.FloorToInt(floatDist[i]);
                if (floatDist[i] == 0)
                {
                    excess = 0;
                }
                else if (rawNumber == 0)
                {
                    excess = floatDist[i];
                }
                else
                {
                    excess = floatDist[i] % rawNumber;
                }


                if (excess >= 0.5f)
                {
                    offspringDist.Add(Mathf.CeilToInt(floatDist[i]));
                    excess = 1f - excess;
                    if (floatDist[i + 1] - excess < 0)
                    {
                        floatDist[i + 1] = 0;
                    }
                    else
                    {
                        floatDist[i + 1] -= excess;
                    }

                }
                else
                {
                    offspringDist.Add(Mathf.FloorToInt(floatDist[i]));
                    floatDist[i + 1] += excess;
                }
            }
            //this portion deals with the last element in floatdist
            int rawNumber2 = Mathf.FloorToInt(floatDist[floatDist.Count - 1]);
            if (rawNumber2 == 0)
            {
                excess = floatDist[floatDist.Count - 1];
            }
            else
            {
                excess = floatDist[floatDist.Count - 1] % rawNumber2;
            }

            if (excess >= 0.5f)
            {
                offspringDist.Add(Mathf.CeilToInt(floatDist[floatDist.Count - 1]));
            }
            else
            {
                offspringDist.Add(Mathf.FloorToInt(floatDist[floatDist.Count - 1]));
                //current problem with thjis:
                //if floatdist[x] is something like 0.99999 it should be a 1, but we round down and it becomes 0

            }


        }
        //should only be the case for a floatdist[0] of 30
        else
        {
            offspringDist.Add((int)floatDist[0]);
        }


        return offspringDist;

    }

    public static void RemoveWorstPerformers(List<Species> speciesses)
    {
        //sort snakes in each species according to fitness
        List<GameObject> orderedSnakes;
        for (int i = 0; i < speciesses.Count; i++)
        {
            if(GameManager.instance.nextGen)
            {
                for (int j = 0; j < speciesses[i].snakes.Count; j++)
                {
                    speciesses[i].snakes[j].GetComponent<Snake>().boardScript.avgFitness /= GameManager.instance.runsPerGen;
                }
                orderedSnakes = Evolution.SelectionSort(speciesses[i].snakes, "avgFitness");
                
            }
            else
            {
                orderedSnakes = Evolution.SelectionSort(speciesses[i].snakes, "fitness");
            }
            
            //only eliminate snakes with at least 3 members in the species
            if (orderedSnakes.Count > 2)
            {
                float percentile = GameManager.instance.eliminationPercentile;
                int n = orderedSnakes.Count;
                int spot = Mathf.RoundToInt(percentile * n);

                //now we have the fitness value which belongs to the eliminationPercentile together with all fitnesses below
                for (int j = 0; j < spot; j++)
                {
                    //preventing to eliminate all but one snakes in a species in which case the next generation is a copy of this one snake
                    if (orderedSnakes.Count > 2)
                    {
                        orderedSnakes.RemoveAt(0);
                    }

                }
            }

        }
       

    }





    public static void Speciate(List<GameObject> newGen)
    {
        //mark each snake in eachh species as parent in the beginning to know which snakes should be removed
        //from thhe species after hhaving the children assigned to the species
        for(int i = 0; i < GameManager.instance.species.Count; i++)
        {
            for(int j = 0; j < GameManager.instance.species[i].snakes.Count; j++)
            {
                GameManager.instance.species[i].snakes[j].GetComponent<Snake>().role = "parent";
            }
        }

        for(int i = 0; i < newGen.Count; i++)
        {
            bool createSpecies = false;
            for(int j = 0; j < GameManager.instance.species.Count; j++)
            {

                int rnd = Random.Range(0, GameManager.instance.species[j].snakes.Count);
                
               
                if (NetworkManager.instance.CompatabilityDistance(newGen[i].GetComponent<Snake>().brain.genomeConnectionGenes,
                    GameManager.instance.species[j].snakes[rnd].GetComponent<Snake>().brain.genomeConnectionGenes)
                    >= GameManager.instance.compat_threshhold)
                {
                    createSpecies = true;
                }
                else
                {
                    createSpecies = false;
                    GameManager.instance.species[j].AddSnake(newGen[i]);
                    break;
                    
                }

            }

            if(createSpecies)
            {
                //find highhest species number
                int highestSpecies = GameManager.instance.species[GameManager.instance.species.Count - 1].getId();
                Species newSpecies = new Species(highestSpecies+1);
                GameManager.instance.species.Add(newSpecies);
                GameManager.instance.species[GameManager.instance.species.Count - 1].AddSnake(newGen[i]);
            }
        }

        //now remove parents since children hhave filled the specieses
        //maybe need to think about having 0 member species
        int it = 0;
        bool done = false;
        //todo:
        //create a better loop for removing parents
        int catchLoop = 0;
        while(it < GameManager.instance.species.Count && catchLoop <9999)
        {
            ++catchLoop;
            if(catchLoop == 9999)
            {
                Debug.Log("removing parents error");
            }
            if (GameManager.instance.species[it].snakes.Count > 0)
            {
                for (int j = 0; j < GameManager.instance.species[it].snakes.Count; j++)
                {
                    if (GameManager.instance.species[it].snakes[j].GetComponent<Snake>().role.Equals("parent"))
                    {
                        GameManager.instance.species[it].snakes.RemoveAt(j);
                        done = false;
                        break;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
            else
            {
                done = true;
            }
            
            if(!done)
            {
                it = 0;
            }
            else
            {
                it++;
            }
        }
        
        GameManager.instance.countSpecies = 0;
        for(int i = 0; i < GameManager.instance.species.Count; i++)
        {
            if (GameManager.instance.species[i].snakes.Count > 0)
            {
                GameManager.instance.countSpecies++;
            }
        }
        GameManager.instance.speciesTxt.text = "Species: " + GameManager.instance.countSpecies;
    }

    

    //todo;
    //it would bbe very hhelpful regarding species maintance to not give a species (which couldve just been created) any offspring
    //because it initially performed bad or didnt move(the problem where thhe snake wants to go where it cant at the start)
    //new species could get some sort of babysafezone to give them a chance to establish
    //we could then let the eliminate species methhod handle the death of a species
    //still need to think about hhow to manage species numbers
    //if species 2 dies, will a new species be called species 2 or do we follow an ordering of how many species exist and existed 

}


