namespace mprCleaner.ViewModels
{
    using Autodesk.Revit.UI;
    using ModPlusAPI.Mvvm;

    /// <summary>
    /// Main view model
    /// </summary>
    public class MainViewModel : VmBase
    {
        public MainViewModel(UIApplication uiApplication)
        {
            GeneralWipeViewModel = new GeneralWipeViewModel(uiApplication);
            ViewTemplatesCleanerViewModel = new ViewTemplatesCleanerViewModel(uiApplication);
        }

        public GeneralWipeViewModel GeneralWipeViewModel { get; }

        public ViewTemplatesCleanerViewModel ViewTemplatesCleanerViewModel { get; }
    }
}
