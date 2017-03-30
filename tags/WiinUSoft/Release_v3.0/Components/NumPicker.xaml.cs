using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for NumPicker.xaml
    /// </summary>
    public partial class NumPicker : UserControl
    {
        /// <summary>
        /// Sets the selected value.
        /// Must be between the current minimum and maximum.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                if (value < _min)
                {
                    _value = _min;
                }
                else if (value > _max)
                {
                    _value = _max;
                }
                else
                {
                    _value = value;
                }

                lblValue.Text = _value.ToString();
            }
        }

        /// <summary>
        /// Sets the Minimum possible value.
        /// Must be less than the current maximum.
        /// </summary>
        public int Min
        {
            get { return _min; }
            set
            {
                if (_min <= _max)
                {
                    _min = value;

                    if (_value < value)
                    {
                        _value = value;
                        lblValue.Text = _value.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Maximum possible value.
        /// Must be greater than the current minimum.
        /// </summary>
        public int Max
        {
            get { return _max; }
            set
            {
                if (_max >= _min)
                {
                    _max = value;

                    if (_value > value)
                    {
                        _value = value;
                        lblValue.Text = _value.ToString();
                    }
                }
            }
        }

        private int _value = 0;
        private int _min = 0;
        private int _max = 100;

        public NumPicker()
        {
            InitializeComponent();
        }

        public NumPicker(int startValue, int minimum, int maximum)
            : this()
        {
            _min = minimum;
            _max = maximum;
            _value = startValue;

            lblValue.Text = _value.ToString();
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= 1;
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            Value += 1;
        }

        private void lblValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            int output = 0;

            if (int.TryParse(lblValue.Text, out output))
            {
                Value = output;
            }
            else
            {
                lblValue.Text = _value.ToString();
            }
        }
    }
}
