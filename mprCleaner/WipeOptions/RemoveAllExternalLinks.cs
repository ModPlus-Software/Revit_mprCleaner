namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;
    using ModPlusAPI.Windows;

    internal class RemoveAllExternalLinks : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllExternalLinks(Document doc, string wipeArgs = null)
        {
            // Удалить все связанные файлы
            Name = Language.GetItem(RevitCommand.LangItem, "w11");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var filepath = _doc.PathName;
            if (filepath != null)
            {
                IList<Element> xRefLinks = new List<Element>();
                var modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filepath);
                var wasException = false;
                try
                {
                    var transData = TransmissionData.ReadTransmissionData(modelPath);
                    var externalReferences = transData.GetAllExternalFileReferenceIds();
                    foreach (var xRefId in externalReferences)
                    {
                        var externalReference = _doc.GetElement(xRefId);
                        if (ConfirmRemoval(externalReference))
                            xRefLinks.Add(externalReference);
                    }

                    HelperMethods.RemoveElements(Name, _doc, xRefLinks);
                }
                catch
                {
                    wasException = true;
                }

                try
                {
                    var linkTypesIds = new FilteredElementCollector(_doc).OfClass(typeof(RevitLinkType)).Select(l => l.Id).ToList();
                    
                    HelperMethods.RemoveElements(Name, _doc, linkTypesIds);
                }
                catch
                {
                    wasException = true;
                }

                return wasException ? 0 : 1;
            }

            // Модель должна быть сохранена для удаления внешних ссылок
            MessageBox.Show(Language.GetItem(RevitCommand.LangItem, "m1"), MessageBoxIcon.Alert);

            return 0;
        }

        private bool ConfirmRemoval(Element linkEl)
        {
            return linkEl is RevitLinkType || linkEl is CADLinkType;
        }
    }
}
