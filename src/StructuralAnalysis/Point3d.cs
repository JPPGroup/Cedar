namespace JPP.StructuralAnalysis
{
    public struct Point3d : IEquatable<Point3d>
    {
        const double TOLERANCE = 0.0001;

        public double X;
        public double Y;
        public double Z;

        bool IEquatable<Point3d>.Equals(Point3d point)
        {
            return Math.Abs(X - point.X) < TOLERANCE && Math.Abs(Y - point.Y) < TOLERANCE && Math.Abs(Z - point.Z) < TOLERANCE;
        }

        public bool MatchesPlan(Point3d point)
        {
            return Math.Abs(X - point.X) < TOLERANCE && Math.Abs(Y - point.Y) < TOLERANCE;
        }

    }
}
