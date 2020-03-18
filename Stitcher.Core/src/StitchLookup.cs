using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Stitcher.Core
{
    public static class StitchLookup
    {
        private static IList<ValueTuple<string, string>> _convertData;

        static StitchLookup()
        {
            Load();
        }

        private static void Load()
        {
            _convertData = new List<ValueTuple<string, string>>();

            var path = GetAssetPath("color_list.csv");

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var dmc_code = values[0];
                    var rgb_code = values[1].ToUpper();

                    _convertData.Add((dmc_code, rgb_code));
                }
            }
        }

        public static string SymbolFromRgb(Color rgb)
        {
            var hexCode = GetHexCode(rgb);

            var matching = _convertData.FirstOrDefault(x => x.Item2 == hexCode);
            return matching.Item1;
        }

        public static string RgbFromSymbol(string symbol)
        {
            var matching = _convertData.FirstOrDefault(x => x.Item1 == symbol);
            return "#" + matching.Item2;
        }

        public static string GetAssetPath(string filename)
        {
            return @"/home/gaston/Projects/Stitcher/assets/" + filename;
        }

        public static string GetHexCode(Color rgb)
        {
            var bytes = new byte[] {rgb.R, rgb.G, rgb.B};
            string hex = BitConverter.ToString(bytes).Replace("-", String.Empty);

            return hex;
        }
    }
}
