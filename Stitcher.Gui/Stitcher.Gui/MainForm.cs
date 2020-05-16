using System;
using Eto.Forms;
using Eto.Drawing;
using Stitcher.Core;
using System.Collections.Generic;


namespace Stitcher
{
    public partial class MainForm : Form
    {
        const int WindowHeight = 800;
        const int WindowWidth = 1200;

        public MainForm(bool makeNew, string filePath)
        {
            Title = "My Eto Form";
            ClientSize = new Size(WindowWidth, WindowHeight);

            ProjectViewModel project;
            if (makeNew)
            {
                project = LoadProjectFromBitmap(filePath);
            }
            else
            {
                project = LoadProjectFromSave(filePath);
            }

            var gridArea = new PatternView(project)
            {
                Size = new Size((int)(WindowWidth * .75f), WindowHeight)
            };
            gridArea.Paint += (sender, pe) => gridArea.Draw(pe.Graphics);

            DataContext = project;

            var symbolsArea = SymbolsArea(project);

            Menu = MenuBar(project, filePath, !makeNew);
            Content = new StackLayout
            {
                Padding = 10,
                Orientation = Orientation.Horizontal,
                Items =
                {
                    gridArea,
                    symbolsArea,
                    MoveButtons(project),
                }
            };

            gridArea.Focus();
            // KeyDown += (s, e) => OnKeyPressed(e, project);
        }

        private Control SymbolsArea(ProjectViewModel project)
        {
            var grid = new ListBox {
                Height = 350
            };

            var allSymbols = project.GetAllSymbols();
            foreach(string symbol in allSymbols)
            {
                grid.Items.Add(symbol.ToString());
            }


            grid.DataContext = project;

            grid.SelectedValueChanged += (s, e) =>
                {
                    var selected = grid.SelectedValue;
                    project.SelectedSymbol = grid.SelectedValue.ToString();
                };

            grid.KeyDown += (s, e) =>
                {
                    e.Handled = true;
                };

            return grid;
        }

        private Control MoveButtons(ProjectViewModel project)
        {

            var selectLeastButton = new Button
            {
                Text = "Select Least"
            };
            selectLeastButton.Click += (s, e) => {
                project.SelectLeastStitchesInWindow();
            };

            return new StackLayout
            {
                Padding = 10,
                Items =
                {
                    MoveCommand.GetMoveButton(Direction.Up, project),
                    MoveCommand.GetMoveButton(Direction.Down, project),
                    MoveCommand.GetMoveButton(Direction.Left, project),
                    MoveCommand.GetMoveButton(Direction.Right, project),
                    selectLeastButton,
                }
            };
        }

        private MenuBar MenuBar(ProjectViewModel project, string fileName, bool isLoaded)
        {
            var saveItem = SaveMenuItem(project, fileName);
            if (!isLoaded)
            {
                saveItem.Enabled = false;
            }
            var menu = new MenuBar
            {
                Items =
                {
                    saveItem,
                    SaveAsMenuItem(project),
                }
            };
            return menu;
        }

        private ButtonMenuItem SaveAsMenuItem(ProjectViewModel project)
        {
            var saveItem = new ButtonMenuItem { Text = "Save As" };
            saveItem.Click += (sender, e) =>
                {
                    var dialog = new SaveFileDialog();
                    var result = dialog.ShowDialog(ParentWindow);

                    if(result == DialogResult.Ok)
                    {
                        project.SaveProject(dialog.FileName);
                    }
                };

            return saveItem;
        }

        private ButtonMenuItem SaveMenuItem(ProjectViewModel project, string fileName)
        {
            var saveItem = new ButtonMenuItem { Text = "Save" };
            saveItem.Click += (sender, e) =>
                {
                    project.SaveProject(fileName);
                };
            return saveItem;
        }

        private void OnKeyPressed(KeyEventArgs keyEvent, ProjectViewModel project)
        {
            var bindings = new Dictionary<Keys, Command>
            {
                [Keys.W] = new MoveCommand(Direction.Up, project),
                [Keys.A] = new MoveCommand(Direction.Left, project),
                [Keys.S] = new MoveCommand(Direction.Down, project),
                [Keys.D] = new MoveCommand(Direction.Right, project),
            };

            if (bindings.ContainsKey(keyEvent.Key))
            {
                var command = bindings[keyEvent.Key];
                command.Execute();
            }
        }

        private ProjectViewModel LoadProjectFromBitmap(string filePath)
        {
            var img = new System.Drawing.Bitmap(filePath);

            var pattern = Pattern.LoadFromBitmap(img);
            var project = new Project(pattern);
            return new Core.ProjectViewModel(project);
        }

        private ProjectViewModel LoadProjectFromSave(string filePath)
        {
            var project = Project.LoadFromFile(filePath);
            return new ProjectViewModel(project);
        }
    }
}
