using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RST_Invent.Commands;
using RST_Invent.Model;
using RST_Invent.Services;

namespace RST_Invent.ViewModel
{
    internal class AppViewModel : INotifyPropertyChanged
    {
        private readonly NotificationService _notificationService;
        private readonly NomenclatureService _nomenclatureService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Item> ReceiverGroups { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Item> ShipmentGroups { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Nomenclature> Nomenclatures { get; set; } = new ObservableCollection<Nomenclature>();

        public ICommand GetReceiverNum { get; }
        public ICommand GetShipmentNum { get; }
        public ICommand ClearData { get; }
        public ICommand AddNomenclatureCommand { get; }
        public ICommand DeleteNomenclatureCommand { get; }
        public ICommand SaveNomenclatureCommand { get; }

        private string _receiverInput;
        private string _shipmentInput;
        private string _nomenclatureId;
        private string _nomenclatureName;
        private bool _saveOnExit = true;

        public string ReceiverInput
        {
            get => _receiverInput;
            set { _receiverInput = value; OnPropertyChanged(); }
        }
        public string ShipmentInput
        {
            get => _shipmentInput;
            set { _shipmentInput = value; OnPropertyChanged(); }
        }
        public string NomenclatureId
        {
            get => _nomenclatureId;
            set { _nomenclatureId = value; OnPropertyChanged(); }
        }
        public string NomenclatureName
        {
            get => _nomenclatureName;
            set { _nomenclatureName = value; OnPropertyChanged(); }
        }
        public bool SaveOnExit
        {
            get => _saveOnExit;
            set { _saveOnExit = value; OnPropertyChanged(); }
        }

        public AppViewModel()
        {
            string userDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string nomenclatureFilePath = Path.Combine(userDataPath, "nomenclature.csv");
            _nomenclatureService = new NomenclatureService(nomenclatureFilePath, new ValidationService());
            _notificationService = new NotificationService();
            _nomenclatureService.LoadNomenclatures(Nomenclatures);

            var groupService = new GroupService(ReceiverGroups, ShipmentGroups, new ValidationService());
            groupService.OnGroupUpdated += (input) =>
            {
                if (input == ReceiverInput)
                    ReceiverInput = string.Empty;
                else if (input == ShipmentInput)
                    ShipmentInput = string.Empty;
            };

            GetReceiverNum = new RelayCommand(param => groupService.AddToGroup(ReceiverInput, ReceiverGroups, Nomenclatures));
            GetShipmentNum = new RelayCommand(param => groupService.AddToGroup(ShipmentInput, ShipmentGroups, Nomenclatures));
            ClearData = new RelayCommand(_ => groupService.ClearGroups());
            AddNomenclatureCommand = new RelayCommand(_ => TryAddNomenclature());
            SaveNomenclatureCommand = new RelayCommand(_ => _nomenclatureService.SaveNomenclatures(Nomenclatures));
            DeleteNomenclatureCommand = new RelayCommand(_ => TryDeleteNomenclature());
        }

        private void TryAddNomenclature()
        {
            if (_nomenclatureService.IsUniqueId(NomenclatureId, Nomenclatures))
            {
                _nomenclatureService.AddNomenclature(NomenclatureId, NomenclatureName, Nomenclatures);
            }
            else
            {
                _notificationService.ShowNotification("Ошибка: ID не уникален или введены некорректные данные.");
            }
        }

        private void TryDeleteNomenclature()
        {
            if (Nomenclatures.Any(n => n.Id == NomenclatureId))
            {
                _nomenclatureService.RemoveNomenclature(NomenclatureId, Nomenclatures);
            }
            else
            {
                _notificationService.ShowNotification($"Номенклатура с ID {NomenclatureId} не найдена.");
            }
        }

        public void SaveNomenclature()
        {
            _nomenclatureService.SaveNomenclatures(Nomenclatures);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
