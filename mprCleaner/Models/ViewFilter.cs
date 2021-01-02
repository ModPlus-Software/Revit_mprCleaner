namespace mprCleaner.Models
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    /// <summary>
    /// Фильтр вида
    /// </summary>
    public class ViewFilter : BaseSelectable
    {
        public ViewFilter(Element filterElement)
        {
            Name = filterElement.Name;
            Id = filterElement.Id;
            OwnerViews = new List<string>();
            OwnerViewTemplates = new List<string>();
        }

        /// <summary>
        /// Filter's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Id
        /// </summary>
        public ElementId Id { get; }

        /// <summary>
        /// Виды, в которых используется фильтр
        /// </summary>
        public List<string> OwnerViews { get; }

        /// <summary>
        /// Шаблоны видов, в которых используется фильтр
        /// </summary>
        public List<string> OwnerViewTemplates { get; }
    }
}
