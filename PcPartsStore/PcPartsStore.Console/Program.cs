using System.Threading;
using PcPartsStore.Common.Models;
using PcPartsStore.Common.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== LAB 2: Async CRUD + Parallel + LINQ + Sync primitives ===");

// Файл для збереження колекції
var filePath = Path.Combine(AppContext.BaseDirectory, "products.json");

// Async CRUD (thread-safe всередині через SemaphoreSlim)
var crud = new InMemoryCrudServiceAsync<Product>(filePath);

// --- Приклад примітива синхронізації: AutoResetEvent ---
using var doneEvent = new AutoResetEvent(false);

// --- Приклад lock (для консольного логу) ---
object consoleLock = new();

int totalToCreate = 1000;
int created = 0;

ThreadPool.QueueUserWorkItem(_ =>
{
    Parallel.For(0, totalToCreate, i =>
    {
        // Генеруємо об'єкт через CreateNew()
        Product p = (i % 2 == 0) ? Cpu.CreateNew() : Gpu.CreateNew();

        // Parallel не дружить з await — для лабы ок: синхронно дочекаємось
        crud.CreateAsync(p).GetAwaiter().GetResult();

        var nowCreated = Interlocked.Increment(ref created);

        // не спамимо 1000 рядків, але покажемо, що lock використовується
        if (nowCreated % 250 == 0)
        {
            lock (consoleLock)
            {
                Console.WriteLine($"Created {nowCreated}/{totalToCreate} ...");
            }
        }
    });

    doneEvent.Set();
});

// Чекаємо завершення генерації (AutoResetEvent)
doneEvent.WaitOne();

Console.WriteLine();
Console.WriteLine($"Created total: {created}");

// --- LINQ: min/max/avg по числових значеннях ---
var snapshot = crud.ToList(); // IEnumerable<T> -> snapshot

var minPrice = snapshot.Min(p => p.Price);
var maxPrice = snapshot.Max(p => p.Price);
var avgPrice = snapshot.Average(p => p.Price);

var minStock = snapshot.Min(p => p.Stock);
var maxStock = snapshot.Max(p => p.Stock);
var avgStock = snapshot.Average(p => p.Stock);

Console.WriteLine();
Console.WriteLine("=== LINQ статистика ===");
Console.WriteLine($"Price  -> min: {minPrice}, max: {maxPrice}, avg: {avgPrice:F2}");
Console.WriteLine($"Stock  -> min: {minStock}, max: {maxStock}, avg: {avgStock:F2}");

// --- Пагінація (показати, що працює) ---
var page1 = crud.ReadAllAsync(page: 1, amount: 10).GetAwaiter().GetResult().ToList();

Console.WriteLine();
Console.WriteLine("=== Pagination (page 1, amount 10) ===");
foreach (var item in page1)
{
    Console.WriteLine(item.GetInfo());
}

// --- Збереження у файл ---
var saved = crud.SaveAsync().GetAwaiter().GetResult();

Console.WriteLine();
Console.WriteLine(saved
    ? $"Saved OK -> {filePath}"
    : "Save FAILED");

Console.WriteLine("=== END ===");
