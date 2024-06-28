using System.Diagnostics;

namespace JPP.StructuralAnalysis
{
    public class AreaBuildup()
    {
        public string Name { get; init; }
        public List<AreaBuildupLayer> Layers { get; } = new List<AreaBuildupLayer>();

        public double PermanentLoad => Layers.Sum(l => l.AreaLoad);
    }

    [DebuggerDisplay("{Name} {AreaLoad} kN/m2")]
    public record AreaBuildupLayer(string Name,
        double Thickness,
        double Density)
    {
        public double AreaLoad => Thickness * Density / 100;
    }
}
