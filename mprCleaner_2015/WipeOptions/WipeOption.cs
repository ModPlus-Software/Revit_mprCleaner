namespace mprCleaner.WipeOptions
{
    using System.Windows;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;

    public abstract class WipeOption : VmBase
    {
        private bool _state;

        public string Name { get; set; }

        public string WipeArgs { get; set; }

        private Visibility _visibility;

        /// <summary></summary>
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (Equals(value, _visibility)) return;
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

        internal WipeOption()
        {
            State = false;
        }

        public virtual string Report()
        {
            return $"{Name} - {Execute(WipeArgs)} {Language.GetItem(RevitCommand.LangItem, "h8")}";
        }

        internal abstract int Execute(string args = null);
    }
}
