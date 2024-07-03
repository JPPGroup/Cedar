using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
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

            Dictionary<AreaBuildup, double> entries = new Dictionary<AreaBuildup, double>();
            WalkWall(wall, entries);

            foreach (var entry in entries)
            {
                var row = layerTable.AddRow(3);
                row.Cells[0].Paragraphs[0].Text = entry.Key.Name;
                row.Cells[0].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[1].Paragraphs[0].Text = $"{entry.Key.PermanentLoad.ToString("F3")} kN/m2 x {entry.Value.ToString("F3")}m";
                row.Cells[1].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Left);
                row.Cells[2].Paragraphs[0].Text = $"{(entry.Key.PermanentLoad * entry.Value).ToString("F3")} kN/m";
                row.Cells[2].Paragraphs[0].SetAlignment(DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right);
            }
        }

        private void WalkWall(AnalyticalWall wall, Dictionary<AreaBuildup, double> entries)
        {
            if (entries.ContainsKey(wall.Buildup))
            {
                entries[wall.Buildup] += wall.Height;
            }
            else
            {
                entries.Add(wall.Buildup, wall.Height);
            }

            //Walk the support chain
            foreach (AnalyticalElement e in wall.SupportedElements)
            {
                if (e is AnalyticalWall w)
                {
                    WalkWall(w, entries);
                }
                if (e is AnalyticalFloor f)
                {
                    double span = 0.5;
                    if (f.Orientation != wall.Orientation)
                    {
                        if (f.Orientation == Orientation.Horizontal)
                        {
                            span = (f.Right - f.Left) / 2;
                        }
                        else
                        {
                            span = (f.Top - f.Bottom) / 2;
                        }
                    }

                    if (entries.ContainsKey(f.Buildup))
                    {
                        entries[f.Buildup] += span;
                    }
                    else
                    {
                        entries.Add(f.Buildup, span);
                    }
                }
            }
        }
    }
}
