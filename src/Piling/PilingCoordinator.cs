using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;

namespace Jpp.Cedar.Piling
{
    public class PilingCoordinator
    {
        private ISharedParameter _easting, _northing, _cutOff, _permanentLoad, _variableLoad, _verticalWindLoad, _horizontalWindLoad;
        private ISharedParameterManager _spManager;

        public PilingCoordinator(ISharedParameterManager spManager)
        {
            _spManager = spManager ?? throw new System.ArgumentNullException(nameof(spManager));

            _easting = PilingParameter.Easting();
            _northing = PilingParameter.Northing();
            _cutOff = PilingParameter.CutOff();
            _permanentLoad = PilingParameter.PermanentLoad();
            _variableLoad = PilingParameter.VariableLoad();
            _verticalWindLoad = PilingParameter.VerticalWindLoad();
            _horizontalWindLoad = PilingParameter.HorizontalWindLoad();

            RegisterParameters();
        }

        public void RegisterDocument(Document document)
        {
            _easting.Bind(_spManager, document);
            _northing.Bind(_spManager, document);
            _cutOff.Bind(_spManager, document);
            _permanentLoad.Bind(_spManager, document);
            _variableLoad.Bind(_spManager, document);
            _verticalWindLoad.Bind(_spManager, document);
            _horizontalWindLoad.Bind(_spManager, document);
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
            _easting.Register(_spManager);
            _northing.Register(_spManager);
            _cutOff.Register(_spManager);
            _permanentLoad.Register(_spManager);
            _variableLoad.Register(_spManager);
            _verticalWindLoad.Register(_spManager);
            _horizontalWindLoad.Register(_spManager);
        }
    }
}
