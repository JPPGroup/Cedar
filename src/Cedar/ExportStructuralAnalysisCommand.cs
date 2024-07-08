using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JPP.Cedar.Rosetta;
using JPP.StandardDocuments;
using JPP.StructuralAnalysis.Exporters;
using System.IO;

namespace JPP.Cedar
{
    [Transaction(TransactionMode.ReadOnly)]
    [Journaling(JournalingMode.UsingCommandData)]
    internal class ExportStructuralAnalysisCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            /*TranslationEngine te = new TranslationEngine(commandData.Application.Application, commandData.Application.ActiveUIDocument.Document);
            var (verified, errors) = te.Verify();
            if (verified)
            {
                var geModel = te.GenerateGravityModel();
                geModel.Analyse();

                var dest = Path.Combine(Path.GetDirectoryName(commandData.Application.ActiveUIDocument.Document.PathName), "report.docx");

                using CalculationReport report = new CalculationReport(dest, new[] { new GravityAnalysisExporter() });
                report.AddInput(geModel);
                report.ProcessInputs();
                report.Finalise();

                return Result.Succeeded;
            }
            else
            {
                message = errors;
                return Result.Failed;
            }*/

            var context = RosettaContext.LoadOrCreate(commandData.Application.ActiveUIDocument.Document);

            var dest = Path.Combine(Path.GetDirectoryName(commandData.Application.ActiveUIDocument.Document.PathName), "report.docx");

            using CalculationReport report = new CalculationReport(dest, new[] { new GravityAnalysisExporter() });
            report.AddInput(context.Model);
            report.ProcessInputs();
            report.Finalise();

            return Result.Succeeded;

        }
    }
}
