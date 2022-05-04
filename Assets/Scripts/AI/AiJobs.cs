using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public struct AiJobs : IJobParallelFor
{
    public int protection;
    public int[,] Board;
    public int[][] moveSet;
    public NativeArray<int> result;

    public void Execute(int index)
    {
        //makeMove
        //Manager.UpdateIntBoard(moveSet[0][1], moveSet[0][2], moveSet[0][3], moveSet[0][4], moveSet[0][0]);
        Board[moveSet[0][1], moveSet[0][2]] = 0;
        Board[moveSet[0][3], moveSet[0][4]] = moveSet[0][0];

        //Manager.UpdateIntBoard(moveSet[1][1], moveSet[1][2], moveSet[1][3], moveSet[1][4], moveSet[1][0]);
        Board[moveSet[1][1], moveSet[1][2]] = 0;
        Board[moveSet[1][3], moveSet[1][4]] = moveSet[0][0];

        //Manager.UpdateIntBoard(moveSet[2][1], moveSet[2][2], moveSet[2][3], moveSet[2][4], moveSet[2][0]);
        Board[moveSet[2][1], moveSet[2][2]] = 0;
        Board[moveSet[2][3], moveSet[2][4]] = moveSet[0][0];

        //call minimax recursively
        //best = Math.Min(best, minimax(board, depth + 1, isMax));
        int score = HeuristicScore(Board, protection);
        result[index] = score;

        //undo the move
        //Manager.UpdateIntBoard(moveSet[0][3], moveSet[0][4], moveSet[0][1], moveSet[0][2], moveSet[0][0]);
        Board[moveSet[0][3], moveSet[0][4]] = 0;
        Board[moveSet[0][1], moveSet[0][2]] = moveSet[0][0];

        //Manager.UpdateIntBoard(moveSet[1][3], moveSet[1][4], moveSet[1][1], moveSet[1][2], moveSet[1][0]);
        Board[moveSet[1][3], moveSet[1][4]] = 0;
        Board[moveSet[1][1], moveSet[1][2]] = moveSet[0][0];

        //Manager.UpdateIntBoard(moveSet[2][3], moveSet[2][4], moveSet[2][1], moveSet[2][2], moveSet[2][0]);
        Board[moveSet[2][3], moveSet[2][4]] = 0;
        Board[moveSet[2][1], moveSet[2][2]] = moveSet[0][0];

    }

    public int HeuristicScore(int[,] theBoard, int protection)
    {
        int score = protection;
        return score;
    }
}
