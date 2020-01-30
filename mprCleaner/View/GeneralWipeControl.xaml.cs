namespace mprCleaner.View
{
    /// <summary>
    /// Логика взаимодействия для GeneralWipeControl.xaml
    /// </summary>
    public partial class GeneralWipeControl 
    {
        public GeneralWipeControl()
        {
            InitializeComponent();

            // change theme
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeStyleForResourceDictionary(Resources);

            // change lang
            ModPlusAPI.Language.SetLanguageProviderForResourceDictionary(Resources);
        }
    }
}
