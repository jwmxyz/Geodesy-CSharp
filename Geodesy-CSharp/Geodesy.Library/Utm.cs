using Geodesy.Library.Classes.Datums;
using Geodesy.Library.Enums;
using Geodesy.Library.Exceptions;
using Geodesy.Library.Extensions;
using System;

namespace Geodesy.Library
{
    public class Utm
    {

        #region Getters
        public double Northing { get; }
        public double Easting { get; }
        public int Zone { get; }
        public char Hemisphere { get; }
        #endregion


        /// <summary>
        /// Constructor for a UTM object
        /// </summary>
        /// <param name="zone">The zone of the utm grid.</param>
        /// <param name="hemisphere">The hemisphere that the grid reference falls in.</param>
        /// <param name="easting">Easting of the grid reference.</param>
        /// <param name="northing">Northing of the grid reference.</param>
        public Utm(int zone, char hemisphere, double easting, double northing)
        {
            Zone = zone;
            Hemisphere = char.ToUpper(hemisphere);
            Easting = easting;
            Northing = northing;
        }

        /// <summary>
        /// Constructor that will take a string and create a new UTM object.
        /// </summary>
        /// <param name="utmString">The utm string that we want convert into an object.</param>
        public Utm(string utmString)
        {
            var stringArray = utmString.Split(' ');

            #region Parse information
            if (stringArray == null || stringArray.Length != 4)
            {
                throw new ReferenceParsingException(GetType(), utmString, "Invalid Format.");
            }

            if (!int.TryParse(stringArray[0], out int zone))
            {
                throw new ReferenceParsingException(GetType(), utmString, "Zone is not a number.");
            }
            
            if (!char.TryParse(stringArray[1], out char hemisphere))
            {
                throw new ReferenceParsingException(GetType(), utmString, "Should be a single char.");
            }

            if (char.ToUpperInvariant(hemisphere) != 'S' && char.ToUpperInvariant(hemisphere) != 'N')
            {
                throw new ReferenceParsingException(GetType(), utmString, "Hemisphere should be N or S.");
            }

            if (!double.TryParse(stringArray[2], out double easting))
            {
                throw new ReferenceParsingException(GetType(), utmString, "Easting should be a number.");
            }

            if (!double.TryParse(stringArray[3], out double northing))
            {
                throw new ReferenceParsingException(GetType(), utmString, "Northing should be a number.");
            }
            #endregion

            #region Check Information

            if (!(1 <= zone && zone <= 60))
            {
                throw new InvalidReferencePropertyException<UtmEnum>(GetType(), UtmEnum.ZONE, utmString);
            }

            if (!(0 <= easting && easting <= 1000e3))
            {
                throw new InvalidReferencePropertyException<UtmEnum>(GetType(), UtmEnum.EASTING, utmString);
            }

            if (char.ToUpperInvariant(hemisphere) == 'N' && !(0 <= northing && northing < 9328094))
            {
                throw new InvalidReferencePropertyException<UtmEnum>(GetType(), UtmEnum.NORTHING, utmString);
            }

            if (char.ToUpperInvariant(hemisphere) == 'S' && !(1118414 < northing && northing <= 10000e3))
            {
                throw new InvalidReferencePropertyException<UtmEnum>(GetType(), UtmEnum.NORTHING, utmString);
            }
            #endregion

            Zone = zone;
            Hemisphere = char.ToUpperInvariant(hemisphere);
            Easting = easting;
            Northing = northing;
        }

