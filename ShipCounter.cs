using System.ComponentModel;

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
