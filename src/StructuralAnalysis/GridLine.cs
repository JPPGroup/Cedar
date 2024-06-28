namespace JPP.StructuralAnalysis
{
    public class GridLine
    {
        public string Name { get; set; }
        public double Location { get; set; }
        public Orientation Orientation { get; set; }
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
}
