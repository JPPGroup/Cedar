using Autodesk.Revit.DB;
using System;

namespace JPP.Cedar.Rosetta.Updaters
{
    public class FoundationUpdater : IUpdater, IDisposable
    {
        UpdaterId _id;
        Document _doc;
        private bool isDisposed;

        public FoundationUpdater(AddInId id, Document doc)
        {
            _id = new UpdaterId(id, new Guid("e7b34c98-706f-457a-b7cd-2d42be8d0b9f"));
            _doc = doc;

            UpdaterRegistry.RegisterUpdater(this, _doc);

            // Change Scope = any Wall element
            //ElementClassFilter wallFilter = new ElementClassFilter(typeof(Material));

            // Change type = element addition
            //UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), wallFilter, Element.GetChangeTypeElementAddition());            
        }

        public void Execute(UpdaterData data)
        {
            throw new NotImplementedException();
        }

        public string GetAdditionalInformation()
        {
            return "Structural appraisal updater";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FreeStandingComponents;
        }

        public UpdaterId GetUpdaterId()
        {
            return _id;
        }

        public string GetUpdaterName()
        {
            return "Cedar Structural Analysis Foundations";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                //UpdaterRegistry.UnregisterUpdater(_id, _doc);
            }

            isDisposed = true;
        }
    }
}
