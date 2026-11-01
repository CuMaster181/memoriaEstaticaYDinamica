using System.Runtime.CompilerServices;

namespace MemoriaDinamica;

/// <summary>
/// Lista enlazada simple generica. Ejemplo clasico de memoria dinamica: no se
/// reserva espacio por adelantado; se pide memoria al Heap (new Nodo) solo cuando
/// se agrega un elemento y se libera cuando se pierde la referencia.
/// </summary>
public class ListaEnlazada<T>
{
    private Nodo<T>? _cabeza; // primer nodo
    private Nodo<T>? _cola;   // ultimo nodo (permite insertar al final en O(1))

    public int Tamanio { get; private set; }

    /// <summary>Inserta al inicio. O(1).</summary>
    public void AgregarInicio(T dato)
    {
        var nuevo = new Nodo<T>(dato);
        nuevo.Siguiente = _cabeza;
        _cabeza = nuevo;
        _cola ??= nuevo;
        Tamanio++;
    }

    /// <summary>Inserta al final. O(1) gracias a la referencia _cola.</summary>
    public void AgregarFinal(T dato)
    {
        var nuevo = new Nodo<T>(dato);
        if (_cola == null)
            _cabeza = nuevo;
        else
            _cola.Siguiente = nuevo;
        _cola = nuevo;
        Tamanio++;
    }

    /// <summary>Elimina y devuelve el primer elemento. O(1).</summary>
    public T EliminarInicio()
    {
        if (_cabeza == null)
            throw new InvalidOperationException("La lista esta vacia");

        T dato = _cabeza.Dato;
        _cabeza = _cabeza.Siguiente; // el nodo viejo queda sin referencias -> basura
        if (_cabeza == null) _cola = null;
        Tamanio--;
        return dato;
    }

    /// <summary>Obtiene el elemento en la posicion i. O(n): hay que recorrer los nodos.</summary>
    public T Obtener(int indice)
    {
        if (indice < 0 || indice >= Tamanio)
            throw new IndexOutOfRangeException($"Indice {indice} fuera de rango");

        Nodo<T> actual = _cabeza!;
        for (int i = 0; i < indice; i++)
            actual = actual.Siguiente!;
        return actual.Dato;
    }

    /// <summary>Vacia la lista: al perder la cabeza, todos los nodos quedan inalcanzables.</summary>
    public void Vaciar()
    {
        _cabeza = null;
        _cola = null;
        Tamanio = 0;
    }

    /// <summary>Muestra cada nodo con su identificador de objeto para visualizar las referencias.</summary>
    public void ImprimirReferencias()
    {
        Nodo<T>? actual = _cabeza;
        int i = 0;
        while (actual != null)
        {
            string sig = actual.Siguiente == null
                ? "null"
                : "@" + RuntimeHelpers.GetHashCode(actual.Siguiente).ToString("x");
            Console.WriteLine($"  Nodo {i++} | @{RuntimeHelpers.GetHashCode(actual):x} | dato = {actual.Dato} | siguiente -> {sig}");
            actual = actual.Siguiente;
        }
    }

    public IEnumerable<string> DescribirReferencias()
    {
        Nodo<T>? actual = _cabeza;
        int i = 0;
        while (actual != null)
        {
            string siguiente = actual.Siguiente == null
                ? "null"
                : "@" + RuntimeHelpers.GetHashCode(actual.Siguiente).ToString("x");
            yield return $"  Nodo {i++} | @{RuntimeHelpers.GetHashCode(actual):x} | dato = {actual.Dato} | siguiente -> {siguiente}";
            actual = actual.Siguiente;
        }
    }
}
