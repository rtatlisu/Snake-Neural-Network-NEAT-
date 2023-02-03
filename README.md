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

##Evolution




![ezgif com-gif-maker (2)](https://user-images.githubusercontent.com/65288418/216702956-0347c684-9e6b-4334-9797-89b6dc215a57.gif)





<img src="https://user-images.githubusercontent.com/65288418/216693970-b29eb104-8f2c-4f13-bc60-a9d0ab2e72e8.gif" width="1000" height="550" />

<img src="https://user-images.githubusercontent.com/65288418/216694947-ce8e24e7-fc66-4710-a2be-7f3fe6887010.gif" width="1000" height="550" />


