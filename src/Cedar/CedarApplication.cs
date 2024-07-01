using Autodesk.Revit.UI;
using JPP.Cedar.Piling;
using JPP.Cedar.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace JPP.Cedar
{
    public class CedarApplication : IExternalApplication
    {
        /// <summary>
        /// Entry point for application
        /// </summary>
        /// <param name="application">Application being launched</param>
        /// <returns>Result of add-in load</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            if (application == null)
                throw new System.ArgumentNullException(nameof(application));

            BuildStructureUI(application);

            PilingCoordinator.Register(application.ActiveAddInId);


            return Result.Succeeded;
        }

        private void BuildStructureUI(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Cedar");
            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            using PushButtonData saButtonData = new PushButtonData("cmdExportStructuralAnalysis", Resources.cmdExportStructuralAnalysis_Title, thisAssemblyPath, typeof(ExportStructuralAnalysisCommand).FullName);
            saButtonData.LargeImage = GetImage(Resources.Synchronize_Large);
            saButtonData.Image = GetImage(Resources.Synchronize);
            saButtonData.ToolTip = Resources.cmdExportStructuralAnalysis_Desc;
            PushButton saPushButton = ribbonPanel.AddItem(saButtonData) as PushButton;

            // Create two push buttons
            using PushButtonData button1 = new PushButtonData("cmdPreprocessModel", Resources.cmdPreprocessModel_Title, thisAssemblyPath, typeof(ExportStructuralAnalysisCommand).FullName);
            button1.Image = GetImage(Resources.Gear);
            button1.ToolTip = Resources.cmdPreprocessModel_Desc;
            using PushButtonData button2 = new PushButtonData("cmdExport", Resources.cmdExport_Title, thisAssemblyPath, typeof(ExportStructuralAnalysisCommand).FullName);
            button2.Image = GetImage(Resources.Synchronize);
            button2.ToolTip = Resources.cmdExport_Desc;

            using PushButtonData button3 = new PushButtonData("cmdUpdateFounds", Resources.cmdUpdateFounds_Title, thisAssemblyPath, typeof(ExportStructuralAnalysisCommand).FullName);
            button3.Image = GetImage(Resources.Spade);
            button3.ToolTip = Resources.cmdUpdateFounds_Desc;
            using PushButtonData button4 = new PushButtonData("cmdUpdateLintels", Resources.cmdUpdateLintels_Title, thisAssemblyPath, typeof(ExportStructuralAnalysisCommand).FullName);
            button4.Image = GetImage(Resources.Brick);
            button4.ToolTip = Resources.cmdUpdateLintels_Desc;


            // Add the buttons to the panel
            List<RibbonItem> projectButtons = new List<RibbonItem>();
            projectButtons.AddRange(ribbonPanel.AddStackedItems(button1, button2));

            List<RibbonItem> projectButtons2 = new List<RibbonItem>();
            projectButtons2.AddRange(ribbonPanel.AddStackedItems(button3, button4));
            projectButtons2[0].Enabled = false;
            projectButtons2[1].Enabled = false;
        }

        /// <summary>
        /// Unload all components on application shutdown
        /// </summary>
        /// <param name="application">Application shutting down</param>
        /// <returns>Result of add-in termination</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            PilingCoordinator.Unregister();
            return Result.Succeeded;
        }

        private BitmapImage GetImage(byte[] resource)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = new MemoryStream(resource);
            img.EndInit();
            return img;
        }
    }
}
