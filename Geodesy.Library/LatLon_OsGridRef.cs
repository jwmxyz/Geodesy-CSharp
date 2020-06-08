/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/* 
 * This is a C# implementation of the below: 
 * 
 * Geodesy tools for an ellipsoidal earth model                      
 * https://www.movable-type.co.uk/scripts/geodesy/docs/osgridref.js.html
 * 
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

using Geodesy.Library.Extensions;
using System;

namespace Geodesy.Library
{
    public class LatLon_OsGridRef : LatLonEllipsoidal
    {
        public LatLon_OsGridRef(double latitude, double longitude, double height)
            : base(latitude, longitude, height)
        {

        }

        public LatLon_OsGridRef(double latitude, double longitude)
            : base(latitude, longitude)
        {

        }

        public OsGridRef ToOSGridRef()
        {

            var φ = Latitude.ToRadians();

            var λ = Longitude.ToRadians();

            var a = 6377563.396;

            var b = 6356256.909;              // Airy 1830 major & minor semi-axes

            var F0 = 0.9996012717;                             // NatGrid scale factor on central meridian

            var φ0 = (49).ToRadians();
            var λ0 = (-2).ToRadians();  // NatGrid true origin is 49°N,2°W

            var N0 = -100000;
            var E0 = 400000;                     // northing & easting of true origin, metres

            var e2 = 1 - (b * b) / (a * a);                          // eccentricity squared

            var n = (a - b) / (a + b);
            var n2 = n * n;
            var n3 = n * n * n;         // n, n², n³

            var cosφ = Math.Cos(φ);
            var sinφ = Math.Sin(φ);

            var ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature

            var ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5); // rho = meridional radius of curvature

            var η2 = ν / ρ - 1;                                    // eta = ?

            var Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);

            var Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);

            var Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));

            var Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));

            var M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

            var cos3φ = cosφ * cosφ * cosφ;

            var cos5φ = cos3φ * cosφ * cosφ;

            var tan2φ = Math.Tan(φ) * Math.Tan(φ);

            var tan4φ = tan2φ * tan2φ;

            var I = M + N0;

            var II = (ν / 2) * sinφ * cosφ;

            var III = (ν / 24) * sinφ * cos3φ * (5 - tan2φ + 9 * η2);

            var IIIA = (ν / 720) * sinφ * cos5φ * (61 - 58 * tan2φ + tan4φ);

            var IV = ν * cosφ;

            var V = (ν / 6) * cos3φ * (ν / ρ - tan2φ);

            var VI = (ν / 120) * cos5φ * (5 - 18 * tan2φ + tan4φ + 14 * η2 - 58 * tan2φ * η2);

            var Δλ = λ - λ0;

            var Δλ2 = Δλ * Δλ;
            var Δλ3 = Δλ2 * Δλ;
            var Δλ4 = Δλ3 * Δλ;
            var Δλ5 = Δλ4 * Δλ;
            var Δλ6 = Δλ5 * Δλ;

            var N = I + II * Δλ2 + III * Δλ4 + IIIA * Δλ6;

            var E = E0 + IV * Δλ + V * Δλ3 + VI * Δλ5;

            N = Math.Round(N, 3); // round to mm precision

            E = Math.Round(E, 3);

            return new OsGridRef(E, N); // gets truncated to SW corner of 1m grid square
        }
    }
}
