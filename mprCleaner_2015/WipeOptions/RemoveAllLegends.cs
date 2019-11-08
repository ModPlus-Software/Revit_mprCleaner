namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllLegends : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllLegends(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только легенды)
            Name = Language.GetItem(RevitCommand.LangItem, "w15");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            var readonlyViews = new List<ViewType>()
            {
                ViewType.ProjectBrowser,
                ViewType.SystemBrowser,
                ViewType.Undefined,
                ViewType.DrawingSheet,
                ViewType.Internal
            };
            var openUiViews = _uiDoc.GetOpenUIViews();
            var openViews = new List<ElementId>();
            foreach (var ov in openUiViews)
                openViews.Add(ov.ViewId);
            IList<Element> legends = new FilteredElementCollector(_uiDoc.Document)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType()
                .Where(legend => ConfirmRemoval(legend as View))
                .ToList();
            if (legends.Count > 0)
                return HelperMethods.RemoveElements(Name, _uiDoc.Document, legends);
            return 0;

            bool ConfirmRemoval(View view)
            {
                if (view.GetType().Equals(typeof(View)) && view.ViewType == ViewType.Legend)
                {
                    if (readonlyViews.Contains(view.ViewType))
                        return false;
                    else if (view.IsTemplate)
                        return false;
                    else if (view.Name.Contains("<"))
                        return false;
                    else if (openViews.Contains(view.Id))
                        return false;
                    else
                        return true;
                }
                else if (view.GetType().Equals(typeof(ViewSchedule)) && (view as ViewSchedule).Definition.CategoryId.IntegerValue == (int)BuiltInCategory.OST_KeynoteTags)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}