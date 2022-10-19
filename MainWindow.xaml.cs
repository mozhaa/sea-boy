using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace sea_boy
{
    public class Constants
    {
        public static Brush boardBackgroundColor = Brushes.AliceBlue;
        public static Brush boardOpponentBackgroundColor = Brushes.Silver;
        public static Brush battleshipColor = Brushes.Aqua;
        public static Brush possibleShipColor = Brushes.LightSeaGreen;
        public static Brush invalidPossibleShipColor = Brushes.MediumVioletRed;
        public static double gridWidth = 500;
        public static double gridHeight = 500;
        public static double tileHeight = gridHeight / Presenter.rows;
        public static double tileWidth = gridWidth / Presenter.columns;

        public static Dictionary<string, Brush> playerPalette = new()
        {
            { "background", boardBackgroundColor },
            { "ship", battleshipColor },
            { "cover", Brushes.Transparent },
            { "point", Brushes.LightBlue },
            { "circle", Brushes.MediumTurquoise },
            { "cross", Brushes.SteelBlue },
            { "noships", Brushes.Azure }
        };
        public static Dictionary<string, Brush> opponentPalette = new()
        {
            { "background", Brushes.Lavender },
            { "ship", Brushes.Violet },
            { "cover", boardOpponentBackgroundColor },
            { "point", Brushes.Transparent },
            { "circle", Brushes.MediumOrchid },
            { "cross", Brushes.MediumVioletRed },
            { "noships", Brushes.LightGray }
        };
        public static Dictionary<Player, Dictionary<string, Brush>> PaletteByPlayer = new()
        {
            { Player.First, playerPalette },
            { Player.Second, opponentPalette }
        };
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IGameView
    {
        internal Presenter presenter;
        public ShipCounter shipCounter1x1 { get; set; } = new ShipCounter(4);
        public ShipCounter shipCounter1x2 { get; set; } = new ShipCounter(3);
        public ShipCounter shipCounter1x3 { get; set; } = new ShipCounter(2);
        public ShipCounter shipCounter1x4 { get; set; } = new ShipCounter(1);

        public Dictionary<ShipType, ShipCounter> shipCounterByType;
        private Rectangle? possibleShip;
        private Rectangle?[,] board = new Rectangle[Presenter.rows, Presenter.columns];
        public BattleShip?[,] boardArray = new BattleShip[Presenter.rows, Presenter.columns];
        private bool boardMouseMoveWheelHandled = false;
        private GameBoard playerBoard;
        private GameBoard opponentBoard;
        private Dictionary<Player, GameBoard> boardByPlayer;
        public MainWindow()
        {
            presenter = new Presenter(this);
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyR);
            shipCounterByType = new()
            {
                { ShipType.s1x1, shipCounter1x1},
                { ShipType.s1x2, shipCounter1x2},
                { ShipType.s1x3, shipCounter1x3},
                { ShipType.s1x4, shipCounter1x4}
            };

            InitializeBoard(Board);
            InitializeBoard(BoardOpponent);

            
        }

        private void InitializeBoard(Grid BoardGrid)
        {
            for (int i = 0; i < Presenter.rows; i++)
                BoardGrid.RowDefinitions.Add(new RowDefinition() { SharedSizeGroup = "CELL" });
            for (int i = 0; i < Presenter.columns; i++)
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition() { SharedSizeGroup = "CELL" });
        }

        public void MainWindow_KeyR(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                Board_MouseWheelEvent(sender, e);
            }
        }

        public void Button1x1Click(object sender, RoutedEventArgs e)
        {
            if (shipCounter1x1.Number > 0)
            {
                presenter.SetCurrentShip(1, 1, 0, 0);
                SetPossibleShip(1, 1);
                HandleMouseMoveWheelOnBoardIfNotAlready();
            }
        }
        public void Button1x2Click(object sender, RoutedEventArgs e)
        {
            if (shipCounter1x2.Number > 0)
            {
                presenter.SetCurrentShip(1, 2, 0, 0);
                SetPossibleShip(1, 2);
                HandleMouseMoveWheelOnBoardIfNotAlready();
            }
        }
        public void Button1x3Click(object sender, RoutedEventArgs e)
        {
            if (shipCounter1x3.Number > 0)
            {
                presenter.SetCurrentShip(1, 3, 0, 0);
                SetPossibleShip(1, 3);
                HandleMouseMoveWheelOnBoardIfNotAlready();
            }
        }
        public void Button1x4Click(object sender, RoutedEventArgs e)
        {
            if (shipCounter1x4.Number > 0)
            {
                presenter.SetCurrentShip(1, 4, 0, 0);
                SetPossibleShip(1, 4);
                HandleMouseMoveWheelOnBoardIfNotAlready();
            }
        }

        private void HandleMouseMoveWheelOnBoardIfNotAlready()
        {
            if (!boardMouseMoveWheelHandled)
            {
                Board.MouseMove += Board_MouseMoveEvent;
                Board.MouseWheel += Board_MouseWheelEvent;
                boardMouseMoveWheelHandled = true;
            }
        }

        private void UnHandleMouseMoveWheelFromBoard()
        {
            Board.MouseMove -= Board_MouseMoveEvent;
            Board.MouseWheel -= Board_MouseWheelEvent;
            boardMouseMoveWheelHandled = false;
        }

        private void UnHandleMouseFromBoard()
        {
            Board.MouseDown -= Board_MouseDown;
        }

        private int GetRowByPoint(Point clickedOn)
        {
            double y = clickedOn.Y;
            int row = (int)(y / Constants.tileHeight);
            return row;
        }

        private int GetColumnByPoint(Point clickedOn)
        {
            double x = clickedOn.X;
            int column = (int)(x / Constants.tileWidth);
            return column;
        }

        private int GetRowClickedOn(MouseButtonEventArgs e, Grid grid)
        {
            return GetRowByPoint(GetPointClickedOn(e, grid));
        }

        private int GetColumnClickedOn(MouseButtonEventArgs e, Grid grid)
        {
            return GetColumnByPoint(GetPointClickedOn(e, grid));
        }

        private (int, int) GetRowColumn(MouseButtonEventArgs e, Grid grid)
        {
            return (GetRowClickedOn(e, grid), GetColumnClickedOn(e, grid));
        }

        private Point GetPointClickedOn(MouseButtonEventArgs e, Grid BoardGrid)
        {
            return e.GetPosition(BoardGrid);
        }


        public void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (int row, int column) = GetRowColumn(e, Board);
            if (possibleShip == null || presenter.currentShip == null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    PickBattleShipFromBoard(row, column);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    RemoveBattleShipFromBoard(row, column);
                }
            }
            else
            {
                if (e.ChangedButton == MouseButton.Left && presenter.DoNotIntersectAndValidPosition(board, presenter.currentShip!, row, column))
                {
                    PutBattleShipOnBoard(presenter.currentShip, row, column);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    PutAwayShipFromHand();
                }
            }
        }

        public void PickBattleShipFromBoard(int row, int column)
        {
            var shipRectangle = board[row, column];
            if (shipRectangle == null)
                return;
            int width = Grid.GetColumnSpan(shipRectangle);
            int height = Grid.GetRowSpan(shipRectangle);
            int elemRow = Grid.GetRow(shipRectangle);
            int elemColumn = Grid.GetColumn(shipRectangle);
            FillBoardListWithRectangle(null, elemRow, elemColumn, height, width);
            FillBoardArrayWithBattleShip(null, elemRow, elemColumn, height, width);
            Board.Children.Remove(shipRectangle);
            IncreaseShipCounter(Presenter.GetType(width, height));
            presenter.SetCurrentShip(width, height, row, column);
            SetPossibleShip(width, height);
            HandleMouseMoveWheelOnBoardIfNotAlready();
        }

        public void RemoveBattleShipFromBoard(int row, int column)
        {
            var shipRectangle = board[row, column];
            if (shipRectangle == null)
                return;
            int width = Grid.GetColumnSpan(shipRectangle);
            int height = Grid.GetRowSpan(shipRectangle);
            int elemRow = Grid.GetRow(shipRectangle);
            int elemColumn = Grid.GetColumn(shipRectangle);
            FillBoardListWithRectangle(null, elemRow, elemColumn, height, width);
            FillBoardArrayWithBattleShip(null, elemRow, elemColumn, height, width);
            Board.Children.Remove(shipRectangle);
            IncreaseShipCounter(Presenter.GetType(width, height));
        }

        public void PutBattleShipOnBoard(BattleShip battleShip, int row, int column)
        {
            var shipRectangle = DrawBattleShip(battleShip, row, column, Constants.battleshipColor, 1.0);
            FillBoardListWithRectangle(shipRectangle, row, column, battleShip.Height, battleShip.Width);
            FillBoardArrayWithBattleShip(battleShip, row, column);
            DecreaseShipCounter(battleShip.Type);
            if (shipCounterByType[presenter.currentShip.Type].Number == 0)
                PutAwayShipFromHand();
            else
            {
                PutAwayShipFromHand();
                presenter.SetCurrentShip(battleShip.Width, battleShip.Height, row, column);
                SetPossibleShip(battleShip.Width, battleShip.Height);
                HandleMouseMoveWheelOnBoardIfNotAlready();
            }
        }
        
        public void PutAwayShipFromHand()
        {
            Board.Children.Remove(possibleShip);
            possibleShip = null;
            presenter.currentShip = null;
            UnHandleMouseMoveWheelFromBoard();
        }

        private void FillBoardListWithRectangle(Rectangle? rectangle, int row, int column, int height, int width)
        { 
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                       board[row + i, column + j] = rectangle;
        }

        private void FillBoardArrayWithBattleShip(BattleShip battleShip, int row, int column)
        {
            for (int i = 0; i < battleShip.Height; i++)
                for (int j = 0; j < battleShip.Width; j++)
                    boardArray[row + i, column + j] = battleShip;
        }

        private void FillBoardArrayWithBattleShip(BattleShip? battleShip, int row, int column, int heigth, int width)
        {
            for (int i = 0; i < heigth; i++)
                for (int j = 0; j < width; j++)
                    boardArray[row + i, column + j] = battleShip;
        }

        private Rectangle DrawBattleShip(BattleShip battleShip, int? row, int? column, Brush fill, double opacity)
        {
            var shipRectangle = new Rectangle()
            {
                Width = Constants.gridWidth * battleShip.Width,
                Height = Constants.gridHeight * battleShip.Height,
                Fill = fill,
                Opacity = opacity
            };
            if (row.HasValue && column.HasValue)
            {
                Grid.SetRow(shipRectangle, row.Value);
                Grid.SetColumn(shipRectangle, column.Value);
            }
            else
                shipRectangle.Visibility = Visibility.Hidden;

            Grid.SetRowSpan(shipRectangle, battleShip.Height);
            Grid.SetColumnSpan(shipRectangle, battleShip.Width);
            Board.Children.Add(shipRectangle);
            return shipRectangle;
        }

        public void Board_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            Point clickedOn = e.GetPosition(Board);
            int row = GetRowByPoint(clickedOn);
            int column = GetColumnByPoint(clickedOn);
            if (presenter.currentShip == null || possibleShip == null)
                return;
            presenter.MoveCurrentShip(row, column);
            ShowPossibleShip();
            MovePossibleShip(row, column);
            UpdatePossibleShipColor(row, column);
        }

        private void UpdatePossibleShipColor(int row, int column)
        {
            if (presenter.currentShip == null || possibleShip == null)
                return;
            if (!presenter.DoNotIntersectAndValidPosition(board, presenter.currentShip, row, column))
                possibleShip.Fill = Constants.invalidPossibleShipColor;
            else
                possibleShip.Fill = Constants.possibleShipColor;
        }

        public void MovePossibleShip(int row, int column)
        {
            Grid.SetRow(possibleShip, row);
            Grid.SetColumn(possibleShip, column);
        }

        public void SetPossibleShip(int width, int height)
        {
            Board.Children.Remove(possibleShip);
            possibleShip = DrawBattleShip(presenter.currentShip!, null, null, Constants.possibleShipColor, 0.6);
        }

        public void HidePossibleShip()
        {
            possibleShip!.Visibility = Visibility.Hidden;
        }

        public void ShowPossibleShip()
        {
            if(possibleShip == null)
                return;
            possibleShip.Visibility = Visibility.Visible;
        }

        private void DecreaseShipCounter(ShipType type)
        {
            shipCounterByType[type].Number--;
            OnCounterModified();
        }

        private void IncreaseShipCounter(ShipType type)
        {
            shipCounterByType[type].Number++;
            OnCounterModified();
        }

        public void Board_MouseWheelEvent(object sender, RoutedEventArgs e)
        {
            if (presenter.currentShip != null && possibleShip != null)
            {
                presenter.currentShip.Rotate();
                var height = possibleShip.Height;
                possibleShip.Height = possibleShip.Width;
                possibleShip.Width = height;

                var row = Grid.GetRow(possibleShip);
                var column = Grid.GetColumn(possibleShip);
                var rowSpan = Grid.GetRowSpan(possibleShip);
                var columnSpan = Grid.GetColumnSpan(possibleShip);
                Grid.SetColumnSpan(possibleShip, rowSpan);
                Grid.SetRowSpan(possibleShip, columnSpan);
                UpdatePossibleShipColor(row, column);
            }
        }

        private void OnCounterModified()
        {
            foreach (var counter in shipCounterByType.Values)
                if (counter.Number > 0)
                {
                    DeactivateSaveButton();
                    return;
                }
            ActivateSaveButton();
        }

        public void ActivateSaveButton()
        {
            SaveButton.IsEnabled = true;
        }

        private void DeactivateSaveButton()
        {
            SaveButton.IsEnabled = false;
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchToPlayingMode();
        }

        private void SwitchToPlayingMode()
        {
            Buttons.Visibility = Visibility.Collapsed;
            BoardOpponent_Border.Visibility = Visibility.Visible;
            Width = 1200;
            ClearPlayerBoard();
            var mode = ModeCheckBox.IsChecked.Value ? Mode.Easy : Mode.Hard;
            presenter.StartGame(boardArray, mode);
            UnHandleMouseFromBoard();
            if (possibleShip != null)
                HidePossibleShip();

            playerBoard = new GameBoard(Board, Constants.PaletteByPlayer[Player.First]);
            playerBoard.SetShips(boardArray);
            opponentBoard = new GameBoard(BoardOpponent, Constants.PaletteByPlayer[Player.Second]);
            opponentBoard.SetShips(presenter.opponentBoard);
            
            boardByPlayer = new()
            {
                { Player.First, playerBoard },
                { Player.Second, opponentBoard }
            };
        }

        public void BoardOpponent_MouseDown(object sendet, MouseButtonEventArgs e)
        {
            if (presenter.currentPlayer == Player.First)
            {
                (int row, int column) = GetRowColumn(e, BoardOpponent);
                presenter.ClickedOn(row, column);
            }
        }

        private void ClearPlayerBoard()
        {
            Board.Children.Clear();
        }

        public void ChangeCellStackByState(int row, int column, CellState state, Player player)
        {
            var board = boardByPlayer[player];
            switch (state)
            {
                case CellState.Empty:
                    board.UncoverCell(row, column);
                    board.SetPoint(row, column);
                    break;
                case CellState.Hit:
                    board.UncoverCell(row, column);
                    board.SetCircle(row, column);
                    break;
                case CellState.Kill:
                    board.UncoverCell(row, column);
                    board.SetCross(row, column);
                    break;
            }
        }

        public void SetShipKilled(BattleShip battleShip, Player player)
        {
            for (int i = battleShip.Row; i < battleShip.Row + battleShip.Height; i++)
            {
                for (int j = battleShip.Column; j < battleShip.Column + battleShip.Width; j++)
                {
                    ChangeCellStackByState(i, j, CellState.Kill, player);
                }
            }
            for (int i = battleShip.Row - 1; i <= battleShip.Row + battleShip.Height; i++)
            {
                for (int j = battleShip.Column - 1; j <= battleShip.Column + battleShip.Width; j++)
                {
                    if (Presenter.IsValidCoordinate(i, j) && (i == battleShip.Row + battleShip.Height || 
                                                              i == battleShip.Row - 1 ||
                                                              j == battleShip.Column + battleShip.Width ||
                                                              j == battleShip.Column - 1))
                    {
                        boardByPlayer[player].SetCoverNoShips(i, j);
                    }
                }
            }
        }

        private void UnhandleMouseFromOpponentBoard()
        {
            BoardOpponent.MouseDown -= BoardOpponent_MouseDown;
        }

        public void GameOver(Player winner)
        {
            UnhandleMouseFromOpponentBoard();
            GameOverText.Text = (winner == Player.First) ? "Вы победили!" : "Вы проиграли!";
            GameOverText_Board.Visibility = Visibility.Visible;
        }
    }
}
