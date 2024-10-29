using System.Text.RegularExpressions;

namespace RST_Invent.Services
{
    internal class ValidationService
    {
        public bool IsValidHex(string input)
        {
            return input.Length == 24 && Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public bool IsValidNomenclature(string id, string name)
        {
            return !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name) && IsValidHex(id);
        }

        public string NormalizeInput(string input)
        {
            return input.ToUpperInvariant();
        }
    }
}