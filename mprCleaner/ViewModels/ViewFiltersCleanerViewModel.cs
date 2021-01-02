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
    using ModPlusAPI.IO;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Windows;

    /// <summary>
    /// Удаление фильтров видов и шаблонов видов
    /// </summary>
    public class ViewFiltersCleanerViewModel : VmBase
    {
        private readonly Document _doc;

        public ViewFiltersCleanerViewModel(UIApplication uiApplication)
        {
            _doc = uiApplication.ActiveUIDocument.Document;
            CollectFilters();
        }

        /// <summary>
        /// Filters
        /// </summary>
        public ObservableCollection<ViewFilter> ViewFilters { get; private set; }

        /// <summary>
        /// Can invoke clean command
        /// </summary>
        public bool CanClean => ViewFilters.Any(p => p.IsSelected);

        /// <summary>
        /// Количество выбранных
        /// </summary>
        public int SelectedCount => ViewFilters.Count(i => i.IsSelected);

        /// <summary>
        /// Удаление выбранные фильтры
        /// </summary>
        public ICommand RemoveSelectedFiltersCommand => new RelayCommandWithoutParameter(() =>
        {
            try
            {
                var elementIds = ViewFilters
                    .Where(f => f.IsSelected)
                    .Select(f => f.Id)
                    .ToList();

                if (elementIds.Any())
                {
                    // Удаление фильтров видов
                    using (var tr = new Transaction(_doc, Language.GetItem("h30")))
                    {
                        tr.Start();
                        _doc.Delete(elementIds);
                        tr.Commit();
                    }

                    CollectFilters();
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        });

        private void CollectFilters()
        {
            try
            {
                var viewFilters = new List<ViewFilter>();

                foreach (var view in new FilteredElementCollector(_doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v => v.AreGraphicsOverridesAllowed()))
                {
                    foreach (var elementId in view.GetFilters().Where(e => e != ElementId.InvalidElementId))
                    {
                        var filterElement = _doc.GetElement(elementId);
                        var existFilter =
                            viewFilters.FirstOrDefault(f => f.Id.IntegerValue == filterElement.Id.IntegerValue);
                        if (existFilter != null)
                        {
                            if (view.IsTemplate)
                                existFilter.OwnerViewTemplates.Add(view.Name);
                            else
                                existFilter.OwnerViews.Add(view.Name);
                        }
                        else
                        {
                            var newFilter = new ViewFilter(filterElement);
                            if (view.IsTemplate)
                                newFilter.OwnerViewTemplates.Add(view.Name);
                            else
                                newFilter.OwnerViews.Add(view.Name);

                            newFilter.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == nameof(ViewFilter.IsSelected))
                                {
                                    OnPropertyChanged(nameof(CanClean));
                                    OnPropertyChanged(nameof(SelectedCount));
                                }
                            };

                            viewFilters.Add(newFilter);
                        }
                    }
                }
                
                ViewFilters = new ObservableCollection<ViewFilter>(
                    viewFilters.OrderBy(v => v.Name, new OrdinalStringComparer()));
                OnPropertyChanged(nameof(ViewFilters));
                OnPropertyChanged(nameof(CanClean));
                OnPropertyChanged(nameof(SelectedCount));
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }
    }
}
