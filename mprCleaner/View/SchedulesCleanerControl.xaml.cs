namespace mprCleaner.View
{
    /// <summary>
    /// Логика взаимодействия для SchedulesCleanerControl.xaml
    /// </summary>
    public partial class SchedulesCleanerControl
    {
        public SchedulesCleanerControl()
        {
            InitializeComponent();
            
            // change theme
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeStyleForResourceDictionary(Resources);

            // change lang
            ModPlusAPI.Language.SetLanguageProviderForResourceDictionary(Resources);
        }
    }
}
