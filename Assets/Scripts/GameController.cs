using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls the main Game Loop. Manages when the game is over. Each turn and legal tiles. 
public class GameController : MonoBehaviour
{
    public static GameController Instance; //Accesible instance of the Gamecontroller script
    [HideInInspector]
    public GameObject[,] board; //2 dimensional array for the board. 
    private TileState[] boardStates; //Array which holds the value of each tile in the board.
    private Minimax minimax; //Instance of the minimax script

    [SerializeField]
    private bool gameOver; //bollean which holds whether the game is over or not.


    GameObject aiTile; 
    private bool playerTurn; 
    public bool PlayerTurn
    {
        get { return playerTurn; }
        set { playerTurn = value; }
    }
    private bool aiTurn;

    public Text turnIndication;

    public GameObject[] tiles; //Array which holds the tile objects, which will be sorted into the board

    private bool isCoroutineExecuting = false; //Prevents the coroutine from executing multiple times.
    private bool placingTile = false; // Prevents game from ending when tile is being placed in coroutine
    void Awake()
    {
        Instance = this; //Sets the value of the instance variable to itself.
    }
    // Start is called before the first frame update
    void Start()
    {
        minimax = Minimax.instance;
        boardStates = new TileState[9]; //Initializing value of the array
        playerTurn = true;
        aiTurn = false;
        gameOver = false;
        board = new GameObject[3, 3];

        int k = 0;
        //Loop that sets all the tiles to empty and populates the 2d array for the board.
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                k++;
                board[i, j] = tiles[k - 1];
                boardStates[k - 1] = TileState.Empty;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < boardStates.Length; i++) //Updates the board with the updated placements.
        {
            boardStates[i] = tiles[i].GetComponent<TileManager>().space;

        }
        if (!gameOver) //When the game is still active, the game will continue updating text to show who's turn it is and completing the ai turn when necessary.
        {
            if (playerTurn)
            {
                turnIndication.text = "Your Turn";
            }
            else
            {
                turnIndication.text = "AI Turn";
            }
            if (aiTurn && !CheckForDraw())
            {
                StartCoroutine(DoAiTurn());
            }

            if (minimax.FindWinner(boardStates) == TileState.X)
            {
                turnIndication.text = "You win!";
                EndGame();
            }
            else if (minimax.FindWinner(boardStates) == TileState.O)
            {
                turnIndication.text = "AI Win";
                if(!placingTile)
                    EndGame();
            }
            else if (CheckForDraw())
            {
                turnIndication.text = "Draw!";
                EndGame();
            }
        }
    }


    private IEnumerator DoAiTurn() //Ai Turn is done in a Coroutine so Placement doesn't happen immediately.
    {
        if (isCoroutineExecuting) //if the coroutine is still executing immediately break out.
        {
            yield break;
        }
        isCoroutineExecuting = true;
        if (!gameOver) //As long as the game is still running choose a tile for the AI
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//Actual Minimax algorithm placement
            int bestMoveIndex = minimax.CalculateBestMove(boardStates, TileState.O);
            boardStates[bestMoveIndex] = TileState.O;
            aiTile = GameObject.Find((bestMoveIndex + 1).ToString());
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//
        }
        placingTile = true; 
        yield return new WaitForSeconds(0.5f); //Waits before placing on selected tile
        if (aiTile.GetComponent<TileManager>().space == TileState.Disabled) //Checks to make sure the Tile is still available before placing the tile.
            yield break; //if the space is diabled before the coroutine is finished it will immediately break out.

        isCoroutineExecuting = false; //Sets back to false so it can execute again.
        aiTile.GetComponent<TileManager>().PlaceAiTile(); //Placement of the tile.
        placingTile = false;
    }


    public bool CheckForDraw() //Method that returns true if none of the tiles are available
    {
        bool noSpaces = false;
        foreach (GameObject tile in tiles)
        {
            noSpaces = tile.GetComponent<TileManager>().space != TileState.Empty; //noSpaces checks each tile to see if its empty or not (if every tile is taken returns true)
            if (!noSpaces) //as soon as an Empty tile is found it breaks out the loop and returns false
            {
                break;
            }

        }
        return noSpaces;
    }

    public void ChangeTurn()
    {
        playerTurn = !playerTurn;
        aiTurn = !aiTurn;
    }

    public void EndGame() //When the game is over every tile is marked as taken to prevent the game from going further
    {
        foreach (GameObject tile in tiles)
        {
            if(tile.GetComponent<TileManager>().space == TileState.Empty)
                tile.GetComponent<TileManager>().space = TileState.Disabled;
        }
        gameOver = true;
    }
}

