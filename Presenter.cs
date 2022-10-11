using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace sea_boy
{
    internal class Presenter
    {
        public readonly static int rows = 10;
        public readonly static int columns = 10;
        public readonly static int ShipLimit = 3;
        public BattleShip? currentShip;
        public static Dictionary<ShipType, (int, int)> sizeByType = new()
        {
            { ShipType.s1x1, (1, 1) },
            { ShipType.s1x2, (1, 2) },
            { ShipType.s1x3, (1, 3) },
            { ShipType.s1x4, (1, 4) }
        };

        public static Dictionary<(int, int), ShipType> typeBySize = sizeByType.ToDictionary(x => x.Value, x => x.Key);

        public void SetCurrentShip(int width, int height)
        {
            currentShip = new BattleShip(width, height);
        }

        public static bool IsValidCoordinate(int row, int column)
        {
            return 0 <= row && row < rows && 0 <= column && column < columns;
        }

        public bool IsValidShipPosition(int row, int column)
        {
            if (currentShip == null)
                return false;
            return IsValidCoordinate(row, column) && IsValidCoordinate(row + currentShip.Height - 1, column + currentShip.Width - 1);
        }

        public bool DoNotIntersectsWithOtherShips(System.Windows.Shapes.Rectangle?[,] boardList, BattleShip battleShip, int row, int column)
        {
            for (int i = -1; i <= battleShip.Height; i++)
            {
                for (int j = -1; j <= battleShip.Width; j++)
                {
                    if (IsValidCoordinate(row + i, column + j) && boardList[row + i, column + j] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool DoNotIntersectAndValidPosition(System.Windows.Shapes.Rectangle?[,] boardList, BattleShip battleShip, int row, int column)
        {
            return DoNotIntersectsWithOtherShips(boardList, battleShip, row, column) && IsValidShipPosition(row, column);
        }
    }
}
