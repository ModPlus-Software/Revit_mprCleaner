namespace mprCleaner.Models
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;

    public class ViewTemplate : VmBase
    {
        private bool _isSelected;
        
        public ViewTemplate(View view, bool isUsed)
        {
            View = view;
            IsUsed = isUsed;
        }

        /// <summary>
        /// Instance of <see cref="Autodesk.Revit.DB.View"/>
        /// </summary>
        public View View { get; } 

        /// <summary>
        /// Is used in model
        /// </summary>
        public bool IsUsed { get; }

        /// <summary>
        /// <see cref="IsUsed"/> display variant
        /// </summary>
        public string IsUsedDisplay => IsUsed 
            ? Language.GetItem(RevitCommand.LangItem, "yes") 
            : Language.GetItem(RevitCommand.LangItem, "no");

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
