namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllDraftingPatterns : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllDraftingPatterns(Document doc, string wipeArgs = null)
        {
            // Удалить все штриховки (условные)
            Name = Language.GetItem(RevitCommand.LangItem, "w7");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            IList<Element> patternElements = new FilteredElementCollector(_doc)
                .OfClass(typeof(FillPatternElement))
                .WhereElementIsNotElementType()
                .ToElements();
            List<Element> draftPatterns = new List<Element>();
            foreach (Element patternElement in patternElements)
            {
                if (((FillPatternElement)patternElement).GetFillPattern().Target == FillPatternTarget.Drafting)
                    draftPatterns.Add(patternElement);
            }
            if (draftPatterns.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, draftPatterns);
            return 0;
        }
    }
}