﻿using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Jpp.Cedar.Core;
using Jpp.Cedar.Piling;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Jpp.Cedar
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
#if !DEBUG
            AppCenter.Start("a0e1a19a-93dc-4e1e-bf4a-a8dfb1556a77", typeof(Analytics), typeof(Crashes));
#endif

            if (application == null)
                throw new System.ArgumentNullException(nameof(application));

#if DEBUG
            //TODO: Remove once full UI is in place. Here for debug purposes to show addin is loaded
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("NewRibbonPanel");
            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData("cmdHelloWorld",
                "Hello World", thisAssemblyPath, "Walkthrough.HelloWorld");

            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;
            pushButton.ToolTip = "Say hello to the entire world.";
#endif
            application.ControlledApplication.ApplicationInitialized += ControlledApplication_ApplicationInitialized;


            Analytics.TrackEvent("Addin Started");
            return Result.Succeeded;
        }

        /// <summary>
        /// Initialize components once the application has loaded
        /// </summary>
        /// <param name="sender">Application</param>
        /// <param name="e">Event args</param>
        private void ControlledApplication_ApplicationInitialized(object sender, ApplicationInitializedEventArgs e)
        {
            Application application = (Application) sender;

            //Initialize shared components
            ISharedParameterManager sharedParameterManager = new SharedParameterManager(application);

            //Register updaters
            PilingUpdater.Register(application, sharedParameterManager);
            Analytics.TrackEvent("Addin Initialized");
        }

        /// <summary>
        /// Unload all components on application shutdown
        /// </summary>
        /// <param name="application">Application shutting down</param>
        /// <returns>Result of add-in termination</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            Analytics.TrackEvent("Addin Stopped");
            return Result.Succeeded;
        }
    }
}
