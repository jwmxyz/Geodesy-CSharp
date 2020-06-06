using System;

namespace Geodesy.Library
{
    public class Utm_Mgrs : Utm
    {
        /*
        * Latitude bands C..X 8° each, covering 80°S to 84°N
        */
        private const string _latBands = "CDEFGHJKLMNPQRSTUVWXX"; // X is repeated for 80-84°N

        /*
        * 100km grid square column (‘e’) letters repeat every third zone
        */
        private readonly string[] _e100kLetters = new[] { "ABCDEFGH", "JKLMNPQR", "STUVWXYZ" };

        /*
        * 100km grid square row (‘n’) letters repeat every other zone
        */
        private readonly string[] _n100kLetters = new[] { "ABCDEFGHJKLMNPQRSTUV", "FGHJKLMNPQRSTUVABCDE" };

        public Utm_Mgrs(string utmReference) : base(utmReference)
        {

        }

        public Utm_Mgrs(Utm utm) : base(utm.ToString())
        {

        }

        /// <summary>
        /// Method that will convert a UTM grid reference into a MGRS grid reference.
        /// </summary>
        /// <returns>A MGRS grid reference for the given UTM co-ordinates/</returns>
        public Mgrs ToMgrs()
        {
            var zone = Zone;

            var latLon = ToLatLon();

            var band = _latBands[(int) Math.Floor(latLon.Latitude / 8 + 10)]; // latitude band

            var col = (int) Math.Floor(Easting / 100e3);

            // (note -1 because eastings start at 166e3 due to 500km false origin)
            var e100k = _e100kLetters[(zone - 1) % 3].ToCharArray()[col - 1];

            //rows in even zones are A-V, in odd zones are F-E
            var row = (int) Math.Floor(Northing / 100e3) % 20;
            var n100k = _n100kLetters[(zone - 1) % 2].ToCharArray()[row];

            // truncate easting/northing to within 100km grid square
            var easting = Easting % 100e3;
            var northing = Northing % 100e3;

            return new Mgrs(zone, band, e100k, n100k, easting, northing);
        }
    }
}
