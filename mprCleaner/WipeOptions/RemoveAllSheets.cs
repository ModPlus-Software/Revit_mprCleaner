﻿namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllSheets : WipeOption
    {
        private readonly UIDocument _uiDoc;
        private readonly IList<ElementId> openViews = new List<ElementId>();

        internal RemoveAllSheets(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все листы (кроме открытых)
            Name = Language.GetItem(RevitCommand.LangItem, "w27");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var openUIViews = _uiDoc.GetOpenUIViews();
            foreach (var ov in openUIViews)
                openViews.Add(ov.ViewId);
            IList<Element> sheets = new FilteredElementCollector(_uiDoc.Document).OfCategory(BuiltInCategory.OST_Sheets)
                .WhereElementIsNotElementType()
                .Where(sheet => ConfirmRemoval(sheet) == true)
                .ToList();
            if (sheets.Count > 0)
                return HelperMethods.RemoveElements(Name, _uiDoc.Document, sheets);
            else
                return 0;
        }

        private bool ConfirmRemoval(Element sheet)
        {
            return sheet is ViewSheet && !openViews.Contains(sheet.Id);
        }
    }
}
