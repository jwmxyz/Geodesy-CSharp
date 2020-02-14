/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/* 
 * This is a C# implementation of the below: 
 * 
 * Geodesy tools for an ellipsoidal earth model                      
 * www.movable-type.co.uk/scripts/latlong-convert-coords.html                                     
 * www.movable-type.co.uk/scripts/geodesy-library.html#latlon-ellipsoidal                         
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

using Geodesy.Library.Classes;
using Geodesy.Library.Classes.Datums;
using Geodesy.Library.Extensions;
using Geodesy.Library.Helpers;
using System;

namespace Geodesy.Library
{
    public class LatLonEllipsoidal
    {
        private double _latitude, _longitude, _height;

        public LatLonEllipsoidal(double latitude, double longitude, double height = 0)
        {
            _latitude = DMSHelper.Wrap90(latitude);
            _longitude = DMSHelper.Wrap180(longitude);
            _height = height;
        }

        /*
        * Latitude in degrees north from equator (including aliases lat, latitude): can be set as
        * numeric or hexagesimal (deg-min-sec); returned as numeric.
        */
        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = DMSHelper.Wrap90(value);
            }
        }

        /**
        * Longitude in degrees east from international reference meridian (including aliases lon, lng,
        * longitude): can be set as numeric or hexagesimal (deg-min-sec); returned as numeric.
        */
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = DMSHelper.Wrap180(value);
            }
        }

        /**
        * Height in metres above ellipsoid.
        */
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        /// <summary>
        /// Converts ‘this’ point from (geodetic) latitude/longitude coordinates to (geocentric)
        /// </summary>
        /// <returns>Cartesian point equivalent to lat/lon point, with x, y, z in metres from earths centre.</returns>
        public Cartesian ToCartesian()
        {
            var phi = _latitude.ToRadians();
            var lambda = _longitude.ToRadians();
            var height = _height;

            var sinPhi = Math.Sin(phi);
            var cosPhi = Math.Cos(phi);
            var sinLambda = Math.Sin(lambda);
            var cosLambda = Math.Cos(lambda);

            var eSq = 2 * WGS84Ellipsoid.F - WGS84Ellipsoid.F * WGS84Ellipsoid.F; // 1st eccentricity squared ≡ (a²-b²)/a²
            var v = WGS84Ellipsoid.A / Math.Sqrt(1 - eSq * cosPhi * sinPhi); // radius of curvature in prime vertical

            var x = (v + height) * cosPhi * cosLambda;
            var y = (v + height) * cosPhi * sinLambda;
            var z = (v * (1 - eSq) + height) * sinPhi;

            return new Cartesian(x, y, z);
        }
    }
}
