namespace mprCleaner.View
{
    /// <summary>
    /// Логика взаимодействия для SharedParametersCleanerControl.xaml
    /// </summary>
    public partial class SharedParametersCleanerControl
    {
        public SharedParametersCleanerControl()
        {
            InitializeComponent();

            // change theme
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeStyleForResourceDictionary(Resources);

            // change lang
            ModPlusAPI.Language.SetLanguageProviderForResourceDictionary(Resources);
        }
    }
}
