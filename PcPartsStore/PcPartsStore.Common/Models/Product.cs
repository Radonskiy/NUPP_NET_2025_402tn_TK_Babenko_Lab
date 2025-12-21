using System;

namespace PcPartsStore.Common.Models;

public abstract class Product
{
    // static поле (лічильник створених продуктів)
    public static int CreatedCount { get; private set; }

    // event (подія при зміні ціни)
    public event Action<Product, decimal, decimal>? PriceChanged;

    // static конструктор
    static Product()
    {
        CreatedCount = 0;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price == value) return;
            var old = _price;
            _price = value;
            PriceChanged?.Invoke(this, old, value);
        }
    }

    protected Product()
    {
        CreatedCount++;
    }

    protected Product(string name, decimal price) : this()
    {
        Name = name;
        Price = price;
    }

    // static метод
    public static decimal ApplyDiscount(decimal price, int percent)
        => price - (price * percent / 100m);

    public virtual string GetInfo()
        => $"{GetType().Name}: {Name}, {Price} грн (Id: {Id})";
}
