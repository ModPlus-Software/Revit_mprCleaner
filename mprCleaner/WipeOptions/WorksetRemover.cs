namespace mprCleaner.WipeOptions
{
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class WorksetRemover : WipeOption
    {
        private readonly Document _doc;

        internal WorksetRemover(Document doc, string wipeArgs)
        {
            Name = $@"{Language.GetItem(RevitCommand.LangItem, "w41")} ""{wipeArgs}""";
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args)
        {
            var worksetList = new FilteredWorksetCollector(_doc)
                .OfKind(WorksetKind.UserWorkset)
                .ToDictionary(wSet => wSet.Name, wSet => wSet.Id);
            ElementFilter elementWorksetFilter = new ElementWorksetFilter(worksetList[args]);
            var elementsOnWorkset = new FilteredElementCollector(_doc).WherePasses(elementWorksetFilter).ToElements();
            return HelperMethods.RemoveElements(Name, _doc, elementsOnWorkset);
        }
    }
}
