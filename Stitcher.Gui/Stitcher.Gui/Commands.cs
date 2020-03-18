using System;
using Eto.Forms;
using Stitcher.Core;

namespace Stitcher
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public class MoveCommand : Command
    {
        private Direction _direction;
        private ProjectViewModel _project;

        public MoveCommand(Direction direction, ProjectViewModel project)
        {
            _direction = direction;
            _project = project;
        }

        public static Button GetMoveButton(Direction direction, ProjectViewModel project)
        {
            Button button = new Button
            {
                Text = direction.ToString()
            };

            button.Command = new MoveCommand(direction, project);
            return button;
        }

        protected override void OnExecuted(EventArgs e)
        {
            const int Delta = 10;

            int deltaX = 0;
            int deltaY = 0;

            switch (_direction)
            {
                case Direction.Up:
                    deltaY -= Delta;
                    break;
                case Direction.Down:
                    deltaY += Delta;
                    break;
                case Direction.Left:
                    deltaX -= Delta;
                    break;
                case Direction.Right:
                    deltaX += Delta;
                    break;
            }

            _project.MoveWindow(deltaX, deltaY);
        }
    }
}
