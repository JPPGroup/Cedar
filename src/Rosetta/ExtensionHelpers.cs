using Autodesk.Revit.DB;
using JPP.StructuralAnalysis;

namespace JPP.Cedar.Rosetta
{
    internal static class ExtensionHelpers
    {
        public static Point3d Convert(this XYZ point)
        {
            return new Point3d()
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
        }
    }
}
