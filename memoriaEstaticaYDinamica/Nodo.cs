namespace MemoriaDinamica;

/// <summary>
/// Nodo de una lista enlazada. Como es una 'class' (tipo por referencia), cada
/// instancia creada con 'new' vive en el Heap y se conecta con otros nodos
/// mediante referencias (el equivalente a los punteros).
/// </summary>
public class Nodo<T>
{
    public T Dato;                 // informacion almacenada
    public Nodo<T>? Siguiente;     // referencia al siguiente nodo (null si es el ultimo)

    public Nodo(T dato)
    {
        Dato = dato;
        Siguiente = null;
    }
}