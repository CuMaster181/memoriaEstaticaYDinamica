using MemoriaDinamica;

namespace MemoriaDinamica;

/// <summary>
/// EJERCICIO 1 - Vectores/arreglos que se generan y redimensionan en tiempo de ejecucion.
///
/// Parte A: arreglo de tamano fijo y su limitacion.
/// Parte B: redimensionar un arreglo a mano (crear uno nuevo y copiar).
/// Parte C: VectorDinamico que se redimensiona solo al llenarse.
/// </summary>
public static class Ejercicio1_RedimensionDinamica
{
    /// <param name="tamanioInicial">Tamano elegido por el usuario en la interfaz (se decide en ejecucion).</param>
    /// <param name="log">Funcion que escribe una linea en la salida de la ventana.</param>
    public static void Ejecutar(int tamanioInicial, Action<string> log)
    {
        log("=== PARTE A: arreglo de tamano fijo ===");
        int[] fijo = new int[tamanioInicial]; // el tamano se decide en ejecucion
        log($"Se reservo un arreglo de {fijo.Length} posiciones.");
        try
        {
            for (int i = 0; i <= fijo.Length; i++) // <= provoca intentar una posicion de mas
                fijo[i] = i * 10;
        }
        catch (IndexOutOfRangeException e)
        {
            log("ERROR esperado: no cabe un elemento mas -> " + e.Message);
        }

        log("");
        log("=== PARTE B: redimensionar un arreglo a mano ===");
        int[] a = { 1, 2, 3, 4, 5 };
        log($"Original       (len {a.Length}): [{string.Join(", ", a)}]");

        int[] mayor = new int[a.Length * 2];     // nuevo arreglo en el Heap
        Array.Copy(a, mayor, a.Length);          // copia
        a = mayor;                               // la variable ahora apunta al nuevo
        log($"Redimensionado (len {a.Length}): [{string.Join(", ", a)}]");

        Array.Resize(ref a, 3);                  // reducir tambien crea un arreglo nuevo
        log($"Reducido       (len {a.Length}): [{string.Join(", ", a)}]");

        log("");
        log("=== PARTE C: VectorDinamico (crece solo al llenarse) ===");
        var v = new VectorDinamico(tamanioInicial);
        int capacidadPrevia = v.Capacidad;
        log("Capacidad inicial = " + capacidadPrevia);
        for (int i = 1; i <= 20; i++)
        {
            v.Agregar(i * 5);
            if (v.Capacidad != capacidadPrevia)
            {
                log($"  Al insertar el elemento #{i} -> capacidad {capacidadPrevia} => {v.Capacidad} (redimension #{v.Redimensiones})");
                capacidadPrevia = v.Capacidad;
            }
        }
        log("Contenido : " + v);
        log($"Tamanio   = {v.Tamanio} | Capacidad = {v.Capacidad} | Redimensiones = {v.Redimensiones}");
        v.Ajustar();
        log($"Tras Ajustar(): Tamanio = {v.Tamanio} | Capacidad = {v.Capacidad}");
    }
}
