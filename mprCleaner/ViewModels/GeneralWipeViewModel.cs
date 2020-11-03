namespace mprCleaner.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Events;
    using Autodesk.Revit.UI;
    using ModPlusAPI;
    using ModPlusAPI.Enums;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Services;
    using ModPlusStyle.Controls.Dialogs;
    using WipeOptions;
    using Visibility = System.Windows.Visibility;

    public class GeneralWipeViewModel : VmBase
    {
        private readonly UIApplication _uiApplication;
        private bool _skipFailures;
        private string _searchString;

        public GeneralWipeViewModel(UIApplication uiApplication)
        {
            _uiApplication = uiApplication;

            WipeOptions = new ObservableCollection<WipeOption>(new WipeOptionFactory().GetWipeOptions(uiApplication));
            foreach (var wipeOption in WipeOptions)
            {
                wipeOption.LoadStateStatusFromSettings();
            }

            SkipFailures = bool.TryParse(
                UserConfigFile.GetValue(RevitCommand.LangItem, nameof(SkipFailures)), out var b) && b; // false
        }

        public ObservableCollection<WipeOption> WipeOptions { get; }

        /// <summary>
        /// Skip failures
        /// </summary>
        public bool SkipFailures
        {
            get => _skipFailures;
            set
            {
                if (_skipFailures == value)
                    return;
                _skipFailures = value;
                OnPropertyChanged();
                UserConfigFile.SetValue(RevitCommand.LangItem, nameof(SkipFailures), value.ToString(), true);
            }
        }

        /// <summary>
        /// Search string
        /// </summary>
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString == value)
                    return;
                _searchString = value;
                OnPropertyChanged();
                Search();
            }
        }

        /// <summary>
        /// Check all
        /// </summary>
        public ICommand CheckAllCommand => new RelayCommandWithoutParameter(() => SetStates());

        /// <summary>
        /// Uncheck all
        /// </summary>
        public ICommand UncheckAllCommand => new RelayCommandWithoutParameter(() => SetStates(false));

        /// <summary>
        /// Toggle selection
        /// </summary>
        public ICommand ToggleSelectionCommand => new RelayCommandWithoutParameter(() => SetStates(flip: true));

        /// <summary>
        /// Accept
        /// </summary>
        public ICommand AcceptCommand => new RelayCommandWithoutParameter(Accept);

        private void SetStates(bool state = true, bool flip = false)
        {
            foreach (var item in WipeOptions)
            {
                if (item is WipeOption wipeOpt && wipeOpt.Visibility == Visibility.Visible)
                {
                    if (flip)
                        wipeOpt.State = !wipeOpt.State;
                    else
                        wipeOpt.State = state;
                }
            }
        }

        private void Accept()
        {
            var wipeOptions = WipeOptions.Where(wo => wo.Visibility == Visibility.Visible && wo.State).ToList();
            if (!wipeOptions.Any())
            {
                // <h7>Вы ничего не выбрали</h7>
                RevitCommand.MainWindowInstance.ShowMessageAsync(Language.GetItem(RevitCommand.LangItem, "h7"), string.Empty);
                return;
            }

            RevitCommand.MainWindowInstance.Close();

            foreach (var wipeOption in WipeOptions)
            {
                wipeOption.SaveSateStatusToSettings(false);
            }

            UserConfigFile.SaveConfigFile();

            var resultService = new ResultService();
            wipeOptions.Reverse();
            var doc = _uiApplication.ActiveUIDocument.Document;
            if (SkipFailures)
                _uiApplication.Application.FailuresProcessing += ApplicationOnFailuresProcessing;
            using (var transactionGroup = new TransactionGroup(doc))
            {
                transactionGroup.Start(Language.GetFunctionLocalName(
                    RevitCommand.LangItem,
                    new ModPlusConnector().LName));

                foreach (var wipeOption in wipeOptions)
                {
                    if (wipeOption.State && wipeOption.Visibility == Visibility.Visible)
                    {
                        resultService.Add(wipeOption.Report(), null, ResultItemType.Info);
                    }
                }

                transactionGroup.Assimilate();
            }

            if (SkipFailures)
                _uiApplication.Application.FailuresProcessing -= ApplicationOnFailuresProcessing;

            resultService.ShowWithoutGrouping();
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

        private void Search()
        {
            foreach (var item in WipeOptions)
            {
                if (item is WipeOption wipeOption)
                {
                    wipeOption.Visibility = wipeOption.Name.ToUpper().Contains(SearchString.ToUpper())
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }
}
