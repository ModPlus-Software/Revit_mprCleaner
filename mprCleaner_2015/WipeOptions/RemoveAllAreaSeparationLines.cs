namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllAreaSeparationLines : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllAreaSeparationLines(Document doc, string wipeArgs = null)
        {
            // Удалить все линии границ зон
            Name = Language.GetItem(RevitCommand.LangItem, "w3");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            var aLines = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_AreaSchemeLines)
                .WhereElementIsNotElementType()
                .ToElements();
            if (aLines.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, aLines);
            else
                return 0;
        }
    }
}
