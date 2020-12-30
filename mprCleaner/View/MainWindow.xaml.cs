namespace mprCleaner.View
{
    using System.Threading;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// <see cref="CancellationTokenSource"/>
        /// </summary>
        public static CancellationTokenSource CancellationTokenSource;
        
        public MainWindow()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetFunctionLocalName(RevitCommand.LangItem, new ModPlusConnector().LName);
            CancellationTokenSource = new CancellationTokenSource();
            Closed += (sender, args) => CancellationTokenSource.Cancel();
        }
    }
}
