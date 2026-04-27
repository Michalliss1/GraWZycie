using System;

namespace GraWZycie
{
    public class GameEngine
    {
        public bool[,] Board { get; private set; }

        public int Rows { get; }
        public int Cols { get; }

        public int Generation { get; private set; }

        // statystyki bieżącej generacji
        public int AliveCount { get; private set; }
        public int BornCount { get; private set; }
        public int DeadCount { get; private set; }

        // statystyki od początku
        public int TotalBorn { get; private set; }
        public int TotalDead { get; private set; }

        public GameEngine(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Board = new bool[rows, cols];
        }

        public void Clear()
        {
            Board = new bool[Rows, Cols];

            Generation = 0;
            AliveCount = 0;
            BornCount = 0;
            DeadCount = 0;

            TotalBorn = 0;
            TotalDead = 0;
        }

        public void Randomize()
        {
            var rand = new Random();

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Board[r, c] = rand.Next(2) == 1;
        }

        public void ToggleCell(int r, int c)
        {
            Board[r, c] = !Board[r, c];
        }

        private int CountNeighbors(int r, int c)
        {
            int count = 0;

            for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0)
                    continue;

                int nr = r + dr;
                int nc = c + dc;

                if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols)
                    if (Board[nr, nc])
                        count++;
            }

            return count;
        }

        public void NextGeneration()
        {
            bool[,] newBoard = new bool[Rows, Cols];

            AliveCount = 0;
            BornCount = 0;
            DeadCount = 0;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    int neighbors = CountNeighbors(r, c);

                    bool isAliveNext = false;

                    if (Board[r, c])
                    {
                        // żyje
                        if (neighbors == 2 || neighbors == 3)
                        {
                            isAliveNext = true;
                        }
                        else
                        {
                            DeadCount++;
                        }
                    }
                    else
                    {
                        // martwa
                        if (neighbors == 3)
                        {
                            isAliveNext = true;
                            BornCount++;
                        }
                    }

                    newBoard[r, c] = isAliveNext;

                    if (isAliveNext)
                        AliveCount++;
                }
            }

            Board = newBoard;
            Generation++;

            // statystyki od początku
            TotalBorn += BornCount;
            TotalDead += DeadCount;
        }
    }
}