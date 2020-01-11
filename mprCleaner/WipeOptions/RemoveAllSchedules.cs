namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllSchedules : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllSchedules(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только спецификации)
            Name = Language.GetItem(RevitCommand.LangItem, "w24");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
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
            IList<Element> schedules = new FilteredElementCollector(_uiDoc.Document)
                .OfClass(typeof(ViewSchedule))
                .WhereElementIsNotElementType()
                .Where(sch => ConfirmRemoval(sch as View))
                .ToList();
            if (schedules.Count > 0)
                return HelperMethods.RemoveElements(Name, _uiDoc.Document, schedules);
            return 0;

            bool ConfirmRemoval(View view)
            {
                if (view.GetType() == typeof(ViewSchedule))
                {
                    if (readonlyViews.Contains(view.ViewType))
                        return false;
                    if (view.IsTemplate)
                        return false;
                    if (view.Name.Contains("<"))
                        return false;
                    if (openViews.Contains(view.Id))
                        return false;
                    if (((ViewSchedule)view).Definition.CategoryId.IntegerValue == (int)BuiltInCategory.OST_KeynoteTags)
                        return false;
                    return true;
                }

                return false;
            }
        }
    }
}