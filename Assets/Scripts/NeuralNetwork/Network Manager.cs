using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager
{
    public static NetworkManager instance;
    public int numOfInputNodes = 12;
    public int nodeCount;
    public double mutationRate = 0.05;
    public float minWeight = -2;
    public float maxWeight = 2;
    public List<Network> listOfNetworks;
    public static int innovationNumber = 1;
    
    


    public NetworkManager()
    {
        instance = this;
        listOfNetworks = new List<Network>();
    }

    public void AddNetwork(Network network)
    {
        listOfNetworks.Add(network);
    }

    public void CreateSynapse(Network network, Node In, Node Out, float weight, bool enabled)
    {
        int innoNumber = -1;
        bool connectionExists = false;
     
        for(int i = 0; i < GameManager.instance.storedConnections.Count; i++)
        {
            
                if(In.nodeNumber == GameManager.instance.storedConnections[i].GetIn().nodeNumber &&
                Out.nodeNumber == GameManager.instance.storedConnections[i].GetOut().nodeNumber)
                {
                    innoNumber = GameManager.instance.storedConnections[i].GetInnovationnumber();
                    connectionExists = true;
                    break;
                }
            
            
        }
        if(connectionExists)
        {
            network.synapse = new Synapse(In,Out,weight,enabled,innoNumber);
            network.genomeConnectionGenes.Add(network.synapse);
        }
        else
        {
            network.synapse = new Synapse(In,Out,weight,enabled,NetworkManager.innovationNumber);
            network.genomeConnectionGenes.Add(network.synapse);
            GameManager.instance.storedConnections.Add(network.synapse);
            NetworkManager.innovationNumber++;
        }
        
    }

    public float CompatabilityDistance(List<Synapse> genomeA, List<Synapse> genomeB)
    {
        List<Synapse> excessOrDisjoint = new List<Synapse>();
        float avgWeightDistance = 0.0f;
        int numOfMatchings = 0;

        //find excess/disjoint genes of both genomes
        for(int i = 0; i < genomeA.Count; i++)
        {
           bool matchingFound = false;

            for(int j = 0; j < genomeB.Count; j++)
            {
               // Debug.Log("parent 1: " + genomeA[i].GetInnovationnumber() +" == "+ genomeB[j].GetInnovationnumber());
                if(genomeA[i].GetInnovationnumber() == genomeB[j].GetInnovationnumber())
                {
                    matchingFound = true;
                    //calculate average weighht difference along all matching connections
                    avgWeightDistance += Mathf.Abs(genomeA[i].GetWeight() - genomeB[j].GetWeight());
                    numOfMatchings++;
                    break;
                }
            
            }  

            if(!matchingFound)
            {
                excessOrDisjoint.Add(genomeA[i]);
            }
           
        }
       // Debug.Log("weight: " + avgWeightDistance + " / " + numOfMatchings);
        avgWeightDistance = (float)avgWeightDistance/numOfMatchings;
        if(float.IsNaN(avgWeightDistance)) avgWeightDistance = 0;


        for(int i = 0; i < genomeB.Count; i++)
        {
           bool matchingFound = false;

            for(int j = 0; j < genomeA.Count; j++)
            {
               // Debug.Log("parent 2: " + genomeB[i].GetInnovationnumber() +" == "+ genomeA[j].GetInnovationnumber());
                if(genomeB[i].GetInnovationnumber() == genomeA[j].GetInnovationnumber())
                {
                    matchingFound = true;
                    break;
                }
            
            }  

            if(!matchingFound)
            {
                excessOrDisjoint.Add(genomeB[i]);
            }
           
        }
        
      //  Debug.Log("Excess: "+excessOrDisjoint.Count);
        float c1 = 1.0f;
        float c2 = 1.0f;

       /* 
        if(genomeA.Count < 20 && genomeB.Count < 20)
        {
            Debug.Log(c1 + " * " + excessOrDisjoint.Count + " + " + c2 + " * " + avgWeightDistance );
            float temp = (c1*excessOrDisjoint.Count) + (c2*avgWeightDistance);
            if(float.IsNaN(temp)) temp = 0;
            return temp;
            
        }
        
        else
        {
            float temp =((c1*excessOrDisjoint.Count)/genomeA.Count > genomeB.Count ? genomeA.Count:genomeB.Count) + (c2*avgWeightDistance);
            if(float.IsNaN(temp)) temp = 0;
            return temp;
             
        }


*/ 
/*     
     for(int i = 0; i < genomeA.Count; i++)
        {
            Debug.Log("A: " +genomeA[i].GetIn().nodeNumber + " " + genomeA[i].GetOut().nodeNumber);
        }
        for (int i = 0; i < genomeB.Count; i++)
        {
            Debug.Log("B: " +genomeB[i].GetIn().nodeNumber + " " + genomeB[i].GetOut().nodeNumber);
        }
*/
        float temp = (c1*excessOrDisjoint.Count) + (c2*avgWeightDistance);
            Debug.Log(excessOrDisjoint.Count + " + " + avgWeightDistance );
            if(float.IsNaN(temp)) temp = 0;
            return temp;        
        

        //plan;
        //setting comp. treshhold to 3
        //since the driving factor for the compatability value are excess/disjoint genes, 3 indicates that likely 3 genes dont match and
        //this is my threshhold for bbeing too different to be in the same species





    }



    
}
