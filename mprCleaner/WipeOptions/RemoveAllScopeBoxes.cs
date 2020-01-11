namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllScopeBoxes : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllScopeBoxes(Document doc, string wipeArgs = null)
        {
            // Удалить все области видимости
            Name = Language.GetItem(RevitCommand.LangItem, "w25");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var scBoxes = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_VolumeOfInterest)
                .WhereElementIsNotElementType()
                .ToElements();
            if (scBoxes.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, scBoxes);
            else
                return 0;
        }
    }
}
