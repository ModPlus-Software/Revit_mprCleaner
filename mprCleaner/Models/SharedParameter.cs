namespace mprCleaner.Models
{
    using System;
    using Autodesk.Revit.DB;

    /// <summary>
    /// Общий параметр
    /// </summary>
    public class SharedParameter : BaseSelectable
    {
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
    }
}
