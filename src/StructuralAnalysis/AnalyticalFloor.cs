namespace JPP.StructuralAnalysis
{
    public class AnalyticalFloor : AnalyticalPanel
    {
        public double Level { get; private set; }

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
        }
    }
}
