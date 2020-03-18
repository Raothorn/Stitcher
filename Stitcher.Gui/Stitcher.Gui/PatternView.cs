using System;
using Eto.Forms;
using Eto.Drawing;
using Stitcher.Core;

namespace Stitcher
{
    public class PatternView : Drawable
    {
        const int SquareSize = 20;
        private Core.ProjectViewModel _project;

        public PatternView(Core.ProjectViewModel project)
        {
            _project = project;
            _project.PropertyChanged += (s, e) => Invalidate();
        }

        public void Draw(Graphics g)
        {
            g.AntiAlias = true;
            g.Clear(Colors.WhiteSmoke);
            DrawGrid(g);
            DrawSquares(g);
            DrawInfo(g);
        }

        private void DrawGrid(Graphics g)
        {
            const int DividerInterval = 10;
            Color GridColor =  Colors.Black;

            int cols = _project.Window.Width;
            int rows = _project.Window.Height;

            float gridWidth = cols * SquareSize;
            float gridHeight = rows * SquareSize;

            for(int r = 0; r <= rows; r++)
            {
                float y = SquareSize * r;
                var pen = GetGridLinePen(r, DividerInterval, GridColor);
                g.DrawLine(pen, new PointF(0.0f, y), new PointF(gridWidth, y));
            }

            for(int c = 0; c <= cols; c++)
            {
                float x = SquareSize * c;
                var pen = GetGridLinePen(c, DividerInterval, GridColor);
                g.DrawLine(pen, new PointF(x, 0.0f), new PointF(x, gridHeight));
            }
        }

        private void DrawInfo(Graphics g)
        {
            const float gridOffset = 20.0f;
            g.TranslateTransform(GridSize + gridOffset, 0.0f);

            string symbol = _project.SelectedSymbol;
            string rgb = StitchLookup.RgbFromSymbol(symbol);
            var color = Color.Parse(rgb);

            string stitchDisplay = "Coordinates: (" + _project.Window.X1 + ", " + _project.Window.Y1 +  ")";
            stitchDisplay += "\nSelected Color: " + symbol;
            g.DrawText(Fonts.Monospace(12.0f), color, 0.0f, 0.0f,  stitchDisplay);
        }

        private void DrawSquares(Graphics g)
        {
            // if (_project.SelectedSymbol == null) return;
            for (int r = 0; r < _project.Window.Height ; r++)
            {
                for (int c = 0; c < _project.Window.Width; c++)
                {
                    DrawSquare(g, r, c);
                }
            }
        }

        private void DrawSquare(Graphics g, int row, int col)
        {
            if (!_project.InBoundsWindowCoordinate(row, col))
            {
                DrawX(g, row, col, Colors.Red);
                return;
            }

            var symbol = _project.GetSymbolAtWindowCoordinate(row, col);
            var rgb = _project.GetRgbAtWindowCoordinate(row, col);
            var color = Color.Parse(rgb);

            var x = col * SquareSize;
            var y = row * SquareSize;

            var completedState = _project.CompletedStateAtWindowCoordinate(row, col);
            if (completedState == CompletedState.Completed)
            {
                var xColor = (symbol == _project.SelectedSymbol) ? color : Colors.Black;
                DrawX(g, row, col, xColor);
            }
            else if (completedState == CompletedState.CompletedSquare)
            {
                var bounds = new RectangleF(x, y, SquareSize, SquareSize);
                g.FillRectangle(color,  bounds);
            }
            else if (symbol == _project.SelectedSymbol)
            {
                var offset = 3.0f;
                var bounds = new RectangleF(x + offset,
                                            y + offset,
                                            SquareSize - offset * 2,
                                            SquareSize - offset * 2);
                g.FillEllipse(color, bounds);
            }
        }

        private void DrawX(Graphics g, int row, int col, Color color)
        {
            var x = col * SquareSize;
            var y = row * SquareSize;

            const float offset = 2.5f;
            float x1 = x + offset;
            float x2 = x + SquareSize - offset;
            float y1 = y + offset;
            float y2 = y + SquareSize - offset;

            var pen = new Pen(color, 1.5f);
            // g.AntiAlias = true;
            g.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
            g.DrawLine(pen, new PointF(x1, y2), new PointF(x2, y1));

        }

        private Pen GetGridLinePen(int n, int interval, Color color)
        {
            Pen pen;
            if(n % interval == 0)
                pen = new Pen(color, 2.0f);
            else
                pen = new Pen(color, 1.0f);

            return pen;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int squareRow = (int)(e.Location.Y / SquareSize);
            int squareCol = (int)(e.Location.X / SquareSize);

            if (e.Modifiers == Keys.Control)
            {
                var symbol = _project.GetSymbolAtWindowCoordinate(squareRow, squareCol);
                _project.SelectedSymbol = symbol;
            }
            else if (e.Modifiers == Keys.Shift)
            {
                var confirmResult = MessageBox.Show("Toggle Square?",
                                                    "Confirm Toggle",
                                                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    _project.ToggleSquareAsCompletedAtWindowCoordinate(squareRow, squareCol);
                }
            }
            else
            {
                _project.ToggleCompletedAtWindowCoordinate(squareRow, squareCol);
            }
        }

        private int GridSize
        {
            get { return SquareSize * _project.Window.Width; }
        }
    }
}
