using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls the main Game Loop. Manages when the game is over. Each turn and legal tiles. 
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameObject[,] board;
    private TileState[] boardStates;
    private Minimax minimax;
    private bool gameOver;
    public bool GameOver
    {
        get { return gameOver; }
    }


    GameObject aiTile;
    private bool playerTurn;
    public bool PlayerTurn
    {
        get { return playerTurn; }
        set { playerTurn = value; }
    }
    private bool aiTurn;

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
        boardStates = new TileState[9];
        playerTurn = true;
        aiTurn = false;
        gameOver = false;
        board = new GameObject[3, 3];

        int k = 0;

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
        if (!gameOver)
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

            if (GetWinner() == TileState.X)
            {
                turnIndication.text = "You win!";
                EndGame();
            }
            else if (GetWinner() == TileState.O)
            {
                turnIndication.text = "AI Win";
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
        if (isCoroutineExecuting)
        {
            yield break;
        }
        isCoroutineExecuting = true;
        if (!gameOver)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//Actual Minimax algorithm placement
            int bestMoveIndex = minimax.CalculateBestMove(boardStates, TileState.O);
            boardStates[bestMoveIndex] = TileState.O;
            aiTile = GameObject.Find((bestMoveIndex + 1).ToString());
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//
        }

        yield return new WaitForSeconds(0.5f); //Waits before placing on selected tile
        if (gameOver) //Checks to make sure the game is still active before placing the tile.
            yield break;

        isCoroutineExecuting = false;
        aiTile.GetComponent<TileManager>().PlaceAiTile();

    }


    private TileState GetWinner()
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0].GetComponent<TileManager>().space != TileState.Empty &&
                board[row, 0].GetComponent<TileManager>().space == board[row, 1].GetComponent<TileManager>().space &&
                board[row, 0].GetComponent<TileManager>().space == board[row, 2].GetComponent<TileManager>().space)
            {
                return board[row, 0].GetComponent<TileManager>().space;
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col].GetComponent<TileManager>().space != TileState.Empty &&
                board[0, col].GetComponent<TileManager>().space == board[1, col].GetComponent<TileManager>().space &&
                board[0, col].GetComponent<TileManager>().space == board[2, col].GetComponent<TileManager>().space)
            {
                return board[0, col].GetComponent<TileManager>().space;
            }
        }

        // Check diagonals
        if (board[0, 0].GetComponent<TileManager>().space != TileState.Empty &&
            board[0, 0].GetComponent<TileManager>().space == board[1, 1].GetComponent<TileManager>().space &&
            board[0, 0].GetComponent<TileManager>().space == board[2, 2].GetComponent<TileManager>().space)
        {
            return board[0, 0].GetComponent<TileManager>().space;
        }

        if (board[0, 2].GetComponent<TileManager>().space != TileState.Empty &&
            board[0, 2].GetComponent<TileManager>().space == board[1, 1].GetComponent<TileManager>().space &&
            board[0, 2].GetComponent<TileManager>().space == board[2, 0].GetComponent<TileManager>().space)
        {
            return board[0, 2].GetComponent<TileManager>().space;
        }

        // No winner
        return TileState.Empty;
    }

    public bool CheckForDraw() //Method that returns true if none of the tiles are available
    {
        bool noSpaces = false;
        foreach (GameObject tile in tiles)
        {
            noSpaces = !tile.GetComponent<TileManager>().spaceAvailable; //noSpaces is set to the oppisite of each tiles availability (if every tile is taken returns true)
            if (!noSpaces) //as soon as an available tile is found it breaks out the loop and returns false
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
            tile.GetComponent<TileManager>().spaceAvailable = false;
        }
        gameOver = true;
    }
}

