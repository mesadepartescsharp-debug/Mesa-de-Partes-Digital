using System;
using System.IO;
using static Avances.Program;

namespace Avances
{
    // TEMAS 1 A 4: CONSULTA DE EXPEDIENTES Y CONTROL DEL JUEZ
    internal static class Jueces
    {
        internal static void AccesoJuez()
        {
            string op = "";
            while (op != "2")
            {
                Console.Clear();
                Titulo("ACCESO DEL JUEZ");
                Console.WriteLine("1. Iniciar sesión\n2. Volver\n");
                op = Leer("Opción: ");
                if (op == "1") IniciarSesionJuez();
                else if (op != "2") { Console.WriteLine("Opción inválida."); Console.ReadKey(); }
            }
        }
        static void IniciarSesionJuez()
        {
            Console.Clear();
            Titulo("INICIAR SESIÓN - JUEZ");
            string correo = Leer("Correo institucional: ").ToLower();
            string pass = Leer("Contraseña: ");
            int pos = -1;
            for (int i = 0; i < juecesCorreos.Length; i++)
                if (juecesCorreos[i].ToLower() == correo && juecesContrasenas[i] == pass) pos = i;

            if (pos == -1)
            {
                Console.WriteLine("Incorrecto.");
                Console.ReadKey();
                return;
            }
            MenuJuez(juecesNombres[pos], juecesEspecialidades[pos]);
        }

        static int ContarExpedientesJuez(string juez)
        {
            int t = 0;
            for (int i = 0; i < cantidad; i++)
                if (jueces[i] == juez) t++;
            return t;
        }

        static int ContarEstadoJuez(string juez, string est)
        {
            int t = 0;
            for (int i = 0; i < cantidad; i++)
                if (jueces[i] == juez && estados[i] == est) t++;
            return t;
        }

