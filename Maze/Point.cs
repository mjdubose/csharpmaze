namespace Maze
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public class Cell
    {
        public Point GridLocation { get; set; }
        public bool Visited { get; set; }
        public Cell Top { get; set; }
        public Cell Bottom { get; set; }
        public Cell Left { get; set; }
        public Cell Right { get; set; }
        public Cell(Point gridlocation)
        {
            GridLocation = gridlocation;
        }
    }
}
