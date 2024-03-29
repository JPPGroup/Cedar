﻿using Autodesk.Revit.DB;
using System;

namespace Jpp.Cedar.Piling
{
    public class CoordinatePilingUpdater : IUpdater
    {
        private static UpdaterId _updaterId;
        private PilingCoordinator _pilingCoordinator;      

        private bool registered = false;

        private CoordinatePilingUpdater(AddInId id, PilingCoordinator coordinator)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (coordinator == null)
                throw new ArgumentNullException(nameof(coordinator));

            _updaterId = new UpdaterId(id, new Guid("a066aabd-7ccd-43c3-9a86-b2089ebabb99"));
            _pilingCoordinator = coordinator;
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

            FilteredElementCollector foundationCollection = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_StructuralFoundation);

            foreach (Element element in foundationCollection)
            {
                _pilingCoordinator.UpdateElement(document, element.Id);
            }
        }

        public string GetAdditionalInformation()
        {
            return "Updates all pile coordinates in response to project coordinate change";
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
            return "Cedar Coordinate Pile Updater";
        }

        internal static void Register(AddInId addInId, PilingCoordinator pilingCoordinator)
        {
            CoordinatePilingUpdater updater = new CoordinatePilingUpdater(addInId, pilingCoordinator);
            UpdaterRegistry.RegisterUpdater(updater);

            ElementCategoryFilter basepointFilter = new ElementCategoryFilter(BuiltInCategory.OST_ProjectBasePoint);
            UpdaterRegistry.AddTrigger(GetUpdaterId(), basepointFilter, Element.GetChangeTypeAny());
        }

        internal static void Unregister()
        {
            UpdaterRegistry.UnregisterUpdater(GetUpdaterId());
        }
    }
}
