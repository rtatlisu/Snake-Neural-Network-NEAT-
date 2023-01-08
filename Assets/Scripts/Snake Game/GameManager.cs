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
    TextMeshProUGUI speciesTxt;
    
    //if all boards are gameover, i need to find out
    //idea: iterate over listofboards and their gameover field and if all are true, we start evolving
    bool startEvolving;
    bool boardsSpawned;
    List<GameObject> offSpring;
    public List<Synapse> storedConnections;
    //public List<List<Snake>> species;
    public List<List<GameObject>> species;
    int speciesNum = 1;
    float compatabilityTreshhold = 3.0f;
    float sumOfAdjFitnesses = 0.0f;
    List<GameObject> snakes;
    List<Network> networks;
    List<GameObject> inactiveBoards;
    public int boardNumber = 0;







    void Awake()
    {
        instance = this;
        //need to take care of this
         nManager = new NetworkManager();
         storedConnections = new List<Synapse>();
         species = new List<List<GameObject>>();
         inactiveBoards = new List<GameObject>();

    }
    void Start()
    {
        generation = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        generation.text = "Generation: " + gen;
        speciesTxt = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        speciesTxt.text = "Species: " + species.Count;
    }

    void Update()
    {
         
         //only called once in the first gen
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

                gen +=1;
                generation.text = "Generation: " + gen;
                speciesTxt.text = "Species: " + species.Count;
                evolved = true;
                startEvolving = false;
                boardsSpawned = false;



                
                snakes = new List<GameObject>(); 
                 //taking in all the snakes
                 //for later: handle speciation if everything works
                 for(int j = 0; j < listOfBoards.Count; j++)
                 {
                     snakes.Add(listOfBoards[j].GetComponent<Board>().snakeInstance);

                 }
       
            //EXPERIMENTAL
            /*
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
               */
            //todo:
            //something is off with the compatability calculation in networkmanager
            //ad-hoc solution: for every species, create a childSpecies list directly correlated to the species list, i.e.
            //species[0] analogue to childSPecies[0]
            //species[x][0] will be the representative for allocating the species to the childs and after all snakes have been
            //assigned a species, i.e. have been put in one of the childSpecies lists, we say species[x] = childSpecies[x] for
            //all lists
            //important case:
            //if one of the x childSpecies remains empty at the end, thhis means that a species "goes extinct" and will no longer
            //bbe represented in the next gen. thhus, after referencing the empty list in species[x], we hhave to check in the next iteration
            //if species[x] is empty and if so, delete it
            //second important case:
            //when a new species is created, we could theoretically just deal with one more list of species than childSpecies, but
            //this could be unhandy for the referencing at the end so we will instead to the following:
            //we create a new species list, but also a new childSpecies list and bboth new lists will referene the same snake(s),
            //the referencing at the end is useless for these lists, but prevents indexoutofbounds exceptions
            //after implementing the strategy, we no longer need the bool list "parentsRemoved"

                //SPECIATION
                //checking if there are members in a species that have bbeen deleted bbut are still referenced in thhe list -> delete the reference
                /*
                for(int i = 0; i < species.Count; i++)
                {
                    for(int j = 0; j < species[i].Count; j++)
                    {
                        if(species[i][j] == null)
                        {
                            print("soecies: "+species[i][j]);
                            species[i].RemoveAt(j);

                        }
                    }

                    if(species[i].Count == 0)
                    {
                        species.RemoveAt(i);
                    }
                    
                }
                 */
               List<bool> parentsRemoved = new List<bool>();
                List<List<GameObject>> childSpecies = new List<List<GameObject>>();

        /*
                if(species.Count > 0)
                {
                    for(int x = 0; x < species.Count; x++)
                    {
                        parentsRemoved.Add(false);
                    }
                }
                else if(species.Count == 0)
                {
                    parentsRemoved.Add(true);
                }
         */    
                for(int i = 0; i < species.Count; i++)
                {
                    childSpecies.Add(new List<GameObject>());
                }
               

                
                for(int i = 0; i < snakes.Count; i++)
                {

                    //here we dont need childspecies list because there are not species yet, so species consists of the childs basically
                    if(species.Count == 0)
                    {
                        species.Add(new List<GameObject>());
                        species[0].Add(snakes[i]);
                        
                 
       
                        snakes[i].GetComponent<Snake>().boardScript.species = speciesNum;
                    }
                    else if(species.Count > 0 && gen == 2)
                    {

                        bool createSpecies = false;
                        for(int j = 0; j < species.Count; j++)
                        {
                            //comp. threshhold = 3
                            //if >= 3, snake doesnt fit in the compared species
                           

                            if(NetworkManager.instance.CompatabilityDistance(snakes[i].GetComponent<Snake>().brain.genomeConnectionGenes,
                            species[j][0].GetComponent<Snake>().brain.genomeConnectionGenes) >= compatabilityTreshhold)
                            {
                                createSpecies = true;
                                
                            }
                            else
                            {
                                createSpecies = false;
                                 int temp = species[j][0].GetComponent<Snake>().boardScript.species;
                             
          
                                 //when the parental genes are still in the list as a reference to know which species a snake should belong to, we
                                 //add the first matchhing snake into that list after we cleared all parents
                                 //from there on, when a snake is added to the same list, the list will not be cleared since it contains only children
/*
                                if(parentsRemoved[j] == false)
                                 {
                                    species[j].Clear();
                                    parentsRemoved[j] = true;
                                 }
*/
                                species[j].Add(snakes[i]);
                           //     Debug.Log("2: "+species[j][species[j].Count-1]);
                                snakes[i].GetComponent<Snake>().boardScript.species = temp;
                                break;
                            }
                        }

                        if(createSpecies)
                        {
                            ++speciesNum;
                            species.Add(new List<GameObject>());
                       
                            species[species.Count-1].Add(snakes[i]);
                
                         //   Debug.Log("3: " + species[species.Count-1][species[species.Count-1].Count-1]);
                            snakes[i].GetComponent<Snake>().boardScript.species = speciesNum;
                            parentsRemoved.Add(true);

                            
                        }
                    }
                    else if(species.Count > 0 && gen > 2)
                    {
                        bool createSpecies = false;
                        for(int j = 0; j < species.Count; j++)
                        {
                            //comp. threshhold = 3
                            //if >= 3, snake doesnt fit in the compared species
                     
                            print("A: " + snakes[i].GetComponent<Snake>().boardScript.boardNumber + " B: " +
                            species[j][0].GetComponent<Snake>().boardScript.boardNumber);
                            if(NetworkManager.instance.CompatabilityDistance(snakes[i].GetComponent<Snake>().brain.genomeConnectionGenes,
                            species[j][0].GetComponent<Snake>().brain.genomeConnectionGenes) >= compatabilityTreshhold)
                            {
                                createSpecies = true;
                            }
                            else
                            {
                                createSpecies = false;
                                 int temp = species[j][0].GetComponent<Snake>().boardScript.species;
                             
          
                                 //when the parental genes are still in the list as a reference to know which species a snake should belong to, we
                                 //add the first matchhing snake into that list after we cleared all parents
                                 //from there on, when a snake is added to the same list, the list will not be cleared since it contains only children

                                childSpecies[j].Add(snakes[i]);
                           //     Debug.Log("2: "+species[j][species[j].Count-1]);
                                snakes[i].GetComponent<Snake>().boardScript.species = temp;
                                break;
                            }
                        }

                        if(createSpecies)
                        {
                            ++speciesNum;
                            species.Add(new List<GameObject>());
                            childSpecies.Add(new List<GameObject>());
                            species[species.Count-1].Add(snakes[i]);
                            childSpecies[childSpecies.Count-1].Add(snakes[i]);
                         //   Debug.Log("3: " + species[species.Count-1][species[species.Count-1].Count-1]);
                            snakes[i].GetComponent<Snake>().boardScript.species = speciesNum;
                 
                            
                        }

                    }
                  
                  
                }

                //this loop solves the case of having more species in the parental generation than in thhe upcoming generation
                //having fewer species in the new generation means we have empty species in the childspecies list whichh shouldnt
                //be passed to the parent
                for(int i = 0; i < childSpecies.Count; i++)
                {
                    if (childSpecies[i].Count == 0)
                    {
                        childSpecies.RemoveAt(i);
                    }
                }

                if(gen > 2)
                {
                    species = childSpecies;
                }

            //todo:
            //problem is the following: when a species consists of only 1 member, it cant reproduce and causes an error
            //how it occured: whhen snake A differs from snake B so much that one of the snakes is put in another species, we have two 1-member species
            //solution: asexual reproduction, i.e. reproduction with 1 member needs to be implemented
            
               networks = new List<Network>();
           
                if(species.Count== 0)
                {
                   
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
                        //EXPERIMENTAL
                       // listOfBoards[i].GetComponent<Board>().childBrain = network;
                       networks.Add(network);
                    }
                  
                }
                else
                {
                    //calculate adjustedfitness for each snake in each species and sum up the adjusted fitness of each snake per species
                    List<float> adjFitnessesList = new List<float>();
                    for(int i = 0; i < species.Count; i++)
                    {
                        for(int j = 0; j < species[i].Count; j++)
                        {
                            //todo: species with that component is destroyed somehow
                         //   Debug.Log(/*species[i][j]*/speciesTest[i][j] + " i: " + i + " j: " + j);
                          sumOfAdjFitnesses +=  AdjustedFitness(species[i][j].GetComponent<Snake>(), species[i]);
        
                        }
                        adjFitnessesList.Add(sumOfAdjFitnesses);
                        sumOfAdjFitnesses = 0;
                        
                    }
                    float divisor = 0.0f;
                    
                    //the divisor to specify how many chhildren a species can hhave is calculated here in form of the sum of all adj fitnesses
                    for(int i = 0; i < adjFitnessesList.Count; i++)
                    {
                        divisor += adjFitnessesList[i];
                    }
                    if(divisor == 0)
                    {
                        divisor = 1.0f;
                    }
                    //list of decimal values representing the percentage of offspring each species gets
                    List<float> offspringStake = new List<float>();
                   
                    for(int i = 0; i < adjFitnessesList.Count; i++)
                    {
                        offspringStake.Add((float)adjFitnessesList[i]/divisor);
//                        print("offspringstake: "+adjFitnessesList[i] + " / " + divisor);
                    }
                    //if all adjusted fitnesses of a species are 0, we assign 1 to the offspringstake since othherwise the species wont reproduce
                    //this could be desirable, but especially when starting the game and having only 1 species, we need to ensure them to reproduce
                    for(int i = 0; i < offspringStake.Count; i++)
                    {
                        if(offspringStake[i] == 0)
                        {
                            offspringStake[i] = 1;
                        }
                    }
                    //turning the percentages into integers, i.e. actual number of offspring
                    List<int> offspringNum = new List<int>();
                    int tracker = 0;
                    for(int i = 0; i < offspringStake.Count; i++)
                    {
                        
                       // print(offspringStake[i] + " * " + numOfBoards + " ceil:  " + Mathf.Ceil(offspringStake[i]*numOfBoards));
                       
                        if((tracker +  (int)Mathf.Ceil(offspringStake[i] * numOfBoards))> numOfBoards)
                        {
                            //31 > 30
                            // add numofspecies - tracker
                         
                            if(tracker == numOfBoards)
                            {
                                offspringNum.Add(1);
                            }
                            else
                            {
                                offspringNum.Add(numOfBoards - tracker);
                            }
                        }
                        //cases < and == are fine in the nested else
                        else
                        {
                            tracker += (int)Mathf.Ceil(offspringStake[i] * numOfBoards);

                            if ((int)Mathf.Ceil(offspringStake[i] * numOfBoards) > species[i].Count)
                            {
                                offspringNum.Add(species[i].Count);
                            }
                            else
                            {
                                offspringNum.Add((int)Mathf.Ceil(offspringStake[i] * numOfBoards));
                            }
                                                  
                        }
                    }

              
                    for(int i = 0; i < offspringNum.Count; i++)
                    {
                        for(int j = 0; j < offspringNum[i]; j++)
                        {

                             Network network =   Evolution.Crossover(Evolution.Selection(species[i]));
                           
//                             print(species[0][0].GetComponent<Snake>().brain.genomeConnectionGenes.Count + " " +
 //                            species[0][1].GetComponent<Snake>().brain.genomeConnectionGenes.Count);

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
                            //EXPERIMENTAL
                            //listOfBoards[i].GetComponent<Board>().childBrain = network;
                            networks.Add(network);
                        }
                    }

                }
                    

                List<int> speciesNums = new List<int>();
                for(int i = 0; i < listOfBoards.Count; i++)
                {
                    speciesNums.Add(listOfBoards[i].GetComponent<Board>().species);
                }

               //todo:
               //when former parents(boards) have been used to speciate the new generation and hece, there are double thhe amount of
               //boards that should be there, i can delete half the boards, i.e. the former generation

                if(listOfBoards.Count != 0)
                {
                    if(inactiveBoards.Count != 0)
                    {
                    while(inactiveBoards.Count > 0)
                    {
                        Destroy(inactiveBoards[0]);
                        inactiveBoards.RemoveAt(0);
                    }
                    }
                    
                    for(int i = 0; i < listOfBoards.Count; i++)
                    {
                        listOfBoards[i].SetActive(false);
                        inactiveBoards.Add(listOfBoards[i]);

                    }
                    listOfBoards = new List<GameObject>();
                    
                }
       
            
                SpawnBoards();
                NetworkManager.instance.listOfNetworks = null;
                NetworkManager.instance.listOfNetworks = new List<Network>();

//            print(networks.Count);
                for (int i = 0; i < listOfBoards.Count; i++)
                {
                   // listOfBoards[i].GetComponent<Board>().species = speciesNums[i];
                   listOfBoards[i].GetComponent<Board>().species = snakes[i].GetComponent<Snake>().boardScript.species;
                    listOfBoards[i].GetComponent<Board>().childBrain = networks[i];
                    
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
                
                listOfBoards[i*boardsPerRow+j].transform.parent = this.gameObject.transform;
                listOfBoards[i*boardsPerRow+j].name = "Board";
                }
            }
        }
    }
    

    float AdjustedFitness(Snake snake, List<GameObject> speciesPop)
    {
       
        return (float)snake.boardScript.fitness/speciesPop.Count;
    }
 


}


