namespace JPP.StructuralAnalysis
{
    public class AnalyticalWall : AnalyticalPanel
    {
        const double TOLERANCE = 0.0001;

        public double Base { get; private set; }
        public double Top { get; private set; }
        public double Height => Top - Base;

        public AnalyticalElement? SupportingElement { get; set; }
        public List<AnalyticalElement> SupportedElements { get; private set; }

        public Point3d Start { get; private set; }
        public Point3d End { get; private set; }

        public Orientation Orientation { get; private set; }

        public string Name { get; set; }

        public double SelfWeightLineLoad => Buildup.PermanentLoad * Height;

        public AnalyticalWall(Point3d a, Point3d b, Point3d c, Point3d d) : base()
        {
            _boundaryPoints.Add(a);
            _boundaryPoints.Add(b);
            _boundaryPoints.Add(c);
            _boundaryPoints.Add(d);

            Top = _boundaryPoints.Select(p => p.Z).Max();
            Base = _boundaryPoints.Select(p => p.Z).Min();

            var orderedPoints = _boundaryPoints.OrderBy(p => p.X).ThenByDescending(p => p.Y);
            Start = orderedPoints.First();
            End = orderedPoints.Last();

            if (Math.Abs(Start.X - End.X) < TOLERANCE)
            {
                Orientation = Orientation.Vertical;
            }
            else
            {
                if (Math.Abs(Start.Y - End.Y) < TOLERANCE)
                {
                    Orientation = Orientation.Horizontal;
                }
                else
                {
                    throw new InvalidOperationException("Non orthogonal walls are not yet supported");
                }
            }

            SupportedElements = new List<AnalyticalElement>();
        }
    }
}
