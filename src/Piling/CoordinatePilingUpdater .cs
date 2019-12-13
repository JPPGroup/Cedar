using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;

namespace Jpp.Cedar.Piling
{
    public class CoordinatePilingUpdater : IUpdater
    {
        private UpdaterId _updaterId;
        private PilingCoordinator _pilingCoordinator;      

        private bool registered = false;
        
        public CoordinatePilingUpdater(AddInId id, PilingCoordinator coordinator)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));
                        
            _updaterId = new UpdaterId(id, new Guid("a066aabd-7ccd-43c3-9a86-b2089ebabb99"));
            _pilingCoordinator = coordinator;
            
        }

        public static void Register(Application application, PilingCoordinator pilingCoordinator)
        {
            if (application == null)
                throw new System.ArgumentNullException(nameof(application));

            if (pilingCoordinator == null)
                throw new System.ArgumentNullException(nameof(pilingCoordinator));

            CoordinatePilingUpdater updater = new CoordinatePilingUpdater(application.ActiveAddInId, pilingCoordinator);
            UpdaterRegistry.RegisterUpdater(updater);

            ElementCategoryFilter basepointFilter = new ElementCategoryFilter(BuiltInCategory.OST_ProjectBasePoint);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), basepointFilter, Element.GetChangeTypeAny());
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

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public string GetUpdaterName()
        {
            return "Cedar Coordinate Pile Updater";
        }
    }
}
