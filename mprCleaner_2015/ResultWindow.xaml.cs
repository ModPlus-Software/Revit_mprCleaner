namespace mprCleaner
{
    public partial class ResultWindow 
    {
        public ResultWindow(string message)
        {
            InitializeComponent();
            TbResultMessage.Text = message;
            Title = ModPlusAPI.Language.GetItem(RevitCommand.LangItem, "h1");
        }

        public static void Show(string message)
        {
            ResultWindow window = new ResultWindow(message);
            window.ShowDialog();
        }
    }
}
