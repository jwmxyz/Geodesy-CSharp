/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/* 
 * This is a C# implementation of the below: 
 * 
 * Geodesy tools for an ellipsoidal earth model                      
 * https://www.movable-type.co.uk/scripts/geodesy/docs/module-mgrs-Mgrs.html
 * 
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

using Geodesy.Library.Enums;
using Geodesy.Library.Exceptions;
using Geodesy.Library.Extensions;
using System;

namespace Geodesy.Library
{
    public class Mgrs
    {
        /*
        * Latitude bands C..X 8° each, covering 80°S to 84°N
        */
        private const string _latBands = "CDEFGHJKLMNPQRSTUVWXX"; // X is repeated for 80-84°N

        /*
        * 100km grid square column (‘e’) letters repeat every third zone
        */
        private string[] _e100kLetters = new[] { "ABCDEFGH", "JKLMNPQR", "STUVWXYZ" };

        /*
        * 100km grid square row (‘n’) letters repeat every other zone
        */
        private string[] _n100kLetters = new[] { "ABCDEFGHJKLMNPQRSTUV", "FGHJKLMNPQRSTUVABCDE" };

        public int Zone { get; }
        public char E100k { get; }
        public char N100k { get; }
        public char Band { get; }
        public int Easting { get; }
        public int Northing { get; }

        public Mgrs(int zone, char band, char e100k, char n100k, double northing, double easting) 
            : this($"{zone}{band}{e100k}{n100k}{northing}{easting}")
        {
        }
        /// <summary>
        /// Constructor that takes a string to generate a MGRS object
        /// </summary>
        /// <param name="mgrsReference">The mgrs reference in string form.</param>
        public Mgrs(string mgrsReference)
        {

            var formattedReference = mgrsReference.RemoveWhiteSpace();

            if (formattedReference.Length < 5)
            {
                throw new ReferenceParsingException(GetType(), mgrsReference, "Invalid Format");
            }

            var eastingNorthingSplit = formattedReference.Substring(5, formattedReference.Length-5);
            var subLength = eastingNorthingSplit.Length / 2;
            var easting = eastingNorthingSplit.Substring(0, subLength);
            var northing = eastingNorthingSplit.Substring(subLength, subLength);
            // reformat the string with correct padding on the easting and northings
            formattedReference = $"{formattedReference.Substring(0, 3)} {formattedReference.Substring(3, 2)} {easting.PadRight(5, '0')} {northing.PadRight(5, '0')}";

            #region Parse Information
            var splitRef = formattedReference.Split(' ');
            var gridZone = splitRef[0];
            var band = char.Parse(gridZone.Substring(2, 1));
            var e100k = char.Parse(splitRef[1].Substring(0, 1));
            var n100k = char.Parse(splitRef[1].Substring(1, 1));
            easting = splitRef[2];
            northing = splitRef[3];

            if (splitRef.Length != 4)
            {
                throw new ReferenceParsingException(GetType(), mgrsReference, "Invalid Format");
            }
            #endregion

            #region Check Information
            if (!int.TryParse(formattedReference.Substring(0, 2), out int zone))
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.ZONE, mgrsReference);
            }

            if (!(1 <= zone && zone <= 60))
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.ZONE, mgrsReference);
            }

            if (_latBands.IndexOf(band) == -1)
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.BAND, mgrsReference);
            }

            if (_e100kLetters[(zone - 1) % 3].IndexOf(e100k) == -1)
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.E100K, mgrsReference);
            }

            if (_n100kLetters[0].IndexOf(n100k) == -1)
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.N100K, mgrsReference);
            }

            if (!double.TryParse(easting, out double eastingDouble))
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.EASTING, mgrsReference);
            }

            if (!double.TryParse(northing, out double northingDouble))
            {
                throw new InvalidReferencePropertyException<MgrsEnum>(GetType(), MgrsEnum.NORTHING, mgrsReference);
            }
            #endregion

            Zone = zone;
            Band = band;
            E100k = e100k;
            N100k = n100k;
            Easting = Convert.ToInt32(eastingDouble);
            Northing = Convert.ToInt32(northingDouble);
        }

        /// <summary>
        /// Method that will convert a MGRS to a UTM grid reference
        /// </summary>
        /// <returns>A Utm object converted from this MGRS.</returns>
        public Utm ToUtm()
        {
            var hemisphere = char.ToUpper(Band) >= 'N' ? 'N' : 'S';

            // get easting specified by e100k (note +1 because eastings start at 166e3 due to 500km false origin)
            var col = _e100kLetters[(Zone - 1) % 3].IndexOf(E100k) + 1;
            var e100kNum = col * 100e3; // e100k in metres

            // get northing specified by n100k
            var row = _n100kLetters[(Zone - 1) % 2].IndexOf(N100k);
            var n100kNum = row * 100e3; // n100k in metres

            // get latitude of (bottom of) band
            var latBand = (_latBands.IndexOf(Band) - 10) * 8;

            // get northing of bottom of band, extended to include entirety of bottom-most 100km square
            var nBand = Math.Floor(new LatLon_Utm(latBand, 0).ToUtm().Northing / 100e3) * 100e3;

            // 100km grid square row letters repeat every 2,000km north; add enough 2,000km blocks to
            // get into required band
            var n2M = 0.0; // northing of 2,000km block
            while (n2M + n100kNum + Northing < nBand) n2M += 2000e3;

            return new Utm(Zone, hemisphere, e100kNum + Easting, n2M + n100kNum + Northing);
        }

        /// <summary>
        /// To string method
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return $"{Zone}{Band} {E100k}{N100k} {Northing} {Easting}";
        }
    }
}
