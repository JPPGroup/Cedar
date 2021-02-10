using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Jpp.Cedar.Piling
{
    public class PilingUpdater : IUpdater
    {
        private static UpdaterId _updaterId;
        private PilingCoordinator _pilingCoordinator;      

        private bool registered = false;
        
        private PilingUpdater(AddInId id, PilingCoordinator coordinator)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (coordinator == null)
                throw new ArgumentNullException(nameof(coordinator));

            _updaterId = new UpdaterId(id, new Guid("ddb23f37-892e-4b43-9e8a-0ad8ff381b2b"));
            _pilingCoordinator = coordinator;
        }

        public void Execute(UpdaterData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

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

        public static UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        UpdaterId IUpdater.GetUpdaterId()
        {
            return GetUpdaterId();
        }

        public string GetUpdaterName()
        {
            return "Cedar Pile Updater";
        }

        internal static void Register(AddInId addInId, PilingCoordinator coordinator)
        {
            PilingUpdater updater = new PilingUpdater(addInId, coordinator);
            UpdaterRegistry.RegisterUpdater(updater);

            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            UpdaterRegistry.AddTrigger(GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
        }

        internal static void Unregister()
        {
            UpdaterRegistry.UnregisterUpdater(GetUpdaterId());
        }
    }
}
