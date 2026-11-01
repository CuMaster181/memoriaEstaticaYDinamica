namespace MemoriaDinamica;

/// <summary>
/// Vector (arreglo) dinamico de enteros implementado a mano.
///
/// Guarda los datos en un arreglo int[] (en C# los arreglos son objetos de tipo
/// referencia, por lo que viven en el Heap, en memoria contigua). Un arreglo NO
/// puede cambiar de tamano: "crecer" significa crear otro arreglo mas grande y
/// copiar el contenido. El arreglo viejo queda sin referencias y el recolector de
/// basura (Garbage Collector) liberara su memoria.
/// </summary>
public class VectorDinamico
{
    private int[] _datos;            // arreglo interno (su Length = capacidad)
    private int _tamanio;            // elementos realmente usados
    private readonly bool _duplicar; // true: capacidad x2 | false: capacidad +1

    public int Tamanio => _tamanio;
    public int Capacidad => _datos.Length;
    public int Redimensiones { get; private set; }

    /// <summary>Crea un vector que crece duplicando su capacidad (estrategia eficiente).</summary>
    public VectorDinamico(int capacidadInicial) : this(capacidadInicial, true) { }

    /// <param name="capacidadInicial">Capacidad con la que inicia el arreglo interno.</param>
    /// <param name="duplicar">true = crecer x2; false = crecer solo +1 (ineficiente).</param>
    public VectorDinamico(int capacidadInicial, bool duplicar)
    {
        if (capacidadInicial < 1)
            throw new ArgumentException("La capacidad inicial debe ser >= 1");

        _datos = new int[capacidadInicial];
        _tamanio = 0;
        _duplicar = duplicar;
    }

    /// <summary>Agrega un elemento al final. Si no hay espacio, redimensiona primero.</summary>
    public void Agregar(int valor)
    {
        if (_tamanio == _datos.Length)
        {
            int nuevaCapacidad = _duplicar ? _datos.Length * 2 : _datos.Length + 1;
            Redimensionar(nuevaCapacidad);
        }
        _datos[_tamanio++] = valor;
    }

    /// <summary>
    /// Reserva un arreglo nuevo, copia los elementos existentes y descarta el viejo.
    /// Costo: O(n) por la copia.
    /// </summary>
    public void Redimensionar(int nuevaCapacidad)
    {
        if (nuevaCapacidad < _tamanio)
            throw new ArgumentException(
                $"La nueva capacidad ({nuevaCapacidad}) es menor que los elementos guardados ({_tamanio})");

        int[] nuevo = new int[nuevaCapacidad];       // nueva reserva en el Heap
        Array.Copy(_datos, nuevo, _tamanio);         // copia de los datos
        _datos = nuevo;                              // el arreglo viejo queda sin referencias
        Redimensiones++;
    }

    /// <summary>Ajusta la capacidad al tamano real (equivalente a TrimExcess).</summary>
    public void Ajustar()
    {
        if (_datos.Length != _tamanio && _tamanio > 0)
            Redimensionar(_tamanio);
    }

    /// <summary>Devuelve el elemento en la posicion indicada. Acceso O(1).</summary>
    public int Obtener(int indice)
    {
        if (indice < 0 || indice >= _tamanio)
            throw new IndexOutOfRangeException($"Indice {indice} fuera de rango [0,{_tamanio - 1}]");
        return _datos[indice];
    }

    public override string ToString()
    {
        return "[" + string.Join(", ", _datos.Take(_tamanio)) + "]";
    }
}