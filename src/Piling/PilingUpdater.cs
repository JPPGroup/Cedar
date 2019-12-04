using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;

namespace Jpp.Cedar.Piling
{
    public class PilingUpdater : IUpdater
    {
        private UpdaterId _updaterId;
        private PilingCoordinator _pilingCoordinator;      

        private bool registered = false;
        
        public PilingUpdater(AddInId id, PilingCoordinator coordinator)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));
                        
            _updaterId = new UpdaterId(id, new Guid("ddb23f37-892e-4b43-9e8a-0ad8ff381b2b"));
            _pilingCoordinator = coordinator;
            
        }

        public static void Register(Application application, PilingCoordinator pilingCoordinator)
        {
            if (application == null)
                throw new System.ArgumentNullException(nameof(application));

            if (pilingCoordinator == null)
                throw new System.ArgumentNullException(nameof(pilingCoordinator));

            PilingUpdater updater = new PilingUpdater(application.ActiveAddInId, pilingCoordinator);
            UpdaterRegistry.RegisterUpdater(updater);
            
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
        }

        public void Execute(UpdaterData data)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));

            Document document = data.GetDocument();

            if (!registered)
            {
                registered = true;
                _pilingCoordinator.RegisterDocument(document);
            }

            List<ElementId> modifiedElementIds = new List<ElementId>();
            modifiedElementIds.AddRange(data.GetAddedElementIds());
            modifiedElementIds.AddRange(data.GetModifiedElementIds());

            foreach (ElementId id in modifiedElementIds)
            {
                _pilingCoordinator.UpdateElement(document, id);
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
