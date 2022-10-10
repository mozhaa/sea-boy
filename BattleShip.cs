using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sea_boy
{
    public class BattleShip
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public BattleShip(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Rotate()
        {
            int t = Width;
            Width = Height;
            Height = t;
        }
    }
}
