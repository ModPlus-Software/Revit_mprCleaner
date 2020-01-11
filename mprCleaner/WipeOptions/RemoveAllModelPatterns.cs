namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllModelPatterns : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllModelPatterns(Document doc, string wipeArgs = null)
        {
            // Удалить все штриховки (моделирующие)
            Name = Language.GetItem(RevitCommand.LangItem, "w17");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var patternElements = new FilteredElementCollector(_doc)
                .OfClass(typeof(FillPatternElement))
                .WhereElementIsNotElementType()
                .ToElements();
            var modelPatterns = new List<Element>();
            foreach (var patternElement in patternElements)
            {
                if (((FillPatternElement)patternElement).GetFillPattern().Target == FillPatternTarget.Model)
                    modelPatterns.Add(patternElement);
            }

            if (modelPatterns.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, modelPatterns);
            return 0;
        }
    }
}