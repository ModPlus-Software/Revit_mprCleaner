namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllImportedLine : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllImportedLine(Document doc, string wipeArgs = null)
        {
            // Удалить все импортированные образцы линий
            Name = Language.GetItem(RevitCommand.LangItem, "w14");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            IList<Element> lines = new FilteredElementCollector(_doc)
                .OfClass(typeof(LinePatternElement))
                .Where(line => ConfirmRemoval(line) == true)
                .ToList();
            if (lines.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, lines);
            else
                return 0;
            
        }

        private bool ConfirmRemoval(Element line)
        {
            return line.Name.ToLower().StartsWith("import")
                   || line.Name.ToLower().StartsWith("импорт");
        }
    }
}
