using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stitcher.Core
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private Project _project;

        public ProjectViewModel(Project project)
        {
            _project = project;
            Window = new Rect(0, 0, 30, 30);
        }

        public IList<string> GetAllSymbols()
        {
            var symbols = new HashSet<string>();
            var bounds = _project.Pattern.GetBounds();

            for(int r = bounds.Y1; r < bounds.Y2; r++)
            {
                for(int c = bounds.X1; c < bounds.X2; c++)
                {
                    var symbol = _project.Pattern.GetSymbol(r, c);
                    symbols.Add(symbol);
                }
            }

            return symbols.ToList();
        }

        public void ToggleSquareAsCompletedAtWindowCoordinate(int windowRow, int windowCol)
        {
            var row = windowRow + Window.Y1;
            var col = windowCol + Window.X1;
            _project.ToggleCompletedSquare(row, col);
            OnPropertyChanged();
        }

        public void ToggleCompletedAtWindowCoordinate(int windowRow, int windowCol)
        {
            var symbol = GetSymbolAtWindowCoordinate(windowRow, windowCol);
            if (symbol != SelectedSymbol)
            {
                return;
            }

            var row = windowRow + Window.Y1;
            var col = windowCol + Window.X1;
            _project.ToggleCompleted(row, col);
            OnPropertyChanged();
        }

        public CompletedState CompletedStateAtWindowCoordinate(int windowRow, int windowCol)
        {
            var row = windowRow + Window.Y1;
            var col = windowCol + Window.X1;
            return _project.GetCompletedState(row, col);
        }

        public string GetRgbAtWindowCoordinate(int windowRow, int windowCol)
        {
            var symbol = GetSymbolAtWindowCoordinate(windowRow, windowCol);
            return StitchLookup.RgbFromSymbol(symbol);
        }

        public string GetSymbolAtWindowCoordinate(int windowRow, int windowCol)
        {
            var row = windowRow + Window.Y1;
            var col = windowCol + Window.X1;
            return _project.Pattern.GetSymbol(row, col);
        }

        public bool InBoundsWindowCoordinate(int windowRow, int windowCol)
        {
            var row = windowRow + Window.Y1;
            var col = windowCol + Window.X1;
            return _project.Pattern.GetBounds().InBounds(col, row);
        }

        public void MoveWindow(int deltaX, int deltaY)
        {
            Window.Translate(deltaX, deltaY);
            OnPropertyChanged();
        }

        public void SaveProject(string savePath)
        {
            _project.SaveProject(savePath);
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Rect Window { get; private set; }

        private string _selectedSymbol = null;

        public string SelectedSymbol
        {
            get { return _selectedSymbol; }
            set
            {
                if (GetAllSymbols().Contains(value))
                {
                    _selectedSymbol = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
