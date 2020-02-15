using Geodesy.Library.Classes.Datums;
using Geodesy.Library.Exceptions;
using Geodesy.Library.Extensions;
using System;

namespace Geodesy.Library
{
    public class LatLon_Utm : LatLonEllipsoidal
    {
        public LatLon_Utm(double latitude, double longitude, double height = 0) : base(latitude, longitude, height)
        {
        }

        public Utm ToUtm()
        {
            if (!(-80 <= Latitude && Latitude <= 84)) {
                throw new ReferenceParsingException(GetType(), Latitude.ToString(), "outside UTM limits");
            }

            var falseEasting = 500e3;
            var falseNorthing = 10000e3;

            var zone = (int)Math.Floor((Longitude + 180) / 6) + 1; // longitudinal zone
            var λ0 = ((zone - 1) * 6 - 180 + 3).ToRadians(); // longitude of central meridian

            // ---- handle Norway/Svalbard exceptions
            // grid zones are 8° tall; 0°N is offset 10 into latitude bands array
            var mgrsLatBands = "CDEFGHJKLMNPQRSTUVWXX"; // X is repeated for 80-84°N
            var latBand = mgrsLatBands.ToCharArray()[(int) Math.Floor((Latitude / 8) + 10)];
            // adjust zone & central meridian for Norway
            if (zone == 31 && latBand == 'V' && Longitude >= 3) { zone++; λ0 += (6).ToRadians(); }
            // adjust zone & central meridian for Svalbard
            if (zone == 32 && latBand == 'X' && Longitude < 9) { zone--; λ0 -= (6).ToRadians(); }
            if (zone == 32 && latBand == 'X' && Longitude >= 9) { zone++; λ0 += (6).ToRadians(); }
            if (zone == 34 && latBand == 'X' && Longitude < 21) { zone--; λ0 -= (6).ToRadians(); }
            if (zone == 34 && latBand == 'X' && Longitude >= 21) { zone++; λ0 += (6).ToRadians(); }
            if (zone == 36 && latBand == 'X' && Longitude < 33) { zone--; λ0 -= (6).ToRadians(); }
            if (zone == 36 && latBand == 'X' && Longitude >= 33) { zone++; λ0 += (6).ToRadians(); }

            var φ = Latitude.ToRadians();      // latitude ± from equator
            var λ = Longitude.ToRadians() - λ0; // longitude ± from central meridian

           
            var k0 = 0.9996; // UTM scale on the central meridian

            // ---- easting, northing: Karney 2011 Eq 7-14, 29, 35:

            var e = Math.Sqrt(WGS84Ellipsoid.F * (2 - WGS84Ellipsoid.F)); // eccentricity
            var n = WGS84Ellipsoid.F / (2 - WGS84Ellipsoid.F);        // 3rd flattening
            var n2 = n * n;
            var n3 = n * n2;
            var n4 = n * n3;
            var n5 = n * n4;
            var n6 = n * n5;

            var cosλ = Math.Cos(λ);
            var sinλ = Math.Sin(λ);
            var tanλ = Math.Tan(λ);

            var τ = Math.Tan(φ); // τ ≡ tanφ, τʹ ≡ tanφʹ; prime (ʹ) indicates angles on the conformal sphere
            var σ = Math.Sinh(e * Math.Atanh(e * τ / Math.Sqrt(1 + τ * τ)));

            var τʹ = τ * Math.Sqrt(1 + σ * σ) - σ * Math.Sqrt(1 + τ * τ);

            var ξʹ = Math.Atan2(τʹ, cosλ);
            var ηʹ = Math.Asinh(sinλ / Math.Sqrt(τʹ * τʹ + cosλ * cosλ));

            var A = WGS84Ellipsoid.A / (1 + n) * (1 + 1 / 4 * n2 + 1 / 64 * n4 + 1 / 256 * n6); // 2πA is the circumference of a meridian

            var α = new double? [] {null, // note α is one-based array (6th order Krüger expressions)
                1 / 2 * n - 2 / 3 * n2 + 5 / 16 * n3 + 41 / 180 * n4 - 127 / 288 * n5 + 7891 / 37800 * n6,
                      13 / 48 * n2 - 3 / 5 * n3 + 557 / 1440 * n4 + 281 / 630 * n5 - 1983433 / 1935360 * n6,
                               61 / 240 * n3 - 103 / 140 * n4 + 15061 / 26880 * n5 + 167603 / 181440 * n6,
                                       49561 / 161280 * n4 - 179 / 168 * n5 + 6601661 / 7257600 * n6,
                                                         34729 / 80640 * n5 - 3418889 / 1995840 * n6,
                                                                      212378941 / 319334400 * n6};

            var ξ = ξʹ;
            for (var j = 1; j <= 6; j++) ξ += α[j].GetValueOrDefault() * Math.Sin(2 * j * ξʹ) * Math.Cosh(2 * j * ηʹ);

            var η = ηʹ;
            for (var j = 1; j <= 6; j++) η += α[j].GetValueOrDefault() * Math.Cos(2 * j * ξʹ) * Math.Sinh(2 * j * ηʹ);

            var x = k0 * A * η;
            var y = k0 * A * ξ;

            // ---- convergence: Karney 2011 Eq 23, 24

            var pʹ = 1.0;
            for (var j = 1; j <= 6; j++) pʹ += 2 * j * α[j].GetValueOrDefault() * Math.Cos(2 * j * ξʹ) * Math.Cosh(2 * j * ηʹ);
            var qʹ = 0.0;
            for (var j = 1; j <= 6; j++) qʹ += 2 * j * α[j].GetValueOrDefault() * Math.Sin(2 * j * ξʹ) * Math.Sinh(2 * j * ηʹ);

            var γʹ = Math.Atan(τʹ / Math.Sqrt(1 + τʹ * τʹ) * tanλ);
            var γʺ = Math.Atan2(qʹ, pʹ);

            var γ = γʹ + γʺ;

            // ---- scale: Karney 2011 Eq 25

            var sinφ = Math.Sin(φ);
            var kʹ = Math.Sqrt(1 - e * e * sinφ * sinφ) * Math.Sqrt(1 + τ * τ) / Math.Sqrt(τʹ * τʹ + cosλ * cosλ);
            var kʺ = A / WGS84Ellipsoid.A * Math.Sqrt(pʹ * pʹ + qʹ * qʹ);

            var k = k0 * kʹ * kʺ;

            // ------------

            // shift x/y to false origins
            x += falseEasting;             // make x relative to false easting
            if (y < 0) y = y + falseNorthing; // make y in southern hemisphere relative to false northing

            // round to reasonable precision
            var convergence = γ.ToDegrees();
            var scale = k;

            var h = Latitude >= 0 ? 'N' : 'S'; // hemisphere

            return new Utm(zone, h, x, y);
        }
    }
}
