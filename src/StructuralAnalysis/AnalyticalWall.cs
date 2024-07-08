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
        public double PermanentLineLoad { get; private set; }
        public double ImposedLineLoad { get; private set; }

        public Dictionary<string, (AreaBuildup, double)> ContributingPermanentLoads { get; private set; }
        public Dictionary<string, (double, double)> ContributingAdditionalPermanentLoads { get; private set; }
        public Dictionary<string, (double, double)> ContributingAdditionalImposedLoads { get; private set; }

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
            ContributingPermanentLoads = new Dictionary<string, (AreaBuildup, double)>();
            ContributingAdditionalImposedLoads = new Dictionary<string, (double, double)>();
            ContributingAdditionalPermanentLoads = new Dictionary<string, (double, double)>();
        }

        public void WalkWall()
        {
            PermanentLineLoad = 0;
            ImposedLineLoad = 0;

            if (ContributingPermanentLoads.ContainsKey(Buildup.Name))
            {
                var (exBuildup, exHeight) = ContributingPermanentLoads[Buildup.Name];
                exHeight += Height;

                ContributingPermanentLoads[Buildup.Name] = (exBuildup, exHeight);
            }
            else
            {
                ContributingPermanentLoads.Add(Buildup.Name, (Buildup, Height));
            }

            //Walk the support chain
            foreach (AnalyticalElement e in SupportedElements)
            {
                if (e is AnalyticalWall w)
                {
                    w.WalkWall();
                }
                if (e is AnalyticalFloor f)
                {
                    double span = 0.5;
                    if (f.Orientation != Orientation)
                    {
                        if (f.Orientation == Orientation.Horizontal)
                        {
                            span = (f.Right - f.Left) / 2;
                        }
                        else
                        {
                            span = (f.Top - f.Bottom) / 2;
                        }
                    }

                    if (ContributingPermanentLoads.ContainsKey(f.Buildup.Name))
                    {
                        var (exBuildup, exSpan) = ContributingPermanentLoads[f.Buildup.Name];
                        exSpan += span;

                        ContributingPermanentLoads[f.Buildup.Name] = (exBuildup, exSpan);
                    }
                    else
                    {
                        ContributingPermanentLoads.Add(f.Buildup.Name, (f.Buildup, span));
                    }

                    //Handle hosted loads
                    foreach (var load in f.AdditionalPermanentLoads)
                    {
                        ContributingAdditionalPermanentLoads.Add($"{f.Buildup.Name} - {load.Key}", (load.Value, span));
                        PermanentLineLoad += load.Value * span;
                    }
                    foreach (var load in f.AdditionalImposedLoads)
                    {
                        ContributingAdditionalImposedLoads.Add($"{f.Buildup.Name} - {load.Key}", (load.Value, span));
                        ImposedLineLoad += load.Value * span;
                    }
                }
            }

            foreach (var entry in ContributingPermanentLoads)
            {
                PermanentLineLoad += entry.Value.Item2 * entry.Value.Item1.PermanentLoad;
            }
        }
    }
}
