﻿using Geodesy.Library.Enums;
using Geodesy.Library.Exceptions;
using Geodesy.Library.Extensions;
using System;
using System.Linq;

namespace Geodesy.Library
{
    public sealed class OsGridRef
    {
        public string Descriptor { get; }
        public double Easting { get; }
        public double Northing { get; }

        private readonly int[] Digits = new int[] { 0, 2, 4, 6, 8, 10, 12, 14, 16 };

        public OsGridRef(double easting, double northing)
        {
            if (easting < 0 || easting > 700e3)
            {
                throw new InvalidReferencePropertyException<OSGridRefEnum>(GetType(), OSGridRefEnum.Easting, $"Invalid Easting {easting}");
            }

            if (northing < 0 || northing > 1300e3)
            {
                throw new InvalidReferencePropertyException<OSGridRefEnum>(GetType(), OSGridRefEnum.Northing, $"Invalid Northing {northing}");
            }

            Easting = easting;
            Northing = northing;
        }

        public OsGridRef(string descriptor, double easting, double northing) : this(easting, northing)
        {
            Descriptor = descriptor;
        }

        public OsGridRef(string osGridReference)
        {
            var gridRefFormatted = osGridReference.RemoveWhiteSpace();


            var descriptor = osGridReference.Substring(0, 2);
            var eastingNorthing = gridRefFormatted[2..];

            var eastingNorthingLength = eastingNorthing.Length / 2;
            var easting = eastingNorthing.Substring(0, eastingNorthingLength);
            var northing = eastingNorthing.Substring(eastingNorthingLength, eastingNorthingLength);

            if (easting.Length != northing.Length)
            {
                throw new ReferenceParsingException(GetType(), osGridReference, "Invalid OsGridReference");
            }

            // get numeric values of letter references, mapping A->0, B->1, C->2, etc:
            var l1 = gridRefFormatted.ToUpperInvariant()[0] - 'A';
            var l2 = gridRefFormatted.ToUpperInvariant()[1] - 'A';
            // shuffle down letters after 'I' since 'I' is not used in grid:
            if (l1 > 7) l1--;
            if (l2 > 7) l2--;

            // sanity check
            if (l1 < 8 || l1 > 18)
            {
                throw new ReferenceParsingException(GetType(), osGridReference, "Invalid OsGridReference");
            }

            // convert grid letters into 100km-square indexes from false origin (grid square SV):
            var e100km = ((l1 - 2) % 5) * 5 + (l2 % 5);
            var n100km = (19 - Math.Floor((double)l1 / 5) * 5) - Math.Floor((double)l2 / 5);

            var eastingPadded = easting.ToString().PadRight(5, '0');
            var northingPadded = northing.ToString().PadRight(5, '0');

            if (!double.TryParse(eastingPadded, out double eastingDouble))
            {
                throw new InvalidReferencePropertyException<OSGridRefEnum>(GetType(), OSGridRefEnum.Easting, osGridReference);
            }

            if (!double.TryParse(northingPadded, out double northingDouble))
            {
                throw new InvalidReferencePropertyException<OSGridRefEnum>(GetType(), OSGridRefEnum.Northing, osGridReference);
            }

            var e = int.Parse(e100km.ToString() + eastingDouble.ToString());
            var n = int.Parse(n100km.ToString() + northingDouble.ToString());

            Easting = e;
            Northing = n;
            Descriptor = descriptor;
        }

        /// <summary>
        /// Method that converts a OSGrid reference to LatLon.
        /// </summary>
        /// <returns>The lat long representation of this OS grid reference.</returns>
        public LatLon_OsGridRef ToLatLon()
        {
            var a = 6377563.396;
            var b = 6356256.909;       // Airy 1830 major & minor semi-axes
            var F0 = 0.9996012717;     // NatGrid scale factor on central meridian
            var φ0 = (49).ToRadians();
            var λ0 = (-2).ToRadians(); // NatGrid true origin is 49°N,2°W
            var N0 = -100e3;
            var E0 = 400e3;            // northing & easting of true origin, metres
            var e2 = 1 - (b * b) / (a * a); // eccentricity squared
            var n = (a - b) / (a + b);
            var n2 = n * n;
            var n3 = n * n * n;        // n, n², n³

            var φ = φ0;
            var M = 0.0;
            do
            {
                φ = (Northing - N0 - M) / (a * F0) + φ;

                var Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);
                var Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
                var Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
                var Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
                M = b * F0 * (Ma - Mb + Mc - Md);               // meridional arc

            } while (Math.Abs(Northing - N0 - M) >= 0.00001);  // ie until < 0.01mm

