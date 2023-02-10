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
    //public int numOfSpecies = 1;
    public float addNodeProb = 0.03f;
    public float addConnectionProb = 0.05f;
    public float alterWeightProb = 0.8f;
    float delayForRestart = 3.0f;
    float restart = 0.0f;
    bool evolved;
    public int gen = 1;
    public NetworkManager nManager;
    TextMeshProUGUI generation;
    [HideInInspector]
    public TextMeshProUGUI speciesTxt;
    TextMeshProUGUI runTxt;
    [HideInInspector]
    public int countSpecies;
    [HideInInspector]
    public int punishedSpecies = -1;

    
    //if all boards are gameover, i need to find out
    //idea: iterate over listofboards and their gameover field and if all are true, we start evolving
    bool startEvolving;
    bool boardsSpawned;
    List<GameObject> offSpring;
    public List<Synapse> storedConnections;
    //public List<List<Snake>> species;
    //public List<List<GameObject>> species;
    public List<Species> species;
    int speciesNum = 1;
    List<int> speciesHighestFitness;
 

    public float compat_threshhold = 4.0f;
    float compat_modifier = 0.3f;
    public int num_species_target = 3;
    public float mutationPower = 2.5f;
    public float geneReenableProb = 0f;
    public bool multipleStructuralMutations = false;
    public float randomWeightProb = 0.1f;
    public bool developMode = false;
    public float eliminationPercentile = 0.8f;
    public int noImprovementDropoff = 15;
    public bool elitism;
    public int speciesMaturationTime = 10;
    //EXPERIMENTAL
    public int runsPerGen = 20;
    [HideInInspector]
    public int run = 0;
    public bool multiRunsPerGen;
    [HideInInspector]
    public bool nextGen = false;
    public bool loadSaveFile;
    //EXPERIMENTAL


    float sumOfAdjFitnesses = 0.0f;
    List<GameObject> snakes;
    public List<Network> networks;
    List<GameObject> inactiveBoards;
    public int boardNumber = 0;
    List<int> offspringNum;
    public List<int> speciesNumsCopy;



    void Awake()
    {
        instance = this;
        //need to take care of this
         nManager = new NetworkManager();
         storedConnections = new List<Synapse>();
        // species = new List<List<GameObject>>();
        species = new List<Species>();
         inactiveBoards = new List<GameObject>();
        speciesHighestFitness = new List<int>();

    }
    void Start()
    {
        generation = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        generation.text = "Generation: " + gen;
        speciesTxt = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        speciesTxt.text = "Species: " + species.Count;
        runTxt = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        runTxt.text = "Run: " + run;
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

        
    }

    void Update()
    {
         
         //only called once in the first gen
        if(listOfBoards.Count == 0)
        {
            boardsSpawned = true;
            SpawnBoards();
            for(int i = 0; i < listOfBoards.Count; i++)
            {
                listOfBoards[i].GetComponent<Board>().spawnSnake();
                listOfBoards[i].GetComponent<Board>().attachSnake();
            }
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
                    if(developMode)
                    {
                        restart = Time.time + 3.0f;
                    }
                    else
                    {
                        restart = Time.time + 0.5f;
                    }
                    
                    
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
                if(!multiRunsPerGen)
                {
                    gen += 1;
                    generation.text = "Generation: " + gen;
                    //  speciesTxt.text = "Species: " + species.Count;
                    evolved = true;
                    startEvolving = false;
                    boardsSpawned = false;




                    snakes = new List<GameObject>();
                    //taking in all the snakes
                    //for later: handle speciation if everything works

                    for (int j = 0; j < listOfBoards.Count; j++)
                    {

                        snakes.Add(listOfBoards[j].GetComponent<Board>().snakeInstance);
                    }

                    int num_species = species.Count;
                    if (num_species < num_species_target)
                    {
                        compat_threshhold -= compat_modifier;
                    }
                    else if (num_species > num_species_target)
                    {
                        compat_threshhold += compat_modifier;
                    }


                    if (compat_threshhold < 0.3f)
                    {
                        compat_threshhold = 0.3f;
                    }

                    //EXPERIMENTAL//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //idea of steps:
                    //evaluate snakes based on fitness
                    //eliminate
                    //calculate offspring
                    //reproduce
                    //speciate
                    Evolution.UpdateSpeciesMetrics();
                    Evolution.Evaluate(snakes);
                    Evolution.Eliminate();
                    Evolution.CalculateOffspring();
                    Evolution.RemoveWorstPerformers(GameManager.instance.species);
                    Evolution.FindElites();
                    Evolution.Reproduce();

                    while (inactiveBoards.Count > 0)
                    {
                        Destroy(inactiveBoards[0]);
                        inactiveBoards.RemoveAt(0);
                    }
                    inactiveBoards = new List<GameObject>();
                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        inactiveBoards.Add(listOfBoards[i]);
                        inactiveBoards[i].SetActive(false);
                    }




                    listOfBoards = new List<GameObject>();

                    SpawnBoards();
                    NetworkManager.instance.listOfNetworks = null;
                    NetworkManager.instance.listOfNetworks = new List<Network>();

                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        listOfBoards[i].GetComponent<Board>().spawnSnake();
                        listOfBoards[i].GetComponent<Board>().attachSnake();
                        listOfBoards[i].GetComponent<Board>().childBrain = networks[i];
                    }

                    snakes = new List<GameObject>();
                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        snakes.Add(listOfBoards[i].GetComponent<Board>().snakeInstance);
                        snakes[i].GetComponent<Snake>().brain = listOfBoards[i].GetComponent<Board>().childBrain;
                    }


                    Evolution.Speciate(snakes);

                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        listOfBoards[i].GetComponent<Board>().species = listOfBoards[i].GetComponent<Board>().snakeScript.species;
                        listOfBoards[i].GetComponent<Board>().start_drawing_nn = true;

                    }

                }
                else
                {
                    //save the gamestate every x runs
                    if(run > 0 && run % 30 == 0)
                    {
                        //determining best performing species
                        int maxFitnessIndex = 0;
                        for(int i = 0; i < species.Count-1; i++)
                        {
                            if (species[i].getMaxFitness() < species[i+1].getMaxFitness())
                            {
                                maxFitnessIndex = i + 1;
                            }
                        }
                        //select a random snake from the species
                        int rnd = Random.Range(0, species[maxFitnessIndex].snakes.Count);
                        Network network = species[maxFitnessIndex].snakes[rnd].GetComponent<Snake>().brain;

                        network.layers1D = new List<Node>();
                        network.nodesPerLayer = new List<int>();
                        for (int i = 0; i < network.layers.Count; i++)
                        {
                            network.nodesPerLayer.Add(network.layers[i].Count);
                            for(int j = 0; j < network.layers[i].Count; j++)
                            {
                                network.layers1D.Add(network.layers[i][j]);
                            }
                        }


                        SaveData.network = network;
                        SaveData.save();
                    }
                   

                   
                    if(run > 0 && run % runsPerGen == 0)
                    {
                        gen += 1;
                        run += 1;
                        nextGen = true;
                    }
                    else
                    {
                        run += 1;
                        nextGen = false;
                    }

                    //sets the run counter active
                    if(transform.GetChild(0).GetChild(2).gameObject.activeSelf == false)
                    {
                        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    }
                    
                    generation.text = "Generation: " + gen;
                    runTxt.text = "Run: " + run;
                    //  speciesTxt.text = "Species: " + species.Count;
                    evolved = true;
                    startEvolving = false;
                    boardsSpawned = false;




                    snakes = new List<GameObject>();
                  
                    for (int j = 0; j < listOfBoards.Count; j++)
                    {

                        snakes.Add(listOfBoards[j].GetComponent<Board>().snakeInstance);
                    }

                    if(nextGen)
                    {
                        int num_species = species.Count;
                        if (num_species < num_species_target)
                        {
                            compat_threshhold -= compat_modifier;
                        }
                        else if (num_species > num_species_target)
                        {
                            compat_threshhold += compat_modifier;
                        }


                        if (compat_threshhold < 0.3f)
                        {
                            compat_threshhold = 0.3f;
                        }
                    }
                   

                    //EXPERIMENTAL//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //idea of steps:
                    //evaluate snakes based on fitness
                    //eliminate
                    //calculate offspring
                    //reproduce
                    //speciate
                    Evolution.UpdateSpeciesMetrics();
                    Evolution.Evaluate(snakes);
                    if(nextGen)
                    {
                        Evolution.Eliminate();
                        Evolution.CalculateOffspring();
                        Evolution.RemoveWorstPerformers(GameManager.instance.species);
                        Evolution.FindElites();
                    }
                    speciesNumsCopy = new List<int>();
                    
                    Evolution.Reproduce();

                    /*         List<int> speciesNumsCopy = new List<int>();
                             for(int i = 0; i < listOfBoards.Count; i++)
                             {
                                 speciesNumsCopy.Add(listOfBoards[i].GetComponent<Board>().snakeScript.species);
                             }
          */
                    List<float> avgFitnesses = new List<float>();
                    if(!nextGen)
                    {
                        for (int i = 0; i < listOfBoards.Count; i++)
                        {
                            avgFitnesses.Add(listOfBoards[i].GetComponent<Board>().avgFitness);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listOfBoards.Count; i++)
                        {
                            avgFitnesses.Add(0);
                        }
                    }
                    

                    if (nextGen &&  44 == 33)
                    {
                        while (inactiveBoards.Count > 0)
                        {
                            Destroy(inactiveBoards[0]);
                            inactiveBoards.RemoveAt(0);
                        }
                        inactiveBoards = new List<GameObject>();
                        for (int i = 0; i < listOfBoards.Count; i++)
                        {
                            inactiveBoards.Add(listOfBoards[i]);
                            inactiveBoards[i].SetActive(false);
                        }
                    }
                    else
                    {
                        while(listOfBoards.Count > 0)
                        {
                            Destroy(listOfBoards[0]);
                            listOfBoards.RemoveAt(0);
                        }
                    }


                   


                    listOfBoards = new List<GameObject>();

                    SpawnBoards();
                    NetworkManager.instance.listOfNetworks = null;
                    NetworkManager.instance.listOfNetworks = new List<Network>();
                 

                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        listOfBoards[i].GetComponent<Board>().spawnSnake();
                        listOfBoards[i].GetComponent<Board>().attachSnake();
                        listOfBoards[i].GetComponent<Board>().childBrain = networks[i];
                        listOfBoards[i].GetComponent<Board>().avgFitness = avgFitnesses[i];
                    }

                    snakes = new List<GameObject>();
                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        snakes.Add(listOfBoards[i].GetComponent<Board>().snakeInstance);
                        snakes[i].GetComponent<Snake>().brain = listOfBoards[i].GetComponent<Board>().childBrain;

                    }

                    if(nextGen)
                    {
                        Evolution.Speciate(snakes);

                        //reset species metrics to 0 where needed
                        for (int i = 0; i < species.Count; i++)
                        {
                            species[i].resetAdjFitness();
                            species[i].resetAvgFitness();
                        }
                    }

                    

                    for (int i = 0; i < listOfBoards.Count; i++)
                    {
                        if(nextGen)
                        {
                            listOfBoards[i].GetComponent<Board>().species = listOfBoards[i].GetComponent<Board>().snakeScript.species;
                        }
                        else
                        {
                            listOfBoards[i].GetComponent<Board>().species = speciesNumsCopy[i];
                            listOfBoards[i].GetComponent<Board>().snakeScript.species = speciesNumsCopy[i];
                        }
                        
                        listOfBoards[i].GetComponent<Board>().start_drawing_nn = true;

                    }

                    //when nextgen and we want the boards to be sorted according to increasing species numbers,
                    //we need to resort the boards in listofboards or the first gen after nextgen will be unsorted
                    //new idea: just try to swap the snakeinstances of the boards, this doesnt need complicated bboard swapping whichh
                    //includes swapping positions
                    //only possible contra: if same species, it shouldnt swap, bbut does this prevent intra-species position swapping?
                  /* listOfBoards = Evolution.SelectionSort(listOfBoards);
                    if(nextGen)
                    {
                        for(int i = 0; i < listOfBoards.Count; i++)
                        {
                            listOfBoards[i].GetComponent<Board>().species = listOfBoards[i].GetComponent<Board>().snakeInstance.GetComponent<Snake>().species;
                            listOfBoards[i].GetComponent<Board>().snakeScript.species = listOfBoards[i].GetComponent<Board>().snakeInstance.GetComponent<Snake>().species;
                        }
                    }
                  */  
                    for (int i = 0; i < species.Count; i++)
                    {
                        species[i].snakes.Clear();
                    }
                    
                    for(int i = 0; i  < snakes.Count; i++)
                    {
                        //todo:
                        //below thhrows error sometimes
                        //and: although i paid attention to always give the same board the same snake with the same network
                        //within !nextgen runs the networks switch around on the boards
                    
                     //   species[snakes[i].GetComponent<Snake>().species - 1].AddSnake(snakes[i]);
                        for(int j = 0; j < species.Count; j++)
                        {
                            if (snakes[i].GetComponent<Snake>().species == species[j].getId())
                            {
                                species[j].AddSnake(snakes[i]);
                            }

                        }

                    }



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
        
        //future plan: calculate bboardsperColumn and boardsPerRow by Math.ceil(sqrt(populationsize))
        for(int i = 0; i < boardsPerColumn; i++)
        {
            for(int j = 0; j < boardsPerRow; j++)
            {
               
                if((numOfBoards - (i*boardsPerRow+j))> 0)
                {

                    ++boardNumber;
                    boardGameObjectLoc = new Vector2((j*Board.boarderSizeX) + (j*30), (i*Board.boarderSizeX)+ (i*15));
                    listOfBoards.Add(Instantiate(board, boardGameObjectLoc, Quaternion.identity));

                    //listOfBoards[i*boardsPerRow+j].transform.parent = this.gameObject.transform;
                    //listOfBoards[i*boardsPerRow+j].name = "Board";
                    listOfBoards[listOfBoards.Count-1].transform.parent = this.gameObject.transform;
                    listOfBoards[listOfBoards.Count - 1].name = "Board";
                }
            }
        }
    }
    
    

}




