using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sea_boy
{
    public enum ShipType
    {
        s1x1,
        s1x2,
        s1x3,
        s1x4
    }

    public class BattleShip
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ShipType Type { get; set; }

        public BattleShip(ShipType Type)
        {
            this.Type = Type;
            (Width, Height) = Presenter.sizeByType[Type];
        }

        public BattleShip(int width, int height)
        {
            Width = width;
            Height = height;
            Type = Presenter.typeBySize[(width < height) ? (width, height) : (height, width)];
        }

        public void Rotate()
        {
            int t = Width;
            Width = Height;
            Height = t;
        }
    }
}
