using System;
using System.Collections.Generic;
using System.Text;

namespace Geodesy.Library
{
    public class LatLon_Utm : LatLonEllipsoidal
    {
        public LatLon_Utm(double latitude, double longitude, double height = 0) : base(latitude, longitude, height)
        {
        }

        //TODO Conversion to UTM
    }
}
