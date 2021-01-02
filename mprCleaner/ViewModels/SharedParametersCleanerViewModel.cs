namespace mprCleaner.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Models;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Windows;
    using View;

    /// <summary>
    /// Неиспользуемые общие параметры
    /// </summary>
    public class SharedParametersCleanerViewModel : VmBase
    {
        private readonly Document _doc;
        private bool _isSearchComplete;
        private string _progressText;
        private int _progressMaximum = 1;
        private int _progressValue;
        private bool _canSearch = true;

        public SharedParametersCleanerViewModel(UIApplication uIApplication)
        {
            _doc = uIApplication.ActiveUIDocument.Document;
            SharedParameters = new ObservableCollection<SharedParameter>();
        }

        /// <summary>
        /// Поиск был выполнен
        /// </summary>
        public bool IsSearchComplete
        {
            get => _isSearchComplete;
            set
            {
                if (_isSearchComplete == value)
                    return;
                _isSearchComplete = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Progress text
        /// </summary>
        public string ProgressText
        {
            get => _progressText;
            set
            {
                if (_progressText == value)
                    return;
                _progressText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Progress maximum value
        /// </summary>
        public int ProgressMaximum
        {
            get => _progressMaximum;
            set
            {
                if (_progressMaximum == value)
                    return;
                _progressMaximum = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Progress value
        /// </summary>
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value)
                    return;
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Can invoke clean command
        /// </summary>
        public bool CanClean => SharedParameters.Any(p => p.IsSelected);
        
        /// <summary>
        /// Количество выбранных
        /// </summary>
        public int SelectedCount => SharedParameters.Count(i => i.IsSelected);

        /// <summary>
        /// Can invoke search
        /// </summary>
        public bool CanSearch
        {
            get => _canSearch;
            set
            {
                if (_canSearch == value)
                    return;
                _canSearch = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Поиск неиспользуемых общих параметров
        /// </summary>
        public ICommand SearchUnusedSharedParametersCommand => new RelayCommandWithoutParameter(async () =>
        {
            try
            {
                if (_doc.IsFamilyDocument)
                {
                    // Работа в документе семейства не предусмотрена
                    MessageBox.Show(Language.GetItem("m2"));
                    return;
                }

                CanSearch = false;
                SharedParameters.Clear();

                var allSharedParameterElements = GetAllSharedParameterElements();
                var used = new List<Guid>();
                used.AddRange(await GetSharedParameterGuidsFromFamilies());

                await ClearProgress();

                // Получение данных из карты привязок документа
                ProgressText = Language.GetItem("m3");
                await Task.Delay(1);

                if (MainWindow.CancellationTokenSource.IsCancellationRequested)
                    return;
                
                used.AddRange(GetProjectSharedParamaterGuids());

                await ClearProgress();

                used = used.Distinct().ToList();

                foreach (var sharedParameterElement in allSharedParameterElements.OrderBy(p => p.Name))
                {
                    if (MainWindow.CancellationTokenSource.IsCancellationRequested)
                        break;
                    
                    if (used.Contains(sharedParameterElement.GuidValue))
                        continue;
                    var sharedParameter = new SharedParameter(sharedParameterElement);
                    sharedParameter.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(SharedParameter.IsSelected))
                        {
                            OnPropertyChanged(nameof(CanClean));
                            OnPropertyChanged(nameof(SelectedCount));
                        }
                    };

                    SharedParameters.Add(sharedParameter);
                }

                IsSearchComplete = true;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
            finally
            {
                CanSearch = true;
            }
        });
        
        /// <summary>
        /// Удаление отмеченных параметров
        /// </summary>
        public ICommand RemoveSelectedParametersCommand => new RelayCommandWithoutParameter(() =>
        {
            try
            {
                var elementIds = SharedParameters
                    .Where(p => p.IsSelected)
                    .Select(p => p.OriginSharedParameterElement.Id)
                    .ToList();

                if (elementIds.Any())
                {
                    // Удаление неиспользуемых общих параметров
                    using (var tr = new Transaction(_doc, Language.GetItem("h27")))
                    {
                        tr.Start();
                        _doc.Delete(elementIds);
                        tr.Commit();
                    }

                    // Удалено {0} общих параметров
                    MessageBox.Show(string.Format(Language.GetItem("h28"), elementIds.Count));
                    
                    IsSearchComplete = false;
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        });

        /// <summary>
        /// Неиспользуемые параметры
        /// </summary>
        public ObservableCollection<SharedParameter> SharedParameters { get; }

        private IEnumerable<SharedParameterElement> GetAllSharedParameterElements()
        {
            return new FilteredElementCollector(_doc)
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>();
        }

        private async Task<List<Guid>> GetSharedParameterGuidsFromFamilies()
        {
            var guids = new List<Guid>();

            var families = new FilteredElementCollector(_doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(f => f.IsEditable)
                .ToList();

            ProgressMaximum = families.Count;

            for (var index = 0; index < families.Count; index++)
            {
                if (MainWindow.CancellationTokenSource.IsCancellationRequested)
                    break;
                
                var family = families[index];
                
                // Чтение семейства
                ProgressText = $"{Language.GetItem("m4")} \"{family.Name}\"{Environment.NewLine}{index + 1}/{ProgressMaximum}";
                ProgressValue = index + 1;

                using (var doc = _doc.EditFamily(family))
                {
                    foreach (var familyParameter in doc.FamilyManager.Parameters.Cast<FamilyParameter>()
                        .Where(p => p.IsShared))
                    {
                        guids.Add(familyParameter.GUID);
                    }

                    try
                    {
                        doc.Close(false);
                    }
                    catch
                    {
                        // ignore
                    }

                    await Task.Delay(1);
                }
            }

            return guids;
        }

        private async Task ClearProgress()
        {
            ProgressText = string.Empty;
            ProgressMaximum = 1;
            ProgressValue = 0;
            await Task.Delay(1);
        }
        
        //// https://github.com/jeremytammik/the_building_coder_samples/blob/master/BuildingCoder/BuildingCoder/CmdProjectParameterGuids.cs

        private IEnumerable<Guid> GetProjectSharedParamaterGuids()
        {
            var projectInfoElement = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_ProjectInformation)
                .FirstElement();

            var firstWallTypeElement = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsElementType()
                .FirstElement();

            var projectParametersData = GetProjectParameterData().ToList();

            foreach (var projectParameterData in projectParametersData)
            {
                if (projectParameterData.Definition != null)
                {
                    var categories = projectParameterData.Binding.Categories;
                    Parameter foundParameter;
                    if (!categories.Contains(projectInfoElement.Category))
                    {
                        using (var tempTransaction = new Transaction(_doc))
                        {
                            tempTransaction.Start("Temporary");
                            
                            if (AddProjectParameterBinding(projectParameterData, projectInfoElement.Category))
                            {
                                foundParameter = projectInfoElement.get_Parameter(projectParameterData.Definition);

                                if (foundParameter == null)
                                {
                                    if (!categories.Contains(firstWallTypeElement.Category))
                                    {
                                        if (AddProjectParameterBinding(projectParameterData, firstWallTypeElement.Category))
                                        {
                                            foundParameter = firstWallTypeElement.get_Parameter(projectParameterData.Definition);
                                        }
                                    }
                                    else
                                    {
                                        foundParameter = firstWallTypeElement.get_Parameter(projectParameterData.Definition);
                                    }

                                    if (foundParameter != null && foundParameter.IsShared)
                                    {
                                        yield return foundParameter.GUID;
                                    }
                                }
                                else
                                {
                                    if (foundParameter.IsShared)
                                        yield return foundParameter.GUID;
                                }
                            }

                            tempTransaction.RollBack();
                        }
                    }
                    else
                    {
                        foundParameter = projectInfoElement.get_Parameter(projectParameterData.Definition);

                        if (foundParameter != null && foundParameter.IsShared)
                        {
                            yield return foundParameter.GUID;
                        }
                    }
                }
            }
        }

        private IEnumerable<ProjectParameterData> GetProjectParameterData()
        {
            var map = _doc.ParameterBindings;
            var it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                yield return new ProjectParameterData
                {
                    Definition = it.Key,
                    Binding = it.Current as ElementBinding
                };
            }
        }

        private bool AddProjectParameterBinding(ProjectParameterData projectParameterData, Category category)
        {
            var result = false;

            var cats = projectParameterData.Binding.Categories;

            if (cats.Contains(category))
            {
                var errorMessage =
                    $"The project parameter \"{projectParameterData.Definition.Name}\" is already bound to the \"{category.Name}\" category.";

                throw new Exception(errorMessage);
            }

            cats.Insert(category);
            
            if (projectParameterData.Binding is InstanceBinding)
            {
                var newInstanceBinding = _doc.Application.Create.NewInstanceBinding(cats);

                if (_doc.ParameterBindings.ReInsert(projectParameterData.Definition, newInstanceBinding))
                    result = true;
            }
            else
            {
                var typeBinding = _doc.Application.Create.NewTypeBinding(cats);
                if (_doc.ParameterBindings.ReInsert(projectParameterData.Definition, typeBinding)) 
                    result = true;
            }

            return result;
        }

        private class ProjectParameterData
        {
            public Definition Definition;
            
            public ElementBinding Binding;
        }
    }
}
