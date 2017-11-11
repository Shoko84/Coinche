/**
 *  @file   Loop.cs
 *  @author Marc-Antoine Leconte
 *  
 *  This file contain the Loop Class.
 */

namespace Common
{
    /**
     *  This class is a variable that have a min and a max, and that return to its min when it's greater than its max.
     */
    public class Loop
    {
        private int _it;
        public int It { get => _it; set => _it = value; }
        private int _max;
        private int _min;

        /**
         *  Constructor
         */
        public Loop(int min, int max, int value = 0)
        {
            if (min >= max)
            {
                _min = 0;
                _max = 1;
            }
            else
            {
                _min = min;
                _max = max;
            }
            if (value > _max)
                _it = _max;
            else if (value < _min)
                _it = _min;
            else
                _it = value;
        }

        /**
         *  Increment the variable.
         */
        public void Next()
        {
            if (_it + 1 > _max)
                _it = _min;
            else
                _it += 1;
        }


        /**
         *  Decrement the variable.
         */
        public void Post()
        {
            if (_it - 1 < _min)
                _it = _max;
            else
                _it += 1;
        }
    }
}