        static void MenuJuez(string juez, string especialidad)
        {
            string op = "";
            while (op != "5")
            {
                Console.Clear();
                Titulo("PANEL DEL JUEZ");
                Console.WriteLine($"Juez         : {juez}\nEspecialidad : {especialidad}");
                Seccion("RESUMEN DE EXPEDIENTES");
                Console.WriteLine($"Asignados    : {ContarExpedientesJuez(juez)}");
                Console.WriteLine($"Registrados  : {ContarEstadoJuez(juez, "REGISTRADO")}");
                Console.WriteLine($"En revisión  : {ContarEstadoJuez(juez, "EN REVISIÓN")}");
                Console.WriteLine($"Atendidos    : {ContarEstadoJuez(juez, "ATENDIDO")}");
                Seccion("OPCIONES");
                Console.WriteLine("1. Expedientes asignados\n2. Consultar expediente\n3. Ordenar por fecha\n4. Estadísticas\n5. Cerrar sesión\n");
                op = Leer("Opción: ");
                if (op == "1") MostrarExpedientesJuez(juez);
                else if (op == "2") ConsultarExpedienteJuez(juez);
                else if (op == "3") OrdenarExpedientesJuez(juez);
                else if (op == "4") MostrarEstadisticasJuez(juez, especialidad);
                else if (op != "5")
                {
                    Console.WriteLine("Opción inválida.");
                    Console.ReadKey();
                }
            }
        }
        static void MostrarExpedientesJuez(string juez)
        {
            Console.Clear();
            Titulo("EXPEDIENTES ASIGNADOS");
            bool enc = false;
            for (int i = 0; i < cantidad; i++)
                if (jueces[i] == juez)
                {
                    enc = true;
                    Console.WriteLine($"{codigos[i]} | {tiposDocumento[i]} | {fechasRegistro[i]} {MostrarHora12(horasRegistro[i])} | {estados[i]}");
                }
            if (!enc) Console.WriteLine("Ninguno asignado.");
            Console.ReadKey();
        }
        static void ConsultarExpedienteJuez(string juez)
        {
            Titulo("CONSULTAR EXPEDIENTE ASIGNADO");
            string codigo = Leer("Código: ").ToUpper();
            int pos = -1;
            for (int i = 0; i < cantidad; i++)
                if (codigos[i] == codigo && jueces[i] == juez) pos = i;

            if (pos == -1)
            {
                Console.WriteLine("No existe o no asignado.");
                Console.ReadKey();
                return;
            }

            string op = "";
            while (op != "4")
            {
                Console.Clear();
                MostrarDetalleExpedienteJuez(pos);
                Console.WriteLine("1. Abrir PDF principal\n2. Abrir medio probatorio\n3. Actualizar estado\n4. Volver\n");
                op = Leer("Opción: ");
                if (op == "1") Expedientes.AbrirArchivo(documentos[pos]);
                else if (op == "2") AbrirMedioProbatorio(pos);
                else if (op == "3") ActualizarEstadoExpediente(pos);
                else if (op != "4") { Console.WriteLine("Opción inválida."); Console.ReadKey(); }
            }
        }
        static void MostrarDetalleExpedienteJuez(int p)
        {
            Titulo("DETALLE DEL EXPEDIENTE " + codigos[p]);
            Console.WriteLine("Estado actual        : " + estados[p]);
            Seccion("DATOS DEL USUARIO");
            Console.WriteLine($"Nombre               : {nombres[p]}\nDNI                  : {dnis[p]}");
            Console.WriteLine($"Teléfono             : {telefonos[p]}\nCorreo               : {correos[p]}");
            Seccion("DATOS DEL EXPEDIENTE");
            Console.WriteLine($"Documento            : {tiposDocumento[p]}\nEspecialidad         : {especialidades[p]}");
            Console.WriteLine($"Fecha de registro    : {fechasRegistro[p]}\nHora de registro     : {MostrarHora12(horasRegistro[p])}");
            Console.WriteLine($"Descripción          : {descripcionDocumento[p]}");
            Console.WriteLine($"Archivo principal    : {Path.GetFileName(documentos[p])}\nFolios               : {foliosDocumentos[p]}");
            Seccion("MEDIOS PROBATORIOS (" + cantidadMedios[p] + ")");
            if (cantidadMedios[p] == 0) Console.WriteLine("No se adjuntaron medios adicionales.");
            for (int i = 0; i < cantidadMedios[p]; i++)
            {
                Console.WriteLine($"\nMedio {i + 1}: {tiposMedios[p, i]}");
                Console.WriteLine("Archivo              : " + Path.GetFileName(archivosMedios[p, i]));
                Console.WriteLine("Descripción          : " + descripcionMedios[p, i]);
            }
            Seccion("OPCIONES DEL JUEZ");
        }

        static void AbrirMedioProbatorio(int p)
        {
            if (cantidadMedios[p] == 0)
            {
                Console.WriteLine("Sin medios.");
                Console.ReadKey();
                return;
            }
            Seccion("SELECCIONAR MEDIO PROBATORIO");
            for (int i = 0; i < cantidadMedios[p]; i++)
                Console.WriteLine($"{i + 1}. {tiposMedios[p, i]} - {Path.GetFileName(archivosMedios[p, i])}");
            int n;
            if (!int.TryParse(Leer("Opción: "), out n) || n < 1 || n > cantidadMedios[p])
            {
                Console.WriteLine("Inválido.");
                Console.ReadKey();
                return;
            }
            Expedientes.AbrirArchivo(archivosMedios[p, n - 1]);
        }

        static void ActualizarEstadoExpediente(int p)
        {
            Console.Clear();
            Titulo("ACTUALIZAR ESTADO");
            Console.WriteLine($"Expediente  : {codigos[p]}\nEstado actual: {estados[p]}");
            Seccion("SELECCIONE EL NUEVO ESTADO");
            Console.WriteLine("1. REGISTRADO\n2. EN REVISIÓN\n3. ATENDIDO\n4. Cancelar\n");
            string op = Leer("Opción: ");
            if (op == "1") estados[p] = "REGISTRADO";
            else if (op == "2") estados[p] = "EN REVISIÓN";
            else if (op == "3") estados[p] = "ATENDIDO";
            else if (op == "4") return;
            else { Console.WriteLine("Inválido."); Console.ReadKey(); return; }

            Persistencia.GuardarExpedientes();
            string pdf = estados[p] == "EN REVISIÓN" ? DocumentosPDF.GenerarPDFRevision(p) : estados[p] == "ATENDIDO" ? DocumentosPDF.GenerarPDFAtendido(p) : "";
            Console.WriteLine("Estado actualizado.");
            if (pdf != "")
            {
                Console.WriteLine("PDF: " + pdf);
                Correos.EnviarCorreoExpediente(p, pdf, estados[p]);
            }
            Console.ReadKey();
        }

