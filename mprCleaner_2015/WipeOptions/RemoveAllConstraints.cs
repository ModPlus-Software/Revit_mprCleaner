namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllConstraints : WipeOption
    {
        readonly Document _uiDoc;

        internal RemoveAllConstraints(UIApplication uiApp, string wipeArgs = null)
        {
            // Удалить все зависимости
            Name = Language.GetItem(RevitCommand.LangItem, "w4");
            WipeArgs = wipeArgs;
            _uiDoc = uiApp?.ActiveUIDocument.Document;
        }

        internal override int Execute(string args = null)
        {
            FilteredElementCollector cl = new FilteredElementCollector(_uiDoc);
            IList<Element> constraints = cl.OfCategory(BuiltInCategory.OST_Constraints)
                .WhereElementIsNotElementType()
                .Where(ConfirmRemoval)
                .ToList();
            if (constraints.Count > 0)
                return HelperMethods.RemoveElements(Name, _uiDoc, constraints);
            return 0;
        }

        private bool ConfirmRemoval(Element constraint)
        {
            return (constraint as Dimension)?.View is null;
        }

    }
}
