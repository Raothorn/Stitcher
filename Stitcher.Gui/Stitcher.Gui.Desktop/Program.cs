using System;
using Eto.Forms;
using System.Text.RegularExpressions;

namespace Stitcher.Desktop
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 2) return;
            var command = args[0];
            var fileName = args[1];

            bool newProject;
            if (command == "new")
            {
                newProject = true;
            }
            else if (command == "load")
            {
                newProject = false;
            }
            else
            {
                return;
            }

            new Application(Eto.Platform.Detect).Run(new MainForm(newProject, fileName));
        }
    }
}