        static void OrdenarExpedientesJuez(string juez)
        {
            int total = ContarExpedientesJuez(juez);
            if (total == 0)
            {
                Console.WriteLine("Ninguno para ordenar.");
                Console.ReadKey();
                return;
            }

            var cod = new string[total];
            var tip = new string[total];
            var fec = new string[total];
            var hor = new string[total];
            var est = new string[total];
            int idx = 0;
            for (int i = 0; i < cantidad; i++)
                if (jueces[i] == juez)
                {
                    cod[idx] = codigos[i];
                    tip[idx] = tiposDocumento[i];
                    fec[idx] = fechasRegistro[i];
                    hor[idx] = horasRegistro[i];
                    est[idx] = estados[i];
                    idx++;
                }

            Console.Clear();
            Console.WriteLine("1. Ascendente  2. Descendente");
            string op = Leer("Opción: ");
            if (op != "1" && op != "2")
            {
                Console.WriteLine("Inválido.");
                Console.ReadKey();
                return;
            }
            for (int i = 0; i < total - 1; i++)
                for (int j = 0; j < total - 1 - i; j++)
                {
                    var f1 = ObtenerFechaHora(fec[j], hor[j]);
                    var f2 = ObtenerFechaHora(fec[j + 1], hor[j + 1]);
                    if ((op == "1" && f1 > f2) || (op == "2" && f1 < f2))
                    {
                        string t;
                        t = cod[j]; cod[j] = cod[j + 1]; cod[j + 1] = t;
                        t = tip[j]; tip[j] = tip[j + 1]; tip[j + 1] = t;
                        t = fec[j]; fec[j] = fec[j + 1]; fec[j + 1] = t;
                        t = hor[j]; hor[j] = hor[j + 1]; hor[j + 1] = t;
                        t = est[j]; est[j] = est[j + 1]; est[j + 1] = t;
                    }
                }

            Titulo("RESULTADO DEL ORDENAMIENTO");
            for (int i = 0; i < total; i++)
                Console.WriteLine($"{cod[i]} | {tip[i]} | {fec[i]} {MostrarHora12(hor[i])} | {est[i]}");
            Console.ReadKey();
        }
        static void MostrarEstadisticasJuez(string juez, string esp)
        {
            int dem = 0, esc = 0, sol = 0, reg = 0, rev = 0, ate = 0;
            for (int i = 0; i < cantidad; i++)
                if (jueces[i] == juez)
                {
                    if (tiposDocumento[i] == "DEMANDA") dem++;
                    else if (tiposDocumento[i] == "ESCRITO") esc++;
                    else if (tiposDocumento[i] == "SOLICITUD") sol++;
                    if (estados[i] == "REGISTRADO") reg++;
                    else if (estados[i] == "EN REVISIÓN") rev++;
                    else if (estados[i] == "ATENDIDO") ate++;
                }

            Console.Clear();
            Titulo("ESTADÍSTICAS - " + esp);
            Seccion("TIPOS DE DOCUMENTO");
            Console.WriteLine($"Total       : {dem + esc + sol}\nDemandas    : {dem}\nEscritos    : {esc}\nSolicitudes : {sol}");
            Seccion("ESTADOS DE LOS EXPEDIENTES");
            Console.WriteLine($"Registrados : {reg}\nEn revisión : {rev}\nAtendidos   : {ate}");
            if (dem + esc + sol == 0) Console.WriteLine("Sin datos.");
            else if (dem > esc && dem > sol) Console.WriteLine($"Más frecuente: DEMANDA ({dem})");
            else if (esc > dem && esc > sol) Console.WriteLine($"Más frecuente: ESCRITO ({esc})");
            else if (sol > dem && sol > esc) Console.WriteLine($"Más frecuente: SOLICITUD ({sol})");
            else Console.WriteLine("Empate entre tipos.");
            Console.ReadKey();
        }
    }
}
