using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGUI
{
    public class Solver
    {
        private int[,] mat;
        public int[] FindEmptySpace(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] == 0)
                    {
                        return new int[] { i, j };
                    }
                }
            }
            return new int[] { -1, -1 };
        }
        public bool SolveBoard(ref int[,] board)
        {
            int[] space = FindEmptySpace(board);

            if (space[0] == -1)
                return true;

            int row = space[0];
            int col = space[1];

            for (int i = 1; i < 10; i++)
            {
                if (ValidBoard(board, i, row, col))
                {
                    board[row, col] = i;

                    if (SolveBoard(ref board))
                    {
                        return true;
                    }
                    board[row, col] = 0;
                }
            }
            return false;
        }
        public bool ValidBoard(int[,] board, int num, int x, int y)
        {
            //row
            for (int i = 0; i < 9; i++)
            {
                if (board[x, i] == num & y != i)
                {
                    //Console.WriteLine("bad row");
                    return false;
                }
            }

            //col
            for (int i = 0; i < 9; i++)
            {
                if (board[i, y] == num & x != i)
                {
                    //Console.WriteLine("bad col");
                    return false;
                }
            }
            //local cube
            int box_x = y / 3;
            int box_y = x / 3;

            for (int i = box_y * 3; i < box_y * 3 + 3; i++)
            {
                for (int j = box_x * 3; j < box_x * 3 + 3; j++)
                {
                    if (board[i, j] == num & i != y & j != x)
                    {
                        //Console.WriteLine("bad box");
                        return false;
                    }
                }
            }

            return true;
        }
        public void PrintBoard(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(string.Format("{0}   ", board[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        private int[,] board = new int[,] {
        {7, 8, 0, 4, 0, 0, 1, 2, 0},
        {6, 0, 0, 0, 7, 5, 0, 0, 9},
        {0, 0, 0, 6, 0, 1, 0, 7, 8},
        {0, 0, 7, 0, 4, 0, 2, 6, 0},
        {0, 0, 1, 0, 5, 0, 9, 3, 0},
        {9, 0, 4, 0, 6, 0, 0, 0, 5},
        {0, 7, 0, 3, 0, 0, 0, 1, 2},
        {1, 2, 0, 0, 0, 7, 4, 0, 0},
        {0, 4, 9, 2, 0, 6, 0, 0, 7}
        };
        public int[,] Board { get => board; set => board = value; }

        // Sudoku Generator 
        public int[,] RandomBoard(int difficulty)
        {
            mat = new int[9, 9];
            // Fill the diagonal of SRN x SRN matrices 
            fillDiagonal();

            // Fill remaining blocks 
            fillRemaining(0, 3);

            // Remove Randomly K digits to make game 
            removeKDigits(difficulty);

            return mat;
        }

        // Fill the diagonal SRN number of SRN x SRN matrices 
        public void fillDiagonal()
        {

            for (int i = 0; i < 9; i = i + 3)
                // for diagonal box, start coordinates->i==j 
                fillBox(i, i);
        }

        // Returns false if given 3 x 3 block contains num. 
        public bool unUsedInBox(int rowStart, int colStart, int num)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (mat[rowStart + i,colStart + j] == num)
                        return false;

            return true;
        }

        // Fill a 3 x 3 matrix. 
        public void fillBox(int row, int col)
        {
            int num;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    do
                    {
                        num = randomGenerator(9);
                    }
                    while (!unUsedInBox(row, col, num));

                    mat[row + i,col + j] = num;
                }
            }
        }

        // Random generator 
        public int randomGenerator(int num)
        {
            Random rnd = new Random();
            return (int)Math.Floor((rnd.NextDouble() * num + 1));
        }

        // Check if safe to put in cell 
        public bool CheckIfSafe(int i, int j, int num)
        {
            return (unUsedInRow(i, num) &&
                    unUsedInCol(j, num) &&
                    unUsedInBox(i - i % 3, j - j % 3, num));
        }

        // check in the row for existence 
        public bool unUsedInRow(int i, int num)
        {
            for (int j = 0; j < 9; j++)
                if (mat[i,j] == num)
                    return false;
            return true;
        }

        // check in the row for existence 
        public bool unUsedInCol(int j, int num)
        {
            for (int i = 0; i < 9; i++)
                if (mat[i,j] == num)
                    return false;
            return true;
        }

        // A recursive function to fill remaining  
        // matrix 
        public bool fillRemaining(int i, int j)
        {
            //  System.out.println(i+" "+j); 
            if (j >= 9 && i < 9 - 1)
            {
                i = i + 1;
                j = 0;
            }
            if (i >= 9 && j >= 9)
                return true;

            if (i < 3)
            {
                if (j < 3)
                    j = 3;
            }
            else if (i < 9 - 3)
            {
                if (j == (int)(i / 3) * 3)
                    j = j + 3;
            }
            else
            {
                if (j == 9 - 3)
                {
                    i = i + 1;
                    j = 0;
                    if (i >= 9)
                        return true;
                }
            }

            for (int num = 1; num <= 9; num++)
            {
                if (CheckIfSafe(i, j, num))
                {
                    mat[i,j] = num;
                    if (fillRemaining(i, j + 1))
                        return true;

                    mat[i,j] = 0;
                }
            }
            return false;
        }

        // Remove the K no. of digits to 
        // complete game 
        public void removeKDigits(int K)
        {
            

            int count = K;
            while (count != 0)
            {
                int cellId = randomGenerator(9 * 9) -1;

                // System.out.println(cellId); 
                // extract coordinates i  and j 
                int i = (cellId / 9);
                int j = cellId % 9;
                if (j != 0)
                    j = j - 1;

                // System.out.println(i+" "+j); 
                if (mat[i,j] != 0)
                {
                    count--;
                    mat[i,j] = 0;
                }
            }

            //PrintBoard(mat);
        }
    }
}
