using Autodesk.Revit.DB;
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Jpp.Cedar.Piling
{
    public class PilingUpdater : IUpdater
    {
        AddInId _appId;
        UpdaterId _updaterId;

        public PilingUpdater(AddInId id)
        {
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("ddb23f37-892e-4b43-9e8a-0ad8ff381b2b"));
        }

        public static void Register(UIControlledApplication application)
        {
            PilingUpdater updater = new PilingUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);
            
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeAny());
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            // Cache the wall type
            /*if (m_wallType == null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfClass(typeof(WallType));
                var wallTypes = from element in collector
                    where
                        element.Name == "Exterior - Brick on CMU"
                    select element;
                if (wallTypes.Count() > 0)
                {
                    m_wallType = wallTypes.Cast().ElementAt(0);
                }
            }

            if (m_wallType != null)
            {
                // Change the wall to the cached wall type.
                foreach (ElementId addedElemId in data.GetAddedElementIds())
                {
                    Wall wall = doc.GetElement(addedElemId) as Wall;
                    if (wall != null)
                    {
                        wall.WallType = m_wallType;
                    }
                }
            }*/
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
