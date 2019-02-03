namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllScopeBoxes : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllScopeBoxes(Document doc, string wipeArgs = null)
        {
            // Удалить все области видимости
            Name = Language.GetItem(RevitCommand.LangItem, "w25");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            IList<Element> scBoxes = new FilteredElementCollector(_doc)
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
