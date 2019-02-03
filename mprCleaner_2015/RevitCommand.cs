namespace mprCleaner
{
    using System;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Events;
    using Autodesk.Revit.UI;
    using ModPlusAPI.Windows;
    using WipeOptions;
    using Visibility = System.Windows.Visibility;

    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class RevitCommand : IExternalCommand
    {
        public static string LangItem = "mprCleaner";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ModPlusAPI.Statistic.SendCommandStarting(new ModPlusConnector());

            try
            {
                var wipeOptions = new WipeOptionFactory().GetWipeOptions(commandData.Application);
                WipeOptionsSelector selector = new WipeOptionsSelector(wipeOptions);
                if (selector.ShowDialog() == true)
                {
                    var skipFailures = selector.ChkSkipFailures.IsChecked != null && 
                                       selector.ChkSkipFailures.IsChecked.Value;
                    var report = string.Empty;
                    wipeOptions.Reverse();
                    var doc = commandData.Application.ActiveUIDocument.Document;
                    if (skipFailures)
                        commandData.Application.Application.FailuresProcessing += ApplicationOnFailuresProcessing;
                    using (TransactionGroup transactionGroup = new TransactionGroup(doc))
                    {
                        transactionGroup.Start(ModPlusAPI.Language.GetFunctionLocalName(LangItem, new ModPlusConnector().LName));

                        foreach (var wipeOption in wipeOptions)
                        {
                            if (wipeOption.State && wipeOption.Visibility == Visibility.Visible)
                            {
                                report += "\n" + wipeOption.Report();
                            }
                        }

                        transactionGroup.Assimilate();
                    }
                    if (skipFailures)
                        commandData.Application.Application.FailuresProcessing -= ApplicationOnFailuresProcessing;

                    ResultWindow.Show(report.TrimStart("\n".ToCharArray()));
                }

                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
                commandData.Application.Application.FailuresProcessing += ApplicationOnFailuresProcessing;
                return Result.Failed;
            }
        }

        private void ApplicationOnFailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            // Inside event handler, get all warnings
            var failList = e.GetFailuresAccessor().GetFailureMessages();
            if (failList.Any())
            {
                // skip all failures
                e.GetFailuresAccessor().DeleteAllWarnings();
                e.SetProcessingResult(FailureProcessingResult.Continue);
            }
        }
    }
}
