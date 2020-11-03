namespace mprCleaner.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Models;
    using ModPlusAPI;
    using ModPlusAPI.Enums;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Services;
    using ModPlusStyle.Controls.Dialogs;
    using View;

    public class ViewTemplatesCleanerViewModel : VmBase
    {
        private readonly UIApplication _uIApplication;
        private List<View> _views;
        private int _onUsedTemplate;
        private List<int> _usedTemplatesIds;

        public ViewTemplatesCleanerViewModel(UIApplication uIApplication)
        {
            _uIApplication = uIApplication;
            var doc = uIApplication.ActiveUIDocument.Document;

            _views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Where(e => e.Id != ElementId.InvalidElementId)
                .Cast<View>()
                .ToList();
            var templates = _views.Where(v => v.IsTemplate);
            _usedTemplatesIds = _views
                .Where(v => !v.IsTemplate && v.ViewTemplateId != ElementId.InvalidElementId)
                .Select(v => v.ViewTemplateId.IntegerValue)
                .Distinct()
                .ToList();

            var viewTemplates = templates
                .Select(view => new ViewTemplate(view, _usedTemplatesIds.Contains(view.Id.IntegerValue)));

            ViewTemplates = 
                new ObservableCollection<ViewTemplate>(viewTemplates.OrderBy(v => v.View.Name, new ModPlusAPI.IO.OrdinalStringComparer()));

            OnUsedTemplate =
                int.TryParse(UserConfigFile.GetValue(RevitCommand.LangItem, nameof(OnUsedTemplate)), out var i) ? i : 0;
        }

        public ObservableCollection<ViewTemplate> ViewTemplates { get; }

        /// <summary>
        /// 0 - skip, 1 - remove reference
        /// </summary>
        public int OnUsedTemplate
        {
            get => _onUsedTemplate;
            set
            {
                if (_onUsedTemplate == value)
                    return;
                _onUsedTemplate = value;
                OnPropertyChanged();
                UserConfigFile.SetValue(RevitCommand.LangItem, nameof(OnUsedTemplate), value.ToString(), true);
            }
        }

        /// <summary>
        /// Accept
        /// </summary>
        public ICommand AcceptCommand => new RelayCommandWithoutParameter(Accept);

        private void Accept()
        {
            var selectedTemplates = ViewTemplates.Where(t => t.IsSelected).ToList();
            if (!selectedTemplates.Any())
            {
                // <h7>Вы ничего не выбрали</h7>
                RevitCommand.MainWindowInstance.ShowMessageAsync(Language.GetItem(RevitCommand.LangItem, "h7"), string.Empty);
                return;
            }

            RevitCommand.MainWindowInstance.Close();

            var doc = _uIApplication.ActiveUIDocument.Document;
            var transactionName = Language.GetItem(RevitCommand.LangItem, "h18");
            if (string.IsNullOrEmpty(transactionName))
                transactionName = "Remove view templates";

            var skipped = 0;
            var removedReferences = 0;
            var idsToRemove = new List<ElementId>();

            using (Transaction transaction = new Transaction(doc, transactionName))
            {
                transaction.Start();

                foreach (var viewTemplate in selectedTemplates)
                {
                    if (_usedTemplatesIds.Contains(viewTemplate.View.Id.IntegerValue))
                    {
                        if (OnUsedTemplate == 0)
                        {
                            skipped++;
                            continue;
                        }

                        foreach (var view in _views)
                        {
                            if (view.ViewTemplateId.Equals(viewTemplate.View.Id))
                                view.ViewTemplateId = ElementId.InvalidElementId;
                        }

                        removedReferences++;
                    }

                    idsToRemove.Add(viewTemplate.View.Id);
                }

                doc.Delete(idsToRemove);

                transaction.Commit();
            }

            var resultService = new ResultService();
            if (skipped > 0)
                resultService.Add($"{Language.GetItem(RevitCommand.LangItem, "h19")} {skipped}", null, ResultItemType.Info);
            if (removedReferences > 0)
                resultService.Add($"{Language.GetItem(RevitCommand.LangItem, "h21")} {removedReferences}", null, ResultItemType.Info);
            if (idsToRemove.Count > 0)
                resultService.Add($"{Language.GetItem(RevitCommand.LangItem, "h20")} {idsToRemove.Count}", null, ResultItemType.Info);

            resultService.ShowWithoutGrouping();
        }
    }
}
