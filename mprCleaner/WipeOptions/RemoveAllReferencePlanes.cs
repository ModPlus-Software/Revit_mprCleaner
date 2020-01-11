namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllReferencePlanes : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllReferencePlanes(Document doc, string wipeArgs = null)
        {
            // Удалить все опорные плоскости
            Name = Language.GetItem(RevitCommand.LangItem, "w20");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var refPlanes = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_CLines)
                .WhereElementIsNotElementType()
                .ToElements();
            if (refPlanes.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, refPlanes);
            else
                return 0;
        }
    }
}
