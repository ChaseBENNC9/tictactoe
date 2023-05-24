using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimax : MonoBehaviour
{
    private CellState[] board;
    private CellState aiPlayer;
    private CellState humanPlayer;
    public static Minimax instance;

    void Awake()
    {
        instance = this;
    }
    private void Start() 
    {

    }


    public List<T> Shuffle<T>(List<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Range(0,n);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
        return list;
    }

    public int CalculateBestMove(CellState[] currentBoard, CellState player)
    {
        board = currentBoard;
        aiPlayer = player;
        humanPlayer = (player == CellState.X) ? CellState.O : CellState.X;
        MoveData bestMove = MiniMax(0,aiPlayer,int.MinValue,int.MaxValue);
        return bestMove.index;
    }
    private List<int> GetAvailableMoves()
    {
         List<int> availableMoves = new List<int>();
         for (int i = 0; i < board.Length; i++)
         {
            if(board[i] == CellState.Empty)
                availableMoves.Add(i);
         }
         return availableMoves;
    }
    private MoveData GetBestMove(List<MoveData> moves, CellState currentPlayer)
    {
        int bestMoveIndex = 0;
        int bestScore = (currentPlayer == aiPlayer) ? int.MinValue : int.MaxValue;
        for(int i = 0; i < moves.Count; i ++)
        {
            if((currentPlayer == aiPlayer && moves[i].score > bestScore) || (currentPlayer == humanPlayer && moves[i].score < bestScore))
            {
                bestScore = moves[i].score;
                bestMoveIndex = i;
            }
        }
        return moves[bestMoveIndex];
    }

private MoveData MiniMax(int depth, CellState currentPlayer,int alpha,int beta) //Minimax algorithm for tic tac toe;
{
    List<int> availableMoves = GetAvailableMoves();
    //availableMoves = Shuffle(availableMoves); //SHuffle the list so 
    if (IsGameOver() || depth == 5 || availableMoves.Count == 0)
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

        if (currentPlayer == aiPlayer)
        {
            MoveData result = MiniMax(depth + 1, humanPlayer,alpha,beta);
            move.score = result.score;


            if(move.score > alpha)
            {
                alpha = move.score;
            }
        }
        else
        {
            MoveData result = MiniMax(depth + 1, aiPlayer,alpha,beta);
            move.score = result.score;


            if(move.score < beta)
            {
                beta = move.score;
            }
        }

        // Restore the board state by removing the move
        board[moveIndex] = CellState.Empty;

        moves.Add(move);
        if(currentPlayer == aiPlayer && move.score >= beta) 
        {
            break;
        }
        else if(currentPlayer == humanPlayer && move.score <= alpha)
        {
            break;
        }

    }

    return GetBestMove(moves, currentPlayer);
}

    private bool IsGameOver()
    {
        return (FindWinner() != CellState.Empty) || (GetAvailableMoves().Count == 0);
    }
    public CellState FindWinner()
    {
    
    // Check rows
    for (int row = 0; row < 3; row++)
    {

        if (board[row * 3] != CellState.Empty && board[row * 3] == board[row * 3 + 1] && board[row * 3] == board[row * 3 + 2])
        {
            return board[row * 3];
        }
    }

    // Check columns
    for (int col = 0; col < 3; col++)
    {
        if (board[col] != CellState.Empty && board[col] == board[col + 3] && board[col] == board[col + 6])
        {
            return board[col];
        }
    }

    // Check diagonals
    if (board[0] != CellState.Empty && board[0] == board[4] && board[0] == board[8])
    {
        return board[0];
    }
    if (board[2] != CellState.Empty && board[2] == board[4] && board[2] == board[6])
    {
        return board[2];
    }

    return CellState.Empty; // No winner
}
    private int EvaluateBoard()
    {
        CellState winner = FindWinner();
        if (winner == CellState.O)
        {

            return 1;
        }
        else if (winner == CellState.X)
        {
            return -1;
        }
        else {return 0;}
    }
}
