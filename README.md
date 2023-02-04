# A Neural Network implementation used on Snake based on the NEAT concept [Unity project]

## This project has the goal of solving Snake with a Neural Network written from scratch. 
The whole concept stems from the paper **"Evolving Neural Networks through Augmenting Topologies" by Kenneth O. Stanley**. This concept is widley known as NEAT (NeuroEvolution of Augmenting Topologies).

The idea is to start minimally and grow large based on performance/survival of the fittest. This prevents overly large topologies and intends to only create nodes and conncetions that will be useful for performance.
<br>
<br>
<br>
**Technical information** <br>
The program is written in C# with the game engine Unity for easy visualization purposes.

## Vision
The snake can look into 4 directions: north, east, west, south. It can see all tiles inbetween its head and either of the walls.
For each of these directions, it gets 2 binary inputs (food/no food, tail/no tail) and 1 integer input (distance to wall) <br> <br>
In total, this makes 4 x 3 = 12 inputs. The output layer consists of 4 nodes responsible for either moving up, down, left or right.


![ezgif com-gif-maker (2)](https://user-images.githubusercontent.com/65288418/216702956-0347c684-9e6b-4334-9797-89b6dc215a57.gif) <br><br>

## Speciation
This concept is very unique and if it already existed, Stanley was the one to popularize it. <br>
It is a very powerful technique that can decrease the number of generations needed to reach the desired outcome. <br>
To put it simply, snakes are put in different species based on their networks' topology. <br>
If the topology differs so much that it exceeds a pre-defined threshold,
the snake will either be put into a species with snakes that have a similar topology or if there is no species that suits the snake, a new species will be created. <br>
The goal of this technique is to give new connections/nodes a chance to develop and express their performance before being prematurely eliminated.
That is acquired by letting the species compete within their own niche opposed to having all species compete with each other.
<br>
<br>

## Evolution
The evolutionary process is the code-heaviest part of the project and consists of many steps including many tweakable parameters. <br>
These steps are: <br>
1. Evaluation
2. Elimination
3. Calculating offspring
4. Removing worst performers
5. Reproduction
6. Speciation

After the entire population, e.g. 30 snakes, finished their game, the above steps will be executed. 
<br><br>
**Evaluation** <br>
* If there are no species yet, a species will be created and all snakes will be assigned to it
* The snakes' fitness will be processed to get the **adjusted fitness** for each snake
  * adjusted fitness is calculated by dividing the snake's fitness by the number of members in its species. This prevents a good performing species with high fitness values to take over the entire population and gives credit to low member species for reaching a high fitness value

**Elimination**<br>
* If a species did not improve (exceed their highest reached fitness) for a pre-defined number of generations, the species will go extinct and room will be made for more promising species

**Calculating offspring**<br>
* The adjusted fitness of each species will be divided by the sum of all species' adjusted fitness to give us a decimal value representing the share of offspring each species will receive
* This value will be multiplied by the population number (e.g. 30) and gives us an integer value showing how much offspring a species gets

**Removing worst performers**<br>
* In each species, we remove the worst performers to nullify the chance of passing on bad performing networks
* With a predefined percentile (e.g. 80%), we remove all snakes that are part of this percentile

**Reproduction**<br>
* Selection:
  * The snakes that remained in each species will be ordered according to their fitness scores. Higher fitness scores give a higher probability to be a parent of the new child. 2 parents will be selected for reproduction
* Crossover:
  * All connections that the parents share (parentA's and parentB's connection both point to the same 2 nodes) will be distributed with a 50% chance from either parent
  * All connections that the parents do not share will be stored for the following
  * parentA's fitness > parentB's fitness
    * The child receives all connections from parentA and non from parentB
  * parentA's fitness < parentB's fitness
    * The child receives all connections from parentB and non from parentA
  * parentA's fitness == parentB's fitness
    * The child receives all connections from parentA and parentB
* Mutation:
  * When the new offspring is created, their networks get a chance to alter a bit
  * **AddConnectionMutation**:
    * Create a connection between some randomly selected nodes, if there is no connection yet
  * **AddNodeMutation**:
    * Disable an existing connection and create a node on this position. The in-node of the disabled connection and the new node will be connected by a new connection and the new node and the out-node of the disabled connection will be connected by a new connection
  * **AlterWeightMutation**:
    * Iterate over all connections and with a certain chance either alter the weight by a little or, with a low chance, assign a random weight to the connection
    
**Speciation**<br>
* Offspring has been created and will now undergo speciation
* snakes with similar topology will be assigned to the same species
* If there is no species that matches a snake's topology, a new species is created and the snake is assigned to it
<br>

The snakes now play again and will undergo the procedure again, when finished.

<br>

**Fitness function**<br>
The fitness function is the driving evolutionary factor that decides in which direction the snakes will develop.
If not set optimally, snakes could find unwishful loopholes that cause large fitness gains but are contradictory to how snake is played.<br>
An exemplary fitness function is the following: <br>
$$fitness = moveToFruitCounter + fruitsEaten*(10+fruitBonus)$$ <br>
*moveToFruitCounter*: Snake moves closer to fruit &#8594; +1 fitness, snake moves away from fruit &#8594; -1.5 fitness<br>
*fruitsEaten*: Number of fruits eaten<br>
*fruitBonus*: increases by 5 for each fruit eaten. This rewards eating multiple fruits as this indicates that this is an intentional behavior and not just luck. <br>
<br>
With this fitness function, snakes quickly develop a tendency towards fruits, but hardly learn to avoid walls.<br>
Below is an example using this fitness function and the problem with avoding walls. <br>
Generation: 250+

<br>

<img src="https://user-images.githubusercontent.com/65288418/216693970-b29eb104-8f2c-4f13-bc60-a9d0ab2e72e8.gif" width="1000" height="550" />

<br>
<br>

**Showcase of the actual program with 9 out of 30 boards**: <br>

<br>
<img src="https://user-images.githubusercontent.com/65288418/216694947-ce8e24e7-fc66-4710-a2be-7f3fe6887010.gif" width="1000" height="550" />
<br>
<br>
The GIF above shows a problem that caused a very neat technique to arise.<br>
There is a roughly 25% chance for a snake to not move at all because its first move it wants to make is illegal, i.e. it wants to move up, but is looking down.<br><br>
The technique to solve this problem turned out to also be handy for a better evolutionary process. When "MultRunsPerGen" is toggled on, all snakes on the board are allowed to play for e.g. 10 rounds until they are evaluated. In these 10 rounds, the fitness of each snake will be stored and at the end will be divided by 10 to get the average fitness. By doing that we greatly reduce lucky performances, but also unlucky performances from dictating the next offspring.<br><br>
On top of that, it ensures that certain networks are truly better performing and prevents lucky performances of bad networks to be spread on to the offspring.