        /// <summary>
        /// Converts UTM zone/easting/northing coordinate to latitude/longitude.
        /// 
        /// Implements Karney’s method, using Krüger series to order n⁶, giving results accurate to 5nm
        /// for distances up to 3900km from the central meridian.
        /// </summary>
        /// <returns>Latitude/longitude of supplied grid reference.</returns>
        public LatLon_Utm ToLatLon()
        {
            var falseEasting = 500e3;
            var falseNorthing = 10000e3;

            var k0 = 0.9996; // UTM scale on the central meridian

            var x = Easting - falseEasting;  // make x ± relative to central meridian
            var y = Hemisphere.Equals("S") ? Northing - falseNorthing : Northing; // make y ± relative to equator

            // ---- from Karney 2011 Eq 15-22, 36:
            var e = Math.Sqrt(WGS84Ellipsoid.F * (2 - WGS84Ellipsoid.F)); // eccentricity
            var n = WGS84Ellipsoid.F / (2 - WGS84Ellipsoid.F);
            var n2 = n * n;
            var n3 = n * n2;
            var n4 = n * n3;
            var n5 = n * n4;
            var n6 = n * n5;

            var a = WGS84Ellipsoid.A / (1 + n) * (1 + 1 / 4 * n2 + 1 / 64 * n4 + 1 / 256 * n6); // 2πA is the circumference of a meridian

            var η = x / (k0 * a);
            var ξ = y / (k0 * a);
            var β = new double?[] {null, // note beta is one-based array (6th order Krüger expressions)
            1 / 2 * n - 2 / 3 * n2 + 37 / 96 * n3 - 1 / 360 * n4 - 81 / 512 * n5 + 96199 / 604800 * n6,
                   1 / 48 * n2 + 1 / 15 * n3 - 437 / 1440 * n4 + 46 / 105 * n5 - 1118711 / 3870720 * n6,
                            17 / 480 * n3 - 37 / 840 * n4 - 209 / 4480 * n5 + 5569 / 90720 * n6,
                                     4397 / 161280 * n4 - 11 / 504 * n5 - 830251 / 7257600 * n6,
                                                   4583 / 161280 * n5 - 108847 / 3991680 * n6,
                                                                 20648693 / 638668800 * n6 };
            var ξʹ = ξ;
            for (int i = 1; i <= 6; i++)
            {
                ξʹ -= β[i].GetValueOrDefault() * Math.Sin(2 * i * ξ) * Math.Cos(2 * i * η);
            }

            var ηʹ = η;
            for (int i = 1; i <= 6; i++)
            {
                ηʹ -= β[i].GetValueOrDefault() * Math.Cos(2 * i * ξ) * Math.Sinh(2 * i * η);

            }

            var sinhηʹ = Math.Sinh(ξʹ);
            var sinξʹ = Math.Sin(ξʹ);
            var cosξʹ = Math.Cos(ξʹ);

            var τʹ = sinξʹ / Math.Sqrt(sinhηʹ * sinhηʹ + cosξʹ * cosξʹ);

            double δτi;
            var τi = τʹ;

            do
            {
                var σi = Math.Sinh(e * Math.Atanh(e * τi / Math.Sqrt(1 + τi * τi)));
                var τiʹ = τi * Math.Sqrt(1 + σi * σi) - σi * Math.Sqrt(1 + τi * τi);
                δτi = (τʹ - τiʹ) / Math.Sqrt(1 + τiʹ * τiʹ) * (1 + (1 - e * e) * τi * τi) / ((1 - e * e) * Math.Sqrt(1 + τi * τi));
                τi += δτi;
            } while (Math.Abs(δτi) > 1e-12);
            // using IEEE 754 δτi -> 0 after 2-3 iterations
            // note relatively large convergence test as δτi toggles on ±1.12e-16 for eg 31 N 400000 5000000

            var τ = τi;
            var φ = Math.Atan(τ);

            var λ = Math.Atan2(sinhηʹ, cosξʹ);
            // ---- convergence: Karney 2011 Eq 26, 27
            double p = 1;
            for (int j = 1; j <= 6; j++) p -= 2 * j * β[j].GetValueOrDefault() * Math.Cos(2 * j * ξ) * Math.Cosh(2 * j * η);
            double q = 0;
            for (int j = 1; j <= 6; j++) q += 2 * j * β[j].GetValueOrDefault() * Math.Sin(2 * j * ξ) * Math.Sinh(2 * j * η);

            var γʹ = Math.Atan(Math.Tan(ξʹ) * Math.Tanh(ηʹ));
            var γʺ = Math.Atan2(q, p);
            var γ = γʹ + γʺ;

            // ---- scale: Karney 2011 Eq 28
            var sinφ = Math.Sin(φ);
            var kʹ = Math.Sqrt(1 - e * e * sinφ * sinφ) * Math.Sqrt(1 + τ * τ) * Math.Sqrt(sinhηʹ * sinhηʹ + cosξʹ * cosξʹ);
            var kʺ = a / WGS84Ellipsoid.A / Math.Sqrt(p * p + q * q);
            var k = k0 * kʹ * kʺ;
            // ------------

            var λ0 = (((Zone - 1) * 6) - 180 + 3).ToRadians(); // longitude of central meridian

            λ += λ0; // move λ from zonal to global coordinates

            // round to reasonable precision

            var lat = φ.ToDegrees();

            var lon = λ.ToDegrees();

            var convergence = γ.ToDegrees();

            var scale = k;

            var latLong = new LatLon_Utm(lat, lon, 0);

            // ... and add the convergence and scale into the LatLon object!
            //latLong.convergence = convergence;
            //latLong.scale = scale;
            return latLong;
        }

        /// <summary>
        /// To string of the current object
        /// </summary>
        /// <returns>A String representation of this object.</returns>
        public override string ToString()
        {
            return $"{Zone} {Hemisphere} {Easting} {Northing}";
        }
    }
}
