using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jpp.Cedar.Core
{
    public static class CoordinateHelper
    {
        public static XYZ GetWorldCoordinates(Document document, XYZ localCoordinate)
        {
            /*FilteredElementCollector locations = new FilteredElementCollector(document).OfClass(typeof(BasePoint));
            foreach (var locationPoint in locations)
            {
                BasePoint basePoint = locationPoint as BasePoint;
                Location svLoc = basePoint.Location;
                double projectSurvpntX = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_EASTWEST_PARAM).AsDouble();
                double projectSurvpntY = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_NORTHSOUTH_PARAM).AsDouble();
                double projectSurvpntZ = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_ELEVATION_PARAM).AsDouble();
                //XYZ loc = (basePoint.Location as LocationPoint).Point;
            }*/


            ProjectPosition projectPosition = document.ActiveProjectLocation.get_ProjectPosition(XYZ.Zero);
            // Create a translation vector for the offsets
            XYZ translationVector = new XYZ(projectPosition.EastWest, projectPosition.NorthSouth, projectPosition.Elevation);
            Transform translationTransform = Transform.CreateTranslation(translationVector);            
            // Create a rotation for the angle about true north
            //const double angleRatio = Math.PI / 180; // angle conversion factor
            Transform rotationTransform = Transform.CreateRotationAtPoint(XYZ.BasisZ, projectPosition.Angle, XYZ.Zero);
            // Combine the transforms
            Transform finalTransform = translationTransform.Multiply(rotationTransform);

            return finalTransform.OfPoint(localCoordinate);
        }
    }
}
