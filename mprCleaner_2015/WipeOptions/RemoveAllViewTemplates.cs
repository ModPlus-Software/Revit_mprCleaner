namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllViewTemplates : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllViewTemplates(Document doc, string wipeArgs = null)
        {
            // Удалить все шаблоны видов
            Name = Language.GetItem(RevitCommand.LangItem, "w40");
            WipeArgs = wipeArgs;
            _doc = doc;
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
            IList<Element> views = new FilteredElementCollector(_doc)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType()
                .Where(view => ConfirmRemoval(view as View))
                .ToList();
            if (views.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, views);
            return 0;

            bool ConfirmRemoval(View view)
            {
                if (view.GetType() == typeof(View)
                    || (view.GetType().IsSubclassOf(typeof(View))
                    && view.IsTemplate
                    && !readonlyViews.Contains(view.ViewType)))
                    return true;

                return false;
            }
        }
    }
}