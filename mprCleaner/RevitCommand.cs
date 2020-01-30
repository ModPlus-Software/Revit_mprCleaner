namespace mprCleaner
{
    using System;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;
    using ModPlusAPI.Windows;
    using View;
    using ViewModels;

    /// <summary>
    /// Main command
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class RevitCommand : IExternalCommand
    {
        /// <summary>
        /// Localization lang item
        /// </summary>
        public static string LangItem = "mprCleaner";

        /// <summary>
        /// Main window static instance
        /// </summary>
        public static MainWindow MainWindowInstance { get; private set; }

        /// <inheritdoc />
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Statistic.SendCommandStarting(new ModPlusConnector());

            try
            {
                var mainViewModel = new MainViewModel(commandData.Application);
                MainWindowInstance = new MainWindow
                {
                    DataContext = mainViewModel
                };
                MainWindowInstance.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
                return Result.Failed;
            }
        }
    }
}
