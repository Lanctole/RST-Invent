using System;
using System.Collections.ObjectModel;
using System.Linq;
using RST_Invent.Model;

namespace RST_Invent.Services
{
    internal class GroupService
    {
        private readonly ObservableCollection<Item> _receiverGroups;
        private readonly ObservableCollection<Item> _shipmentGroups;
        private readonly ValidationService _validationService;

        public event Action<string> OnGroupUpdated;

        public GroupService(ObservableCollection<Item> receiverGroups, ObservableCollection<Item> shipmentGroups,
            ValidationService validationService)
        {
            _receiverGroups = receiverGroups;
            _shipmentGroups = shipmentGroups;
            _validationService = validationService;
        }

        public void AddToGroup(string input, ObservableCollection<Item> group,
            ObservableCollection<Nomenclature> nomenclatures)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            var ids = _validationService.NormalizeInput(input)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var id in ids)
            {
                if (!_validationService.IsValidHex(id)) continue;

                var nomenclature = nomenclatures.FirstOrDefault(n => n.Id == id);
                if (nomenclature != null)
                    UpdateGroups(group, nomenclature.Name);
            }

            OnGroupUpdated?.Invoke(input);
        }

        public void UpdateGroups(ObservableCollection<Item> group, string itemName)
        {
            AddItemToGroup(group, itemName);
            RemoveItemFromOppositeGroup(group == _receiverGroups ? _shipmentGroups : _receiverGroups, itemName);
        }

        public void ClearGroups()
        {
            _receiverGroups.Clear();
            _shipmentGroups.Clear();
        }

        private void AddItemToGroup(ObservableCollection<Item> group, string itemName)
        {
            var existingItem = group.FirstOrDefault(item => item.Name == itemName);
            if (existingItem != null)
                existingItem.Count++;
            else
                group.Add(new Item { Name = itemName, Count = 1 });
        }

        private void RemoveItemFromOppositeGroup(ObservableCollection<Item> oppositeGroup, string itemName)
        {
            var existingItem = oppositeGroup.FirstOrDefault(item => item.Name == itemName);
            if (existingItem != null)
            {
                existingItem.Count--;
                if (existingItem.Count <= 0)
                    oppositeGroup.Remove(existingItem);
            }
        }
    }
}