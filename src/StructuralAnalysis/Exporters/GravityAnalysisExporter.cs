using JPP.StandardDocuments;
using JPP.StructuralAnalysis.Models;
using OfficeIMO.Word;

namespace JPP.StructuralAnalysis.Exporters
{
    public class GravityAnalysisExporter : ICalculationContributor<GravityAnalysisModel>
    {
        public void AddOutput(GravityAnalysisModel input, WordSection bodyReport, WordSection appendix)
        {
            //Buildups
            var heading = bodyReport.AddParagraph("Buildups");
            heading.Style = WordParagraphStyles.Heading1;

            //Area Loads
            var heading1 = bodyReport.AddParagraph("Area Loads");
            heading1.Style = WordParagraphStyles.Heading2;
            ExportAreaBuildups(bodyReport, input.AreaBuildups.Values);

            //Wall Loads
            var heading2 = bodyReport.AddParagraph("Wall Loads");
            heading2.Style = WordParagraphStyles.Heading2;
            ExportAreaBuildups(bodyReport, input.WallBuildups.Values);
        }

        private void ExportAreaBuildups(WordSection bodyReport, IEnumerable<AreaBuildup> areaBuildups)
        {
            foreach (var buildup in areaBuildups)
            {
                var buildupName = bodyReport.AddParagraph(buildup.Name);
                buildupName.Style = WordParagraphStyles.Heading5;

                var breakPara = bodyReport.AddParagraph();

                var layerTable = bodyReport.AddTable(breakPara, 0, 3, WordTableStyle.TableNormal);
                /*layerTable.WidthType = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Pct;
                layerTable.Width = 200;*/
                //var elements = bodyReport.AddParagraph();
                foreach (var l in buildup.Layers)
                {
                    var row = layerTable.AddRow(3);
                    row.Cells[0].AddParagraph(l.Name).SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                    row.Cells[1].AddParagraph($"{l.Density.ToString("F0")} kg/m3 x {l.Thickness.ToString("F3")}m").SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                    row.Cells[2].AddParagraph($"{l.AreaLoad.ToString("F3")} kN/m2").SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
                }
            }
        }
    }
}
