using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;

namespace Jpp.Cedar.Piling
{
    public class PilingCoordinator
    {
        private ISharedParameter _easting, _northing, _cutOff, _permanentLoad, _variableLoad, _verticalWindLoad, _horizontalWindLoad;

        public PilingCoordinator(ISharedParameterManager spManager)
        {
            _easting = PilingParameter.Easting(spManager);
            _northing = PilingParameter.Northing(spManager);
            _cutOff = PilingParameter.CutOff(spManager);
            _permanentLoad = PilingParameter.PermanentLoad(spManager);
            _variableLoad = PilingParameter.VariableLoad(spManager);
            _verticalWindLoad = PilingParameter.VerticalWindLoad(spManager);
            _horizontalWindLoad = PilingParameter.HorizontalWindLoad(spManager);

            RegisterParameters();
        }

        public void RegisterDocument(Document document)
        {
            _easting.Bind(document);
            _northing.Bind(document);
            _cutOff.Bind(document);
            _permanentLoad.Bind(document);
            _variableLoad.Bind(document);
            _verticalWindLoad.Bind(document);
            _horizontalWindLoad.Bind(document);
        }

        public void UpdateElement(Document document, ElementId id)
        {
            Element foundation = document.GetElement(id);

            if (foundation.Location != null)
            {
                if (foundation.Location is LocationPoint locationPoint)
                {
                    XYZ location = CoordinateHelper.GetWorldCoordinates(document, locationPoint.Point);

                    foreach (Parameter para in foundation.Parameters)
                    {
                        if (para.Definition.Name.Equals(_easting.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            para.Set(location.X);
                        }

                        if (para.Definition.Name.Equals(_northing.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            para.Set(location.Y);
                        }

                        if (para.Definition.Name.Equals(_cutOff.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            para.Set(location.Z);
                        }
                    }
                }
            }
        }

        private void RegisterParameters()
        {
            _easting.Register();
            _northing.Register();
            _cutOff.Register();
            _permanentLoad.Register();
            _variableLoad.Register();
            _verticalWindLoad.Register();
            _horizontalWindLoad.Register();
        }
    }
}
