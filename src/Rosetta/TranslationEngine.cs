using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using JPP.StructuralAnalysis;
using JPP.StructuralAnalysis.Models;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace JPP.Cedar.Rosetta
{
    public class TranslationEngine
    {

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
            var rAnalyticalManager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(_rDoc);

            using ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_AnalyticalPanel);
            using FilteredElementCollector collector = new FilteredElementCollector(_rDoc);
            IList<Element> panels = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Element panelElement in panels)
            {
                var aPanel = panelElement as AnalyticalPanel;
                var physicalElementId = rAnalyticalManager.GetAssociatedElementId(panelElement.Id);
                var physicalElement = _rDoc.GetElement(physicalElementId);

                if (physicalElement is Floor f)
                {
                    var floorType = f.FloorType;
                    if (!model.AreaBuildups.ContainsKey(floorType.Name))
                        model.AreaBuildups.Add(floorType.Name, ConvertFloor(floorType));
                }
            }

            return model;
        }

        private AreaBuildup ConvertFloor(FloorType floorType)
        {
            var abResult = new AreaBuildup()
            {
                Name = floorType.Name
            };

            var buildup = floorType.GetCompoundStructure();
            var layers = buildup.GetLayers();

            foreach (var layer in layers)
            {
                var mat = _rDoc.GetElement(layer.MaterialId) as Material;
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
    }
}
