using System;
using System.Reflection;
using Autodesk.Revit.UI;
using Jpp.Cedar.Piling;

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
            if(application == null)
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
            Console.WriteLine("Debug");
#endif

            PilingCoordinator.Register(application.ActiveAddInId);


            return Result.Succeeded;
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
    }
}
