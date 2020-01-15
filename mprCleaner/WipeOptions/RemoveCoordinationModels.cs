namespace mprCleaner.WipeOptions
{
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveCoordinationModels : WipeOption
    {
        private readonly Document _doc;

        internal RemoveCoordinationModels(Document doc, string wipeArgs = null)
        {
            // Удалить все координационные модели
            Name = Language.GetItem(RevitCommand.LangItem, "w42");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
#if !R2015 && !R2016 && !R2017
            var ids = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Coordination_Model)
                .ToElementIds()
                .ToList();

            if (ids.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, ids);      
#endif
            return 0;
        }
    }
}
