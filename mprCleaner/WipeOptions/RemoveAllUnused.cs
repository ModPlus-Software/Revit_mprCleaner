namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Reflection;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllUnused : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllUnused(Document doc, string wipeArgs = null)
        {
            // Удалить все неиспользуемые элементы
            Name = Language.GetItem(RevitCommand.LangItem, "w38");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var getUnusedAppearances = _doc.GetType().GetMethod("GetUnusedAppearances", BindingFlags.NonPublic | BindingFlags.Instance);
            var getUnusedMaterials = _doc.GetType().GetMethod("GetUnusedMaterials", BindingFlags.NonPublic | BindingFlags.Instance);
            var getUnusedFamilies = _doc.GetType().GetMethod("GetUnusedFamilies", BindingFlags.NonPublic | BindingFlags.Instance);
            var getUnusedImportCategories = _doc.GetType().GetMethod("GetUnusedImportCategories", BindingFlags.NonPublic | BindingFlags.Instance);
            var getUnusedStructures = _doc.GetType().GetMethod("GetUnusedStructures", BindingFlags.NonPublic | BindingFlags.Instance);
            var getUnusedSymbols = _doc.GetType().GetMethod("GetUnusedSymbols", BindingFlags.NonPublic | BindingFlags.Instance);
            var getUnusedThermals = _doc.GetType().GetMethod("GetUnusedThermals", BindingFlags.NonPublic | BindingFlags.Instance);
            var getNonDeletableUnusedElements = _doc.GetType().GetMethod("GetNonDeletableUnusedElements", BindingFlags.NonPublic | BindingFlags.Instance);

            var num = 0;
            while (true)
            {
                var hashSet = new HashSet<ElementId>();

                var unusedAppearances = getUnusedAppearances?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current in unusedAppearances)
                {
                    hashSet.Add(current);
                }

                var unusedMaterials = getUnusedMaterials?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current2 in unusedMaterials)
                {
                    hashSet.Add(current2);
                }

                var unusedFamilies = getUnusedFamilies?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current3 in unusedFamilies)
                {
                    hashSet.Add(current3);
                }

                var unusedImportCategories = getUnusedImportCategories?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current4 in unusedImportCategories)
                {
                    hashSet.Add(current4);
                }

                var unusedStructures = getUnusedStructures?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current5 in unusedStructures)
                {
                    hashSet.Add(current5);
                }

                var unusedSymbols = getUnusedSymbols?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current6 in unusedSymbols)
                {
                    hashSet.Add(current6);
                }

                var unusedThermals = getUnusedThermals?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current7 in unusedThermals)
                {
                    hashSet.Add(current7);
                }

                var nonDeletableUnusedElements = getNonDeletableUnusedElements?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (var current8 in nonDeletableUnusedElements)
                {
                    hashSet.Remove(current8);
                }

                if (hashSet.Count != num && hashSet.Count != 0)
                {
                    num = hashSet.Count;
                    using (var transaction = new Transaction(_doc, "purge unused"))
                    {
                        transaction.Start();
                        _doc.Delete(hashSet);
                        transaction.Commit();
                        continue;
                    }
                }

                break;
            }

            return num;
        }
    }
}
