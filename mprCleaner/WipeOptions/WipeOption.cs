namespace mprCleaner.WipeOptions
{
    using System.Windows;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;

    public abstract class WipeOption : VmBase
    {
        private bool _state;
        private Visibility _visibility;
        
        internal WipeOption()
        {
            State = false;
        }

        public string Name { get; set; }

        public string WipeArgs { get; set; }

        /// <summary></summary>
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (Equals(value, _visibility)) 
                    return;
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Load <see cref="State"/> status form user config file
        /// </summary>
        public virtual void LoadStateStatusFromSettings()
        {
            if (bool.TryParse(UserConfigFile.GetValue("mprCleaner", GetType().Name), out var b))
                State = b;
        }
        
        /// <summary>
        /// Save <see cref="State"/> status to user config file
        /// </summary>
        /// <param name="saveFile">Save user config file immediately</param>
        public virtual void SaveSateStatusToSettings(bool saveFile)
        {
            UserConfigFile.SetValue("mprCleaner", GetType().Name, State.ToString(), saveFile);
        }

        public virtual string Report()
        {
            return $"{Name} - {Execute(WipeArgs)} {Language.GetItem(RevitCommand.LangItem, "h8")}";
        }

        internal abstract int Execute(string args = null);
    }
}
