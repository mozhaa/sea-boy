using System;
using System.Collections.Generic;
using System.Windows;

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
            for (int i = 0; i < Presenter.rows; i++)
            {
                for (int j = 0; j < Presenter.columns; j++)
                {
                    if (board[i, j] == CellState.Hit)
                    {
                        return NextIfHit(i, j);
                    }
                }
            }
            return RandomMove();
        }

        private (int, int) RandomMove()
        {
            var indexes = new List<(int, int)>();
            for (int i = 0; i < Presenter.rows; i++)
                for (int j = 0; j < Presenter.columns; j++)
                    if (board[i, j] == CellState.Unknown && AroundNoShips(i, j))
                        indexes.Add((i, j));
            int index = random.Next(indexes.Count);
            return indexes[index];
        }

        private bool AroundNoShips(int row, int column)
        {
            foreach ((int i, int j) in new List<(int, int)> { (-1, 0), (0, 1), (1, 0), (0, -1) })
                if (Valid(row + i, column + j) && (board[row + i, column + j] == CellState.Hit || board[row + i, column + j] == CellState.Kill))
                    return false;
            return true;
        }

        private bool Valid(int row, int column)
        {
            return 0 <= row && row < Presenter.rows && 0 <= column && column < Presenter.columns;
        }
        private (int, int) NextIfHit(int row, int column)
        {
            (int, int) unknownCell = (row, column);
            foreach ((int i, int j) in new List<(int, int)> { (-1, 0), (0, 1), (1, 0), (0, -1) })
            {
                if (Valid(row + i, column + j) && board[row + i, column + j] == CellState.Hit)
                    return NextInLineMove(row, column, i, j);
                if (Valid(row + i, column + j) && board[row + i, column + j] == CellState.Unknown)
                    unknownCell = (row + i, column + j);
            }
            if (unknownCell == (row, column))
                return RandomMove();
            return unknownCell;
        }

        private (int, int) NextInLineMove(int row, int column, int dx, int dy)
        {
            if (dy == 0)
            {
                for (int i = row; i < Presenter.rows; i++)
                {
                    if (board[i, column] == CellState.Unknown)
                        return (i, column);
                    if (board[i, column] == CellState.Empty)
                        break;
                }
                for (int i = row; i >= 0; i--)
                {
                    if (board[i, column] == CellState.Unknown)
                        return (i, column);
                    if (board[i, column] == CellState.Empty)
                        break;
                }
            }
            if (dx == 0)
            {
                for (int i = column; i < Presenter.columns; i++)
                {
                    if (board[row, i] == CellState.Unknown)
                        return (row, i);
                    if (board[row, i] == CellState.Empty)
                        break;
                }
                for (int i = column; i >= 0; i--)
                {
                    if (board[row, i] == CellState.Unknown)
                        return (row, i);
                    if (board[row, i] == CellState.Empty)
                        break;
                }
            }
            return RandomMove();
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
