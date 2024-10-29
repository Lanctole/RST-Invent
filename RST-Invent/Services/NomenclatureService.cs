using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using RST_Invent.Model;

namespace RST_Invent.Services
{
    internal class NomenclatureService
    {
        private readonly string _filePath;
        private readonly ValidationService _validationService;

        public NomenclatureService(string filePath, ValidationService validationService)
        {
            _filePath = filePath;
            _validationService = validationService;
        }

        public void AddNomenclature(string id, string name, ObservableCollection<Nomenclature> nomenclatures)
        {
            if (_validationService.IsValidNomenclature(id, name) && IsUniqueId(id, nomenclatures))
                nomenclatures.Add(new Nomenclature
                {
                    Id = _validationService.NormalizeInput(id),
                    Name = name
                });
        }

        public void RemoveNomenclature(string id, ObservableCollection<Nomenclature> nomenclatures)
        {
            var nomenclatureToRemove = nomenclatures.FirstOrDefault(n => n.Id == id);
            if (nomenclatureToRemove != null) nomenclatures.Remove(nomenclatureToRemove);
        }

        public void LoadNomenclatures(ObservableCollection<Nomenclature> nomenclatures)
        {
            if (!File.Exists(_filePath)) return;
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    if (values.Length == 2) nomenclatures.Add(new Nomenclature { Id = values[0], Name = values[1] });
                }
            }
        }

        public void SaveNomenclatures(ObservableCollection<Nomenclature> nomenclatures)
        {
            using (var writer = new StreamWriter(_filePath))
            {
                foreach (var item in nomenclatures) writer.WriteLine($"{item.Id},{item.Name}");
            }
        }

        public bool IsUniqueId(string id, ObservableCollection<Nomenclature> nomenclatures)
        {
            return nomenclatures.All(n => n.Id != id);
        }
    }
}