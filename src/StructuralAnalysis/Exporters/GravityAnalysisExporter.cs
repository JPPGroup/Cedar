using DocumentFormat.OpenXml.Vml.Office;
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

            bodyReport.AddPageBreak();

            //Load takedown
            var ltdHeading = bodyReport.AddParagraph("Load Takedowns");
            ltdHeading.Style = WordParagraphStyles.Heading1;

            foreach (AnalyticalWall wall in input.Walls.Where(wall => wall.SupportingElement is null).OrderBy(wall => wall.Name))
            {
                ExportWallTakedown(bodyReport, wall);
            }
        }

        private void ExportAreaBuildups(WordSection bodyReport, IEnumerable<AreaBuildup> areaBuildups)
        {
            foreach (var buildup in areaBuildups)
            {
                var buildupName = bodyReport.AddParagraph(buildup.Name);
                buildupName.Style = WordParagraphStyles.Heading5;

                var breakPara = bodyReport.AddParagraph();

                var layerTable = bodyReport.AddTable(breakPara, 0, 3, WordTableStyle.TableNormal);
                layerTable.WidthType = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Pct;
                layerTable.Width = 5000;
                foreach (var l in buildup.Layers)
                {
                    var row = layerTable.AddRow(3);
                    row.Cells[0].Paragraphs[0].Text = l.Name;
                    row.Cells[0].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                    row.Cells[1].Paragraphs[0].Text = $"{l.Density.ToString("F0")} kg/m3 x {l.Thickness.ToString("F3")}m";
                    row.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                    row.Cells[2].Paragraphs[0].Text = $"{l.AreaLoad.ToString("F3")} kN/m2";
                    row.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
                }
                var endRow = layerTable.AddRow(3);
                endRow.Cells[1].Paragraphs[0].Text = "Total Permanent Load";
                endRow.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                endRow.Cells[1].Paragraphs[0].Bold = true;

                endRow.Cells[2].Paragraphs[0].Text = $"{buildup.PermanentLoad.ToString("F3")} kN/m2";
                endRow.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
                endRow.Cells[2].Paragraphs[0].Bold = true;
            }
        }

        private void ExportWallTakedown(WordSection bodyReport, AnalyticalWall wall)
        {
            var heading = bodyReport.AddParagraph(wall.Name);
            heading.Style = WordParagraphStyles.Heading5;

            var breakPara = bodyReport.AddParagraph();

            var layerTable = bodyReport.AddTable(breakPara, 0, 3, WordTableStyle.TableNormal);
            layerTable.WidthType = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Pct;
            layerTable.Width = 5000;

            /*Dictionary<AreaBuildup, double> entries = new Dictionary<AreaBuildup, double>();
            WalkWall(wall, entries);*/

            foreach (var entry in wall.ContributingPermanentLoads)
            {
                var row = layerTable.AddRow(3);
                row.Cells[0].Paragraphs[0].Text = entry.Value.Item1.Name;
                row.Cells[0].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[1].Paragraphs[0].Text = $"{entry.Value.Item1.PermanentLoad.ToString("F3")} kN/m2 x {entry.Value.Item2.ToString("F3")}m";
                row.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[2].Paragraphs[0].Text = $"{(entry.Value.Item1.PermanentLoad * entry.Value.Item2).ToString("F3")} kN/m";
                row.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
            }
            foreach (var entry in wall.ContributingAdditionalPermanentLoads)
            {
                var row = layerTable.AddRow(3);
                row.Cells[0].Paragraphs[0].Text = entry.Key;
                row.Cells[0].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[1].Paragraphs[0].Text = $"{entry.Value.Item1.ToString("F3")} kN/m2 x {entry.Value.Item2.ToString("F3")}m";
                row.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[2].Paragraphs[0].Text = $"{(entry.Value.Item1 * entry.Value.Item2).ToString("F3")} kN/m";
                row.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
            }

            var permRow = layerTable.AddRow(3);
            permRow.Cells[1].Paragraphs[0].Text = $"Permanent Load at Foundation";
            permRow.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
            permRow.Cells[1].Paragraphs[0].Bold = true;
            permRow.Cells[2].Paragraphs[0].Text = $"{wall.PermanentLineLoad.ToString("F3")} kN/m";
            permRow.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
            permRow.Cells[2].Paragraphs[0].Bold = true;

            foreach (var entry in wall.ContributingAdditionalImposedLoads)
            {
                var row = layerTable.AddRow(3);
                row.Cells[0].Paragraphs[0].Text = entry.Key;
                row.Cells[0].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[1].Paragraphs[0].Text = $"{entry.Value.Item1.ToString("F3")} kN/m2 x {entry.Value.Item2.ToString("F3")}m";
                row.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[2].Paragraphs[0].Text = $"{(entry.Value.Item1 * entry.Value.Item2).ToString("F3")} kN/m";
                row.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
            }
            var impRow = layerTable.AddRow(3);
            impRow.Cells[1].Paragraphs[0].Text = $"Imposed Load at Foundation";
            impRow.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
            impRow.Cells[1].Paragraphs[0].Bold = true;
            impRow.Cells[2].Paragraphs[0].Text = $"{wall.ImposedLineLoad.ToString("F3")} kN/m";
            impRow.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
            permRow.Cells[2].Paragraphs[0].Bold = true;
        }


    }
}
