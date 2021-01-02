namespace mprCleaner.View
{
    /// <summary>
    /// Логика взаимодействия для ViewFiltersCleanerControl.xaml
    /// </summary>
    public partial class ViewFiltersCleanerControl
    {
        public ViewFiltersCleanerControl()
        {
            InitializeComponent();
            
            // change theme
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeStyleForResourceDictionary(Resources);

            // change lang
            ModPlusAPI.Language.SetLanguageProviderForResourceDictionary(Resources);
        }
    }
}
