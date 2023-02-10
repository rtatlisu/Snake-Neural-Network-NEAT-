using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Snake : MonoBehaviour
{
    
     GameObject board;
    public Board boardScript;
    public GameObject head;
    public GameObject singeTail;
    public List<GameObject> snakeComposites;
     Vector2 headPos;
     bool up,right,left,down;
     bool coroutineRunning;
    public  List<Vector2> formerPosition;
     public static int numOfSnakes = 0;
     public int whichSnakeAmI;
     public int customBoarder;
     int distanceToFood;
      public GameObject foodLine = null;
      public GameObject northLine = null;
      public GameObject eastLine = null;
      public GameObject southLine = null;
      public GameObject westLine = null;
      LineRenderer lr;
    int distanceToNorthWall;
    int distanceToEastWall;
    int distanceToSouthWall;
    int distanceToWestWall;
    bool foodInSight;
    bool tailInSight;
    public VisionInfo north,east,south,west;
    public Network brain = null;
    public List<VisionInfo> inputs;
     public List<List<VisionInfo>> visionList;
     public double probability = 0d;
     
     public bool snakeCanMove = true;
    public string role;
    public int species = 0;
    public bool elite = false;

    void Awake()
    {
        snakeComposites = new List<GameObject>();
        role = "child";
    }

    void Start()
    {        
        board = transform.parent.gameObject;
        boardScript = board.GetComponent<Board>();
        boardScript.gameRunning = false;
        visionList = new List<List<VisionInfo>>();
        
        for(int i = 0; i < GameManager.instance.listOfBoards.Count; i++)
        {
            if(GameManager.instance.listOfBoards[i] == board)
            {
                whichSnakeAmI = i;
                break;
            }
        }

        //instantiate the snakes head and single tail
        snakeComposites.Add(Instantiate(head, new Vector2(
        Random.Range(2+(int)board.transform.position.x,17+(int)board.transform.position.x),
        Random.Range(2+(int)board.transform.position.y,17+(int)board.transform.position.y)),Quaternion.identity));
        
     
        snakeComposites[0].transform.parent = this.gameObject.transform;
        formerPosition = new List<Vector2>();
        
        headPos = snakeComposites[0].transform.localPosition;
        int rndX=-1;
        int rndY=-1;
        
        while(rndX != 0 && rndY != 0 || (rndX == 0 && rndY == 0))
        {
            rndX = Random.Range(-1,2);
            rndY = Random.Range(-1,2);
        }

        snakeComposites.Add(Instantiate(singeTail, new Vector2(rndX,
        rndY)+headPos,Quaternion.identity));

        snakeComposites[snakeComposites.Count-1].transform.parent = this.gameObject.transform;

        formerPosition.Add(snakeComposites[0].transform.position);
        formerPosition.Add(snakeComposites[1].transform.position);

        customBoarder = whichSnakeAmI*25 + 20;
        numOfSnakes++;
    }
    

    void Update()
    {
        if(!coroutineRunning &&boardScript && boardScript.gameRunning)
        {
             StartCoroutine(Direction());           
        }

        if(brain.whichMove == null)
        {
           Vision(snakeComposites[0].transform.position);

            inputs = new List<VisionInfo>{north, east,
            south, west};

          
            brain.UpdateInputNodes(inputs);
        }


       
        if(brain != null && brain.whichMove[0] &&!boardScript.gameOver && !down && snakeComposites[0].transform.position
        +Vector3.up != snakeComposites[1].transform.position)
        {
            up = true;
            down = false;
            right = false;
            left = false;
            boardScript.gameRunning = true;
        }
        else if (brain != null && brain.whichMove[1] && !boardScript.gameOver && !up&& snakeComposites[0].transform.position
        +Vector3.down != snakeComposites[1].transform.position)
        {
            up = false;
            down = true;
            right = false;
            left = false;
            boardScript.gameRunning = true;
        }
        else if (brain != null && brain.whichMove[2] && !boardScript.gameOver && !left&& snakeComposites[0].transform.position
        +Vector3.right != snakeComposites[1].transform.position)
        {
            up = false;
            down = false;
            right = true;
            left = false;
            boardScript.gameRunning = true;
        }
        else if (brain != null && brain.whichMove[3] &&!boardScript.gameOver && !right&& snakeComposites[0].transform.position
        +Vector3.left != snakeComposites[1].transform.position)
        {
            up = false;
            down = false;
            right = false;
            left = true;
            boardScript.gameRunning = true;
        }
        //important if snakes want to move illegal moves, i.e. moving up if immediate tail is up
        else if (brain != null && !boardScript.gameOver && !boardScript.gameRunning)
        {
            boardScript.gameOver = true;
            boardScript.gameRunning = false;
        }
    }

    IEnumerator Direction()
    {
        coroutineRunning = true;

        //draw visions, i.e. to the food, north, east, west and south wall
       /*
       if(boardScript.fruitScript != null)
        {
            distanceToFood = distanceToObject(snakeComposites[0].transform.position,
            boardScript.fruitScript.transform.position);   
        }
        distanceToNorthWall = distanceToObject(snakeComposites[0].transform.position,
        WallPosition(snakeComposites[0].transform.position,boardScript.northWall));

        distanceToEastWall = distanceToObject(snakeComposites[0].transform.position,
        WallPosition(snakeComposites[0].transform.position,boardScript.eastWall));

        distanceToSouthWall = distanceToObject(snakeComposites[0].transform.position,
        WallPosition(snakeComposites[0].transform.position,boardScript.southWall));

        distanceToWestWall = distanceToObject(snakeComposites[0].transform.position,
        WallPosition(snakeComposites[0].transform.position,boardScript.westWall));
      */
 
  
        if(up)
        {   
            if((snakeComposites[0].transform.localPosition+Vector3.up).y < board.transform.position.y
            +Board.boarderSizeX-1)
            {
                for(int i = 1; i < snakeComposites.Count; i++)
                {
                    if((snakeComposites[0].transform.localPosition+Vector3.up).y ==
                    snakeComposites[i].transform.position.y &&
                    (snakeComposites[0].transform.localPosition+Vector3.up).x ==
                    snakeComposites[i].transform.position.x)
                    {
                        boardScript.HandleCollision();
                    }
                }

                if(!boardScript.gameOver)
                {
                    AlignTailOnMove();
                    snakeComposites[0].transform.localPosition+=Vector3.up;  
                    boardScript.distTravelled++;
                    boardScript.movesLeft--;
                    boardScript.CalcFitness();   
                }   
            }
        
            else
            {
                boardScript.HandleCollision();     
            }
        }
        else if(down)
        {
            if((snakeComposites[0].transform.localPosition+Vector3.down).y > board.transform.position.y)
            {
                for(int i = 1; i < snakeComposites.Count; i++)
                {
                    if((snakeComposites[0].transform.localPosition+Vector3.down).y ==
                    snakeComposites[i].transform.position.y &&
                    (snakeComposites[0].transform.localPosition+Vector3.down).x ==
                    snakeComposites[i].transform.position.x)
                    {
                        boardScript.HandleCollision();               
                    }
                }

                if(!boardScript.gameOver)
                {
                    AlignTailOnMove();
                    snakeComposites[0].transform.localPosition+=Vector3.down; 
                    boardScript.distTravelled++;
                    boardScript.movesLeft--;
                    boardScript.CalcFitness();   
                }                
            }
            else
            {
                boardScript.HandleCollision();
            }
        }
        else if(right)
        {
            if((snakeComposites[0].transform.localPosition+Vector3.right).x < board.transform.position.x+
            Board.boarderSizeX-1)
            {
                for(int i = 1; i < snakeComposites.Count; i++)
                {
                    if((snakeComposites[0].transform.localPosition+Vector3.right).y ==
                    snakeComposites[i].transform.position.y &&
                    (snakeComposites[0].transform.localPosition+Vector3.right).x ==
                    snakeComposites[i].transform.position.x)
                    {
                        boardScript.HandleCollision();
                    }
                }
                if(!boardScript.gameOver)
                {
                    AlignTailOnMove();
                    snakeComposites[0].transform.localPosition+=Vector3.right;
                    boardScript.distTravelled++;  
                    boardScript.movesLeft--;
                    boardScript.CalcFitness();      
                }               
            }
            else
            {
                boardScript.HandleCollision();
            }
        }
        else if(left)
        {   
            if((snakeComposites[0].transform.localPosition+Vector3.left).x > board.transform.position.x)
            {
                for(int i = 1; i < snakeComposites.Count; i++)
                {
                    if((snakeComposites[0].transform.localPosition+Vector3.left).y ==
                    snakeComposites[i].transform.position.y &&
                    (snakeComposites[0].transform.localPosition+Vector3.left).x ==
                    snakeComposites[i].transform.position.x)
                    {
                        boardScript.HandleCollision();
                    }
                }
                if(!boardScript.gameOver)
                {
                    AlignTailOnMove();
                    snakeComposites[0].transform.localPosition+=Vector3.left;  
                    boardScript.distTravelled++;
                    boardScript.movesLeft--;
                    boardScript.CalcFitness();
                }             
            }
            else
            {
                boardScript.HandleCollision();
            }
        }

        //update the vision lines
        /*
       if(foodLine == null)
       {
        foodLine = DrawLine(foodLine, snakeComposites[0].transform.position, boardScript.fruitScript.transform.position);
        
       }
       else if(foodLine != null)
       {
        GameObject.Destroy(foodLine);
        foodLine = DrawLine(foodLine, snakeComposites[0].transform.position, boardScript.fruitScript.transform.position);
       }


       if(northLine == null)
       {
        northLine = DrawLine(northLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.northWall));
        
       }
       else if(northLine != null)
       {
        GameObject.Destroy(northLine);
        northLine = DrawLine(northLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.northWall));
       }

       if(eastLine == null)
       {
        eastLine = DrawLine(eastLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.eastWall));
        
       }
       else if(eastLine != null)
       {
        GameObject.Destroy(eastLine);
        eastLine = DrawLine(eastLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.eastWall));
       }

       if(southLine == null)
       {
        southLine = DrawLine(southLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.southWall));
        
       }
       else if(southLine != null)
       {
        GameObject.Destroy(southLine);
        southLine = DrawLine(southLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.southWall));
       }

       if(westLine == null)
       {
        westLine = DrawLine(westLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.westWall));
        
       }
       else if(westLine != null)
       {
        GameObject.Destroy(westLine);
        westLine = DrawLine(foodLine, snakeComposites[0].transform.position, WallPosition(snakeComposites[0].transform.position, boardScript.westWall));
       }
        */

        List<VisionInfo> inputsCopy = new List<VisionInfo>();
        visionList = new List<List<VisionInfo>>();     
        visionList.Add(new List<VisionInfo>());
        visionList.Add(new List<VisionInfo>());
        
        visionList[1] = inputs;
            
        Vision(snakeComposites[0].transform.position);
        
        inputs = new List<VisionInfo>();
        inputs.Add(north);
        inputs.Add(east);
        inputs.Add(south);
        inputs.Add(west);

        visionList[0] = inputs;

        brain.UpdateInputNodes(inputs);
        boardScript.ColorizeNetwork();

  
        yield return new WaitForSeconds(GameManager.instance.gameSpeed);
        coroutineRunning = false;
    }

  

    void AlignTailOnMove()
    {
        for(int i = 0; i < snakeComposites.Count; i++)
        {
            formerPosition[i] = snakeComposites[i].transform.position;
        }
        for(int i = snakeComposites.Count-1; i > 0; i--)
        {
            snakeComposites[i].transform.localPosition = formerPosition[i-1];
        }     
    }

    GameObject DrawLine(GameObject line,Vector3 snakePos, Vector3 fruitPos)
    {
        line = new GameObject(); 
        line.transform.position = snakePos;
        line.transform.parent = this.gameObject.transform;
        line.AddComponent<LineRenderer>();
         lr = line.GetComponent<LineRenderer>();
        lr.material =  new Material(Shader.Find("Unlit/Texture"));
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, snakePos);
        lr.SetPosition(1, fruitPos);
        return line;
    }

    Vector2 WallPosition(Vector2 snakePos, GameObject wall) 
    {
        for(int i = 0; i < wall.transform.childCount; i++)
        {
            if(snakePos.x == wall.transform.GetChild(i).transform.position.x)
            {
                return new Vector2(wall.transform.GetChild(i).transform.position.x,
                 wall.transform.GetChild(i).transform.position.y);
            }
            else if(snakePos.y == wall.transform.GetChild(i).transform.position.y)
            {
                return new Vector2(wall.transform.GetChild(i).transform.position.x,
                 wall.transform.GetChild(i).transform.position.y);
            }
        }
        return Vector3.zero;
    }

    int distanceToObject(Vector2 snakePos, Vector2 objPos)
    {
       return (int)Mathf.Abs((Mathf.Abs(snakePos.x - objPos.x))+
       (Mathf.Abs(snakePos.y - objPos.y)));
    }


    public void Vision(Vector2 snakePos)
    {
        
        north = new VisionInfo(-1,false,false);
        east = new VisionInfo(-1,false,false);
        south = new VisionInfo(-1,false,false);
        west = new VisionInfo(-1,false,false);
        Vector2 currentTile = snakePos;
        int xMin = (int)board.transform.position.x;
        int xMax = (int)board.transform.position.x + Board.boarderSizeX-1;
        int yMin = (int)board.transform.position.y;
        int yMax = (int)board.transform.position.y + Board.boarderSizeX-1;
        
        //north
        while((currentTile+Vector2.up).y < yMax)
        {
            currentTile+=Vector2.up;
           
            if(currentTile == (Vector2)boardScript.fruitScript.transform.position)
            {
                north.food = true;
            }
            
            for(int i = 1; i < snakeComposites.Count; i++)
            {
                if(currentTile == (Vector2)snakeComposites[i].transform.position)
                {
                    north.tail = true;
                }
            }

        }
        north.wallDist = (int) Mathf.Abs(((currentTile+Vector2.up).y - snakePos.y));
        
        
    //east
    currentTile = snakePos;
        while((currentTile+Vector2.right).x < xMax)
        {
            currentTile += Vector2.right;

            if(currentTile == (Vector2)boardScript.fruitScript.transform.position)
            {
                east.food = true;
            }
            
            for(int i = 1; i < snakeComposites.Count; i++)
            {
                if(currentTile == (Vector2)snakeComposites[i].transform.position)
                {
                    east.tail = true;
                }
            }

        }
        east.wallDist = (int) Mathf.Abs(((currentTile+Vector2.right).x - snakePos.x));


    //south
    currentTile = snakePos;
        while((currentTile+Vector2.down).y > yMin)
        {
            currentTile += Vector2.down;

            if(currentTile == (Vector2)boardScript.fruitScript.transform.position)
            {
                south.food = true;
            }
            
            for(int i = 1; i < snakeComposites.Count; i++)
            {
                if(currentTile == (Vector2)snakeComposites[i].transform.position)
                {
                    south.tail = true;
                }
            }
        }
        south.wallDist = (int) Mathf.Abs(((currentTile+Vector2.down).y - snakePos.y));


    //west
    currentTile = snakePos;
        while((currentTile+Vector2.left).x > xMin)
        {
            currentTile += Vector2.left;

            if(currentTile == (Vector2)boardScript.fruitScript.transform.position)
            {
                west.food = true;
            }
            
            for(int i = 1; i < snakeComposites.Count; i++)
            {
                if(currentTile == (Vector2)snakeComposites[i].transform.position)
                {
                    west.tail = true;
                }
            }
        }
        west.wallDist = (int)Mathf.Abs( ((currentTile+Vector2.left).x - snakePos.x));
    }

   
    
    
    

}

public struct VisionInfo
{
    public bool food;
    public int wallDist;
    public bool tail;
  
   public VisionInfo(int wallDist, bool food, bool tail)
    {
        this.wallDist = wallDist;
        this.food = food;
        this.tail = tail;     
    }

 
    
}
