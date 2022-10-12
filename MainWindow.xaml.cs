﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sea_boy
{
    public class Constants
    {
        public static Brush boardBackgroundColor = Brushes.AliceBlue;
        public static Brush boardOpponentBackgroundColor = Brushes.LightGray;
        public static Brush battleshipColor = Brushes.Coral;
        public static Brush possibleShipColor = Brushes.LightSeaGreen;
        public static Brush invalidPossibleShipColor = Brushes.MediumVioletRed;
        public static double gridWidth = 500;
        public static double gridHeight = 500;
        public static double tileHeight = gridHeight / Presenter.rows;
        public static double tileWidth = gridWidth / Presenter.columns;

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal Presenter presenter = new Presenter();
        public ShipCounter shipCounter1x1 { get; set; } = new ShipCounter(4);
        public ShipCounter shipCounter1x2 { get; set; } = new ShipCounter(3);
        public ShipCounter shipCounter1x3 { get; set; } = new ShipCounter(2);
        public ShipCounter shipCounter1x4 { get; set; } = new ShipCounter(1);
        public Dictionary<ShipType, ShipCounter> shipCounterByType;
        private Rectangle? possibleShip;
        private Rectangle?[,] boardList = new Rectangle[Presenter.rows, Presenter.columns];
        private bool boardMouseHandled = false;
        public MainWindow()
        {
            InitializeComponent();

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

        public void Button1x1Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Clicked 1x1");
            if (shipCounter1x1.Number > 0)
            {
                presenter.SetCurrentShip(1, 1);
                SetPossibleShip(1, 1);
                HandleMouseOnBoardIfNotAlready();
            }
        }
        public void Button1x2Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Clicked 1x2");
            if (shipCounter1x2.Number > 0)
            {
                presenter.SetCurrentShip(1, 2);
                SetPossibleShip(1, 2);
                HandleMouseOnBoardIfNotAlready();
            }
        }
        public void Button1x3Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Clicked 1x3");
            if (shipCounter1x3.Number > 0)
            {
                presenter.SetCurrentShip(1, 3);
                SetPossibleShip(1, 3);
                HandleMouseOnBoardIfNotAlready();
            }
        }
        public void Button1x4Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Clicked 1x4");
            if (shipCounter1x4.Number > 0)
            {
                presenter.SetCurrentShip(1, 4);
                SetPossibleShip(1, 4);
                HandleMouseOnBoardIfNotAlready();
            }
        }

        private void HandleMouseOnBoardIfNotAlready()
        {
            if (!boardMouseHandled)
            {
                Board.MouseMove += Board_MouseMoveEvent;
                Board.MouseWheel += Board_MouseWheelEvent;
                boardMouseHandled = true;
            }
        }

        private void UnHandleMouseFromBoard()
        {
            Board.MouseMove -= Board_MouseMoveEvent;
            Board.MouseWheel -= Board_MouseWheelEvent;
            boardMouseHandled = false;
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
                if (e.ChangedButton == MouseButton.Left && presenter.DoNotIntersectAndValidPosition(boardList, presenter.currentShip!, row, column))
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
            var shipRectangle = boardList[row, column];
            if (shipRectangle == null)
                return;
            int width = Grid.GetColumnSpan(shipRectangle);
            int height = Grid.GetRowSpan(shipRectangle);
            int elemRow = Grid.GetRow(shipRectangle);
            int elemColumn = Grid.GetColumn(shipRectangle);
            FillBoardListWithRectangle(null, elemRow, elemColumn, height, width);
            Board.Children.Remove(shipRectangle);
            IncreaseShipCounter(new BattleShip(width, height));
            presenter.SetCurrentShip(width, height);
            SetPossibleShip(width, height);
            HandleMouseOnBoardIfNotAlready();
        }

        public void RemoveBattleShipFromBoard(int row, int column)
        {
            var shipRectangle = boardList[row, column];
            if (shipRectangle == null)
                return;
            int width = Grid.GetColumnSpan(shipRectangle);
            int height = Grid.GetRowSpan(shipRectangle);
            int elemRow = Grid.GetRow(shipRectangle);
            int elemColumn = Grid.GetColumn(shipRectangle);
            FillBoardListWithRectangle(null, elemRow, elemColumn, height, width);
            Board.Children.Remove(shipRectangle);
            IncreaseShipCounter(new BattleShip(width, height));
        }

        public void PutBattleShipOnBoard(BattleShip battleShip, int row, int column)
        {
            var shipRectangle = DrawBattleShip(battleShip, row, column, Constants.battleshipColor, 1.0);
            FillBoardListWithRectangle(shipRectangle, row, column, battleShip.Height, battleShip.Width);
            DecreaseShipCounter(battleShip);
            PutAwayShipFromHand();
        }

        public void PutAwayShipFromHand()
        {
            Board.Children.Remove(possibleShip);
            possibleShip = null;
            presenter.currentShip = null;
            UnHandleMouseFromBoard();
        }

        private void FillBoardListWithRectangle(Rectangle? rectangle, int row, int column, int height, int width)
        {
            // boardList.Where((line, index) => index >= row && index < row + height).
            boardList[row, column] = rectangle;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (!(i == 0 && j == 0))
                        boardList[row + i, column + j] = rectangle;
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
            ShowPossibleShip();
            MovePossibleShip(row, column);
            UpdatePossibleShipColor(row, column);
        }

        private void UpdatePossibleShipColor(int row, int column)
        {
            if (presenter.currentShip == null || possibleShip == null)
                // Log
                return;
            if (!presenter.DoNotIntersectAndValidPosition(boardList, presenter.currentShip, row, column))
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
            {
                // Log
                return;
            }
            possibleShip.Visibility = Visibility.Visible;
        }

        private void DecreaseShipCounter(BattleShip battleShip)
        {
            shipCounterByType[battleShip.Type].Number--;
            OnCounterModified();
        }

        private void IncreaseShipCounter(BattleShip battleShip)
        {
            shipCounterByType[battleShip.Type].Number++;
            OnCounterModified();
        }

        public void Board_MouseWheelEvent(object sender, RoutedEventArgs e)
        {
            if (presenter.currentShip != null && possibleShip != null)
            {
                presenter.currentShip.Rotate();
                // rotate Rectangle possibleShip
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

        }

        public void BoardOpponent_MouseDown(object sendet, MouseButtonEventArgs e)
        {

        }
    }
}
