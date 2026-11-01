using MemoriaDinamica;

namespace MemoriaDinamica;

/// <summary>
/// Ventana principal. Toda la interfaz se construye por codigo (no requiere el diseñador de Visual Studio).
/// </summary>
public class MainForm : Form
{
    private readonly TextBox _salida = new()
    {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Both,
        WordWrap = false,
        Font = new Font("Consolas", 10),
        BackColor = Color.White,
        Dock = DockStyle.Fill
    };

    private readonly DataGridView _tabla = new()
    {
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White,
        Dock = DockStyle.Fill
    };

    private readonly NumericUpDown _numTamanio = new() { Minimum = 1, Maximum = 1000, Value = 5, Width = 70 };
    private readonly Button _btn1 = new() { Text = "1. Redimension de arreglos", AutoSize = true };
    private readonly Button _btn2 = new() { Text = "2. Memoria dinamica", AutoSize = true };
    private readonly Button _btn3 = new() { Text = "3. Comparativa de rendimiento", AutoSize = true };
    private readonly Button _btnLimpiar = new() { Text = "Limpiar", AutoSize = true };
    private readonly Label _estado = new() { Text = "Listo.", AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
    private readonly TabControl _pestanas = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        Text = "Tarea 1.2 - Memoria estatica vs. memoria dinamica";
        Width = 1000;
        Height = 650;
        StartPosition = FormStartPosition.CenterScreen;

        // Panel superior con los controles
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(8),
            WrapContents = true
        };
        panel.Controls.Add(new Label { Text = "Tamano inicial del arreglo:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_numTamanio);
        panel.Controls.Add(_btn1);
        panel.Controls.Add(_btn2);
        panel.Controls.Add(_btn3);
        panel.Controls.Add(_btnLimpiar);
        panel.Controls.Add(_estado);

        // Pestanas: salida de texto y tabla de rendimiento
        var tabSalida = new TabPage("Salida");
        tabSalida.Controls.Add(_salida);
        var tabTabla = new TabPage("Tabla de rendimiento");
        tabTabla.Controls.Add(_tabla);
        _pestanas.TabPages.Add(tabSalida);
        _pestanas.TabPages.Add(tabTabla);

        Controls.Add(_pestanas);
        Controls.Add(panel); // se agrega al final para que quede arriba

        ConfigurarTabla();

        _btn1.Click += (_, _) => EjecutarEjercicio1();
        _btn2.Click += async (_, _) => await EjecutarEjercicio2Async();
        _btn3.Click += async (_, _) => await EjecutarEjercicio3Async();
        _btnLimpiar.Click += (_, _) => { _salida.Clear(); _tabla.Rows.Clear(); _estado.Text = "Listo."; };
    }

    private void ConfigurarTabla()
    {
        _tabla.Columns.Add("N", "N (elementos)");
        _tabla.Columns.Add("ArregloFijo", "Arreglo fijo (ms)");
        _tabla.Columns.Add("VectorX2", "Vector x2 (ms)");
        _tabla.Columns.Add("VectorMasUno", "Vector +1 (ms)");
        _tabla.Columns.Add("ListaNet", "List<int> (ms)");
        _tabla.Columns.Add("ListaEnlazada", "Lista enlazada (ms)");
    }

    /// <summary>Escribe una linea en la salida; es seguro llamarlo desde otro hilo.</summary>
    private void Escribir(string texto)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => Escribir(texto));
            return;
        }
        _salida.AppendText(texto + Environment.NewLine);
    }

    private void HabilitarBotones(bool habilitar)
    {
        _btn1.Enabled = _btn2.Enabled = _btn3.Enabled = _btnLimpiar.Enabled = _numTamanio.Enabled = habilitar;
    }

    private void EjecutarEjercicio1()
    {
        _pestanas.SelectedIndex = 0;
        _salida.Clear();
        Ejercicio1_RedimensionDinamica.Ejecutar((int)_numTamanio.Value, Escribir);
        _estado.Text = "Ejercicio 1 terminado.";
    }

    private async Task EjecutarEjercicio2Async()
    {
        _pestanas.SelectedIndex = 0;
        _salida.Clear();
        HabilitarBotones(false);
        _estado.Text = "Ejecutando ejercicio 2...";
        await Task.Run(() => Ejercicio2_MemoriaDinamica.Ejecutar(Escribir));
        _estado.Text = "Ejercicio 2 terminado.";
        HabilitarBotones(true);
    }

    private async Task EjecutarEjercicio3Async()
    {
        _pestanas.SelectedIndex = 0;
        _salida.Clear();
        _tabla.Rows.Clear();
        HabilitarBotones(false);
        _estado.Text = "Midiendo... espera unos segundos.";

        ResultadoRendimiento r = await Task.Run(() => Ejercicio3_ComparativaRendimiento.Medir(Escribir));

        foreach (var f in r.Filas)
        {
            _tabla.Rows.Add(
                f.N.ToString("N0"),
                f.ArregloFijo.ToString("F3"),
                f.VectorX2.ToString("F3"),
                f.VectorMasUno?.ToString("F3") ?? "n/a",
                f.ListaNet.ToString("F3"),
                f.ListaEnlazada.ToString("F3"));
        }

        Escribir("");
        Escribir($"=== ACCESO ALEATORIO: 100,000 lecturas con N = 100,000 ===");
        Escribir($"Arreglo        : {r.AccesoArreglo:F3} ms");
        Escribir($"Lista enlazada : {r.AccesoLista:F3} ms");
        Escribir($"(control para evitar optimizaciones del compilador: suma = {r.Suma})");
        Escribir("");
        Escribir("Los tiempos de insercion estan en la pestana 'Tabla de rendimiento'.");

        _pestanas.SelectedIndex = 1;
        _estado.Text = "Ejercicio 3 terminado.";
        HabilitarBotones(true);
    }
}