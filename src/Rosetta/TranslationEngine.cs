using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using JPP.StructuralAnalysis;
using JPP.StructuralAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JPP.Cedar.Rosetta
{
    public class TranslationEngine
    {
        const double TOLERANCE = 0.0001;

        Application _revitApp;
        Document _rDoc;

        public TranslationEngine(Application revitApp, Document rDoc)
        {
            _revitApp = revitApp;
            _rDoc = rDoc;
        }

        public GravityAnalysisModel GenerateGravityModel()
        {
            GravityAnalysisModel model = new GravityAnalysisModel();

            BuildGrids(model);
            BuildPanels(model);
            LinkElements(model);

            return model;
        }

        private void BuildGrids(GravityAnalysisModel model)
        {
            using ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
            using FilteredElementCollector collector = new FilteredElementCollector(_rDoc);
            IList<Element> grids = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Grid g in grids)
            {
                var name = g.Name;
                var start = g.Curve.GetEndPoint(0);
                var end = g.Curve.GetEndPoint(1);

                GridLine gl = new GridLine()
                {
                    Name = name,
                };

                if (Math.Abs(start.X - end.X) < TOLERANCE)
                {
                    gl.Orientation = Orientation.Vertical;
                    gl.Location = start.X;
                    model.VerticalGrids.Add(gl);
                }
                else
                {
                    if (Math.Abs(start.Y - end.Y) < TOLERANCE)
                    {
                        gl.Orientation = Orientation.Horizontal;
                        gl.Location = start.Y;
                        model.HorizontalGrids.Add(gl);
                    }
                    else
                    {
                        throw new InvalidOperationException("Slanted grids are not yet supported");
                    }
                }
            }
        }

        private void BuildPanels(GravityAnalysisModel model)
        {
            var rAnalyticalManager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(_rDoc);

            using ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_AnalyticalPanel);
            using FilteredElementCollector collector = new FilteredElementCollector(_rDoc);
            IList<Element> panels = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Element panelElement in panels)
            {
                var aPanel = panelElement as rAnalyticalPanel;
                var physicalElementId = rAnalyticalManager.GetAssociatedElementId(panelElement.Id);
                var physicalElement = _rDoc.GetElement(physicalElementId);

                if (physicalElement is Floor f)
                {
                    BuildFloor(model, aPanel, f);
                }

                if (physicalElement is Wall w)
                {
                    BuildWall(model, aPanel, w);
                }
            }
        }

        private void BuildFloor(GravityAnalysisModel model, rAnalyticalPanel aPanel, Floor f)
        {
            var floorType = f.FloorType;
            if (!model.AreaBuildups.ContainsKey(floorType.Name))
                model.AreaBuildups.Add(floorType.Name, ConvertPanelType(floorType));

            var spanAngle = f.SpanDirectionAngle;

            var boundCurve = aPanel.GetOuterContour();
            List<Point3d> points = new List<Point3d>();

            foreach (var curve in boundCurve)
            {
                var start = curve.GetEndPoint(0).Convert();
                var end = curve.GetEndPoint(1).Convert();

                if (!points.Contains(start))
                    points.Add(start);

                if (!points.Contains(end))
                    points.Add(end);
            }

            if (points.Count != 4)
                throw new InvalidOperationException("Non rectangular floors not currently supported");

            AnalyticalFloor newFloor = new AnalyticalFloor(points[0], points[1], points[2], points[3]);
            newFloor.Buildup = model.AreaBuildups[floorType.Name];

            model.Floors.Add(newFloor);
        }

        private void BuildWall(GravityAnalysisModel model, rAnalyticalPanel aPanel, Wall w)
        {
            var wallType = w.WallType;
            if (!model.WallBuildups.ContainsKey(wallType.Name))
                model.WallBuildups.Add(wallType.Name, ConvertPanelType(wallType));

            var boundCurve = aPanel.GetOuterContour();
            List<Point3d> points = new List<Point3d>();

            foreach (var curve in boundCurve)
            {
                var start = curve.GetEndPoint(0).Convert();
                var end = curve.GetEndPoint(1).Convert();

                if (!points.Contains(start))
                    points.Add(start);

                if (!points.Contains(end))
                    points.Add(end);
            }

            AnalyticalWall newWall = new AnalyticalWall(points[0], points[1], points[2], points[3]);
            newWall.Buildup = model.WallBuildups[wallType.Name];

            //Name wall from grids
            if (newWall.Orientation == Orientation.Horizontal)
            {
                var parallelGrid = model.HorizontalGrids.OrderBy(gl => Math.Abs(gl.Location - newWall.Start.Y)).First();
                var startGrid = model.VerticalGrids.OrderBy(gl => Math.Abs(gl.Location - newWall.Start.X)).First();
                var endGrid = model.VerticalGrids.OrderBy(gl => Math.Abs(gl.Location - newWall.End.X)).First();

                newWall.Name = $"{parallelGrid.Name}({startGrid.Name}-{endGrid.Name})";
            }
            else
            {
                var parallelGrid = model.VerticalGrids.OrderBy(gl => Math.Abs(gl.Location - newWall.Start.X)).First();
                var startGrid = model.HorizontalGrids.OrderBy(gl => Math.Abs(gl.Location - newWall.Start.Y)).First();
                var endGrid = model.HorizontalGrids.OrderBy(gl => Math.Abs(gl.Location - newWall.End.Y)).First();

                newWall.Name = $"{parallelGrid.Name}({startGrid.Name}-{endGrid.Name})";
            }

            model.Walls.Add(newWall);
        }

        private AreaBuildup ConvertPanelType(HostObjAttributes panelType)
        {
            var abResult = new AreaBuildup()
            {
                Name = panelType.Name
            };

            var buildup = panelType.GetCompoundStructure();
            var layers = buildup.GetLayers();

            foreach (var layer in layers)
            {
                var mat = _rDoc.GetElement(layer.MaterialId) as Material;

                if (mat is null)
                    continue;

                double thickness = UnitUtils.ConvertFromInternalUnits(layer.Width, UnitTypeId.Meters);
                double density = 0;
                var pse = _rDoc.GetElement(mat.StructuralAssetId) as PropertySetElement;
                if (pse != null)
                {
                    var sAsset = pse.GetStructuralAsset();
                    density = UnitUtils.ConvertFromInternalUnits(sAsset.Density, UnitTypeId.KilogramsPerCubicMeter);
                }

                abResult.Layers.Add(new AreaBuildupLayer(mat.Name, thickness, density));
            }

            return abResult;
        }

        private void LinkElements(GravityAnalysisModel model)
        {
            LinkWalls(model);
            LinkFloors(model);
        }

        private void LinkWalls(GravityAnalysisModel model)
        {
            /*var wallsByLevels = model.Walls.GroupBy(wall => wall.Base).OrderBy(wallgroup => wallgroup.Key);
            //Skip last level
            for (int i = wallsByLevels.Count() - 1; i > 0; i--)
            {
                var currentLevel = wallsByLevels.ElementAt(i).ToList();
                var lowerLevel = wallsByLevels.ElementAt(i - 1).ToList();
            }*/

            foreach (AnalyticalWall wall in model.Walls)
            {
                var matches = model.Walls.Where(w => Math.Abs(w.Top - wall.Base) < TOLERANCE);
                foreach (var match in matches)
                {
                    if (wall.Start.MatchesPlan(match.Start) && wall.End.MatchesPlan(match.End))
                    {
                        //Walls align
                        match.SupportedElements.Add(wall);
                        wall.SupportingElement = match;
                        break;
                    }
                }
            }
        }

        private void LinkFloors(GravityAnalysisModel model)
        {
        }
    }
}
