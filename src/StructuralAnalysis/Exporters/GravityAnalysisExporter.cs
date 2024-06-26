using JPP.StandardDocuments;
using JPP.StructuralAnalysis.Models;
using OfficeIMO.Word;

namespace JPP.StructuralAnalysis.Exporters
{
    public class GravityAnalysisExporter : ICalculationContributor<GravityAnalysisModel>
    {
        public void AddOutput(GravityAnalysisModel input, WordSection bodyReport, WordSection appendix)
        {
            var heading = bodyReport.AddParagraph("Area Loads");
            heading.Style = WordParagraphStyles.Heading1;

            foreach (var buildup in input.AreaBuildups)
            {
                var buildupName = bodyReport.AddParagraph(buildup.Value.Name);
                buildupName.Style = WordParagraphStyles.Heading5;

                var breakPara = bodyReport.AddParagraph();

                var layerTable = bodyReport.AddTable(breakPara, 0, 3, WordTableStyle.TableNormal);
                /*layerTable.WidthType = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Pct;
                layerTable.Width = 200;*/
                //var elements = bodyReport.AddParagraph();
                foreach (var l in buildup.Value.Layers)
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
