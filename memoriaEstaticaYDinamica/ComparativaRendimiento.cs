using System.Diagnostics;

namespace MemoriaDinamica;

public sealed record FilaRendimiento(
    int N,
    double ArregloFijo,
    double VectorX2,
    double? VectorMasUno,
    double ListaNet,
    double ListaEnlazada);

public sealed record ResultadoRendimiento(
    IReadOnlyList<FilaRendimiento> Filas,
    double AccesoArreglo,
    double AccesoLista,
    long Suma);

/// <summary>
/// EJERCICIO 3 - Comparacion de desempeno: memoria estatica vs. memoria dinamica.
///
/// Mide (en milisegundos) el tiempo de:
///   1) Insertar N elementos en: arreglo fijo, VectorDinamico x2, VectorDinamico +1,
///      List&lt;int&gt; de .NET y lista enlazada propia.
///   2) Acceder a 100,000 posiciones aleatorias (arreglo vs. lista enlazada).
///
/// IMPORTANTE: ejecutar en modo Release (dotnet run -c Release) para tiempos realistas.
/// Los tiempos varian segun el equipo; lo importante es la tendencia.
/// </summary>
public static class Ejercicio3_ComparativaRendimiento
{
    private static readonly int[] Tamanios = { 10_000, 100_000, 1_000_000 };
    private const int LimiteCrecimientoUno = 100_000;
    private const int Accesos = 100_000;

    private static double MedirArregloFijo(int n)
    {
        var sw = Stopwatch.StartNew();
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    private static double MedirVector(int n, bool duplicar)
    {
        var sw = Stopwatch.StartNew();
        var v = new VectorDinamico(1, duplicar);
        for (int i = 0; i < n; i++) v.Agregar(i);
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    private static double MedirListaNet(int n)
    {
        var sw = Stopwatch.StartNew();
        var l = new List<int>();
        for (int i = 0; i < n; i++) l.Add(i);
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    private static double MedirListaEnlazada(int n)
    {
        var sw = Stopwatch.StartNew();
        var l = new ListaEnlazada<int>();
        for (int i = 0; i < n; i++) l.AgregarFinal(i);
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    private static string Ms(double valor) => valor.ToString("F3").PadLeft(12);

    public static ResultadoRendimiento Medir(Action<string>? escribir = null)
    {
        escribir ??= Console.WriteLine;

        var filas = new List<FilaRendimiento>();
        foreach (int n in Tamanios)
        {
            double? vectorMasUno = n <= LimiteCrecimientoUno ? MedirVector(n, false) : null;
            var fila = new FilaRendimiento(
                n,
                MedirArregloFijo(n),
                MedirVector(n, true),
                vectorMasUno,
                MedirListaNet(n),
                MedirListaEnlazada(n));
            filas.Add(fila);
            escribir($"N = {n:N0}: arreglo {fila.ArregloFijo:F3} ms, vector x2 {fila.VectorX2:F3} ms, lista enlazada {fila.ListaEnlazada:F3} ms");
        }

        const int tam = 100_000;
        int[] arreglo = new int[tam];
        var lista = new ListaEnlazada<int>();
        for (int i = 0; i < tam; i++)
        {
            arreglo[i] = i;
            lista.AgregarFinal(i);
        }

        var rnd = new Random(42);
        int[] posiciones = new int[Accesos];
        for (int i = 0; i < Accesos; i++) posiciones[i] = rnd.Next(tam);

        long suma = 0;
        var sw = Stopwatch.StartNew();
        foreach (int p in posiciones) suma += arreglo[p];
        sw.Stop();
        double accesoArreglo = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        foreach (int p in posiciones) suma += lista.Obtener(p);
        sw.Stop();

        return new ResultadoRendimiento(filas, accesoArreglo, sw.Elapsed.TotalMilliseconds, suma);
    }

    public static void Ejecutar()
    {
        // Calentamiento (para que el JIT no distorsione la primera medicion)
        for (int i = 0; i < 3; i++)
        {
            MedirArregloFijo(100_000);
            MedirVector(100_000, true);
            MedirListaNet(100_000);
            MedirListaEnlazada(100_000);
        }

        Console.WriteLine("=== 1) INSERCION DE N ELEMENTOS (tiempo en ms) ===");
        Console.WriteLine($"{"N",-12}{"Arreglo fijo",14}{"Vector x2",14}{"Vector +1",14}{"List<int>",14}{"Lista enlazada",16}");
        foreach (int n in Tamanios)
        {
            string uno = n <= LimiteCrecimientoUno ? Ms(MedirVector(n, false)) : "n/a".PadLeft(12);
            Console.WriteLine($"{n,-12}{Ms(MedirArregloFijo(n)),14}{Ms(MedirVector(n, true)),14}{uno,14}{Ms(MedirListaNet(n)),14}{Ms(MedirListaEnlazada(n)),16}");
        }

        Console.WriteLine();
        Console.WriteLine($"=== 2) ACCESO ALEATORIO: {Accesos} lecturas (tiempo en ms) ===");
        const int tam = 100_000;
        int[] arreglo = new int[tam];
        var lista = new ListaEnlazada<int>();
        for (int i = 0; i < tam; i++) { arreglo[i] = i; lista.AgregarFinal(i); }

        var rnd = new Random(42);
        int[] posiciones = new int[Accesos];
        for (int i = 0; i < Accesos; i++) posiciones[i] = rnd.Next(tam);

        long suma = 0;
        var sw = Stopwatch.StartNew();
        foreach (int p in posiciones) suma += arreglo[p];          // O(1) cada acceso
        sw.Stop();
        double tArreglo = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        foreach (int p in posiciones) suma += lista.Obtener(p);    // O(n) cada acceso
        sw.Stop();
        double tLista = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"N = {tam} elementos");
        Console.WriteLine($"Arreglo        : {Ms(tArreglo)} ms");
        Console.WriteLine($"Lista enlazada : {Ms(tLista)} ms");
        Console.WriteLine($"(control, evita que el compilador elimine el codigo: suma = {suma})");
    }
}