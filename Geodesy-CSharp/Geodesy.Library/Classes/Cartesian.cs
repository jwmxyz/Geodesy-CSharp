using Geodesy.Library.Classes.Datums;
using System;

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


        public LatLonEllipsoidal ToLatLon()
        {
            var e2 = 2 * WGS84Ellipsoid.F - WGS84Ellipsoid.F * WGS84Ellipsoid.F;
            var epsilon2 = e2 / (1 - e2);
            var p = Math.Sqrt(_x * _x + _y * _y);
            var R = Math.Sqrt(p * p + _z * _z);

            var tanBeta = (WGS84Ellipsoid.B * _z) / (WGS84Ellipsoid.A * p) * (1 + epsilon2 * WGS84Ellipsoid.B / R);
            var sinBeta = tanBeta / Math.Sqrt(1 + tanBeta * tanBeta);
            var cosBeta = sinBeta / tanBeta;

            var phi = Math.Atan2(_z + epsilon2 * WGS84Ellipsoid.B * sinBeta * sinBeta * sinBeta, p - epsilon2 * WGS84Ellipsoid.A * cosBeta * cosBeta * cosBeta);

            var lambda = Math.Atan2(_y, _x);

            var sinPhi = Math.Sin(phi);
            var cosPhi = Math.Cos(phi);

            var v = WGS84Ellipsoid.A / Math.Sqrt(1 - epsilon2 * sinPhi * sinPhi);
            var h = p * cosPhi + _z * sinPhi - (WGS84Ellipsoid.A * (WGS84Ellipsoid.A / v));

            var point = new LatLonEllipsoidal(phi * 180 / Math.PI, lambda * 180 / Math.PI, h);

            return point;                             
        }
    }
}
