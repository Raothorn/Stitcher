using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System;
using Newtonsoft.Json;

namespace Stitcher.Core
{
    [JsonObjectAttribute(MemberSerialization.Fields)]
    public class Pattern
    {
        private string[,] _symbols;

        internal Pattern(string[,] symbols)
        {
            _symbols = symbols;
        }

        public static Pattern LoadFromBitmap(Bitmap img)
        {
            var symbols = new string[img.Height, img.Width];

            for(int r = 0; r < img.Height; r++)
            {
                for(int c = 0; c < img.Width; c++)
                {
                    Color pixelColor = img.GetPixel(c, r);
                    string symbol =  StitchLookup.SymbolFromRgb(pixelColor);
                    symbols[r, c] = symbol;
                }
            }

            return new Pattern(symbols);
        }

        public string GetSymbol(int row, int column)
        {
            return _symbols[row, column];
        }

        public IList<string> GetAllSymbolsInRegion(Rect region)
        {
            var symbols = new HashSet<string>();

            for(int r = region.Y1; r < region.Y2; r++)
            {
                for(int c = region.X1; c < region.X2; c++)
                {
                    symbols.Add(GetSymbol(r, c));
                }
            }

            return symbols.ToList();
        }

        public int Width => _symbols.GetLength(1);
        public int Height => _symbols.GetLength(0);

        public Rect GetBounds()
        {
            return new Rect(0, 0, Width, Height);
        }
    }
}
