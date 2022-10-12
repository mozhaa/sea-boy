using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace sea_boy
{
    public enum Cell
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
    internal class Computer
    {

        public Cell[,] board;
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

        public void TellResult(int row, int column, Outcome result, int width=1, int height=1)
        {
            switch (result)
            {
                case Outcome.Miss:
                    board[row, column] = Cell.Empty; break;
                case Outcome.Hit:
                    board[row, column] = Cell.Hit; break;
                case Outcome.Kill:
                    for (int i = row; i < height + row; i++)
                        for (int j = column; j < width + column; j++)
                            board[i, j] = Cell.Kill;
                    break;
            }
        }

        public BattleShip?[,] GenerateBoard()
        {
            var newBoard = new BattleShip?[Presenter.rows, Presenter.columns];
            for (int i = 0; i < Presenter.rows; i++)
                for (int j = 0; j < Presenter.columns; j++)
                    newBoard[i, j] = null;
            var types = new ShipType[] { ShipType.s1x4, ShipType.s1x3, ShipType.s1x2, ShipType.s1x1 };
            for (int i = 0; i < 4; i++)
            {
                var shipType = types[i];
                var amount = i + 1;
                for (int j = 0; j < amount; j++)
                {
                    BattleShip battleShip;
                    int row, column;
                    while (true)
                    {
                        bool orientation = random.Next(3) % 2 == 0;
                        row = random.Next(Presenter.rows);
                        column = random.Next(Presenter.columns);
                        battleShip = new BattleShip(shipType, row, column);
                        if (orientation)
                            battleShip.Rotate();
                        if(Presenter.DoNotIntersectAndValidPositionByArray(newBoard, battleShip, row, column))
                            break;
                    }
                    for (int w = row; w < battleShip.Height + row; w++)
                        for (int h = column; h < battleShip.Width + column; h++)
                            newBoard[w, h] = battleShip;
                }
            }
            return newBoard;
        }
    }
}
