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
        MoveData bestMove = MiniMax(0,aiPlayer);
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

    private MoveData MiniMax(int depth, CellState currentPlayer)
    {
        List<int> availableMoves = GetAvailableMoves();
        availableMoves = Shuffle(availableMoves);
        if (IsGameOver() || depth == 5 || availableMoves.Count == 0)
        {
            MoveData move = new MoveData();
            move.score = EvaluateBoard();
            return move;
        }
        List<MoveData> moves = new List<MoveData>();
        foreach(int moveIndex in availableMoves)
        {
            MoveData move = new MoveData();
            move.index = moveIndex;
            board[moveIndex] = currentPlayer;

            if(currentPlayer == aiPlayer) //max
            {
                MoveData result = MiniMax(depth + 1, humanPlayer);
                move.score = result.score;
            }
            else
            {
                MoveData result = MiniMax(depth + 1, aiPlayer);
                move.score = result.score;
            }
            board[moveIndex] = CellState.Empty;
            moves.Add(move);

        }

            return GetBestMove(moves,currentPlayer);
    }

    private bool IsGameOver()
    {
        return (GetWinner() != CellState.Empty) || (GetAvailableMoves().Count == 0);
    }
    private CellState GetWinner()
    {
        if(GameController.Instance.CheckForWin(CellState.X))
        {
            return CellState.X;
        }
        else if (GameController.Instance.CheckForWin(CellState.O))
        {
            return CellState.O;
        }
        else
        {
            return CellState.Empty;
        }
    }
    private int EvaluateBoard()
    {
        CellState winner = GetWinner();
        if (winner == aiPlayer)
        {
            return 1;
        }
        else if (winner == humanPlayer)
        {
            return -1;
        }
        else {return 0;}
    }
}
