namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllFilters : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllFilters(Document doc, string wipeArgs = null)
        {
            // Удалить все фильтры
            Name = Language.GetItem(RevitCommand.LangItem, "w12");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var filters = new FilteredElementCollector(_doc)
                .OfClass(typeof(FilterElement))
                .WhereElementIsNotElementType()
                .ToElements();
            if (filters.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, filters);
            return 0;
        }
    }
}