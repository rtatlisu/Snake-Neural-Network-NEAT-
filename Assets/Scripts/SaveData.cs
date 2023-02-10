using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public static class SaveData
{
   // public static SaveData instance;
   public static Network network;
/*
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("duplicate instances shouldnt exist");
        }
    }
*/

    public static void save()
    {
       
        string s = JsonUtility.ToJson(network,true);
        File.WriteAllText("Assets/Scripts" + "/testData.json", s);
        Debug.Log("json file created");
        Debug.Log(Application.persistentDataPath);
    }

    public static void load()
    {
        if(File.Exists("Assets/Scripts/testData.json"))
        {
            string contents = File.ReadAllText("Assets/Scripts/testData.json");
            network = JsonUtility.FromJson<Network>(contents);
            network.layers = new List<List<Node>>();
            /*
            for(int i = 0; i < network.nodesPerLayer.Count; i++)
            {
                network.layers.Add(new List<Node>());
                for (int j = 0; j < network.nodesPerLayer[i]; j++)
                {
                    if(i == 0)
                    {
                        network.layers[network.layers.Count - 1].Add(network.layers1D[(i * j) + j]);
                    }
                    else
                    {
                        network.layers[network.layers.Count - 1].Add(network.layers1D[(i * network.nodesPerLayer[i - 1]) + j]);
                    }
         
                }
            }
            */

            for(int i = 0; i < network.layers1D.Count; i++)
            {
                if (network.layers.Count - 1 < network.layers1D[i].layer)
                {
                    network.layers.Add(new List<Node>());
                }
                network.layers[network.layers.Count - 1].Add(network.layers1D[i]);
            }

        }
    }


}

[System.Serializable]
public static class CollectData
{
   
    
}

