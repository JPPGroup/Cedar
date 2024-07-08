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
                X = UnitUtils.ConvertFromInternalUnits(point.X, UnitTypeId.Meters),
                Y = UnitUtils.ConvertFromInternalUnits(point.Y, UnitTypeId.Meters),
                Z = UnitUtils.ConvertFromInternalUnits(point.Z, UnitTypeId.Meters)
            };
        }
    }
}
