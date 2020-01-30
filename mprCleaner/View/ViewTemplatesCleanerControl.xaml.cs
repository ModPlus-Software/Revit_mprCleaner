namespace mprCleaner.View
{
    /// <summary>
    /// Логика взаимодействия для ViewTemplatesCleanerControl.xaml
    /// </summary>
    public partial class ViewTemplatesCleanerControl
    {
        public ViewTemplatesCleanerControl()
        {
            InitializeComponent();

            // change theme
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeStyleForResourceDictionary(Resources);

            // change lang
            ModPlusAPI.Language.SetLanguageProviderForResourceDictionary(Resources);
        }
    }
}
