namespace mprCleaner.Models
{
    using ModPlusAPI.Mvvm;

    /// <summary>
    /// Базовый выбираемый объекта
    /// </summary>
    public abstract class BaseSelectable : VmBase
    {
        private bool _isSelected;
        
        /// <summary>
        /// Is selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