            var cosφ = Math.Cos(φ);
            var sinφ = Math.Sin(φ);
            var ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);             // nu = transverse radius of curvature
            var ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5);     // rho = meridional radius of curvature
            var η2 = ν / ρ - 1;                                   // eta = ?

            var tanφ = Math.Tan(φ);
            var tan2φ = tanφ * tanφ;
            var tan4φ = tan2φ * tan2φ;
            var tan6φ = tan4φ * tan2φ;
            var secφ = 1 / cosφ;
            var ν3 = ν * ν * ν;
            var ν5 = ν3 * ν * ν;
            var ν7 = ν5 * ν * ν;
            var VII = tanφ / (2 * ρ * ν);
            var VIII = tanφ / (24 * ρ * ν3) * (5 + 3 * tan2φ + η2 - 9 * tan2φ * η2);
            var IX = tanφ / (720 * ρ * ν5) * (61 + 90 * tan2φ + 45 * tan4φ);
            var X = secφ / ν;
            var XI = secφ / (6 * ν3) * (ν / ρ + 2 * tan2φ);
            var XII = secφ / (120 * ν5) * (5 + 28 * tan2φ + 24 * tan4φ);
            var XIIA = secφ / (5040 * ν7) * (61 + 662 * tan2φ + 1320 * tan4φ + 720 * tan6φ);

            var dE = (Easting - E0);
            var dE2 = dE * dE;
            var dE3 = dE2 * dE;
            var dE4 = dE2 * dE2;
            var dE5 = dE3 * dE2;
            var dE6 = dE4 * dE2;
            var dE7 = dE5 * dE2;
            φ = φ - VII * dE2 + VIII * dE4 - IX * dE6;
            var λ = λ0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;

            var point = new LatLon_OsGridRef(φ.ToDegrees(), λ.ToDegrees(), 0);

            return point;
        }

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int digits)
        {
            if (!Array.Exists(Digits, x => x == digits))
            {
                throw new Exception($"Digits must be one of {string.Join(",", Digits.Select(x => x.ToString()))}");
            }


            if (digits == 0)
            {
                var format = new { useGrouping = false, minimumIntegerDigits = 6, maximumFractionDigits = 3 };
                var ePad = Easting;
                var nPad = Northing;

                return $"{ePad},${nPad}";
            }

            // get the 100km-grid indices
            var e100km = Math.Floor(Easting / 100000);
            var n100km = Math.Floor(Northing / 100000);

            // translate those into numeric equivalents of the grid letters

            var l1 = (19 - n100km) - (19 - n100km) % 5 + Math.Floor((e100km + 10) / 5);

            var l2 = (19 - n100km) * 5 % 25 + e100km % 5;

            // compensate for skipped 'I' and build grid letter-pairs

            if (l1 > 7) l1++;

            if (l2 > 7) l2++;

            l1 += 'A';
            l2 += 'A';

            var letterPair = new string(new char[] { (char)l1, (char)l2 });

            // strip 100km-grid indices from easting & northing, and reduce precision

            var e = Math.Floor((Easting % 100000) / Math.Pow(10, 5 - (digits / 2)));

            var n = Math.Floor((Northing % 100000) / Math.Pow(10, 5 - (digits / 2)));

            // pad eastings & northings with leading zeros

            var eString = e.ToString().PadLeft(digits / 2, '0');

            var nString = n.ToString().PadLeft(digits / 2, '0');

            return $"{letterPair} {eString} {nString}";
        }
    }
}
