using System.Runtime.CompilerServices;

namespace MemoriaDinamica;

/// <summary>
/// EJERCICIO 2 - Uso de memoria dinamica: instancias y referencias a objetos en el Heap.
///
/// Parte A: dos referencias al mismo objeto (class) vs. copia de valor (struct).
/// Parte B: lista enlazada, nodos creados con 'new' y enlazados por referencias.
/// Parte C: consumo y liberacion de memoria mediante el recolector de basura (GC).
/// </summary>
public static class Ejercicio2_MemoriaDinamica
{
    /// <summary>Tipo por REFERENCIA: las instancias viven en el Heap.</summary>
    private class Persona
    {
        public string Nombre;
        public int Edad;
        public Persona(string nombre, int edad) { Nombre = nombre; Edad = edad; }
        public override string ToString() => $"{Nombre} ({Edad})";
    }

    /// <summary>Tipo por VALOR: se copia completo al asignarlo.</summary>
    private struct Punto
    {
        public int X;
        public int Y;
        public Punto(int x, int y) { X = x; Y = y; }
        public override string ToString() => $"({X}, {Y})";
    }

    private static long MemoriaUsadaKB() => GC.GetTotalMemory(true) / 1024;

    private static void LimpiarMemoria()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    public static void Ejecutar(Action<string> log)
    {
        log("=== PARTE A: instancias y referencias en el Heap ===");
        var p1 = new Persona("Ana", 20);    // objeto en el Heap; p1 es la referencia
        var p2 = p1;                        // p2 apunta AL MISMO objeto (no se copia)
        var p3 = new Persona("Luis", 22);   // otro objeto distinto
        log($"p1 -> @{RuntimeHelpers.GetHashCode(p1):x}  {p1}");
        log($"p2 -> @{RuntimeHelpers.GetHashCode(p2):x}  {p2}");
        log($"p3 -> @{RuntimeHelpers.GetHashCode(p3):x}  {p3}");
        p2.Nombre = "Ana Maria";            // modificar por p2 tambien cambia lo que ve p1
        log($"Tras p2.Nombre = \"Ana Maria\": p1 = {p1}  (ReferenceEquals(p1, p2) = {ReferenceEquals(p1, p2)})");

        log("");
        log("Contraste con un struct (tipo por valor):");
        var s1 = new Punto(1, 2);
        var s2 = s1;                        // se COPIA el valor
        s2.X = 99;
        log($"s1 = {s1} | s2 = {s2}  -> son independientes");

        log("");
        log("=== PARTE B: lista enlazada (nodos dinamicos) ===");
        var lista = new ListaEnlazada<string>();
        lista.AgregarFinal("B");
        lista.AgregarFinal("C");
        lista.AgregarInicio("A");
        lista.AgregarFinal("D");
        log($"Lista con {lista.Tamanio} nodos:");
        foreach (var l in lista.DescribirReferencias()) log(l);
        log("Se elimina el primero: " + lista.EliminarInicio());
        log($"Quedan {lista.Tamanio} nodos:");
        foreach (var l in lista.DescribirReferencias()) log(l);

        log("");
        log("=== PARTE C: consumo y liberacion de memoria ===");
        LimpiarMemoria();
        long antes = MemoriaUsadaKB();
        log($"Memoria usada al inicio          : {antes} KB");

        ListaEnlazada<int>? grande = new ListaEnlazada<int>();
        const int n = 1_000_000;
        for (int i = 0; i < n; i++)
            grande.AgregarFinal(i);

        long durante = MemoriaUsadaKB();
        log($"Con {n} nodos en la lista : {durante} KB  (+{durante - antes} KB)");

        grande.Vaciar();   // se pierde la referencia a la cabeza
        grande = null;     // los nodos ya no son alcanzables
        LimpiarMemoria();  // se fuerza al recolector de basura
        long despues = MemoriaUsadaKB();
        log($"Tras liberar y ejecutar el GC     : {despues} KB");
    }
}