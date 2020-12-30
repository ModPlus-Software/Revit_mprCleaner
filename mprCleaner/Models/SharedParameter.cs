namespace mprCleaner.Models
{
    using System;
    using Autodesk.Revit.DB;
    using ModPlusAPI.Mvvm;

    /// <summary>
    /// Общий параметр
    /// </summary>
    public class SharedParameter : VmBase
    {
        private bool _isChecked;
        
        public SharedParameter(SharedParameterElement sharedParameterElement)
        {
            OriginSharedParameterElement = sharedParameterElement;
            Name = sharedParameterElement.Name;
            Guid = sharedParameterElement.GuidValue;
        }
        
        /// <summary>
        /// Origin <see cref="SharedParameterElement"/>
        /// </summary>
        public SharedParameterElement OriginSharedParameterElement { get; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Guid
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Элемент отмечен
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }
    }
}
