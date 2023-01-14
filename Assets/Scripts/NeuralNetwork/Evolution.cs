using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
          orderedFitness = SelectionSort(orderedFitness); 
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
                bool AllZero=false;
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

   public static Network Crossover(List<GameObject> parents) //needed parameters: taking in 2 snakes 
   {
    if(parents.Count > 1)
    {
        int parent1Size = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes.Count;
        int parent2Size = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes.Count;
        List<Synapse> parent1Genes = new List<Synapse>();
        List<Synapse> parent2Genes = new List<Synapse>();
        Network childBrain = new Network(12);
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
                if(parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[i].GetInnovationnumber() ==
                parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[j].GetInnovationnumber())
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
                    Node In = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetIn();
                    Node Out = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetOut();
                    float weight = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetWeight();
                    bool enabled = parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP1].GetEnabled();

                    NetworkManager.instance.CreateSynapse(childBrain,In,Out,weight,enabled);
                }
                else
                {
                    //childBrain.genomeConnectionGenes.Add(parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2]);
                    Node In = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetIn();
                    Node Out = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetOut();
                    float weight = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetWeight();
                    bool enabled = parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[matchingIndexP2].GetEnabled();
    
                    NetworkManager.instance.CreateSynapse(childBrain,In,Out,weight,enabled);
                }
            } 
            else
            {
                //disjoint/excess genes
                parent1Genes.Add(parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[i]);
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
                if(parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[i].GetInnovationnumber() ==
                parents[0].GetComponent<Snake>().brain.genomeConnectionGenes[j].GetInnovationnumber())
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
               parent2Genes.Add((parents[1].GetComponent<Snake>().brain.genomeConnectionGenes[i]));
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
                            childBrain.genomeConnectionGenes.Insert(j, parent1Genes[i]);
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
                            break;
                        }
                    }

                }
               else
                {
                    childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
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
                            childBrain.genomeConnectionGenes.Insert(j, parent2Genes[i]);
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                            break;
                        }
                    }

                }
                else
                {
                    childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
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
                            childBrain.genomeConnectionGenes.Insert(j, parent1Genes[i]);
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
                            break;
                        }
                    }

                }
                else
                {
                    childBrain.genomeConnectionGenes.Add(parent1Genes[i]);
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
                            childBrain.genomeConnectionGenes.Insert(j, parent2Genes[i]);
                            break;
                        }
                        else if (j == childBrain.genomeConnectionGenes.Count - 1)
                        {
                            childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                            break;
                        }
                    }

                }
                else
                {
                    childBrain.genomeConnectionGenes.Add(parent2Genes[i]);
                }
                
            }
        }

        //now nodes have to be added and inserted into the right layer according to the connection genes the child now has
        for(int i = 0; i < childBrain.genomeConnectionGenes.Count; i++)
        {
            bool add = false;
            //making sure to not add the same geneNode twice
            //problem:
            //chhild is not inheriting new nodes based on the connections
            
            //in the following: check the connections IN and OUT nodes separtely, if either is not correlatable to an existing node, create and add one
            for(int j = 0; j < childBrain.genomeNodeGenes.Count; j++)
            {
                if(childBrain.genomeConnectionGenes[i].GetIn().nodeNumber == childBrain.genomeNodeGenes[j].nodeNumber)
                {
                    add = false;
                    break;
                }
                else
                {
                    add = true;
                }
            }
            if(add)
            {     
                if(childBrain.genomeConnectionGenes[i].GetIn().value == -99)
                {
                    Debug.Log("should work");
                    childBrain.AddNode(childBrain.genomeConnectionGenes[i].GetIn().state, "Hidden", childBrain.genomeConnectionGenes[i].GetIn().layer);
                }
                else
                {
                    Debug.Log("should work");
                    childBrain.AddNode((int)childBrain.genomeConnectionGenes[i].GetIn().value, "Hidden", childBrain.genomeConnectionGenes[i].GetIn().layer);
                }
            }
            add = false;

            for(int j = 0; j < childBrain.genomeNodeGenes.Count; j++)
            {
                if(childBrain.genomeConnectionGenes[i].GetOut().nodeNumber == childBrain.genomeNodeGenes[j].nodeNumber)
                {
                    add = false;
                    break;
                }
                else
                {
                    add = true;
                }
            }
            
            if(add)
            {       
                if(childBrain.genomeConnectionGenes[i].GetOut().value == -99)
                {
                    childBrain.AddNode(childBrain.genomeConnectionGenes[i].GetOut().state, "Hidden", childBrain.genomeConnectionGenes[i].GetOut().layer);
                }
                else
                {
                    childBrain.AddNode((int)childBrain.genomeConnectionGenes[i].GetOut().value, "Hidden", childBrain.genomeConnectionGenes[i].GetOut().layer);
                }
            }
            
         
        }
        return childBrain;
    }
    //for 1-member species
    else
    {
        return parents[0].GetComponent<Snake>().brain;
    }
   }


    public static Network AddNodeMutation(Network network)
    {
        Debug.Log("shhould work");
        if(network.genomeConnectionGenes.Count != 0)
        {
            Debug.Log("works");
            int rnd2 = Random.Range(0,network.genomeConnectionGenes.Count);
            
            //disable existing connection
            network.genomeConnectionGenes[rnd2].SetEnabled(false);
            //create new node
            //nodenumer-1 is experimental
            int nodeInLayer = network.genomeNodeGenes[network.genomeConnectionGenes[rnd2].GetIn().nodeNumber-1].layer;
            int nodeOutLayer = network.genomeNodeGenes[network.genomeConnectionGenes[rnd2].GetOut().nodeNumber-1].layer;
            
            //i.e one or more layer(s) inbetween
            if(nodeOutLayer - nodeInLayer > 1)
            {
                network.AddNode(0, "Hidden", (nodeInLayer+1));
            }
            //i.e. no layer inbetween -> we need a new layer
            else if(nodeOutLayer - nodeInLayer == 1)
            {
                network.AddLayer(nodeOutLayer);
                network.AddNode(0, "Hidden", nodeOutLayer);
            }
            //synapse leading into the new node
            NetworkManager.instance.CreateSynapse(network,network.genomeConnectionGenes[rnd2].GetIn(), network.genomeNodeGenes[network.genomeNodeGenes.Count-1]
            ,1, true);
            //synapse leading out of the new node
            NetworkManager.instance.CreateSynapse(network,network.genomeNodeGenes[network.genomeNodeGenes.Count-1], network.genomeConnectionGenes[rnd2].GetOut(),
            network.genomeConnectionGenes[rnd2].GetWeight(), true);

        }
        return network;
    }
    public static Network AddConnectionMutation(Network network)
    {
        
            //addconnection
            //check if connection is viable
            //specifically: check for structurally impossible connections
            int In = Random.Range(0,network.genomeNodeGenes.Count);
            while(network.genomeNodeGenes[In].type.Equals("Output"))
            {
                In = Random.Range(0,network.genomeNodeGenes.Count);
            }
            int Out = Random.Range(0,network.genomeNodeGenes.Count);
            while(network.genomeNodeGenes[Out].type.Equals("Input"))
            {
                Out = Random.Range(0,network.genomeNodeGenes.Count);
            }

            //check if connection already exists or is not viable
            bool connectionViable = false;
            while(!connectionViable)
            {
                if(network.genomeConnectionGenes.Count > 0)
                {
                    for (int i = 0; i < network.genomeConnectionGenes.Count; i++)
                    {
                        //check if the exact connection exists
                        if (network.genomeConnectionGenes[i].GetIn().nodeNumber == In + 1 && network.genomeConnectionGenes[i].GetOut().nodeNumber == Out + 1)
                        {
                            connectionViable = false;
                            In = Random.Range(0, network.genomeNodeGenes.Count);
                            while (network.genomeNodeGenes[In].type.Equals("Output"))
                            {
                                In = Random.Range(0, network.genomeNodeGenes.Count);
                            }
                            Out = Random.Range(0, network.genomeNodeGenes.Count);
                            while (network.genomeNodeGenes[Out].type.Equals("Input"))
                            {
                                Out = Random.Range(0, network.genomeNodeGenes.Count);
                            }
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
            //float weightChange = (float)System.Math.Round(Random.Range(-0.1f,0.1f),2);
            float weightChange = (float)System.Math.Round(Random.Range(-GameManager.instance.mutationPower, GameManager.instance.mutationPower), 2);
            network.genomeConnectionGenes[randomConnection].SetWeight(network.genomeConnectionGenes[randomConnection]
            .GetWeight()+weightChange);
        }
        return network;
    }


   public static List<GameObject> SelectionSort(List<GameObject> list)
   {
        int insertIndex = 0;
        int maxIndex = list.Count-1;
        int minElement = 0;
        for(int i = 0; i < list.Count-1; i++)
        {
            minElement = insertIndex;

            for(int j = insertIndex+1; j < maxIndex+1; j++)
            {
                if(list[j].GetComponent<Snake>().boardScript.fitness < list[minElement].GetComponent<Snake>().boardScript.fitness)
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


   
}
