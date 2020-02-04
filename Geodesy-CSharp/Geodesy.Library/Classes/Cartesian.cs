namespace Geodesy.Library.Classes
{
    public class Cartesian
    {
        private double _x, _y, _z;

        public Cartesian(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public double X
        {
            get
            {
                return _x;
            }
        }

        public double Z
        {
            get
            {
                return _z;
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
        }

    }
}
