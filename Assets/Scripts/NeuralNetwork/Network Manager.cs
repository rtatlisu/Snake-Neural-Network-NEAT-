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
        Debug.Log("NETWORK CREATED");
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



    
}
