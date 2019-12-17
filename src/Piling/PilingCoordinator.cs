using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;

namespace Jpp.Cedar.Piling
{
    public class PilingCoordinator
    {
        private readonly FailureDefinitionId _warnNoLocationId;
        private readonly FailureDefinition _warnNoLocationDef;
        private ISharedParameter _easting, _northing, _cutOff, _permanentLoad, _variableLoad, _verticalWindLoad, _horizontalWindLoad;

        private PilingCoordinator(ISharedParameterManager spManager)
        {
            _easting = PilingParameter.Easting(spManager);
            _northing = PilingParameter.Northing(spManager);
            _cutOff = PilingParameter.CutOff(spManager);
            _permanentLoad = PilingParameter.PermanentLoad(spManager);
            _variableLoad = PilingParameter.VariableLoad(spManager);
            _verticalWindLoad = PilingParameter.VerticalWindLoad(spManager);
            _horizontalWindLoad = PilingParameter.HorizontalWindLoad(spManager);

            _warnNoLocationId = new FailureDefinitionId(new Guid("2c644284-59fe-4f5c-b8b3-e89977af7d15"));
            _warnNoLocationDef = FailureDefinition.CreateFailureDefinition(_warnNoLocationId, FailureSeverity.Warning, "Unable to determine location.");
        }

        public static void Register(AddInId addInId)
        {
            ISharedParameterManager parameterManager = new SharedParameterManager();
            PilingCoordinator coordinator = new PilingCoordinator(parameterManager);

            PilingUpdater.Register(addInId, coordinator);
            CoordinatePilingUpdater.Register(addInId, coordinator);
        }

        public static void Unregister(AddInId addInId)
        {
            ISharedParameterManager parameterManager = new SharedParameterManager();
            PilingCoordinator coordinator = new PilingCoordinator(parameterManager);

            PilingUpdater.Unregister(addInId, coordinator);
            CoordinatePilingUpdater.Unregister(addInId, coordinator);
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
                    if (location == null)
                    {
                        PostNoLocationMessage(document, id);
                    }
                    else
                    {
                        UpdateParameters(foundation, location);
                    }
                }
            }
        }

        private void UpdateParameters(Element foundation, XYZ location)
        {
            foreach (Parameter para in foundation.Parameters)
            {
                _easting.SetParameterValue(para, location.X);
                _northing.SetParameterValue(para, location.Y);
                _cutOff.SetParameterValue(para, location.Z);
            }
        }

        private void PostNoLocationMessage(Document document, ElementId id)
        {
            using (FailureMessage message = new FailureMessage(_warnNoLocationId))
            {
                message.SetFailingElement(id);
                document.PostFailure(message);
            }
        }
    }
}
