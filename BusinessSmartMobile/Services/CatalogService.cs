using BusinessSmartMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessSmartMobile.Services
{
    public class CatalogService
    {
        public List<Stock> SelectedStocks { get; } = new();

        public event Action? Changed;

        public bool TryAdd(Stock stock)
        {
            if (stock is null) return false;
            if (SelectedStocks.Any(s => s.nStokID == stock.nStokID && s.sBarkod == stock.sBarkod))
                return false;

            SelectedStocks.Add(stock);
            Changed?.Invoke();
            return true;
        }

        public bool Remove(string nStokID, string sBarkod)
        {
            var ix = SelectedStocks.FindIndex(s => s.nStokID == nStokID && s.sBarkod == sBarkod);
            if (ix < 0) return false;

            SelectedStocks.RemoveAt(ix);
            Changed?.Invoke();
            return true;
        }

        public void Clear()
        {
            if (SelectedStocks.Count == 0) return;
            SelectedStocks.Clear();
            Changed?.Invoke();
        }

        public int Count => SelectedStocks.Count;

        public int TotalQuantity => SelectedStocks.Sum(s => s.Miktar);

        // Hepsi double olduğu için toplamı da double veriyoruz
        public double TotalPrice(Func<Stock, double>? priceSelector = null)
            => SelectedStocks.Sum(s => ((priceSelector?.Invoke(s)) ?? s.lFiyat1) * s.Miktar);
    }
}
