namespace JPP.StructuralAnalysis
{
    public class AnalyticalPanel : AnalyticalElement
    {
        IEnumerable<Point3d> BoundaryPoints => _boundaryPoints;
        protected List<Point3d> _boundaryPoints;

        public AreaBuildup Buildup { get; set; }

        public AnalyticalPanel()
        {
            _boundaryPoints = new List<Point3d>();
        }
    }
}
