using System;
using System.IO;
using static Avances.Program;

namespace Avances
{
    // TEMA 5: PERSISTENCIA DE TEXTO, ESCRITURA Y LECTURA
    internal static class Persistencia
    {
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
                    sw.WriteLine($"{codigos[i]}|{nombres[i]}|{dnis[i]}|{telefonos[i]}|{correos[i]}|{tiposDocumento[i]}|{especialidades[i]}|{jueces[i]}|{documentos[i]}|{descripcionDocumento[i]}|{cantidadMedios[i]}|{propietariosDni[i]}|{fechasRegistro[i]}|{horasRegistro[i]}|{estados[i]}|{foliosDocumentos[i]}");
                    for (int j = 0; j < cantidadMedios[i]; j++)
                        sw.WriteLine($"MEDIO|{tiposMedios[i, j]}|{archivosMedios[i, j]}|{descripcionMedios[i, j]}");
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
                    jueces[cantidad] = d[7]; documentos[cantidad] = d[8]; descripcionDocumento[cantidad] = d[9];
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
                        tiposMedios[cantidad, j] = m[1]; archivosMedios[cantidad, j] = m[2];
                        descripcionMedios[cantidad, j] = m[3]; cantidadMedios[cantidad]++;
                    }
                    cantidad++;
                }
            }
        }
    }
}
