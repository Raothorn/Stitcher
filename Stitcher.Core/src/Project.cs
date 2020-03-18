using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Stitcher.Core
{
    public class Project
    {
        public Pattern Pattern { get; private set; }

        [JsonPropertyAttribute]
        private CompletedState[,] _completed;

        public Project(Pattern pattern)
        {
            Pattern = pattern;
            _completed = new CompletedState[pattern.Height, pattern.Width];

            for (int r = 0; r < pattern.Height; r++)
            {
                for (int c = 0; c < pattern.Width; c++)
                {
                    _completed[r, c] = CompletedState.Uncompleted;
                }
            }
        }

        [JsonConstructor]
        public Project(Pattern pattern, CompletedState[,] completed)
        {
            Pattern = pattern;
            _completed = completed;
        }

        public void ToggleCompleted(int row, int column)
        {
            if(!Pattern.GetBounds().InBounds(column, row))
            {
                return;
            }

            if (_completed[row, column] == CompletedState.Completed)
            {
                _completed[row, column] = CompletedState.Uncompleted;
            }
            else if (_completed[row, column] == CompletedState.Uncompleted)
            {
                _completed[row, column] = CompletedState.Completed;
            }
        }

        public void ToggleCompletedSquare(int row, int column)
        {

            int squareSize = 10;
            Func<int, int> roundDown = (n) => n - n % squareSize;
            int r1 = roundDown(row);
            int c1 = roundDown(column);

            for(int r = r1; r < r1 + squareSize; r++)
            {
                for(int c = c1; c < c1 + squareSize; c++)
                {
                    if(!Pattern.GetBounds().InBounds(c, r))
                    {
                        continue;
                    }
                    if(_completed[r, c] == CompletedState.CompletedSquare)
                    {
                        _completed[r, c] = CompletedState.Uncompleted;
                    }
                    else
                    {
                        _completed[r, c] = CompletedState.CompletedSquare;
                    }
                }
            }
        }

        public CompletedState GetCompletedState(int row, int column)
        {
            if(!Pattern.GetBounds().InBounds(column, row))
            {
                return CompletedState.Completed;
            }
            return _completed[row, column];
        }

        public void SaveProject(string savePath)
        {
            string serialized = JsonConvert.SerializeObject(this);
            Console.WriteLine("Saved: " + savePath);
            System.IO.File.WriteAllText(savePath, serialized);
        }

        public static Project LoadFromFile(string filePath)
        {
            Project project = null;
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                project = (Project) serializer.Deserialize(file, typeof(Project));
            }

            return project;
        }
    }

    public enum CompletedState
    {
        Uncompleted,
        Completed,
        CompletedSquare
    }
}
