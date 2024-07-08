using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using JPP.StructuralAnalysis;
using JPP.StructuralAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPP.Cedar.Rosetta
{
    public class TranslationEngine
    {
        const double TOLERANCE = 0.0001;

        Document _rDoc;
        IEnumerable<AreaLoad> _areas;

        public TranslationEngine(Document rDoc)
        {
            _rDoc = rDoc;
        }

        public (bool, string) Verify()
        {
            var builder = new StringBuilder("The following errors were found with the model:");
            var verified = true;

            using ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
            using FilteredElementCollector collector = new FilteredElementCollector(_rDoc);
            IList<Element> grids = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
            if (!grids.Any())
            {
                builder.AppendLine("No gridlines found");
                verified = false;
            }

            return (verified, builder.ToString());
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

            using ElementCategoryFilter loadFilter = new ElementCategoryFilter(BuiltInCategory.OST_AreaLoads);
            using FilteredElementCollector loadCollector = new FilteredElementCollector(_rDoc);
            _areas = loadCollector.WherePasses(loadFilter).WhereElementIsNotElementType().ToElements().Select(l => l as AreaLoad);

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
            if (spanAngle == 0)
            {
                newFloor.Orientation = Orientation.Horizontal;
            }
            else
            {
                if (Math.Abs(spanAngle - Math.PI / 2) < TOLERANCE)
                {
                    newFloor.Orientation = Orientation.Vertical;
                }
                else
                {
                    throw new InvalidOperationException("Non-orthognonal floor orientation");
                }
            }
            newFloor.Buildup = model.AreaBuildups[floorType.Name];

            //Hosted loads
            var hostedLoads = _areas.Where(l => l.IsHosted).Where(l => l.HostElementId == aPanel.Id);
            foreach (var load in hostedLoads)
            {
                /*var loads = (BuiltInCategory)load.LoadCategoryName .Category.Id.Value;

                switch ((BuiltInCategory)load.Category.Id.Value)
                {
                    case BuiltInCategory.OST_LoadCasesDead:
                        if (newFloor.AdditionalPermanentLoads.ContainsKey(load.LoadCaseName))
                        {
                            newFloor.AdditionalPermanentLoads[load.LoadCaseName] += UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.Kilonewtons);
                        }
                        else
                        {
                            newFloor.AdditionalPermanentLoads[load.LoadCaseName] = UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.Kilonewtons);
                        }
                        break;

                    case BuiltInCategory.OST_LoadCasesLive:
                    case BuiltInCategory.OST_LoadCasesRoofLive:
                        if (newFloor.AdditionalImposedLoads.ContainsKey(load.LoadCaseName))
                        {
                            newFloor.AdditionalPermanentLoads[load.LoadCaseName] += UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.Kilonewtons);
                        }
                        else
                        {
                            newFloor.AdditionalImposedLoads[load.LoadCaseName] = UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.Kilonewtons);
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported load case type {load.LoadCategoryName}");
                }*/
                switch (load.LoadNatureName)
                {
                    case "Dead":
                        if (newFloor.AdditionalPermanentLoads.ContainsKey(load.LoadCaseName))
                        {
                            newFloor.AdditionalPermanentLoads[load.LoadCaseName] += -UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.KilonewtonsPerSquareMeter);
                        }
                        else
                        {
                            newFloor.AdditionalPermanentLoads[load.LoadCaseName] = -UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.KilonewtonsPerSquareMeter);
                        }
                        break;

                    case "Live":
                        if (newFloor.AdditionalImposedLoads.ContainsKey(load.LoadCaseName))
                        {
                            newFloor.AdditionalPermanentLoads[load.LoadCaseName] += -UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.KilonewtonsPerSquareMeter);
                        }
                        else
                        {
                            newFloor.AdditionalImposedLoads[load.LoadCaseName] = -UnitUtils.ConvertFromInternalUnits(load.ForceVector1.Z, UnitTypeId.KilonewtonsPerSquareMeter);
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported load case type {load.LoadNatureName}");
                }
            }

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
            foreach (AnalyticalWall wall in model.Walls)
            {
                var matchedFloors = model.Floors.Where(f => Math.Abs(f.Level - wall.Top) < TOLERANCE);
                foreach (var match in matchedFloors)
                {
                    if (wall.Orientation == Orientation.Horizontal)
                    {
                        if (Math.Abs(wall.Start.Y - match.Top) < TOLERANCE || Math.Abs(wall.Start.Y - match.Bottom) < TOLERANCE)
                        {
                            wall.SupportedElements.Add(match);
                        }
                    }
                    else
                    {
                        if (Math.Abs(wall.Start.X - match.Left) < TOLERANCE || Math.Abs(wall.Start.X - match.Right) < TOLERANCE)
                        {
                            wall.SupportedElements.Add(match);
                        }
                    }
                }
            }
        }
    }
}
