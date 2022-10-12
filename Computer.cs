using System;
using System.Collections.Generic;
using System.Linq;
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

        public BattleShip[,] GenerateBoard()
        {
            var newBoard = new BattleShip[Presenter.rows, Presenter.columns];
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
                        battleShip = new BattleShip(shipType);
                        if (orientation)
                            battleShip.Rotate();
                        if(Presenter.DoNotIntersectAndValidPositionByArray(newBoard, battleShip, row, column))
                            break;
                    }
                    for (int w = row; w < battleShip.Width + row; w++)
                        for (int h = column; h < battleShip.Height + column; h++)
                            newBoard[w, h] = battleShip;
                }
            }
            return newBoard;
        }
    }
}
