using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace sea_boy
{
    public class GameBoard
    {
        private int Height = Presenter.rows;
        private int Width = Presenter.columns;

        private Dictionary<string, Brush> Palette;

        private Grid ParentGrid;

        public struct CellStack
        {
            public Rectangle Background;
            public UIElement Figure;
            public Rectangle Cover;
        }

        private CellStack[,] Board;

        public GameBoard(Grid grid, Dictionary<string, Brush> palette)
        {
            ParentGrid = grid;
            Board = new CellStack[Height, Width];
            Palette = palette;
            InitializeStacks();
        }

        private void InitializeStacks()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Board[i, j] = CreateStack(i, j);
                }
            }
        }

        private CellStack CreateStack(int row, int column)
        {
            var cellStack = new CellStack();

            cellStack.Background = new Rectangle()
            {
                Height = (int)Constants.tileHeight,
                Width = (int)Constants.tileWidth,
                Fill = Palette["background"]
            };
            cellStack.Figure = new Path()
            {
                Width = (int)Constants.tileWidth,
                Height = (int)Constants.tileHeight
            };
            cellStack.Cover = new Rectangle()
            {
                Height = (int)Constants.tileHeight,
                Width = (int)Constants.tileWidth,
                Fill = Palette["cover"]
            };

            ParentGrid.Children.Add(cellStack.Background);
            ParentGrid.Children.Add(cellStack.Figure);
            ParentGrid.Children.Add(cellStack.Cover);
            Grid.SetRow(cellStack.Background, row);
            Grid.SetColumn(cellStack.Background, column);
            Grid.SetRow(cellStack.Figure, row);
            Grid.SetColumn(cellStack.Figure, column);
            Grid.SetRow(cellStack.Cover, row);
            Grid.SetColumn(cellStack.Cover, column);
            return cellStack;
        }

        public void SetShips(BattleShip?[,] battleShips)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (battleShips[i, j] != null)
                    {
                        Board[i, j].Background.Fill = Palette["ship"];
                    }
                }
            }
        }

        public void UncoverCell(int row, int column)
        {
            Board[row, column].Cover.Fill = Brushes.Transparent;
        }

        private UIElement Circle(string color)
        {
            var scale = 0.8;
            var radiusX = (int)(Constants.tileWidth * scale / 2);
            var radiusY = (int)(Constants.tileHeight * scale / 2);
            var centerX = (int)(Constants.tileWidth / 2);
            var centerY = (int)(Constants.tileHeight / 2);
            return new Path()
            {
                Data = Geometry.Parse($"M {centerX - radiusX},{centerY} A {radiusX},{radiusY} 0 1 1 {centerX + radiusX},{centerY} M {centerX - radiusX},{centerY} A {radiusX},{radiusY} 0 1 0 {centerX + radiusX},{centerY}"),
                StrokeThickness = 3,
                Stroke = Palette[color]
            };
        }

        private UIElement Cross(string color)
        {
            var scale = 0.8;
            var radiusX = (int)(Constants.tileWidth * scale / 2);
            var radiusY = (int)(Constants.tileHeight * scale / 2);
            var centerX = (int)(Constants.tileWidth / 2);
            var centerY = (int)(Constants.tileHeight / 2);
            return new Path()
            {
                Data = Geometry.Parse($"M {centerX - radiusX},{centerY - radiusY} L {centerX + radiusX},{centerY + radiusY} M {centerX + radiusX},{centerY - radiusY} L {centerX - radiusX},{centerY + radiusY}"),
                StrokeThickness = 3,
                Stroke = Palette[color]
            };
        }

        private UIElement Point(string color)
        {
            var scale = 0.3;
            var diameterX = (int)(Constants.tileWidth * scale);
            var diameterY = (int)(Constants.tileHeight * scale);
            return new Ellipse()
            {
                Width = diameterY,
                Height = diameterX,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Fill = Palette[color]
            };

        }
        private void AddElementToGrid(UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            ParentGrid.Children.Add(element);
        }
        private void SetFigure(int row, int column, UIElement figure)
        {
            ParentGrid.Children.Remove(Board[row, column].Figure);
            Board[row, column].Figure = figure;
            AddElementToGrid(Board[row, column].Figure, row, column);
        }

        public void SetCross(int row, int column, string color = "cross")
        {
            SetFigure(row, column, Cross(color));
        }

        public void SetCircle(int row, int column, string color = "circle")
        {
            SetFigure(row, column, Circle(color));
        }

        public void SetPoint(int row, int column, string color = "point")
        {
            SetFigure(row, column, Point(color));
        }
    }
}
