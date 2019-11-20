using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Jpp.Cedar.Core;

namespace Jpp.Cedar.Piling
{
    public class PilingUpdater : IUpdater
    {
        AddInId _appId;
        UpdaterId _updaterId;

        ISharedParameterManager _spManager;

        bool registered = false;
        Definition _eastingDefinition, _northingDefinition, _cutOffDefinition;

        public PilingUpdater(AddInId id, ISharedParameterManager spManager)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));

            if (spManager == null)
                throw new System.ArgumentNullException(nameof(spManager));

            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("ddb23f37-892e-4b43-9e8a-0ad8ff381b2b"));
            _spManager = spManager;

            _eastingDefinition = _spManager.RegisterParameter("Piling", "Easting", ParameterType.Length, false, "Easting");
            _northingDefinition = _spManager.RegisterParameter("Piling", "Northing", ParameterType.Length, false, "Northing");
            _cutOffDefinition = _spManager.RegisterParameter("Piling", "Cut-Off", ParameterType.Length, false, "Cut Off Level");
        }

        public static void Register(Application application, ISharedParameterManager sharedParameterManager)
        {
            if (application == null)
                throw new System.ArgumentNullException(nameof(application));

            if (sharedParameterManager == null)
                throw new System.ArgumentNullException(nameof(sharedParameterManager));

            PilingUpdater updater = new PilingUpdater(application.ActiveAddInId, sharedParameterManager);
            UpdaterRegistry.RegisterUpdater(updater);
            
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());            
        }

        public void RegisterDocument(Document document)
        {
            _spManager.BindParameters(document, _eastingDefinition);
            _spManager.BindParameters(document, _northingDefinition);
            _spManager.BindParameters(document, _cutOffDefinition);
        }

        public void Execute(UpdaterData data)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));


            Document document = data.GetDocument();

            if (!registered)
                RegisterDocument(document);

            List<ElementId> modifiedElementIds = new List<ElementId>();
            modifiedElementIds.AddRange(data.GetAddedElementIds());
            modifiedElementIds.AddRange(data.GetModifiedElementIds());

            foreach (ElementId id in modifiedElementIds)
            {
                Element foundation = document.GetElement(id);
                if (foundation.Location != null)
                {
                    XYZ location = CoordinateHelper.GetWorldCoordinates(document, (foundation.Location as LocationPoint).Point);
                   
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

        public string GetAdditionalInformation()
        {
            return "Updates all pile coordinates";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Structure;
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public string GetUpdaterName()
        {
            return "Cedar Pile Updater";
        }
    }
}
