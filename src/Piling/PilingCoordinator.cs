using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;

namespace Jpp.Cedar.Piling
{
    public class PilingCoordinator
    {
        private Definition _eastingDefinition, _northingDefinition, _cutOffDefinition, _permanentLoad, _variableLoad, _windVertical, _windHorizontal;
        private ISharedParameterManager _spManager;

        public PilingCoordinator(ISharedParameterManager spManager)
        {
            _spManager = spManager ?? throw new System.ArgumentNullException(nameof(spManager));

            _eastingDefinition = _spManager.RegisterParameter("Piling", "Easting", ParameterType.Length, false, "Easting", new Guid("76af35ad-70d4-41ab-bdb9-e930aea81bf3"));
            _northingDefinition = _spManager.RegisterParameter("Piling", "Northing", ParameterType.Length, false, "Northing", new Guid("828e2c7f-416c-452b-91ae-69c9058634a8"));
            _cutOffDefinition = _spManager.RegisterParameter("Piling", "Cut-Off", ParameterType.Length, false, "Cut Off Level", new Guid("95282567-0631-4ace-87bd-55b04ca2f222"));
            _permanentLoad = _spManager.RegisterParameter("Piling", "Permanent Load", ParameterType.Force, true, "Permanent Vertical Load", new Guid("f2b69461-d8cf-43e1-a4e1-8c58ffdb82c1"));
            _variableLoad = _spManager.RegisterParameter("Piling", "Variable Load", ParameterType.Force, true, "Variable Vertical Load", new Guid("02ba8899-560d-479a-bacd-81ec071da663"));
            _windVertical = _spManager.RegisterParameter("Piling", "Vertical Wind Load", ParameterType.Force, true, "Vertical Wind Load", new Guid("e3e3d7ca-5a04-45fa-8f22-a1a04c8f88ad"));
            _windHorizontal = _spManager.RegisterParameter("Piling", "Horizontal Wind Load", ParameterType.Force, true, "Horizontal Wind Load", new Guid("e15c5f1f-5350-4168-8b17-72680be90c84"));
        }

        public void RegisterDocument(Document document)
        {
            _spManager.BindParameters(document, _eastingDefinition);
            _spManager.BindParameters(document, _northingDefinition);
            _spManager.BindParameters(document, _cutOffDefinition);
            _spManager.BindParameters(document, _permanentLoad);
            _spManager.BindParameters(document, _variableLoad);
            _spManager.BindParameters(document, _windVertical);
            _spManager.BindParameters(document, _windHorizontal);
        }

        public void UpdateElement(Document document, ElementId id)
        {
            Element foundation = document.GetElement(id);
            if (foundation.Location != null)
            {
                XYZ location = CoordinateHelper.GetWorldCoordinates(document, ((LocationPoint)foundation.Location).Point);

                foreach (Parameter para in foundation.Parameters)
                {
                    if (para.Definition.Name.Equals(_eastingDefinition.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        para.Set(location.X);
                    }
                    if (para.Definition.Name.Equals(_northingDefinition.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        para.Set(location.Y);
                    }
                    if (para.Definition.Name.Equals(_cutOffDefinition.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        para.Set(location.Z);
                    }
                }
            }
        }
    }
}
