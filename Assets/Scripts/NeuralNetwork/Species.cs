using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Species
{
    int id;
    int age;
    int maxFitness;
    int avgFitness;
    public List<GameObject> snakes;
    int offspring;
    float adjFitness;
    bool young;
    int gensSinceLastImprovement;

    Species newSpecies;
    int punishedSpecies;
    List<float> adjFitnessesList;
    

    public Species(int id)
    {
        this.id = id;
        age = 0;
        maxFitness = 0;
        avgFitness = 0;
        snakes = new List<GameObject>();
        offspring = 0;
        adjFitness = 0.0f;
        young = true;
        gensSinceLastImprovement = 0;
    }

    public int getId()
    {
        return this.id;
    }
    public void setAge(int age)
    {
        this.age = age;
    }
    public int getAge()
    {
        return this.age;
    }
    public void setMaxFitness(int fitness)
    {
        this.maxFitness = fitness;
    }
    public int getMaxFitness()
    {
        return this.maxFitness;
    }
    public void setAvgFitness(int avgFitness)
    {
        this.avgFitness = avgFitness;
    }
    public int getAvgFitness()
    {
        return this.avgFitness;
    }
    public GameObject getSnake(int index)
    {
        return this.snakes[index];
    }
    public int getOffspring()
    {
        return this.offspring;
    }
    public void setOffspring(int offspring)
    {
        this.offspring = offspring;
    }
    public float getAdjFitness()
    {
        return adjFitness;
    }
    public void setAdjFitness(float adjFitness)
    {
        this.adjFitness = adjFitness;
    }
    public bool getYoung()
    {
        return this.young;
    }
    public void setYoung(bool state)
    {
        this.young = state;
    }
    public int getLastImprovement()
    {
        return this.gensSinceLastImprovement;
    }
    public void setLastImprovement(int num)
    {
        this.gensSinceLastImprovement = num;
    }



    public void AddSnake(GameObject snake)
    {
        snakes.Add(snake);
        snake.GetComponent<Snake>().species = this.id;
    }

}
