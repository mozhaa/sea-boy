using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace sea_boy
{
    internal class Computer
    {
        private enum Cell
        {
            Unknown,
            Empty,
            Hit,
            Kill
        }

        public enum Outcome
        {
            Miss,
            Hit,
            Kill
        }

        private Cell[,] board;
        private Random random = new Random();

        public Computer()
        {
            board = new Cell[Presenter.rows, Presenter.columns];
            for(int i = 0; i < Presenter.rows; i++)
                for(int j = 0; j < Presenter.columns; j++)
                    board[i, j] = Cell.Unknown;
        }

        public (int, int) AskMove()
        {
            int row, col;
            while (true)
            {
                row = random.Next(Presenter.rows);
                col = random.Next(Presenter.columns);
                if (board[row, col] == Cell.Unknown)
                    break;
            }
            return (row, col);
        }

        public void TellResult(int row, int column, Outcome result)
        {
            switch (result)
            {
                case Outcome.Miss:
                    board[row, column] = Cell.Empty; break;
                case Outcome.Hit:
                    board[row, column] = Cell.Hit; break;
                case Outcome.Kill:
                    board[row, column] = Cell.Kill; break;
            }
        }
    }
}
