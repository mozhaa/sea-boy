using System.Windows.Media;

namespace sea_boy
{
    public interface IGameView
    {
        void ChangeCellStackByState(int row, int column, CellState state, Player player);
        void GameOver(Player winner);
        void SetShipKilled(BattleShip battleShip, Player player);
    }
}