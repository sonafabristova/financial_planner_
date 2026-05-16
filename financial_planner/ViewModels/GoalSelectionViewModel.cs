using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class GoalSelectionViewModel : ViewModelBase
    {
        private ObservableCollection<Goal> _goals;
        private Goal _selectedGoal;

        public ObservableCollection<Goal> Goals
        {
            get => _goals;
            set => SetProperty(ref _goals, value);
        }

        public Goal SelectedGoal
        {
            get => _selectedGoal;
            set
            {
                SetProperty(ref _selectedGoal, value);
                OnPropertyChanged(nameof(CanSelect));
            }
        }

        public bool CanSelect => SelectedGoal != null;

        public ICommand SelectCommand { get; }
        public ICommand CancelCommand { get; }

        public GoalSelectionViewModel()
        {
            Goals = new ObservableCollection<Goal>();

            SelectCommand = new RelayCommand(ExecuteSelect, CanExecuteSelect);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        public void SetGoals(System.Collections.Generic.List<Goal> goals)
        {
            Goals = new ObservableCollection<Goal>(goals);
        }

        private bool CanExecuteSelect(object parameter) => CanSelect;

        private void ExecuteSelect(object parameter)
        {
            (parameter as Window)?.Close();
        }

        private void ExecuteCancel(object parameter)
        {
            SelectedGoal = null;
            (parameter as Window)?.Close();
        }
    }
}