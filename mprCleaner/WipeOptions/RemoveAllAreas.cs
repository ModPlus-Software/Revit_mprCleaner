namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllAreas : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllAreas(Document doc, string wipeArgs = null)
        {
            // Удалить все зоны
            Name = Language.GetItem(RevitCommand.LangItem, "w2");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var areas = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType()
                .ToElements();
            if (areas.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, areas);
            else
                return 0;
        }
    }
}
