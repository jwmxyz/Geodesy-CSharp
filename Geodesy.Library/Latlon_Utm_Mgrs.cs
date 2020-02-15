using System;
using System.Collections.Generic;
using System.Text;

namespace Geodesy.Library
{
    public class Latlon_Utm_Mgrs : LatLon_Utm
    {
        public Latlon_Utm_Mgrs(double latitude, double longitude, double height = 0) : base(latitude, longitude, height)
        {
        }
    }
}
