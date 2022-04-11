using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveFinder : BaseAI
{
    public static bool findPlayerMoves;

    private GameObject Manager;

    // Start is called before the first frame update
    void Start()
    {
        findPlayerMoves = false;

        Manager = GameObject.FindGameObjectWithTag("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        if (findPlayerMoves == true)
        {
            int[,] newBoard = new int[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    newBoard[i, j] = Manager.GetComponent<GameManager>().board[i, j];
                }
            }

            foreach (GameObject piece in AI.PlayerBishopLPieces)
            {
                findPieceType(piece, newBoard);
            }
            foreach (GameObject piece in AI.PlayerBishopRPieces)
            {
                findPieceType(piece, newBoard);
            }
            foreach (GameObject piece in AI.PlayerKingPieces)
            {
                findPieceType(piece, newBoard);
            }

            findPlayerMoves = false;
        }
    }

    void findPieceType(GameObject piece, int[,] newBoard)
    {
        piece.GetComponent<BasePiece>().validActions.Clear();

        if (piece.GetComponent<BasePiece>().PieceID % 10 == 1)
            findPawnValid(piece.GetComponent<BasePiece>().CurrColPos, piece.GetComponent<BasePiece>().CurrRowPos, newBoard, piece);
        else if (piece.GetComponent<BasePiece>().PieceID % 10 == 2)
            findRookValid(piece.GetComponent<BasePiece>().CurrColPos, piece.GetComponent<BasePiece>().CurrRowPos, newBoard, piece);
        else if (piece.GetComponent<BasePiece>().PieceID % 10 == 3)
            findKnightValid(piece.GetComponent<BasePiece>().CurrColPos, piece.GetComponent<BasePiece>().CurrRowPos, newBoard, piece);
        else if (piece.GetComponent<BasePiece>().PieceID % 10 == 4)
            findBishopValid(piece.GetComponent<BasePiece>().CurrColPos, piece.GetComponent<BasePiece>().CurrRowPos, newBoard, piece);
        else if (piece.GetComponent<BasePiece>().PieceID % 10 == 5)
            findQueenValid(piece.GetComponent<BasePiece>().CurrColPos, piece.GetComponent<BasePiece>().CurrRowPos, newBoard, piece);
        else if (piece.GetComponent<BasePiece>().PieceID % 10 == 6)
            findKingValid(piece.GetComponent<BasePiece>().CurrColPos, piece.GetComponent<BasePiece>().CurrRowPos, newBoard, piece);
    }


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Code that finds valid moves for each piece type
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    void findPawnValid(int currRow, int currCol, int[,] newBoard, GameObject piece)
    {
        int[] validAction = new int[5];

        //add "no move" to the valid actions
        //validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow, currCol };
        //piece.GetComponent<BasePiece>().validActions.Add(validAction);

        for (int i = -1; i <= 1; i++)
        {
            if (currRow + 1 < 8 && currCol + i < 8 && currCol + i > -1 && (newBoard[currRow + 1, currCol + i] == 0 || newBoard[currRow + 1, currCol + i] == 21 || newBoard[currRow + 1, currCol + i] == 22 ||
            newBoard[currRow + 1, currCol + i] == 23 || newBoard[currRow + 1, currCol + i] == 24 || newBoard[currRow + 1, currCol + i] == 25 ||
            newBoard[currRow + 1, currCol + i] == 26))
            {
                validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow + 1, currCol + i };
                piece.GetComponent<BasePiece>().validActions.Add(validAction);
                //print("found action");
            }
        }


        UpdateProtectionMap(currRow, currCol, newBoard, piece);

        //AI.protectionBoard += protectionLevel;
    }

    public void findRookValid(int currRow, int currCol, int[,] newBoard, GameObject piece)
    {
        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[5];

        //add "no move" to the valid actions
        //validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow, currCol };
        //piece.GetComponent<BasePiece>().validActions.Add(validAction);

        //check moves
        //moves must be in a straight line
        for (int direc = 0; direc < 8; direc++)
        {
            for (int move = 1; move <= 2; move++)
            {
                //diagonal up left \
                if (direc == 0)
                {
                    int x = currRow - move;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight up |
                if (direc == 1)
                {
                    int x = currRow;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal up right /
                if (direc == 2)
                {
                    int x = currRow + move;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight left 
                if (direc == 3)
                {
                    int x = currRow - move;
                    int y = currCol;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight right 
                if (direc == 4)
                {
                    int x = currRow + move;
                    int y = currCol;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal down left /
                if (direc == 5)
                {
                    int x = currRow - move;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight down 
                if (direc == 6)
                {
                    int x = currRow;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal down right \  
                if (direc == 7)
                {
                    int x = currRow + move;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }
            }
        }

        //search possible attacks
        //attacks can be up to 2 spaces away and do not need to be in a straight line
        for (int x = Mathf.Max(0, currRow - 2); x <= Mathf.Min(currRow + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 2); y <= Mathf.Min(currCol + 2, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {

                    //check possible attacks
                    //since rook has the same move speed as attack range we can just check if the spot we found has an enemy on it
                    if (newBoard[x, y] == 21 || newBoard[x, y] == 22 || newBoard[x, y] == 23 ||
                        newBoard[x, y] == 24 || newBoard[x, y] == 25 || newBoard[x, y] == 26)
                    {
                        validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 3 || newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 1)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 2)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("Rook protection level: " + protectionLevel);
    }

    public void findKnightValid(int currRow, int currCol, int[,] newBoard, GameObject piece)
    {
        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[5];

        //add "no move" to the valid actions
        //validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow, currCol };
        //piece.GetComponent<BasePiece>().validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 3); x <= Mathf.Min(currRow + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 3); y <= Mathf.Min(currCol + 3, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);

                    if (moves <= 4 && newBoard[x, y] == 0)
                    {
                        float howGood = AssumeGoodness(currRow, currCol, newBoard);
                        if (howGood > -4)
                        {
                            validAction = new int[] { 23, currRow, currCol, x, y };
                            validActions.Add(validAction);
                        }
                    }

                    //check possible attacks
                    //since knight can move and attack in the same turn we can check through their whole move range
                    if (moves <= 4 && (newBoard[x, y] == 21 || newBoard[x, y] == 22 || newBoard[x, y] == 23 ||
                        newBoard[x, y] == 24 || newBoard[x, y] == 25 || newBoard[x, y] == 26))
                    {
                        validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 3 || newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 1)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 2)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("Knight protection level: " + protectionLevel);
    }

    public void findBishopValid(int currRow, int currCol, int[,] newBoard, GameObject piece)
    {
        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[5];

        //add "no move" to the valid actions
        validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow, currCol };
        piece.GetComponent<BasePiece>().validActions.Add(validAction);

        //check moves
        //moves must be in a straight line
        for (int direc = 0; direc < 8; direc++)
        {
            for (int move = 1; move <= 2; move++)
            {
                //diagonal up left \
                if (direc == 0)
                {
                    int x = currRow - move;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight up |
                if (direc == 1)
                {
                    int x = currRow;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal up right /
                if (direc == 2)
                {
                    int x = currRow + move;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight left 
                if (direc == 3)
                {
                    int x = currRow - move;
                    int y = currCol;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight right 
                if (direc == 4)
                {
                    int x = currRow + move;
                    int y = currCol;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal down left /
                if (direc == 5)
                {
                    int x = currRow - move;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //straight down 
                if (direc == 6)
                {
                    int x = currRow;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal down right \  
                if (direc == 7)
                {
                    int x = currRow + move;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard, currRow, currCol, piece);
                    if (isBlocked == -1)
                        break;
                }
            }
        }

        //search possible attacks
        for (int x = Mathf.Max(0, currRow - 2); x <= Mathf.Min(currRow + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 2); y <= Mathf.Min(currCol + 2, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check possible attacks
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 21 || newBoard[x, y] == 22 || newBoard[x, y] == 23 ||
                        newBoard[x, y] == 24 || newBoard[x, y] == 25 || newBoard[x, y] == 26) &&
                        (x <= currRow + 1) && (y <= currCol + 1) && (x >= currRow - 1) && (y >= currCol - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { 14, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 3 || newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 1)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 2)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("Bishop protection level: " + protectionLevel);
    }

    public void findQueenValid(int currRow, int currCol, int[,] newBoard, GameObject piece)
    {
        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[5];

        //add "no move" to the valid actions
        //validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow, currCol };
        //piece.GetComponent<BasePiece>().validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 3); x <= Mathf.Min(currRow + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 3); y <= Mathf.Min(currCol + 3, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);
                    if (moves <= 3 && newBoard[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { 25, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //check possible attacks
                    if ((newBoard[x, y] == 21 || newBoard[x, y] == 22 || newBoard[x, y] == 23 ||
                        newBoard[x, y] == 24 || newBoard[x, y] == 25 || newBoard[x, y] == 26) &&
                        (x <= currRow + 1) && (y <= currCol + 1) && (x >= currRow - 1) && (y >= currCol - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { 25, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //update protection map

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 3 || newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 1)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 2)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("Queen protection level: " + protectionLevel);
    }

    public void findKingValid(int currRow, int currCol, int[,] newBoard, GameObject piece)
    {
        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[5];

        //add "no move" to the valid actions
        validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, currRow, currCol };
        piece.GetComponent<BasePiece>().validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 3); x <= Mathf.Min(currRow + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 3); y <= Mathf.Min(currCol + 3, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);
                    if (moves <= 3 && newBoard[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { 26, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //check possible attacks
                    if ((newBoard[x, y] == 21 || newBoard[x, y] == 22 || newBoard[x, y] == 23 ||
                        newBoard[x, y] == 24 || newBoard[x, y] == 25 || newBoard[x, y] == 26) &&
                        (x <= currRow + 1) && (y <= currCol + 1) && (x >= currRow - 1) && (y >= currCol - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { 26, currRow, currCol, x, y };
                        piece.GetComponent<BasePiece>().validActions.Add(validAction);
                    }

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 3 || newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 1)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 2)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("King protection level: " + protectionLevel);
    }


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Helper functions for finding valid moves for each piece type
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    int CheckActions(int x, int y, int move, int[,] newBoard, int currRow, int currCol, GameObject piece)
    {
        //int currCol = piece.GetComponent<IPieceBase>().CurrColPos;
        //int currRow = piece.GetComponent<IPieceBase>().CurrRowPos;

        //check moves
        if (x < 8 && y < 8 && x > -1 && y > -1 && newBoard[x, y] == 0)
        {
            int[] validAction = new int[] { piece.GetComponent<BasePiece>().PieceID, currRow, currCol, x, y };
            piece.GetComponent<BasePiece>().validActions.Add(validAction);
        }

        if (x < 8 && y < 8 && x > -1 && y > -1 && newBoard[x, y] != 0)
            return -1;
        else
            return 1;
    }

    public void UpdateProtectionMap(int row, int col, int[,] board, GameObject piece)
    {
        int row_limit = 7;
        int column_limit = 7;

        //AI.protectionBoard -= protectionLevel;
        piece.GetComponent<BasePiece>().protectionLevel = 0;

        for (int x = Mathf.Max(0, row - 2); x <= Mathf.Min(row + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, col - 2); y <= Mathf.Min(col + 2, column_limit); y++)
            {
                if (board[x, y] != 0 && x != row || y != col)
                {
                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= row + 1 && x >= row - 1 && y <= col + 1 && y >= col - 1 &&
                        (board[x, y] == 23 || board[x, y] == 24 || board[x, y] == 25 || board[x, y] == 26))
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= row + 1 && x > row && y <= col + 1 && y >= col - 1 && board[x, y] == 21)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (board[x, y] == 22)
                    {
                        piece.GetComponent<BasePiece>().protectionLevel += 1;
                    }
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
    }

    private float AssumeGoodness(int row, int col, int[,] board)
    {
        int row_limit = 7;
        int column_limit = 7;

        float goodness = 0;
        float badness = 0;

        for (int x = Mathf.Max(0, row - 3); x <= Mathf.Min(row + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, col - 3); y <= Mathf.Min(col + 3, column_limit); y++)
            {
                if (board[x, y] == 21 || board[x, y] == 22 || board[x, y] == 23 || board[x, y] == 24 || board[x, y] == 25)
                {
                    goodness += 1;
                }

                if (board[x, y] == 24)
                {
                    goodness += 2;
                }

                if (board[x, y] == 26)
                {
                    goodness += 4;
                }

                if (x <= row + 2 && x >= row - 2 && y <= col + 2 && y >= col - 2 && (board[x, y] == 1 || board[x, y] == 3 || board[x, y] == 4))
                {
                    badness += 1;
                }

                if (x <= row + 2 && x >= row - 2 && y <= col + 2 && y >= col - 2 && (board[x, y] == 2 || board[x, y] == 5))
                {
                    badness += 2;
                }
            }
        }

        return goodness - badness;
    }

}
