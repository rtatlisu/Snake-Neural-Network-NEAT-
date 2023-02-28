using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
   
    public Vector2 fruitSpot;
     Board board;

    void Start()
    {
         board = transform.parent.GetComponent<Board>();
        
    }
    
   

    public Vector2 spawnLocation(List<GameObject> g, int snakeNum)
    {
        board = transform.parent.GetComponent<Board>();
       //int rndX = Random.Range(1 + (snakeNum*25), 18+(snakeNum*25));
       // int rndY= Random.Range(1,Board.boarderSizeX-1);
       int rndX = Random.Range(1 + (int)board.transform.position.x, /*18*/GameManager.instance.boardSize - 2 + (int)board.transform.position.x);
       //1-18
        int rndY= Random.Range(1 + (int)board.transform.position.y, /*18*/GameManager.instance.boardSize - 2 + (int)board.transform.position.y);
    
        bool locationFound = false;

        while(!locationFound )
        { 
            for(int i = 0; i < g.Count; i++)
            {
                if(rndX == g[i].transform.position.x && rndY == g[i].transform.position.y)
                {
                    locationFound = false;
                    rndX = Random.Range(1 + (int)board.transform.position.x, /*18*/GameManager.instance.boardSize - 2 + (int)board.transform.position.x);
                    rndY= Random.Range(1 + (int)board.transform.position.y, /*18*/GameManager.instance.boardSize - 2 + (int)board.transform.position.y);
                    break;
                }
                else
                {
                    locationFound = true;
                }
            }
        }
        
        fruitSpot = new Vector2(rndX,rndY);
        return new Vector2(rndX,rndY);
        
    }

}

