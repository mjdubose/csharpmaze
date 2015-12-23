using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Maze
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private readonly Color _cell = Color.White;
        private readonly Color _wall = Color.Black;
        private readonly Color _solvecell = Color.Red;
        private readonly Color _solved = Color.Green;
        private bool[,] _grid;
        private int _size;

        private void button1_Click(object sender, EventArgs e)
        {
            _grid = null;
            if ( string.IsNullOrEmpty(textBox2.Text))
            {
                return;
            }
            _size = int.Parse(textBox2.Text);
            if (_size > 840)
            {
                return;
            }
            _grid = new bool[_size + 1, _size + 1];

            InitializeWalls(_grid, _size);

            RenderWalls(_grid, _size);
            var builderlist = new List<Cell>();
            var cells = new Cell[_size / 2, _size / 2];
            InitializeCells(cells, _size);
            SetupCellNeighbors(cells, _size);
         
            var random = new Random();
            builderlist.Add(cells[random.Next(0, (_size / 2) - 1), random.Next(0, (_size / 2) - 1)]);
            while (builderlist.Count > 0)
            {
                label2.Text = (builderlist.Count).ToString(CultureInfo.InvariantCulture);
                label2.Invalidate();
                label2.Update();
                label2.Refresh();
                Application.DoEvents();
              builderlist =  BuildMaze(builderlist, _grid, random);
            }

            label2.Text = (builderlist.Count).ToString(CultureInfo.InvariantCulture);
            label2.Invalidate();
            label2.Update();
            label2.Refresh();
            Application.DoEvents();

        }
       
        private void SolveMaze(ICollection<Cell> mazeCells , Random random)
        {
            label2.Text = (mazeCells.Count).ToString(CultureInfo.InvariantCulture);
            label2.Invalidate();
            label2.Update();
            label2.Refresh();
            Application.DoEvents();
        
              var goingtopossibleList = GoingtopossibleList(mazeCells.Last());
            if (mazeCells.Last().Visited && goingtopossibleList.Count == 0)
            {
                mazeCells.Remove(mazeCells.Last());
                return;
            }
            Draw(_solvecell, mazeCells.Last().GridLocation.X, mazeCells.Last().GridLocation.Y);
            mazeCells.Last().Visited = true;
            if (mazeCells.Last() == _lastone)
            {
                Draw(_solved, mazeCells.Last().GridLocation.X, mazeCells.Last().GridLocation.Y);
                solved = true;
                return;
            }
          
            while (0 < goingtopossibleList.Count)
            {

                var whichway = random.Next(0, goingtopossibleList.Count);
                var cellgoingto = goingtopossibleList[whichway];
                goingtopossibleList.Clear();
                mazeCells.Add(cellgoingto);
            }
            label2.Text = (int.Parse(label2.Text) - 1).ToString(CultureInfo.InvariantCulture);
            label2.Invalidate();
            label2.Update();
            label2.Refresh();
        }
        private List<Cell> BuildMaze(List<Cell>mazeCell , bool[,] grid, Random random)
        {
              var goingtopossibleList = GoingtopossibleList(mazeCell.Last());
            if (mazeCell.Last().Visited && goingtopossibleList.Count > 0)
            {
                var whichway = random.Next(0, goingtopossibleList.Count);
                Removewall(mazeCell.Last(), goingtopossibleList[whichway], grid);
                mazeCell.Add(goingtopossibleList[whichway]);
                return mazeCell;
            }
            RemoveCurrentPosition(mazeCell.Last(), grid);
            mazeCell.Last().Visited = true;


            if (0 < goingtopossibleList.Count)
            {
                var whichway = random.Next(0, goingtopossibleList.Count);

                Removewall(mazeCell.Last(), goingtopossibleList[whichway], grid);
                mazeCell.Add(goingtopossibleList[whichway]);
                return mazeCell;
            }
            mazeCell.Remove(mazeCell.Last());
            return mazeCell;
        }

        private void Draw(Color color, int x, int y)
        {
            using (var myBrush = new SolidBrush(color))
            {
                using (var formGraphics = CreateGraphics())
                {
                    formGraphics.FillRectangle(myBrush, x , y , 1, 1);
                }
            }
        }
        private void RemoveCurrentPosition(Cell mazeCell, bool[,] grid)
        {
            grid[mazeCell.GridLocation.X, mazeCell.GridLocation.Y] = true;
            Draw(_cell, mazeCell.GridLocation.X, mazeCell.GridLocation.Y);
        }

        private static List<Cell> GoingtopossibleList(Cell mazeCell)
        {
            var goingtopossibleList = new List<Cell>();
            if (!mazeCell.Top.Visited)
                goingtopossibleList.Add(mazeCell.Top);
            if (!mazeCell.Bottom.Visited)
            { goingtopossibleList.Add(mazeCell.Bottom);
           }
            if (!mazeCell.Right.Visited)
            {
                goingtopossibleList.Add(mazeCell.Right);
              
            }
            if (!mazeCell.Left.Visited)
            {
                goingtopossibleList.Add(mazeCell.Left);
              
            }
            return goingtopossibleList;
        }

        private void Removewall(Cell mazeCell, Cell whichway, bool[,] grid)
        {


            if (mazeCell.GridLocation.X < whichway.GridLocation.X)
            {
                grid[mazeCell.GridLocation.X + 1, mazeCell.GridLocation.Y] = true;
                Draw(_cell, mazeCell.GridLocation.X + 1, mazeCell.GridLocation.Y);
            }
            if (mazeCell.GridLocation.X > whichway.GridLocation.X)
            {
                grid[mazeCell.GridLocation.X - 1, mazeCell.GridLocation.Y] = true;
                Draw(_cell, mazeCell.GridLocation.X - 1, mazeCell.GridLocation.Y);
            }
            if (mazeCell.GridLocation.Y < whichway.GridLocation.Y)
            {
                grid[mazeCell.GridLocation.X, mazeCell.GridLocation.Y + 1] = true;
                Draw(_cell, mazeCell.GridLocation.X, mazeCell.GridLocation.Y + 1);
            }
            if (mazeCell.GridLocation.Y <= whichway.GridLocation.Y) return;
            grid[mazeCell.GridLocation.X, mazeCell.GridLocation.Y - 1] = true;
            Draw(_cell, mazeCell.GridLocation.X, mazeCell.GridLocation.Y - 1);
        }


        private void RenderWalls(bool[,] grid, int gridSize)
        {
            using (var myBrush = new SolidBrush(_cell))
            {
                using (var blackBrush = new SolidBrush(_wall))
                {
                    using (var formGraphics = CreateGraphics())
                    {
                        formGraphics.FillRectangle(myBrush, new Rectangle(0, 0, (_size + 1) , (_size + 1) ));

                        for (var count = 0; count < gridSize + 1; count++)
                        {
                            for (var count1 = 0; count1 < gridSize + 1; count1++)
                            {
                                if (!grid[count, count1])
                                {
                                    formGraphics.FillRectangle(blackBrush, count, count1, 1, 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void InitializeWalls(bool[,] grid, int size)
        {
            for (var j = 0; j < size + 1; j++)
            {
                for (var i = 0; i < size + 1; i++)
                {
                    grid[i, j] = false;
                }
            }
        }

        private static void InitializeSolverCells(Cell[,] cells, bool[,] grid, int size)
        {
            for (var j = 0; j <= (size) - 1; j++)
            {
                for (var i = 0; i <= (size) - 1; i++)
                {
                    if (grid[j, i])
                    {
                        cells[j, i] = new Cell(new Point(j, i)) { Visited = false };
                    }
                    else
                    {
                        cells[j, i] = new Cell(new Point(j, i)) { Visited = true };
                    }
                }
            }
        }
        private static void InitializeCells(Cell[,] cells, int size)
        {
            for (var j = 0; j <= (size / 2) - 1; j++)
            {
                for (var i = 0; i <= (size / 2) - 1; i++)
                {
                    cells[j, i] = new Cell(new Point(i * 2 + 1, j * 2 + 1)) { Visited = false };
                }
            }
        }

        private static void SetupCellNeighbors(Cell[,] cells, int size)
        {
            for (var j = 0; j <= (size / 2) - 1; j++)
            {
                for (var i = 0; i <= (size / 2) - 1; i++)
                {
                    if (i == 0)
                    {
                        cells[i, j].Top = cells[i, j];
                    }
                    if (j == 0)
                    {
                        cells[i, j].Left = cells[i, j];
                    }
                    if (i == size / 2 - 1)
                    {
                        cells[i, j].Bottom = cells[i, j];
                    }
                    if (j == size / 2 - 1)
                    {
                        cells[i, j].Right = cells[i, j];
                    }

                    if (cells[i, j].Top == null)
                    {
                        cells[i, j].Top = cells[i - 1, j];

                    }
                    if (cells[i, j].Bottom == null)
                    {
                        cells[i, j].Bottom = cells[i + 1, j];

                    }
                    if (cells[i, j].Left == null)
                    {
                        cells[i, j].Left = cells[i, j - 1];

                    }
                    if (cells[i, j].Right == null)
                    {
                        cells[i, j].Right = cells[i, j + 1];

                    }
                }
            }
        }
        private static void SetupSolveCellNeighbors(Cell[,] cells, int size)
        {
            for (var j = 0; j <= (size) - 1; j++)
            {
                for (var i = 0; i <= (size) - 1; i++)
                {
                    if (i == 0)
                    {
                        cells[i, j].Top = cells[i, j];
                    }
                    if (j == 0)
                    {
                        cells[i, j].Left = cells[i, j];
                    }
                    if (i == size - 1)
                    {
                        cells[i, j].Bottom = cells[i, j];
                    }
                    if (j == size - 1)
                    {
                        cells[i, j].Right = cells[i, j];
                    }

                    if (cells[i, j].Top == null)
                    {
                        cells[i, j].Top = cells[i - 1, j];

                    }
                    if (cells[i, j].Bottom == null)
                    {
                        cells[i, j].Bottom = cells[i + 1, j];

                    }
                    if (cells[i, j].Left == null)
                    {
                        cells[i, j].Left = cells[i, j - 1];

                    }
                    if (cells[i, j].Right == null)
                    {
                        cells[i, j].Right = cells[i, j + 1];

                    }
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string sFileContents;

            using (var oStreamReader = new StreamReader(File.OpenRead("c:\\temp\\Tester.csv")))
            {
                sFileContents = oStreamReader.ReadToEnd();
            }

            var sFileLines = sFileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var oCsvList = sFileLines.Select(sFileLine => sFileLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).ToList();
            _size = oCsvList.Count - 1;
            _grid = new bool[oCsvList.Count, oCsvList.Count];
            for (var i = 0; i < oCsvList.Count; i++)
            {
                for (var j = 0; j < oCsvList.Count; j++)
                {
                    _grid[i, j] = (oCsvList[i][j] == "True");
                }
            }
            RenderWalls(_grid, _size);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            const string filePath = @"C:\temp\tester.csv";

            var output = _grid;

            var length = output.GetLength(0);
            var sb = new StringBuilder();
            for (var index = 0; index < length; index++)
            {
                var write = "";
                for (var index1 = 0; index1 < length; index1++)
                {
                    if (index1 == length - 1)
                    {
                        write += output[index, index1].ToString();
                    }
                    else
                        write += output[index, index1] + ",";

                }
                write = write + Environment.NewLine;
                sb.AppendLine(write);
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            solved = false;
            var cells = new Cell[_size, _size];
            InitializeSolverCells(cells, _grid, _size);
            SetupSolveCellNeighbors(cells, _size);
            var num = new Random();
            var thisone = cells[1, 1];
            var cellsProcessed = new List<Cell>();
            _lastone = cells[_size - 1, _size - 1];
            Draw(_solved, _lastone.GridLocation.X, _lastone.GridLocation.Y);
            cellsProcessed.Add(thisone);
            while (cellsProcessed.Count > 0)
            {

                if (solved)
                {
                    while (cellsProcessed.Count > 0)
                    {
                        Draw(_solved, cellsProcessed.Last().GridLocation.X, cellsProcessed.Last().GridLocation.Y);
                        cellsProcessed.Remove(cellsProcessed.Last());
                        label2.Text = (int.Parse(label2.Text) - 1).ToString(CultureInfo.InvariantCulture);
                        label2.Invalidate();
                        label2.Update();
                        label2.Refresh();

                    }
                    
                }
                else
                {
                    SolveMaze(cellsProcessed, num);
                }
            }
            
            label2.Text = @"0";
        }
    }
}
