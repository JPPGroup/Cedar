namespace JPP.StructuralAnalysis
{
    public class AnalyticalFloor : AnalyticalPanel
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }

        public double Level { get; private set; }

        public Orientation Orientation { get; set; }

        public Dictionary<string, double> AdditionalPermanentLoads { get; private set; }
        public Dictionary<string, double> AdditionalImposedLoads { get; private set; }

        public AnalyticalFloor(Point3d a, Point3d b, Point3d c, Point3d d) : base()
        {
            _boundaryPoints.Add(a);
            _boundaryPoints.Add(b);
            _boundaryPoints.Add(c);
            _boundaryPoints.Add(d);

            var levels = _boundaryPoints.Select(p => p.Z).Distinct();

            if (levels.Count() > 1)
                throw new NotImplementedException("Sloped floors not yet supported");

            Level = levels.First();

            Left = _boundaryPoints.Select(p => p.X).Min();
            Right = _boundaryPoints.Select(p => p.X).Max();

            Top = _boundaryPoints.Select(p => p.Y).Max();
            Bottom = _boundaryPoints.Select(p => p.Y).Min();

            AdditionalPermanentLoads = new Dictionary<string, double>();
            AdditionalImposedLoads = new Dictionary<string, double>();
        }
    }
}
