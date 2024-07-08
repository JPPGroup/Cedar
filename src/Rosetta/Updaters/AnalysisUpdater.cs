using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;

namespace JPP.Cedar.Rosetta.Updaters
{
    public class AnalysisUpdater : IUpdater, IDisposable
    {
        UpdaterId _id;
        Document _doc;
        private bool isDisposed;

        public AnalysisUpdater(AddInId id)
        {
            _id = new UpdaterId(id, new Guid("2758ff3e-86ec-4e71-8b65-17520dc870ee"));
        }

        public void RegisterForDocument(Document doc)
        {
            _doc = doc;

            UpdaterRegistry.RegisterUpdater(this, _doc);

            // Change Scope = any Wall element
            ElementClassFilter materialFilter = new ElementClassFilter(typeof(Material));
            UpdaterRegistry.AddTrigger(this.GetUpdaterId(), materialFilter, Element.GetChangeTypeAny());

            ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));
            UpdaterRegistry.AddTrigger(this.GetUpdaterId(), wallFilter, Element.GetChangeTypeAny());
            ElementClassFilter floorFilter = new ElementClassFilter(typeof(Floor));
            UpdaterRegistry.AddTrigger(this.GetUpdaterId(), floorFilter, Element.GetChangeTypeAny());
            ElementClassFilter analtyicalFilter = new ElementClassFilter(typeof(rAnalyticalPanel));
            UpdaterRegistry.AddTrigger(this.GetUpdaterId(), analtyicalFilter, Element.GetChangeTypeAny());
            ElementClassFilter loadFilter = new ElementClassFilter(typeof(AreaLoad));
            UpdaterRegistry.AddTrigger(this.GetUpdaterId(), loadFilter, Element.GetChangeTypeAny());
        }

        public void Execute(UpdaterData data)
        {
            var rDoc = data.GetDocument();
            string errors = "Uknown error";
            try
            {
                var context = RosettaContext.LoadOrCreate(rDoc);

                TranslationEngine te = new TranslationEngine(rDoc);
                (var verified, errors) = te.Verify();
                if (verified)
                {
                    var geModel = te.GenerateGravityModel();
                    geModel.Analyse();

                    context.Model = geModel;
                    context.Save();
                }
                else
                {
                    throw new InvalidOperationException("Analysis failed");
                    //message = errors;                
                }
            }
            catch (Exception e)
            {
                TaskDialog mainDialog = new TaskDialog("Error");
                mainDialog.MainInstruction = "Structural Analysis Error";
                mainDialog.MainContent = $"An error has occurred, {e.Message}\n{errors}";

                // Set common buttons and default button. If no CommonButton or CommandLink is added,
                // task dialog will show a Close button by default
                mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
                mainDialog.DefaultButton = TaskDialogResult.Close;

                TaskDialogResult tResult = mainDialog.Show();
            }
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
            return "Cedar Structural Analysis";
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
