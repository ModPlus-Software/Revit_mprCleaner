namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllMaterials : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllMaterials(Document doc, string wipeArgs = null)
        {
            // Удалить все материалы
            Name = Language.GetItem(RevitCommand.LangItem, "w16");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            IList<Element> mats = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Materials)
                .Where(mat => ConfirmRemoval(mat) == true)
                .ToList();
            if (mats.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, mats);
            else
                return 0;
        }

        private bool ConfirmRemoval(Element mat)
        {
            return !mat.Name.ToLower().Contains("poche");
        }
    }
}
