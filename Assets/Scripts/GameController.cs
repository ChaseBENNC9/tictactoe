using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public static GameController Instance;
   
    public GameObject[,] board;
    private CellState[] boardStates;
    private Minimax minimax;
    private bool gameOver;
    public bool GameOver
    {
        get{return gameOver;}
    }


    GameObject cpuTile;
    private bool playerTurn;
    public bool PlayerTurn
    {
        get{return playerTurn;}
        set{playerTurn = value;}
    }
    private bool cpuTurn;
    public bool CpuTurn
    {
        get{return cpuTurn;}
        set{cpuTurn = value;}
    }

    public Text turnIndication;

    public GameObject[] tiles;

    private bool isCoroutineExecuting = false;
     void Awake() 
     {
        Instance = this;
     }
    // Start is called before the first frame update
    void Start()
    {
        minimax = Minimax.instance;
        boardStates = new CellState[9];
        playerTurn = true;
        cpuTurn = false;
        gameOver = false;
        board = new GameObject[3,3];

        int k = 0;
    
        for(int i = 0; i < 3;i++)
        {
            for (int j = 0; j < 3;j++)
            {
                k++;
                board[i,j] = tiles[k-1];
                boardStates[k-1] = CellState.Empty;
            }
        } 

   
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i< boardStates.Length; i++)
        {
            boardStates[i] = tiles[i].GetComponent<TileManager>().space;
        }
        if(!gameOver)
        {
            if(playerTurn)
            {
                turnIndication.text = "Your Turn";
            }
            else
            {
                turnIndication.text = "CPU Turn";
            }
            if(cpuTurn && !CheckForDraw())
            {
                StartCoroutine(DoCpuTurn());
            }
  
            if( CheckForWin(CellState.O))
            {
                turnIndication.text = "Cpu win";
                EndGame();
            }
            else if (CheckForWin(CellState.X))
            {
                turnIndication.text = "You win!";
                EndGame();
            }
            else if( CheckForDraw())
            {
                turnIndication.text = "Draw!";
                EndGame();
            }
        }    
    }

    private IEnumerator DoCpuTurn()
    {
        if(isCoroutineExecuting)
        {
            yield break;
        }
        isCoroutineExecuting = true;
        if(!gameOver)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//Random the Cpu tile for play testing
           do
           {
            int r = UnityEngine.Random.Range(1,10);
            cpuTile = GameObject.Find(r.ToString());
            }while(!cpuTile.GetComponent<TileManager>().spaceAvailable); 
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//   
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//Actual Minimax algorithm placement
            int bestMoveIndex = minimax.CalculateBestMove(boardStates,CellState.O);
            boardStates[bestMoveIndex] = CellState.O;
            cpuTile = GameObject.Find((bestMoveIndex+1).ToString());
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//
            
        }

        yield return new WaitForSeconds(0.5f); //Waits before placing on selected tile
        if(gameOver) //Checks to make sure the game is still active before placing the tile.
            yield break;

        cpuTile.GetComponent<TileManager>().PlaceCpuTile(); 
        isCoroutineExecuting = false; 
    }
    
    public bool CheckForWin(CellState space) //Method that tests every possible scenario for a win. Cols, Rows and Angle. Returns a boolean.
    {
    if (board[0, 0].GetComponent<TileManager>().space == space && board[0, 1].GetComponent<TileManager>().space == space && board[0, 2].GetComponent<TileManager>().space == space) { print("rows"); return true; }
    if (board[1, 0].GetComponent<TileManager>().space == space && board[1, 1].GetComponent<TileManager>().space == space && board[1, 2].GetComponent<TileManager>().space == space) { print("rows"); return true; }
    if (board[2, 0].GetComponent<TileManager>().space == space && board[2, 1].GetComponent<TileManager>().space == space && board[2, 2].GetComponent<TileManager>().space == space) { print("rows"); return true; }
    // check columns
    if (board[0, 0].GetComponent<TileManager>().space == space && board[1, 0].GetComponent<TileManager>().space == space && board[2, 0].GetComponent<TileManager>().space == space) { print("cols"); return true; }
    if (board[0, 1].GetComponent<TileManager>().space == space && board[1, 1].GetComponent<TileManager>().space == space && board[2, 1].GetComponent<TileManager>().space == space) { print("cols"); return true; }
    if (board[0, 2].GetComponent<TileManager>().space == space && board[1, 2].GetComponent<TileManager>().space == space && board[2, 2].GetComponent<TileManager>().space == space) { print("cols"); return true; }
    // check diags
    if (board[0, 0].GetComponent<TileManager>().space == space && board[1, 1].GetComponent<TileManager>().space == space && board[2, 2].GetComponent<TileManager>().space == space) { print("angle"); return true; }
    if (board[0, 2].GetComponent<TileManager>().space == space && board[1, 1].GetComponent<TileManager>().space == space && board[2, 0].GetComponent<TileManager>().space == space) {print("angle"); return true; }

    return false;
    }

    public bool CheckForDraw() //Method that returns true if none of the tiles are available
    {
        bool noSpaces = false;
        foreach(GameObject tile in tiles) 
        {
             noSpaces = !tile.GetComponent<TileManager>().spaceAvailable; //noSpaces is set to the oppisite of each tiles availability (if every tile is taken returns true)
             if(!noSpaces) //as soon as an available tile is found it breaks out the loop and returns false
             {
                break;
             }
           
        }
        return noSpaces;
    }

    public void ChangeTurn()
    {
        playerTurn = !playerTurn;
        cpuTurn = !cpuTurn;
    }

    public void EndGame() //When the game is over every tile is marked as taken to prevent the game from going further
    {
        foreach(GameObject tile in tiles)
        {
            tile.GetComponent<TileManager>().spaceAvailable = false;
        }
        gameOver = true;
    }
}

