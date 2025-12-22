using PcPartsStore.Common.Models;

namespace PcPartsStore.Common.Extensions
{
    public static class ProductExtensions
    {
        // Extension method (вимога ТЗ)
        public static string ToShortString(this Product p)
        {
            if (p == null) return "<null>";
            return $"{p.GetType().Name}: {p.Name} ({p.Price} грн) [{p.Id}]";
        }
    }
}
