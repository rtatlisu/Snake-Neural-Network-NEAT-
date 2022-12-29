using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int numOfBoards = 1;
   public static int curBoards = 0;
    public GameObject board;
    public List<GameObject> listOfBoards;
    Vector2 boardGameObjectLoc;
    public int boardsPerRow = 1;
    public int boardsPerColumn = 1;
    [Range(0.01f,2.0f)]
    public float gameSpeed;
    public int POPULATIONSIZE = 30;
    public int numOfSpecies = 1;
    public float addNodeProb = 0.03f;
    public float addConnectionProb = 0.05f;
    public float alterWeightProb = 0.8f;
    float delayForRestart = 3.0f;
    float restart = 0.0f;
    bool evolved;
    public int gen = 1;
    public NetworkManager nManager;
    TextMeshProUGUI generation;
    
    //if all boards are gameover, i need to find out
    //idea: iterate over listofboards and their gameover field and if all are true, we start evolving
    bool startEvolving;
    bool boardsSpawned;
    List<GameObject> offSpring;
    public List<Synapse> storedConnections;

    

    void Awake()
    {
        instance = this;
        //need to take care of this
         nManager = new NetworkManager();
         storedConnections = new List<Synapse>();

    }
    void Start()
    {
        generation = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        generation.text = "Generation: " + gen;
    }

    void Update()
    {
        if(listOfBoards.Count == 0)
        {
            boardsSpawned = true;
            SpawnBoards();
        }
        

        //could in the future maybe be better implemented via a subscription call
        if(!startEvolving && listOfBoards.Count != 0)
        {
            for(int i = 0; i < listOfBoards.Count; i++)
            {
                if(listOfBoards[i].GetComponent<Board>().gameOver)
                {
                    startEvolving = true;
                    evolved = false;
                    restart = Time.time+3.0f;
                    
                }
                else
                {
                    startEvolving = false;
                    break;
                }
            }

        }
        if(startEvolving)
        {   
            if(Time.time > restart && !evolved)
            {
                 
                gen+=1;
                generation.text = "Generation: " + gen;
                evolved = true;
                startEvolving = false;
                boardsSpawned = false;
                
                List<GameObject> snakes = new List<GameObject>();
                //taking in all the snakes
                //for later: handle speciation if everything works
                for(int j = 0; j < listOfBoards.Count; j++)
                {
                    snakes.Add(listOfBoards[j].GetComponent<Board>().snakeInstance);
                }
              
                if(listOfBoards.Count != 0)
                {
                    while(listOfBoards.Count > 0)
                    {
                        Destroy(listOfBoards[0]);
                        listOfBoards.RemoveAt(0);
           
                    }
                listOfBoards = new List<GameObject>();
                }
                SpawnBoards();
                NetworkManager.instance.listOfNetworks = null;
                NetworkManager.instance.listOfNetworks = new List<Network>();
                Debug.Log("NETWORK DELETED");
                for(int i = 0; i < listOfBoards.Count; i++)
                {

                    Network network =   Evolution.Crossover(Evolution.Selection(snakes));

                    int rnd = Random.Range(1,101);
                    if(rnd <= (addNodeProb*100))
                    {
                        Evolution.AddNodeMutation(network);
                    }
                    rnd = Random.Range(1,101);
                    if(rnd <= (addConnectionProb*100))
                    {
                        //error happens here but i dont know why and whhen
                        Evolution.AddConnectionMutation(network);
                    }
                    rnd = Random.Range(1,101);
                    if(rnd <= (alterWeightProb*100))
                    {
                        Evolution.AlterWeightMutation(network);
                    }
                    listOfBoards[i].GetComponent<Board>().childBrain = network;
                }
            }
        }

    }
   
   public void setCurBoard()
   {
    curBoards++;
   }

    public Vector2 boardSpawn()
    {
        return new Vector2(curBoards*Board.boarderSizeX+curBoards*5, 0);
    }

    void SpawnBoards()
    {
        for(int i = 0; i < boardsPerColumn; i++)
        {
            for(int j = 0; j < boardsPerRow; j++)
            {
               
                if((numOfBoards - (i*boardsPerRow+j))> 0)
                {

               
                boardGameObjectLoc = new Vector2((j*Board.boarderSizeX) + (j*30), (i*Board.boarderSizeX)+ (i*15));
                listOfBoards.Add(Instantiate(board, boardGameObjectLoc, Quaternion.identity));

                listOfBoards[i*boardsPerRow+j].transform.parent = this.gameObject.transform;
                listOfBoards[i*boardsPerRow+j].name = "Board";
                }
            }
        }
    }
}
