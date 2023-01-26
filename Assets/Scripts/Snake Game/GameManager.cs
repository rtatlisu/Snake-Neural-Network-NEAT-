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
    int punishedSpecies = -1;
    
    //if all boards are gameover, i need to find out
    //idea: iterate over listofboards and their gameover field and if all are true, we start evolving
    bool startEvolving;
    bool boardsSpawned;
    List<GameObject> offSpring;
    public List<Synapse> storedConnections;
    //public List<List<Snake>> species;
    public List<List<GameObject>> species;
    int speciesNum = 1;
    List<int> speciesHighestFitness;
    List<speciesInfos> speciesInfos;

    public float compat_threshhold = 3.0f;
    float compat_modifier = 0.3f;
    public int num_species_target = 3;
    public float mutationPower = 2.5f;
    public float geneReenableProb = 0f;
    public bool multipleStructuralMutations = false;
    public float randomWeightProb = 0.1f;
    public bool developMode = false;
    public float eliminationPercentile = 0.8f;
    public int noImprovementDropoff = 15;


    float sumOfAdjFitnesses = 0.0f;
    List<GameObject> snakes;
    List<Network> networks;
    List<GameObject> inactiveBoards;
    public int boardNumber = 0;
    List<int> offspringNum;


    void Awake()
    {
        instance = this;
        //need to take care of this
         nManager = new NetworkManager();
         storedConnections = new List<Synapse>();
         species = new List<List<GameObject>>();
         inactiveBoards = new List<GameObject>();
        speciesHighestFitness = new List<int>();
        speciesInfos = new List<speciesInfos>();

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

                gen +=1;
                generation.text = "Generation: " + gen;
              //  speciesTxt.text = "Species: " + species.Count;
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

                int num_species = species.Count;
                if (num_species < num_species_target)
                {
                    compat_threshhold -= compat_modifier;
                }
                else
                {
                    compat_threshhold += compat_modifier;
                }

                if(compat_threshhold < 0.3f)
                {
                    compat_threshhold = 0.3f;
                }



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
                List<List<GameObject>> childSpecies = new List<List<GameObject>>();

           
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
                         //snakes[i].GetComponent<Snake>().boardScript.species =/* speciesNum*/species.Count;
                        //listOfBoards[i].GetComponent<Board>().species = species.Count;
                    }
                    else if(species.Count > 0 && gen == 2)
                    {
                        bool createSpecies = false;
                        for(int j = 0; j < species.Count; j++)
                        {
                            //comp. threshhold = 3
                            //if >= 3, snake doesnt fit in the compared species
                            int rnd = Random.Range(0, species[j].Count);
                            if(NetworkManager.instance.CompatabilityDistance(snakes[i].GetComponent<Snake>().brain.genomeConnectionGenes,
                            species[j][rnd].GetComponent<Snake>().brain.genomeConnectionGenes) >= compat_threshhold)
                            {
                                createSpecies = true;                            
                            }
                            else
                            {
                                createSpecies = false;
                                int temp = species[j][rnd].GetComponent<Snake>().boardScript.species;
                                species[j].Add(snakes[i]);
                               //  snakes[i].GetComponent<Snake>().boardScript.species = temp;
                               // listOfBoards[i].GetComponent<Board>().species = temp;
                                break;
                            }
                        }

                        if(createSpecies)
                        {
                           // ++speciesNum;
                            species.Add(new List<GameObject>());                       
                            species[species.Count-1].Add(snakes[i]);
                            // snakes[i].GetComponent<Snake>().boardScript.species = /*speciesNum*/species.Count;
                           // listOfBoards[i].GetComponent<Board>().species = species.Count;   
                        }
                    }
                    //prolem:
                    //if a new species is created, it is not considered for future snakes to be classified, hence there is the scenario
                    //where we go from 1-2 species to 5-6 instantly, even though many of the species could belong together
                    //ad-hoc solution: when a new species is created, we re-classify every snake
                    //2nd solution: normally the species list only consists of the parental generation, but as we create a new species(childspecies)
                    //we could also create a new species(parental) and let it function as if it were from the last gen
                    //2nd solution will be attempted first
                    else if(species.Count > 0 && gen > 2)
                    {
                        bool createSpecies = false;
                        for(int j = 0; j < species.Count; j++)
                        {
                            //comp. threshhold = 3
                            //if >= 3, snake doesnt fit in the compared species
                            //problem:
                            //we seem to have a higher species count than actual species in the parental generation before
                            //assigning the now children to thhe species list

                            //print(species.Count + " " + j + " " + species[j].Count);
                            int rnd = Random.Range(0, species[j].Count);
                            if (NetworkManager.instance.CompatabilityDistance(snakes[i].GetComponent<Snake>().brain.genomeConnectionGenes,
                            species[j][rnd].GetComponent<Snake>().brain.genomeConnectionGenes) >= compat_threshhold)
                            {
                                createSpecies = true;
                            }
                            else
                            {
                                createSpecies = false;
                                int temp = species[j][rnd].GetComponent<Snake>().boardScript.species;
                                childSpecies[j].Add(snakes[i]);
                                // snakes[i].GetComponent<Snake>().boardScript.species = temp;
                                //listOfBoards[i].GetComponent<Board>().species = temp;
                                break;
                            }
                        }

                        if(createSpecies)
                        {
                            //speciesnum needs to be reworked probably
                           // ++speciesNum;
                            species.Add(new List<GameObject>());
                            childSpecies.Add(new List<GameObject>());
                            species[species.Count-1].Add(snakes[i]);
                            childSpecies[childSpecies.Count-1].Add(snakes[i]);
                            // snakes[i].GetComponent<Snake>().boardScript.species = /*speciesNum*/species.Count;
                            //listOfBoards[i].GetComponent<Board>().species = species.Count;
                        }
                    }       
                }

                //this loop solves the case of having more species in the parental generation than in thhe upcoming generation
                //having fewer species in the new generation means we have empty species in the childspecies list whichh shouldnt
                //be passed to the parent

           
                int it = 0;
                while (it < childSpecies.Count)
                {
                    if (childSpecies[it].Count == 0)
                    {
                        childSpecies.RemoveAt(it);
                        it = 0;
                    }
                    else
                    {
                        it++;
                    }
                }
               

                if(gen > 2)
                {
                    species = childSpecies;
                }



                //selection, crossover, mutation
                networks = new List<Network>();
           
               
                
                    //calculate adjustedfitness for each snake in each species and sum up the adjusted fitness of each snake per species
                    List<float> adjFitnessesList = new List<float>();
                    for(int i = 0; i < species.Count; i++)
                    {
                        
                        for (int j = 0; j < species[i].Count; j++)
                        {
                            sumOfAdjFitnesses += AdjustedFitness(species[i][j].GetComponent<Snake>(), species[i]);
                        }
                        adjFitnessesList.Add(sumOfAdjFitnesses);
                        sumOfAdjFitnesses = 0;
                       

                    }
                    if (punishedSpecies != -1)
                    {
                      
                        for(int i = 0; i < species.Count; i++)
                        {
                            if (species[i][0].GetComponent<Snake>().boardScript.species == punishedSpecies)
                            {
                                adjFitnessesList[i] = 0;
                                break;
                            }
                        }
                        
                        punishedSpecies = -1;
                 
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
                    }
                    //if all adjusted fitnesses of a species are 0, we assign 1 to the offspringstake since othherwise the species wont reproduce
                    //this could be desirable, but especially when starting the game and having only 1 species, we need to ensure them to reproduce
                    //adjustment to above:
                    //case 1: if we only have 1 species and the sum of adjusted fitnesses is 0, below applies
                    //case 2: if we have multiple species and every species has a sum of adjusted fitnesses of 0, below applies for one species
                    
                    bool allZero = false;
                    if(offspringStake.Count == 1 && offspringStake[0] == 0)
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
                        if(allZero)
                        {
                            int rnd = Random.Range(0, offspringStake.Count);
                            offspringStake[rnd] = 1;
                        }

                    }

                    
        
                    //turning the percentages into integers, i.e. actual number of offspring
                    offspringNum = new List<int>();
                    offspringNum = OffspringDistribution(offspringStake);
                    int countSpecies = 0;
                    //todo:
                    //imlement the speciestxt code in the first for loop but only if offspringnum[i].count != 0
                    for(int i = 0; i < offspringNum.Count; i++)
                    {
                        EliminateWorst(species[i]);
                        if (offspringNum[i] != 0)
                        {
                            countSpecies++;
                        }
                        for(int j = 0; j < offspringNum[i]; j++)
                        {
                            Network network =   Evolution.Crossover(Evolution.Selection(species[i]));

                            //EXPERIMENTAL
                            //the for loops are experimental and give each node/connection of a network the chance to mutate instead of only
                            //having the chance of mutating 3 times at max (weight, add node, add connection)
                            int rnd;
                            int prob = (int)Mathf.Ceil(addNodeProb * 100);
                            if (multipleStructuralMutations)
                            {
                                for (int x = 0; x < network.genomeConnectionGenes.Count; x++)
                                {
                                    rnd = Random.Range(1, 101);
                                    if (rnd <= prob)
                                    {
                                        Evolution.AddNodeMutation(network);
                                    }
                                }

                                prob = (int)Mathf.Ceil(addConnectionProb * 100);
                                for (int x = 0; x < network.genomeNodeGenes.Count - 4; x++) //-4 because i exclude the output nodes
                                {
                                    rnd = Random.Range(1, 101);
                                    if (rnd <= prob)
                                    {
                                        Evolution.AddConnectionMutation(network);
                                    }
                                }

                                prob = (int)Mathf.Ceil(alterWeightProb * 100);
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

                                prob = (int)Mathf.Ceil(addConnectionProb * 100);
                                rnd = Random.Range(1, 101);
                                if (rnd <= prob)
                                {
                                    Evolution.AddConnectionMutation(network);
                                }

                                prob = (int)Mathf.Ceil(alterWeightProb * 100);
                                rnd = Random.Range(1, 101);
                                if (rnd <= prob)
                                {
                                    Evolution.AlterWeightMutation(network);
                                }

                            }

                            networks.Add(network);
                        }
                    }
                    speciesTxt.text = "Species: " + countSpecies;

                

                //potential problem:
                //the species list we have and the number of snakes in each species will not be alligned with the actual offspring in the end
                //after offspring has been created, we possibly need to update the species list hhere with the correct quantity of snakes in
                //each species

                
                List<int> speciesNums = new List<int>();
                
                for(int i = 0; i < offspringNum.Count; i++)
                {
                    for(int j = 0; j < offspringNum[i]; j++)
                    {
                        speciesNums.Add(i + 1);
                    }
                   // speciesNums.Add(listOfBoards[i].GetComponent<Board>().species);
                }

                //todo:
                //when former parents(boards) have been used to speciate the new generation and hece, there are double thhe amount of
                //boards that should be there, i can delete half the boards, i.e. the former generation
                /*
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
                            print("Species: "+inactiveBoards[i].GetComponent<Board>().species);

                        }
                        listOfBoards = new List<GameObject>();

                    }

                    */
                while(inactiveBoards.Count > 0)
                {
                    Destroy(inactiveBoards[0]);
                    inactiveBoards.RemoveAt(0);
                }
                inactiveBoards = new List<GameObject>();
                for(int i = 0; i < listOfBoards.Count; i++)
                {
                    inactiveBoards.Add(listOfBoards[i]);
                    inactiveBoards[i].SetActive(false);
                }
       



                listOfBoards = new List<GameObject>();
                //todo;
                //problem right now is that the inactive boards species indicator is not displaying the correct number thhat can be seen in the scene
                //probably the cause:
                //to apply the species number the board i.e. the snake bbelongs to, i use the list snakes.
                //snakes hholds listofboards[x].getcomponent<board>().snakeinstance
                //important: the board holds the species value, so i should use listofboards[x].getcomponent<bboard>().species, bbut instead
                //i use snakes[x].getcomponent<snake>().species which is in long:
                //listofboards[x].getcomponent<board>().snnakeinstance.getcomponent<snake>().boardscript.species
                //thhis should work as i understand it, bbut is nevertheless an unneccessary long reference

                SpawnBoards();
                NetworkManager.instance.listOfNetworks = null;
                NetworkManager.instance.listOfNetworks = new List<Network>();


                for (int i = 0; i < listOfBoards.Count; i++)
                {
                    
                        listOfBoards[i].GetComponent<Board>().species = speciesNums[i];
                        listOfBoards[i].GetComponent<Board>().childBrain = networks[i];
                        listOfBoards[i].GetComponent<Board>().start_drawing_nn = true;
                          
                }





            //implement elimination code here

            //check if a species is not present in the list
                for (int i = 0; i < species.Count; i++)
                {
                    bool add = false;
                    if (speciesInfos.Count > 0)
                    {
                        for (int j = 0; j < speciesInfos.Count; j++)
                        {
                            if (species[i][0].GetComponent<Snake>().boardScript.species == speciesInfos[j].speciesNum)
                            {
                                add = false;
                                break;
                            }
                            else
                            {
                                if(species[i][0].GetComponent<Snake>().boardScript.species != 0)
                                {
                                    add = true;
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        if (species[i][0].GetComponent<Snake>().boardScript.species != 0)
                        {
                            add = true;
                        }
                    }
                    if (add)
                    {
                        speciesInfos.Add(new speciesInfos(0, 0, species[i][0].GetComponent<Snake>().boardScript.species));
                    }
                }
                //determine if fitness has surpassed maxfitness for each species
                int maxFitness = 0;
                
                for (int i = 0; i < species.Count; i++)
                {

                    for (int j = 0; j < species[i].Count; j++)
                    {
                        if (maxFitness < species[i][j].GetComponent<Snake>().boardScript.fitness)
                        {
                            maxFitness = species[i][j].GetComponent<Snake>().boardScript.fitness;
                       
                        }

                    }
                    //assigning maxfitness, if maxfitness hhas increased and punishh the species if not
                    for (int j = 0; j < speciesInfos.Count; j++)
                    {
                        if (speciesInfos[j].speciesNum == species[i][0].GetComponent<Snake>().boardScript.species)
                        {
           
                            if(maxFitness > speciesInfos[j].maxFitness)
                            {
                                speciesInfos[j].maxFitness = maxFitness;
                                speciesInfos[j].numGensNoImprovement = 0;

                            }
                            else
                            {
                                speciesInfos[j].numGensNoImprovement += 1;
                            }
                                
                            break;
                            

                        }
                    }
                }
                //find the species that are not present on the board anymore and punishh them.
                //thhis is a nice byproduct to not only remove active species if they reached
                //numGensNoImprovement >= x, but also species that dont exist anymore
                bool inactive = false;
                for (int i = 0; i < speciesInfos.Count; i++)
                {
                    for (int j = 0; j < species.Count; j++)
                    {
                        if (species[j][0].GetComponent<Snake>().boardScript.species == speciesInfos[i].speciesNum)
                        {
                            inactive = false;
                            break;
                        }
                        else
                        {
                            inactive = true;
                        }
                    }
                    if (inactive)
                    {
                        speciesInfos[i].numGensNoImprovement += 1;
                    }
                }

                //species' that have exceded the threshhold for no improvement wont be able to reproduce

                for (int i = 0; i < species.Count; i++)
                {
                    for (int j = 0; j < speciesInfos.Count; j++)
                    {
                        if (species[i][0].GetComponent<Snake>().boardScript.species == speciesInfos[j].speciesNum)
                        {
                            if (speciesInfos[j].numGensNoImprovement >= noImprovementDropoff)
                            {
                                punishedSpecies = speciesInfos[j].speciesNum;
                                speciesInfos.RemoveAt(j);
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
    

    float AdjustedFitness(Snake snake, List<GameObject> speciesPop)
    {    
        return (float)snake.boardScript.fitness/speciesPop.Count;
    }

    //huge problem maybe here or elsewhere:
    //example: i have 2 species and one of them is not allowed to reproduce at all (stake == 0) and therefore, the othher species
    //will be allowed to reproduce the entire next generation, i still see occurences of getting 2 bboards with species A and 2 boards
    //with species B
    List<int> OffspringDistribution(List<float> stake)
    {
        List<float> floatDist = new List<float>();
        List<int> offspringDist = new List<int>();
        float excess = 0.0f;

        for(int i = 0; i < stake.Count; i++)
        {
            floatDist.Add(stake[i]*numOfBoards);
        }

        if(floatDist.Count > 1)
        {
            for (int i = 0; i < floatDist.Count - 1; i++)
            {
                int rawNumber = Mathf.FloorToInt(floatDist[i]);
                if (floatDist[i] == 0)
                {
                    excess = 0;
                }
                else if(rawNumber == 0)
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
                    if (floatDist[i+1] - excess < 0)
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
            if(rawNumber2 == 0)
            {
                excess = floatDist[floatDist.Count - 1];
            }
            else
            {
                excess = floatDist[floatDist.Count - 1] % rawNumber2;
            }
            
            if(excess >= 0.5f)
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

    void EliminateWorst(List<GameObject> species)
    {
        //sort snakes in each species according to fitness
        List<GameObject> orderedSnakes = Evolution.SelectionSort(species);
        //only eliminate snakes with at least 3 members in the species
        if(orderedSnakes.Count > 2)
        {
            float percentile = eliminationPercentile;
            int n = orderedSnakes.Count;
            int spot = Mathf.RoundToInt(percentile * n);

            int rank = 0;
            
            rank = orderedSnakes[spot-1].GetComponent<Snake>().boardScript.fitness;
             
            //now we have the fitness value which belongs to the eliminationPercentile together with all fitnesses below
            for(int i = 0; i < spot; i++)
            {
                //preventing to eliminate all but one snakes in a species in which case the next generation is a copy of this one snake
                if(orderedSnakes.Count > 2)
                {
                    orderedSnakes.RemoveAt(0);
                }
                
            }
            
            
               
            
        }

    }
 


}

public class speciesInfos
{
    public int maxFitness = 0;
    public int numGensNoImprovement;
    public int speciesNum;

    public speciesInfos(int maxFitness, int numGensNoImprovement, int speciesNum)
    {
        this.maxFitness = maxFitness;
        this.numGensNoImprovement = numGensNoImprovement;
        this.speciesNum = speciesNum;
    }

}


