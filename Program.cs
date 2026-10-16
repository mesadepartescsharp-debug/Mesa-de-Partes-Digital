using System;

namespace Avances
{
    // PROGRAMA PRINCIPAL: Main, menú principal, variables compartidas y utilidades de consola.
    // Los demás módulos (Usuarios, Validaciones, Expedientes, Jueces, Persistencia,
    // DocumentosPDF y Correos) leen estos arreglos con "using static Avances.Program;".
    internal static class Program
    {
        // TEMA 3: Arreglos unidimensionales (paralelos)
        internal static string[] codigos = new string[100];
        internal static string[] nombres = new string[100];
        internal static string[] dnis = new string[100];
        internal static string[] telefonos = new string[100];
        internal static string[] correos = new string[100];
        internal static string[] tiposDocumento = new string[100];
        internal static string[] especialidades = new string[100];
        internal static string[] jueces = new string[100];
        internal static string[] documentos = new string[100];
        internal static string[] descripcionDocumento = new string[100];
        internal static string[] propietariosDni = new string[100];
        internal static string[] fechasRegistro = new string[100];
        internal static string[] horasRegistro = new string[100];
        internal static string[] estados = new string[100];
        internal static int[] foliosDocumentos = new int[100];

        // TEMA 3: Arreglos bidimensionales
        internal static string[,] tiposMedios = new string[100, 3];
        internal static string[,] archivosMedios = new string[100, 3];
        internal static string[,] descripcionMedios = new string[100, 3];
        internal static int[] cantidadMedios = new int[100];
        internal static int cantidad = 0;
        // Encapsulamiento: lectura pública interna, modificación controlada en el registro.
        internal static int TotalExpedientes { get { return cantidad; } }
        internal static string archivoDatos = "expedientes.txt";

        // Usuarios (arreglos paralelos - TEMA 4)
        internal static string[] usuariosNombres = new string[100];
        internal static string[] usuariosDnis = new string[100];
        internal static string[] usuariosTelefonos = new string[100];
        internal static string[] usuariosCorreos = new string[100];
        internal static string[] usuariosContrasenas = new string[100];
        internal static int[] intentosFallidos = new int[100];
        internal static bool[] usuariosBloqueados = new bool[100];
        internal static int cantidadUsuarios = 0;
        internal static int usuarioActual = -1;
        internal static string archivoUsuarios = "usuarios.txt";
        internal static string[] juecesCorreos = { "carlos.mendoza@pj.gob.pe", "patricia.salazar@pj.gob.pe" };
        internal static string[] juecesContrasenas = { "Carlos2026*", "Patricia2026*" };
        internal static string[] juecesNombres = { "Dr. Carlos Mendoza Rojas", "Dra. Patricia Salazar Torres" };
        internal static string[] juecesEspecialidades = { "PENAL", "FAMILIA" };

        // TEMAS 1 y 2: Funciones y modularidad (utilidades de consola compartidas)
        internal static void Seccion(string nombre)
        {
            Console.WriteLine("\n-----------------------------------------------");
            Console.WriteLine("  " + nombre);
            Console.WriteLine("-----------------------------------------------");
        }
        internal static string Leer(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine()?.Trim() ?? "";
        }
        internal static string LeerHasta(string msg, Func<string, bool> v)
        {
            string s;
            do { s = Leer(msg); } while (!v(s));
            return s;
        }
        internal static void Titulo(string texto)
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("  " + texto);
            Console.WriteLine("========================================");
        }
        internal static string MostrarHora12(string hora)
        {
            if (DateTime.TryParseExact(hora, "HH:mm", null, System.Globalization.DateTimeStyles.None, out var dt))
                return dt.ToString("hh:mm tt").Replace("AM", "a. m.").Replace("PM", "p. m.");
            return hora;
        }
        internal static DateTime ObtenerFechaHora(string f, string h)
        {
            DateTime r;
            return DateTime.TryParseExact(f + " " + h, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out r) ? r : DateTime.MaxValue;
        }

        [STAThread]
        static void Main()
        {
            Persistencia.CargarExpedientes();
            Persistencia.CargarUsuarios();
            string op = "";
            while (op != "3")
            {
                Console.Clear();
                Titulo("MESA DE PARTES DIGITAL");
                Console.WriteLine("1. Usuario\n2. Juez\n3. Salir\n");
                op = Leer("Opción: ");
                if (op == "1") Usuarios.AccesoUsuario();
                else if (op == "2") Jueces.AccesoJuez();
                else if (op == "3") Console.WriteLine("Saliendo...");
                else { Console.WriteLine("Opción inválida."); Console.ReadKey(); }
            }
        }
    }
}
