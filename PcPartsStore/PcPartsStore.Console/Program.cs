using System;
using PcPartsStore.Common.Extensions;
using PcPartsStore.Common.Models;
using PcPartsStore.Common.Services;

namespace PcPartsStore.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ICrudService<Product> crud = new CrudService<Product>();

            var gpu = new Gpu("RTX 5070", 32000m, 12, "NVIDIA", 250);
            var cpu = new Cpu("Ryzen 7 9800X3D", 22000m, 8, 16, 4.7m);

            // CREATE
            crud.Create(gpu);
            crud.Create(cpu);

            global::System.Console.WriteLine("=== ПІСЛЯ CREATE ===");
            foreach (var p in crud.ReadAll())
                global::System.Console.WriteLine(p.ToShortString()); // extension method

            // READ
            global::System.Console.WriteLine("\n=== READ (GPU) ===");
            var found = crud.Read(gpu.Id);
            global::System.Console.WriteLine(found != null ? found.ToShortString() : "Не знайдено");

            // UPDATE
            global::System.Console.WriteLine("\n=== UPDATE (GPU price) ===");
            gpu.Price = 29999m;
            crud.Update(gpu);

            foreach (var p in crud.ReadAll())
                global::System.Console.WriteLine(p.ToShortString());

            // REMOVE
            global::System.Console.WriteLine("\n=== REMOVE (CPU) ===");
            crud.Remove(cpu.Id);

            foreach (var p in crud.ReadAll())
                global::System.Console.WriteLine(p.ToShortString());

            global::System.Console.WriteLine("\nГотово. Enter...");
            global::System.Console.ReadLine();
        }
    }
}
