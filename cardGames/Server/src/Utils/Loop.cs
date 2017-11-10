namespace Common
{
    public class Loop
    {
        private int _it;
        public int It { get => _it; set => _it = value; }
        private int _max;
        private int _min;

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

        public void Next()
        {
            if (_it + 1 > _max)
                _it = _min;
            else
                _it += 1;
        }

        public void Post()
        {
            if (_it - 1 < _min)
                _it = _max;
            else
                _it += 1;
        }
    }
}

