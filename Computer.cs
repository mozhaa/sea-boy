using System;
using System.Collections.Generic;

namespace sea_boy
{
    public enum CellState
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

        public CellState[,] board;
        private Random random = new Random();

        public Computer()
        {
            board = new CellState[Presenter.rows, Presenter.columns];
            for(int i = 0; i < Presenter.rows; i++)
                for(int j = 0; j < Presenter.columns; j++)
                    board[i, j] = CellState.Unknown;
        }

        public (int, int) AskMove()
        {
            int row, col;
            var indexes = new List<(int, int)>();
            for (int i = 0; i < Presenter.rows; i++)
                for (int j = 0; j < Presenter.columns; j++)
                    if (board[i, j] == CellState.Unknown)
                        indexes.Add((i, j));
            int index = random.Next(indexes.Count);
            return indexes[index];
        }

        public void TellResult(int row, int column, Outcome result, int width=1, int height=1)
        {
            switch (result)
            {
                case Outcome.Miss:
                    board[row, column] = CellState.Empty; break;
                case Outcome.Hit:
                    board[row, column] = CellState.Hit; break;
                case Outcome.Kill:
                    for (int i = row; i < height + row; i++)
                        for (int j = column; j < width + column; j++)
                            board[i, j] = CellState.Kill;
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
