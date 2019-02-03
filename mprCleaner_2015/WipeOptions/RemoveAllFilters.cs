namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllFilters : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllFilters(Document doc, string wipeArgs = null)
        {
            // Удалить все фильтры
            Name = Language.GetItem(RevitCommand.LangItem, "w12");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            IList<Element> filters = new FilteredElementCollector(_doc)
                .OfClass(typeof(FilterElement))
                .WhereElementIsNotElementType()
                .ToElements();
            if (filters.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, filters);
            return 0;
        }
    }
}