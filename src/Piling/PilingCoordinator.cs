using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jpp.Cedar.Piling
{
    public class PilingCoordinator
    {
        Definition _eastingDefinition, _northingDefinition, _cutOffDefinition;
        ISharedParameterManager _spManager;

        public PilingCoordinator(ISharedParameterManager spManager)
        {
            _spManager = spManager ?? throw new System.ArgumentNullException(nameof(spManager));

            _eastingDefinition = _spManager.RegisterParameter("Piling", "Easting", ParameterType.Length, false, "Easting");
            _northingDefinition = _spManager.RegisterParameter("Piling", "Northing", ParameterType.Length, false, "Northing");
            _cutOffDefinition = _spManager.RegisterParameter("Piling", "Cut-Off", ParameterType.Length, false, "Cut Off Level");
        }

        public void RegisterDocument(Document document)
        {
            _spManager.BindParameters(document, _eastingDefinition);
            _spManager.BindParameters(document, _northingDefinition);
            _spManager.BindParameters(document, _cutOffDefinition);
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
