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
            SharedParametersCleanerViewModel = new SharedParametersCleanerViewModel(uiApplication);
            ViewFiltersCleanerViewModel = new ViewFiltersCleanerViewModel(uiApplication);
            SchedulesCleanerViewModel = new SchedulesCleanerViewModel(uiApplication);
        }

        /// <summary>
        /// Общие функции очистки
        /// </summary>
        public GeneralWipeViewModel GeneralWipeViewModel { get; }

        /// <summary>
        /// Шаблоны видов
        /// </summary>
        public ViewTemplatesCleanerViewModel ViewTemplatesCleanerViewModel { get; }
        
        /// <summary>
        /// Неиспользуемые общие параметры
        /// </summary>
        public SharedParametersCleanerViewModel SharedParametersCleanerViewModel { get; }
        
        /// <summary>
        /// Фильтры видов
        /// </summary>
        public ViewFiltersCleanerViewModel ViewFiltersCleanerViewModel { get; }
        
        /// <summary>
        /// Спецификации
        /// </summary>
        public SchedulesCleanerViewModel SchedulesCleanerViewModel { get; }
    }
}
