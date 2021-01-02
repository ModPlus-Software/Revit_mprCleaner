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

    public class SchedulesCleanerViewModel : VmBase
    {
        private readonly Document _doc;
        
        public SchedulesCleanerViewModel(UIApplication uiApplication)
        {
            _doc = uiApplication.ActiveUIDocument.Document;
            CollectSchedules();
        }
        
        /// <summary>
        /// Спецификации
        /// </summary>
        public ObservableCollection<Schedule> Schedules { get; private set; }
        
        /// <summary>
        /// Can invoke clean command
        /// </summary>
        public bool CanClean => Schedules.Any(p => p.IsSelected);

        /// <summary>
        /// Количество выбранных
        /// </summary>
        public int SelectedCount => Schedules.Count(i => i.IsSelected);

        /// <summary>
        /// Удалить выбранные спецификации
        /// </summary>
        public ICommand RemoveSelectedSchedulesCommand => new RelayCommandWithoutParameter(() =>
        {
            try
            {
                var elementIds = Schedules
                    .Where(f => f.IsSelected)
                    .Select(f => f.Id)
                    .ToList();

                if (elementIds.Any())
                {
                    // Удаление фильтров видов
                    using (var tr = new Transaction(_doc, Language.GetItem("h34")))
                    {
                        tr.Start();
                        _doc.Delete(elementIds);
                        tr.Commit();
                    }

                    CollectSchedules();
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        });

        private void CollectSchedules()
        {
            try
            {
                var schedules = new List<Schedule>();
                
                foreach (var viewSchedule in new FilteredElementCollector(_doc)
                    .OfClass(typeof(ViewSchedule))
                    .Cast<ViewSchedule>())
                {
                    var schedule = new Schedule(viewSchedule);
                    schedule.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(Schedule.IsSelected))
                        {
                            OnPropertyChanged(nameof(CanClean));
                            OnPropertyChanged(nameof(SelectedCount));
                        }
                    };
                    
                    schedules.Add(schedule);
                }
                
                foreach (var scheduleSheetInstance in new FilteredElementCollector(_doc)
                    .OfClass(typeof(ScheduleSheetInstance))
                    .Cast<ScheduleSheetInstance>())
                {
                    var schedule = schedules.FirstOrDefault(s =>
                        s.Id.IntegerValue == scheduleSheetInstance.ScheduleId.IntegerValue);
                    if (schedule != null && scheduleSheetInstance.OwnerViewId != ElementId.InvalidElementId &&
                        _doc.GetElement(scheduleSheetInstance.OwnerViewId) is ViewSheet sheet)
                    {
                        schedule.PlacedOnSheets.Add($"{sheet.SheetNumber} - {sheet.Name}");
                    }
                }

                Schedules = new ObservableCollection<Schedule>(
                    schedules.OrderBy(s => s.Name, new OrdinalStringComparer()));
                
                OnPropertyChanged(nameof(Schedules));
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
