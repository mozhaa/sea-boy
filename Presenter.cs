using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace sea_boy
{
    public enum Player
    {
        First,
        Second
    }
    public class Presenter
    {
        public readonly static int rows = 10;
        public readonly static int columns = 10;
        public readonly static int ShipLimit = 3;
        public IGameView view;
        public BattleShip? currentShip;
        public static Dictionary<ShipType, (int, int)> sizeByType = new()
        {
            { ShipType.s1x1, (1, 1) },
            { ShipType.s1x2, (1, 2) },
            { ShipType.s1x3, (1, 3) },
            { ShipType.s1x4, (1, 4) }
        };

        public static Dictionary<(int, int), ShipType> typeBySize = sizeByType.ToDictionary(x => x.Value, x => x.Key);
        public Player currentPlayer = Player.First;
        private Computer computer;
        private BattleShip?[,] playerBoard;
        private BattleShip[,] opponentBoard;
        private CellState[,] opponentBoardPlayerView;

        public Presenter(IGameView view)
        {
            this.view = view;
        }

        public void SetCurrentShip(int width, int height, int row, int column)
        {
            currentShip = new BattleShip(width, height, row, column);
        }

        public void MoveCurrentShip(int row, int column)
        {
            currentShip.Row = row;
            currentShip.Column = column;
        }

        public static bool IsValidCoordinate(int row, int column)
        {
            return 0 <= row && row < rows && 0 <= column && column < columns;
        }

        public bool IsValidCurrentShipPosition(int row, int column)
        {
            if (currentShip == null)
                return false;
            return IsValidCoordinate(row, column) && IsValidCoordinate(row + currentShip.Height - 1, column + currentShip.Width - 1);
        }

        public static bool DoNotIntersectsWithOtherShips(System.Windows.Shapes.Rectangle?[,] boardList, BattleShip battleShip, int row, int column)
        {
            for (int i = -1; i <= battleShip.Height; i++)
                for (int j = -1; j <= battleShip.Width; j++)
                    if (IsValidCoordinate(row + i, column + j) && boardList[row + i, column + j] != null)
                        return false;
            return true;
        }

        public static bool IsValidShipPosition(BattleShip battleShip, int row, int column)
        {
            return IsValidCoordinate(row, column) && IsValidCoordinate(row + battleShip.Height - 1, column + battleShip.Width - 1);
        }

        public static bool DoNotIntersectsWithOtherShipsByArray(BattleShip?[,] boardList, BattleShip battleShip, int row, int column)
        {
            for (int i = -1; i <= battleShip.Height; i++)
                for (int j = -1; j <= battleShip.Width; j++)
                    if (IsValidCoordinate(row + i, column + j) && boardList[row + i, column + j] != null)
                        return false;
            return true;
        }

        public static bool DoNotIntersectAndValidPositionByArray(BattleShip?[,] boardList, BattleShip battleShip, int row, int column)
        {
            return DoNotIntersectsWithOtherShipsByArray(boardList, battleShip, row, column) && IsValidShipPosition(battleShip, row, column);
        }

        public bool DoNotIntersectAndValidPosition(System.Windows.Shapes.Rectangle?[,] boardList, BattleShip battleShip, int row, int column)
        {
            return DoNotIntersectsWithOtherShips(boardList, battleShip, row, column) && IsValidCurrentShipPosition(row, column);
        }

        private List<CellState> GetAllBattleShipState(BattleShip battleShip, CellState[,] board)
        {
            int row = battleShip.Row, column = battleShip.Column, width = battleShip.Width, height = battleShip.Height;
            var result = new List<CellState>();
            for (int i = row; i < height + row; i++)
                for (int j = column; j < width + column; j++)
                    result.Add(board[i, j]);
            return result;
        }

        private bool IsShipDead(BattleShip battleShip, CellState[,] board)
        {
            foreach (var cell in GetAllBattleShipState(battleShip, board))
                if (cell == CellState.Unknown)
                    return false;
            return true;
        }

        public void StartGame(BattleShip?[,] boardArray)
        {
            computer = new Computer();
            opponentBoard = computer.GenerateBoard();
            playerBoard = boardArray;
            opponentBoardPlayerView = new CellState[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    opponentBoardPlayerView[i, j] = CellState.Unknown;
        }

        public void ClickedOn(int row, int column)
        {
            Trace.WriteLine("Player Clicked");
            if (opponentBoardPlayerView[row, column] != CellState.Unknown)
                return;

            if (opponentBoard[row, column] == null)
            {
                opponentBoardPlayerView[row, column] = CellState.Empty;
                view.PaintCellByState(row, column, CellState.Empty, Player.Second);
                EndOfPlayerMove();
                return;
            }

            var battleShip = opponentBoard[row, column];
            if (IsShipDead(battleShip, opponentBoardPlayerView))
            {
                for (int i = battleShip.Row; i < battleShip.Height + battleShip.Row; i++)
                    for (int j = battleShip.Column; j < battleShip.Width + battleShip.Column; i++)
                    {
                        opponentBoardPlayerView[i, j] = CellState.Kill;
                        view.PaintCellByState(i, j, CellState.Kill, Player.Second);
                    }
                return;
            }

            opponentBoardPlayerView[row, column] = CellState.Hit;
            view.PaintCellByState(row, column, CellState.Hit, Player.Second);
            return;
        }

        private void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == Player.First) ? Player.Second : Player.First;
        }

        private void EndOfPlayerMove()
        {
            SwitchPlayer();
            MakeComputerMove();
            SwitchPlayer();
        }

        private void MakeComputerMove()
        {
            while (true)
            {
                (int row, int column) = computer.AskMove();
                var battleShip = playerBoard[row, column];
                if (battleShip == null)
                {
                    computer.TellResult(row, column, Outcome.Miss);
                    view.PaintCellByState(row, column, CellState.Empty, Player.First);
                    break;
                }
                if (IsShipDead(battleShip, computer.board))
                {
                    computer.TellResult(row, column, Outcome.Kill, battleShip.Width, battleShip.Height);
                    for (int i = battleShip.Row; i < battleShip.Row + battleShip.Height; i++)
                        for (int j = battleShip.Column; j < battleShip.Column + battleShip.Width; j++)
                            view.PaintCellByState(i, j, CellState.Kill, Player.First);
                }
                else
                {
                    computer.TellResult(row, column, Outcome.Hit);
                    view.PaintCellByState(row, column, CellState.Hit, Player.First);
                }
            }
        }
    }
}