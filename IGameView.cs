using System.Windows.Media;

namespace sea_boy
{
    public interface IGameView
    {
        void PaintCell(int row, int column, Brush brush, Player player);
        void PaintCellByState(int row, int column, CellState state, Player player);
        void ShowPossibleShip();
    }
}