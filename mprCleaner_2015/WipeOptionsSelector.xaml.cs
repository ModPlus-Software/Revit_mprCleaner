namespace mprCleaner
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using ModPlusAPI;
    using ModPlusStyle.Controls.Dialogs;
    using WipeOptions;

    public partial class WipeOptionsSelector
    {
        public WipeOptionsSelector(List<WipeOption> wipeOptions)
        {
            InitializeComponent();
            LbWipeOptions.ItemsSource = wipeOptions;
            Title = ModPlusAPI.Language.GetFunctionLocalName(RevitCommand.LangItem, new ModPlusConnector().LName);
            ChkSkipFailures.IsChecked = 
                bool.TryParse(UserConfigFile.GetValue("mprCleaner", "SkipFailures"), out var b) && b; // false
        }

        private void BtAccept_OnClick(object sender, RoutedEventArgs e)
        {
            if (LbWipeOptions.Items.Cast<WipeOption>().Any(wo => wo.Visibility == Visibility.Visible && wo.State))
                DialogResult = true;
            else
            {
                // <h7>Вы ничего не выбрали</h7>
                this.ShowMessageAsync(ModPlusAPI.Language.GetItem(RevitCommand.LangItem, "h7"), string.Empty);
            }
        }

        private void SetStates(bool state = true, bool flip = false)
        {
            foreach (var item in LbWipeOptions.Items)
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

        private void BtCheckAll_OnClick(object sender, RoutedEventArgs e)
        {
            SetStates();
        }

        private void BtUncheckAll_OnClick(object sender, RoutedEventArgs e)
        {
            SetStates(false);
        }

        private void BtToggleAll_OnClick(object sender, RoutedEventArgs e)
        {
            SetStates(flip: true);
        }

        private void TbSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                foreach (var item in LbWipeOptions.Items)
                {
                    if (item is WipeOption wipeOption)
                    {
                        wipeOption.Visibility = wipeOption.Name.ToUpper().Contains(tb.Text.ToUpper()) 
                            ? Visibility.Visible 
                            : Visibility.Collapsed;
                    }
                }
            }
        }

        private void ChkSkipFailures_OnChecked(object sender, RoutedEventArgs e)
        {
            UserConfigFile.SetValue("mprCleaner", "SkipFailures", "True", true);
        }

        private void ChkSkipFailures_OnUnchecked(object sender, RoutedEventArgs e)
        {
            UserConfigFile.SetValue("mprCleaner", "SkipFailures", "False", true);
        }
    }
}
