using System;
using System.IO;
using static Avances.Program;

namespace Avances
{
    // TEMA 5: PERSISTENCIA DE TEXTO, ESCRITURA Y LECTURA
    internal static class Persistencia
    {
        // Copia un adjunto a Datos\Adjuntos\<código>\ para que viaje con el proyecto.
        internal static string CopiarAdjunto(string origen, string codigo)
        {
            string carpeta = Path.Combine(carpetaDatos, "Adjuntos", codigo);
            Directory.CreateDirectory(carpeta);
            string destino = Path.Combine(carpeta, Path.GetFileName(origen));
            for (int n = 2; File.Exists(destino); n++)
                destino = Path.Combine(carpeta, Path.GetFileNameWithoutExtension(origen) + "_" + n + Path.GetExtension(origen));
            File.Copy(origen, destino);
            return destino;
        }

        // En el .txt se guardan rutas relativas a Datos\, así funcionan en cualquier PC.
        static string ARelativa(string ruta)
        {
            string prefijo = carpetaDatos + Path.DirectorySeparatorChar;
            return ruta != null && ruta.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase) ? ruta.Substring(prefijo.Length) : ruta;
        }
        static string AAbsoluta(string ruta)
        {
            return string.IsNullOrEmpty(ruta) || Path.IsPathRooted(ruta) ? ruta : Path.Combine(carpetaDatos, ruta);
        }

        internal static void GuardarUsuarios()
        {
            using (var sw = new StreamWriter(archivoUsuarios, false))
                for (int i = 0; i < cantidadUsuarios; i++)
                    sw.WriteLine($"{usuariosNombres[i]}|{usuariosDnis[i]}|{usuariosTelefonos[i]}|{usuariosCorreos[i]}|{usuariosContrasenas[i]}");
        }
        internal static void CargarUsuarios()
        {
            if (!File.Exists(archivoUsuarios)) return;
            foreach (var linea in File.ReadAllLines(archivoUsuarios))
            {
                var d = linea.Split('|');
                if (d.Length == 5 && cantidadUsuarios < 100)
                {
                    usuariosNombres[cantidadUsuarios] = d[0];
                    usuariosDnis[cantidadUsuarios] = d[1];
                    usuariosTelefonos[cantidadUsuarios] = d[2];
                    usuariosCorreos[cantidadUsuarios] = d[3];
                    usuariosContrasenas[cantidadUsuarios] = d[4];
                    cantidadUsuarios++;
                }
            }
        }

        internal static void GuardarExpedientes()
        {
            using (var sw = new StreamWriter(archivoDatos, false))
                for (int i = 0; i < cantidad; i++)
                {
                    sw.WriteLine($"{codigos[i]}|{nombres[i]}|{dnis[i]}|{telefonos[i]}|{correos[i]}|{tiposDocumento[i]}|{especialidades[i]}|{jueces[i]}|{ARelativa(documentos[i])}|{descripcionDocumento[i]}|{cantidadMedios[i]}|{propietariosDni[i]}|{fechasRegistro[i]}|{horasRegistro[i]}|{estados[i]}|{foliosDocumentos[i]}");
                    for (int j = 0; j < cantidadMedios[i]; j++)
                        sw.WriteLine($"MEDIO|{tiposMedios[i, j]}|{ARelativa(archivosMedios[i, j])}|{descripcionMedios[i, j]}");
                }
        }

        // StreamReader, lectura secuencial como en la práctica de clase.
        internal static void CargarExpedientes()
        {
            if (!File.Exists(archivoDatos)) return;
            using (StreamReader lector = new StreamReader(archivoDatos))
            {
                string linea;
                while (cantidad < 100 && (linea = lector.ReadLine()) != null)
                {
                    string[] d = linea.Split('|');
                    if (d.Length == 0 || d[0] == "MEDIO" ||
                        (d.Length != 11 && d.Length != 15 && d.Length != 16)) continue;
                    int medios;
                    if (!int.TryParse(d[10], out medios) || medios < 0 || medios > 3) break;
                    codigos[cantidad] = d[0]; nombres[cantidad] = d[1]; dnis[cantidad] = d[2];
                    telefonos[cantidad] = d[3]; correos[cantidad] = d[4];
                    tiposDocumento[cantidad] = d[5]; especialidades[cantidad] = d[6];
                    jueces[cantidad] = d[7]; documentos[cantidad] = AAbsoluta(d[8]); descripcionDocumento[cantidad] = d[9];
                    propietariosDni[cantidad] = d.Length >= 15 ? d[11] : "";
                    fechasRegistro[cantidad] = d.Length >= 15 ? d[12] : "SIN FECHA";
                    horasRegistro[cantidad] = d.Length >= 15 ? d[13] : "--:--";
                    estados[cantidad] = d.Length >= 15 ? d[14] : "REGISTRADO";
                    int folios;
                    foliosDocumentos[cantidad] = d.Length == 16 && int.TryParse(d[15], out folios)
                        ? folios : (File.Exists(documentos[cantidad]) ? Validaciones.ContarFoliosPDF(documentos[cantidad]) : 0);
                    cantidadMedios[cantidad] = 0;
                    for (int j = 0; j < medios; j++)
                    {
                        string medio = lector.ReadLine();
                        if (medio == null) break;
                        string[] m = medio.Split('|');
                        if (m.Length != 4 || m[0] != "MEDIO") break;
                        tiposMedios[cantidad, j] = m[1]; archivosMedios[cantidad, j] = AAbsoluta(m[2]);
                        descripcionMedios[cantidad, j] = m[3]; cantidadMedios[cantidad]++;
                    }
                    cantidad++;
                }
            }
        }
    }
}
