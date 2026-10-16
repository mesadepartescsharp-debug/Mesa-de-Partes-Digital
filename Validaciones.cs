using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;
using static Avances.Program;

namespace Avances
{
    // TEMA 4: CADENAS Y VALIDACIONES DE DATOS
    internal static class Validaciones
    {
        internal static bool ValidarNombre(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
            {
                Console.WriteLine("ERROR: El nombre no puede quedar vacío.");
                return false;
            }
            foreach (char c in n)
                if (!char.IsLetter(c) && c != ' ')
                {
                    Console.WriteLine("ERROR: Solo letras y espacios.");
                    return false;
                }
            return true;
        }
        internal static bool ValidarDNI(string d)
        {
            if (d.Length != 8)
            {
                Console.WriteLine("ERROR: El DNI debe contener exactamente 8 dígitos.");
                return false;
            }
            foreach (char c in d)
                if (!char.IsDigit(c))
                {
                    Console.WriteLine("ERROR: Solo dígitos.");
                    return false;
                }
            return true;
        }
        internal static bool ValidarTelefono(string t)
        {
            if (t.Length != 9 || t[0] != '9')
            {
                Console.WriteLine("ERROR: Teléfono debe tener 9 dígitos y empezar con 9.");
                return false;
            }
            foreach (char c in t)
                if (!char.IsDigit(c))
                {
                    Console.WriteLine("ERROR: Solo dígitos.");
                    return false;
                }
            return true;
        }
        internal static bool ValidarCorreo(string c)
        {
            c = c.Trim();
            if (c == "" || !c.Contains("@") || !c.Contains(".") || c.Contains(" ") || c.Contains("|"))
            {
                Console.WriteLine("ERROR: Correo inválido.");
                return false;
            }
            return true;
        }
        internal static bool ValidarDescripcion(string d)
        {
            if (d.Length < 10)
            {
                Console.WriteLine("ERROR: Descripción mínima 10 caracteres.");
                return false;
            }
            return SinSeparador(d);
        }
        internal static bool ValidarTipoDocumento(string o)
        {
            if (o != "1" && o != "2" && o != "3")
            {
                Console.WriteLine("ERROR: Opción 1-3.");
                return false;
            }
            return true;
        }
        internal static bool ValidarEspecialidad(string o)
        {
            if (o != "1" && o != "2")
            {
                Console.WriteLine("ERROR: 1 Penal / 2 Familia.");
                return false;
            }
            return true;
        }
        internal static bool ValidarContrasena(string c)
        {
            if (c.Length < 6)
            {
                Console.WriteLine("ERROR: Mínimo 6 caracteres.");
                return false;
            }
            return SinSeparador(c);
        }
        // '|' separa los campos en los .txt; si se escribe en un dato, la línea no vuelve a cargarse.
        static bool SinSeparador(string s)
        {
            if (s.Contains("|"))
            {
                Console.WriteLine("ERROR: No se permite el carácter '|'.");
                return false;
            }
            return true;
        }
        internal static bool DNIUsuarioRegistrado(string dni)
        {
            for (int i = 0; i < cantidadUsuarios; i++)
                if (usuariosDnis[i] == dni)
                {
                    Console.WriteLine("ERROR: DNI ya registrado.");
                    return true;
                }
            return false;
        }
        internal static bool CorreoUsuarioRegistrado(string correo)
        {
            for (int i = 0; i < cantidadUsuarios; i++)
                if (usuariosCorreos[i].Equals(correo, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("ERROR: Correo ya registrado.");
                    return true;
                }
            return false;
        }

        // TEMAS 3 Y 4: VALIDACIÓN DE DOCUMENTO PDF Y MEDIOS PROBATORIOS
        internal static bool ValidarDocumento(string ruta, string nombreEsperado)
        {
            string ext = Path.GetExtension(ruta).ToLower();
            string nombre = Path.GetFileName(ruta);
            if (ext != ".pdf")
            {
                Console.WriteLine("ERROR: Solo PDF.");
                return false;
            }
            if (!nombre.Equals(nombreEsperado, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("ERROR: Nombre incorrecto. Requerido: " + nombreEsperado);
                return false;
            }
            if (new FileInfo(ruta).Length > 30 * 1024 * 1024)
            {
                Console.WriteLine("ERROR: Máximo 30 MB.");
                return false;
            }
            return true;
        }
        internal static int ContarFoliosPDF(string ruta)
        {
            try
            {
                using (var doc = PdfReader.Open(ruta, PdfDocumentOpenMode.Import))
                    return doc.PageCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR al contar folios: " + ex.Message);
                return 0;
            }
        }
        internal static bool ValidarMedio(string ruta, string tipo, int maximo)
        {
            string ext = Path.GetExtension(ruta).ToLower();
            Console.WriteLine($"Tipo: {tipo} | Extensión: {ext} | Máx: {maximo} MB");
            bool ok = (tipo == "IMAGEN" && (ext == ".jpg" || ext == ".jpeg")) ||
                      (tipo == "AUDIO" && ext == ".mp3") ||
                      (tipo == "VIDEO" && ext == ".mp4");
            if (!ok)
            {
                Console.WriteLine("ERROR: Formato no permitido.");
                return false;
            }
            if (new FileInfo(ruta).Length > maximo * 1024 * 1024)
            {
                Console.WriteLine($"ERROR: Supera {maximo} MB.");
                return false;
            }
            Console.WriteLine(tipo + " validado.");
            return true;
        }
    }
}
