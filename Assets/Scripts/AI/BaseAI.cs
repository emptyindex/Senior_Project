using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseAI : MonoBehaviour, IPieceBase, IProtectionBoard
{
    public int bestScore;
    public int[] bestMove;
    public bool moveFound;
    public bool hasFinished;

    public List<int[]> validActions = new List<int[]>();
    public int protectionLevel;
    public int dangerLevel;

    public AI AIManager { get; set; }
    public int CurrRowPos { get; set; }
    public int CurrColPos { get; set; }
    public int PieceID { get; set; }

    public void UpdateProtectionMap(int row, int col, int[,] board)
    {
        int row_limit = 7;
        int column_limit = 7;

        AI.protectionBoard -= protectionLevel;
        protectionLevel = 0;

        for (int x = Mathf.Max(0, row - 2); x <= Mathf.Min(row + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, col - 2); y <= Mathf.Min(col + 2, column_limit); y++)
            {
                if (board[x, y] != 0 && x != row || y != col)
                {            
                    var pieceID = board[x, y];
                    var adjustedID = pieceID > 20 ? pieceID -= 20 : pieceID;

                    var canDefend = x <= row + 1 && y <= col + 1 && y >= col - 1;

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (canDefend && x >= row - 1 && (adjustedID == 3 || adjustedID == 4 || adjustedID == 5 || adjustedID == 6))
                    {
                        protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (canDefend && x > row && board[x, y] == 21)
                    {
                        protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (adjustedID == 2)
                    {
                        protectionLevel += 1;
                    }
                }
            }
        }
        AI.protectionBoard += protectionLevel;
    }

    //method to find the score of a given board
    //since attacks are not guarenteed the current attack is also passed if a piece is attacking another
    //right now just returns a random value
    public int HeuristicScore(int[,] theBoard)
    {
        int score = Random.Range(1, 1000);
        return score;
    }

    //method that checks if a move is valid and returns the least number of squares the piece has to move to reach the destination
    //this is used to find the valid spaces that the AI pieces can move to without jumping over other pieces
    public int isMoveValid(int[,] grid, int currRow, int currCol, int destRow, int destCol)
    {
        bool[,] closedList = new bool[8, 8];
        cell[,] cellDetails = new cell[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                cellDetails[i, j].f = 9999999999999;
                cellDetails[i, j].g = 9999999999999;
                cellDetails[i, j].h = 9999999999999;
                cellDetails[i, j].parent_i = -1;
                cellDetails[i, j].parent_j = -1;
            }
        }

        int row = currRow;
        int col = currCol;
        cellDetails[row, col].f = 0;
        cellDetails[row, col].g = 0;
        cellDetails[row, col].h = 0;
        cellDetails[row, col].parent_i = row;
        cellDetails[row, col].parent_j = col;

        List<float[]> openList = new List<float[]>();
        float[] start = { 0, row, col };
        openList.Add(start);

        bool reachedEnd = false;
        //print(openList[0]);

        while (openList.Any())
        {
            //int count = 0;
            //print(count);
            //count++;
            float[] p = openList[0];

            openList.RemoveAt(0);

            row = (int)p[1];
            col = (int)p[2];
            closedList[row, col] = true;

            float gNew, hNew, fNew;

            //check North
            if (isValid(row - 1, col) == true)
            {
                if (isDestination(row - 1, col, destRow, destCol) == true )
                {
                    cellDetails[row - 1, col].parent_i = row;
                    cellDetails[row - 1, col].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row - 1, col] == false &&
                    isUnBlocked(grid, row - 1, col) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row - 1, col, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row - 1, col].f == 9999999999999 ||
                        cellDetails[row - 1, col].f > fNew)
                    {
                        float[] newAdd = { fNew, row - 1, col };
                        openList.Add(newAdd);

                        cellDetails[row - 1, col].f = fNew;
                        cellDetails[row - 1, col].g = gNew;
                        cellDetails[row - 1, col].h = hNew;
                        cellDetails[row - 1, col].parent_i = row;
                        cellDetails[row - 1, col].parent_j = col;
                    }
                }
            }

            //check South
            if (isValid(row + 1, col) == true)
            {
                if (isDestination(row + 1, col, destRow, destCol) == true )
                {
                    cellDetails[row + 1, col].parent_i = row;
                    cellDetails[row + 1, col].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row + 1, col] == false &&
                    isUnBlocked(grid, row + 1, col) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row + 1, col, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row + 1, col].f == 9999999999999 ||
                        cellDetails[row + 1, col].f > fNew)
                    {
                        float[] newAdd = { fNew, row + 1, col };
                        openList.Add(newAdd);

                        cellDetails[row + 1, col].f = fNew;
                        cellDetails[row + 1, col].g = gNew;
                        cellDetails[row + 1, col].h = hNew;
                        cellDetails[row + 1, col].parent_i = row;
                        cellDetails[row + 1, col].parent_j = col;
                    }
                }
            }

            //check East
            if (isValid(row, col + 1) == true)
            {
                if (isDestination(row, col + 1, destRow, destCol) == true)
                {
                    cellDetails[row, col + 1].parent_i = row;
                    cellDetails[row, col + 1].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row, col + 1] == false &&
                    isUnBlocked(grid, row, col + 1) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row, col + 1, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row, col + 1].f == 9999999999999 ||
                        cellDetails[row, col + 1].f > fNew)
                    {
                        float[] newAdd = { fNew, row, col + 1 };
                        openList.Add(newAdd);

                        cellDetails[row, col + 1].f = fNew;
                        cellDetails[row, col + 1].g = gNew;
                        cellDetails[row, col + 1].h = hNew;
                        cellDetails[row, col + 1].parent_i = row;
                        cellDetails[row, col + 1].parent_j = col;
                    }
                }
            }

            //check West
            if (isValid(row, col - 1) == true)
            {
                if (isDestination(row, col - 1, destRow, destCol) == true)
                {
                    cellDetails[row, col - 1].parent_i = row;
                    cellDetails[row, col - 1].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row, col - 1] == false &&
                    isUnBlocked(grid, row, col - 1) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row, col - 1, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row, col - 1].f == 9999999999999 ||
                        cellDetails[row, col - 1].f > fNew)
                    {
                        float[] newAdd = { fNew, row, col - 1 };
                        openList.Add(newAdd);

                        cellDetails[row, col - 1].f = fNew;
                        cellDetails[row, col - 1].g = gNew;
                        cellDetails[row, col - 1].h = hNew;
                        cellDetails[row, col - 1].parent_i = row;
                        cellDetails[row, col - 1].parent_j = col;
                    }
                }
            }

            //check North-West
            if (isValid(row - 1, col - 1) == true)
            {
                if (isDestination(row - 1, col - 1, destRow, destCol) == true )
                {
                    cellDetails[row - 1, col - 1].parent_i = row;
                    cellDetails[row - 1, col - 1].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row - 1, col - 1] == false &&
                    isUnBlocked(grid, row - 1, col - 1) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row - 1, col - 1, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row - 1, col - 1].f == 9999999999999 ||
                        cellDetails[row - 1, col - 1].f > fNew)
                    {
                        float[] newAdd = { fNew, row - 1, col - 1 };
                        openList.Add(newAdd);

                        cellDetails[row - 1, col - 1].f = fNew;
                        cellDetails[row - 1, col - 1].g = gNew;
                        cellDetails[row - 1, col - 1].h = hNew;
                        cellDetails[row - 1, col - 1].parent_i = row;
                        cellDetails[row - 1, col - 1].parent_j = col;
                    }
                }
            }

            //check North-East
            if (isValid(row - 1, col + 1) == true)
            {
                if (isDestination(row - 1, col + 1, destRow, destCol) == true)
                {
                    cellDetails[row - 1, col + 1].parent_i = row;
                    cellDetails[row - 1, col + 1].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row - 1, col + 1] == false &&
                    isUnBlocked(grid, row - 1, col + 1) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row - 1, col + 1, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row - 1, col + 1].f == 9999999999999 ||
                        cellDetails[row - 1, col + 1].f > fNew)
                    {
                        float[] newAdd = { fNew, row - 1, col + 1 };
                        openList.Add(newAdd);

                        cellDetails[row - 1, col + 1].f = fNew;
                        cellDetails[row - 1, col + 1].g = gNew;
                        cellDetails[row - 1, col + 1].h = hNew;
                        cellDetails[row - 1, col + 1].parent_i = row;
                        cellDetails[row - 1, col + 1].parent_j = col;
                    }
                }
            }

            //check South-East
            if (isValid(row + 1, col + 1) == true)
            {
                if (isDestination(row + 1, col + 1, destRow, destCol) == true)
                {
                    cellDetails[row + 1, col + 1].parent_i = row;
                    cellDetails[row + 1, col + 1].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row + 1, col + 1] == false &&
                    isUnBlocked(grid, row + 1, col + 1) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row + 1, col + 1, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row + 1, col + 1].f == 9999999999999 ||
                        cellDetails[row + 1, col + 1].f > fNew)
                    {
                        float[] newAdd = { fNew, row + 1, col + 1 };
                        openList.Add(newAdd);

                        cellDetails[row + 1, col + 1].f = fNew;
                        cellDetails[row + 1, col + 1].g = gNew;
                        cellDetails[row + 1, col + 1].h = hNew;
                        cellDetails[row + 1, col + 1].parent_i = row;
                        cellDetails[row + 1, col + 1].parent_j = col;
                    }
                }
            }

            //check South-West
            if (isValid(row + 1, col - 1) == true)
            {
                if (isDestination(row + 1, col - 1, destRow, destCol) == true )
                {
                    cellDetails[row + 1, col - 1].parent_i = row;
                    cellDetails[row + 1, col - 1].parent_j = col;
                    //print("found path");
                    int totalMoves = tracePath(cellDetails, destRow, destCol);

                    reachedEnd = true;
                    return totalMoves;
                }
                else if (closedList[row + 1, col - 1] == false && isUnBlocked(grid, row + 1, col - 1) == true)
                {
                    gNew = cellDetails[row, col].g + 1;
                    hNew = calculateHValue(row + 1, col - 1, destRow, destCol);
                    fNew = gNew + hNew;

                    if (cellDetails[row + 1, col - 1].f == 9999999999999 ||
                        cellDetails[row + 1, col - 1].f > fNew)
                    {
                        float[] newAdd = { fNew, row + 1, col - 1 };
                        openList.Add(newAdd);

                        cellDetails[row + 1, col - 1].f = fNew;
                        cellDetails[row + 1, col - 1].g = gNew;
                        cellDetails[row + 1, col - 1].h = hNew;
                        cellDetails[row + 1, col - 1].parent_i = row;
                        cellDetails[row + 1, col - 1].parent_j = col;
                    }
                }
            }
        }

        //if no end was found print failed
        if (reachedEnd == false)
        {
            //print("failed");
            return 100;
        }

        return 100;
    }

    //checks if a move is within the board
    public bool isValid(int row, int col)
    {
        return (row >= 0) && (row < 8) && (col >= 0)
               && (col < 8);
    }

    //checks if a square on the board is empty
    bool isUnBlocked(int[,] grid, int row, int col)
    {
        if (grid[row, col] == 0)
            return (true);
        else
            return (false);
    }

    //checks if the square is the destination square
    bool isDestination(int row, int col, int destRow, int destCol)
    {
        if (row == destRow && col == destCol)
            return (true);
        else
            return (false);
    }

    //helper method that calculated H value for A* search
    float calculateHValue(int row, int col, int destRow, int destCol)
    {
        float dx = Mathf.Abs(row - destRow);
        float dy = Mathf.Abs(col - destCol);

        float h = (dx + dy) + (Mathf.Sqrt(2) - 2) * Mathf.Min(dx, dy);
        return h;
    }

    //traces the A* search path
    //returns the number of moves from start to end
    int tracePath(cell[,] cellDetails, int destRow, int destCol)
    {
        //print("The Path is to: " + destRow + " " + destCol);
        int row = destRow;
        int col = destCol;
        int count = 0;

        Stack Path = new Stack();

        while (!(cellDetails[row, col].parent_i == row
                 && cellDetails[row, col].parent_j == col))
        {
            int[] arr = { row, col };
            Path.Push(arr);
            int temp_row = cellDetails[row, col].parent_i;
            int temp_col = cellDetails[row, col].parent_j;
            row = temp_row;
            col = temp_col;
        }

        int[] arr2 = { row, col };
        Path.Push(arr2);
        while (Path.Count > 0)
        {
            //int[]p = (int[])Path.Peek();
            Path.Pop();
            count++;
            //print("-> (%d,%d) " + p[0] + p[1]);
        }

        count = count - 1;
        //print(count);
        return count;
    }

    //cell structure to assist in the A* search algorithm
    struct cell
    {
        public int parent_i, parent_j;
        public float f, g, h;
    }
}
