using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace sea_boy
{
    public class ShipCounter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _number;

        public int Number
        {
            get
            {
                return _number;
            }
            set
            {
                if (_number != value)
                {
                    _number = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
                }
            }
        }

        public int ShipLimit;

        public string Label 
        { 
            get 
            { 
                return Number.ToString() + "/" + ShipLimit.ToString(); 
            } 
        }
        public ShipCounter(int shipLimit)
        {
            Number = shipLimit;
            ShipLimit = shipLimit;
        }
    }
}
