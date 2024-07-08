using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JPP.Cedar.Rosetta.Updaters;

namespace JPP.Cedar
{
    [Transaction(TransactionMode.ReadOnly)]
    [Journaling(JournalingMode.UsingCommandData)]
    internal class EnableStructuralAnalysisCommand : IExternalCommand, IExternalCommandAvailability
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using var updater = new AnalysisUpdater(CedarApplication.AddInId);
            updater.RegisterForDocument(commandData.Application.ActiveUIDocument.Document);

            return Result.Succeeded;

        }

        public bool IsCommandAvailable(Autodesk.Revit.UI.UIApplication applicationData, CategorySet selectedCategories)
        {
            if (applicationData.ActiveUIDocument is null || applicationData.ActiveUIDocument.Document is null)
                return false;

            return !UpdaterRegistry.IsUpdaterRegistered(new AnalysisUpdater(CedarApplication.AddInId).GetUpdaterId(), applicationData.ActiveUIDocument.Document);
        }
    }
}
