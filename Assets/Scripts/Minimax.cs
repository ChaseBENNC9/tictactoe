//Minimax script , implemented from the Chess Game and some suggestions from Chat GPT. This script manages the AI Minimax algorithm and determines the best possible moves for it to take.

using System.Collections.Generic;

using UnityEngine;

public class Minimax : MonoBehaviour
{
    private TileState[] board; //Get the State of each Tile on the board
    private TileState aiPlayer; //Gives the AI and player the corresponding piece "X" or "O".
    private TileState humanPlayer;
    public static Minimax instance; //Create an accessible instance of the Minimax script
    public const int MAXDEPTH = 5; //Max depth for the recursive minimax algorithm
    public const int GRIDSIZE = 3; //the width and height of the grid.

    void Awake()
    {
        instance = this;
    }

    public List<T> Shuffle<T>(List<T> list)  //Shuffles the Inputed List so each playthrough is different.
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
    public int CalculateBestMove(TileState[] currentBoard, TileState player) //Method that returns the index of the best tile for the AI.
    {
        board = currentBoard;
        aiPlayer = player;
        humanPlayer = (player == TileState.X) ? TileState.O : TileState.X;
        MoveData bestMove = MiniMax(0, aiPlayer, int.MinValue, int.MaxValue);
        return bestMove.index;
    }
    private List<int> GetAvailableMoves()
    {
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == TileState.Empty)
                availableMoves.Add(i);
        }
        return availableMoves;
    }
    private MoveData GetBestMove(List<MoveData> moves, TileState currentPlayer) //Depending on whether the player is the Ai or the human, the best score will be the highest or lowest score.
    {
        int bestMoveIndex = 0;
        int bestScore = (currentPlayer == aiPlayer) ? int.MinValue : int.MaxValue; //Determines whether to target thr lowest or highest score depending on if its the ai or player turn
        for (int i = 0; i < moves.Count; i++)
        {
            if ((currentPlayer == aiPlayer && moves[i].score > bestScore) || (currentPlayer == humanPlayer && moves[i].score < bestScore))
            {
                bestScore = moves[i].score;
                bestMoveIndex = i;
            }
        }
        return moves[bestMoveIndex];
    }

    private MoveData MiniMax(int depth, TileState currentPlayer, int alpha, int beta) //Recursive Minimax algorithm for tic tac toe;
    {
        List<int> availableMoves = GetAvailableMoves(); //Fills the list with the valid moves the AI is allowed to make.
        availableMoves = Shuffle(availableMoves); //Shuffles the Moves list so the game is not repetitive 
        if (IsGameOver() || depth == MAXDEPTH || availableMoves.Count == 0)
        {
            MoveData move = new MoveData();
            move.score = EvaluateBoard();
            return move;
        }

        List<MoveData> moves = new List<MoveData>();

        foreach (int moveIndex in availableMoves)
        {
            MoveData move = new MoveData();
            move.index = moveIndex;

            // Apply the current player's move to the board
            board[moveIndex] = currentPlayer;

            if (currentPlayer == aiPlayer) //This is the Max , Algorithm tries to maximize the score 
            {
                MoveData result = MiniMax(depth + 1, humanPlayer, alpha, beta);
                move.score = result.score;


                if (move.score > alpha)
                {
                    alpha = move.score;
                }
            }
            else
            {
                MoveData result = MiniMax(depth + 1, aiPlayer, alpha, beta); //This is the min, where the algorithm is trying to minimise the score.
                move.score = result.score;


                if (move.score < beta)
                {
                    beta = move.score;
                }
            }

            // Restore the board state by removing the move
            board[moveIndex] = TileState.Empty;

            moves.Add(move);
            if (currentPlayer == aiPlayer && move.score >= beta) //Alpha beta pruning for improving the algorithm performance, Prunes any branches that return lower or higher values than needed.
            {
                break;
            }
            else if (currentPlayer == humanPlayer && move.score <= alpha)
            {
                break;
            }

        }

        return GetBestMove(moves, currentPlayer); //Returns the best move from a list of moves and which player is currently active.
    }

    private bool IsGameOver()
    {
        return (FindWinner(board) != TileState.Empty) || (GetAvailableMoves().Count == 0);
    }
    public TileState FindWinner(TileState[] board) //Win method, Used to evaluate when the algorithm should stop.
    {

        // Check rows
        for (int row = 0; row < GRIDSIZE; row++)
        {

            if (board[row * GRIDSIZE] != TileState.Empty && board[row * GRIDSIZE] == board[row * GRIDSIZE + 1] && board[row * GRIDSIZE] == board[row * GRIDSIZE + 2])
            {
                return board[row * GRIDSIZE]; //returns the state of the selected row to determine who won the game
            }
        }

        // Check columns
        for (int col = 0; col < GRIDSIZE; col++)
        {
            if (board[col] != TileState.Empty && board[col] == board[col + GRIDSIZE] && board[col] == board[col + (GRIDSIZE * 2)]) //Check each Individual column for a win
            {
                return board[col]; //Returns the state of the selected column , to determine who won the game
            }
        }

        // Check diagonals
        if (board[0] != TileState.Empty && board[0] == board[4] && board[0] == board[8])
        {
            return board[0];
        }
        if (board[2] != TileState.Empty && board[2] == board[4] && board[2] == board[6])
        {
            return board[2];
        }

        return TileState.Empty; // No winner
    }
    private int EvaluateBoard() //Return a value, if the game is over check if there is a winner otherwise it could be a draw or the match isnt over.
    {
        TileState winner = FindWinner(board);
        if (winner == TileState.O)
        {
            return 1;
        }
        else if (winner == TileState.X)
        {
            return -1;
        }
        else { return 0; }
    }
}
