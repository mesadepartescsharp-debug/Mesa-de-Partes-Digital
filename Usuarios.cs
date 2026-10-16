using System;
using static Avances.Program;

namespace Avances
{
    // TEMAS 1, 2 Y 3: REGISTRO, BÚSQUEDA Y MODIFICACIÓN DE USUARIOS
    internal static class Usuarios
    {
        internal static void RegistrarUsuario()
        {
            if (cantidadUsuarios >= usuariosNombres.Length) { Console.WriteLine("Capacidad máxima de usuarios."); Console.ReadKey(); return; }
            Console.Clear();
            Titulo("REGISTRO DE USUARIO");
            string nombre = LeerHasta("Nombre completo: ", Validaciones.ValidarNombre);
            string dni;
            do { dni = LeerHasta("DNI: ", Validaciones.ValidarDNI); } while (Validaciones.DNIUsuarioRegistrado(dni));
            string telefono = LeerHasta("Teléfono: ", Validaciones.ValidarTelefono);
            string correo;
            do { correo = LeerHasta("Correo: ", Validaciones.ValidarCorreo); } while (Validaciones.CorreoUsuarioRegistrado(correo));
            string pass = LeerHasta("Contraseña: ", Validaciones.ValidarContrasena);
            string conf = Leer("Confirmar contraseña: ");
            while (conf != pass) { Console.WriteLine("ERROR: No coinciden."); conf = Leer("Confirmar: "); }

            usuariosNombres[cantidadUsuarios] = nombre;
            usuariosDnis[cantidadUsuarios] = dni;
            usuariosTelefonos[cantidadUsuarios] = telefono;
            usuariosCorreos[cantidadUsuarios] = correo;
            usuariosContrasenas[cantidadUsuarios] = pass;
            usuarioActual = cantidadUsuarios++;
            Persistencia.GuardarUsuarios();

            Console.WriteLine($"\nCuenta creada. Bienvenido(a), {nombre}.\nPresione una tecla...");
            Console.ReadKey();
            MenuUsuarioRegistrado();
            usuarioActual = -1;
        }
        internal static bool IniciarSesionUsuario()
        {
            Console.Clear();
            Titulo("INICIAR SESIÓN - USUARIO");

            string correo = Leer("Correo: ");
            int pos = -1;

            for (int i = 0; i < cantidadUsuarios; i++)
            {
                if (usuariosCorreos[i].Equals(correo, StringComparison.OrdinalIgnoreCase))
                {
                    pos = i;
                    break;
                }
            }

            if (pos == -1)
            {
                Console.WriteLine("Correo no registrado.");
                Console.WriteLine("1. Recuperar contraseña");
                Console.WriteLine("2. Volver");

                if (Leer("Opción: ") == "1")
                    RecuperarContrasenaUsuario();

                return false;
            }
            if (usuariosBloqueados[pos])
            {
                Console.WriteLine("\nCUENTA BLOQUEADA.");
                Console.WriteLine("Debe recuperar su contraseña.");
                Console.ReadKey();
                RecuperarContrasenaUsuario(correo);
                return false;
            }
            while (intentosFallidos[pos] < 3)
            {
                string pass = Leer("\nContraseña: ");

                if (usuariosContrasenas[pos] == pass)
                {
                    intentosFallidos[pos] = 0;
                    usuarioActual = pos;

                    Console.WriteLine(
                        "\nInicio correcto. Bienvenido(a), " +
                        usuariosNombres[pos] + ".");

                    Console.ReadKey();
                    return true;
                }
                intentosFallidos[pos]++;

                Console.WriteLine(
                    "\nContraseña incorrecta. Intento " +
                    intentosFallidos[pos] + " de 3.");

                if (intentosFallidos[pos] < 3)
                {
                    Console.WriteLine(
                        "Intente nuevamente introducir la contraseña.");
                }
            }
            usuariosBloqueados[pos] = true;

            Console.WriteLine("\nCUENTA BLOQUEADA POR 3 INTENTOS.");
            Console.WriteLine("Se iniciará la recuperación de contraseña.");
            Console.ReadKey();

            RecuperarContrasenaUsuario(correo);
            return false;
        }
        internal static bool RecuperarContrasenaUsuario(string correoConocido = "")
        {
            Console.Clear();
            Titulo("RECUPERAR CONTRASEÑA");
            string correo = correoConocido;

            if (correo == "")
                correo = Leer("Correo registrado: ");
            else
                Console.WriteLine("Correo: " + correo);

            int pos = -1;

            for (int i = 0; i < cantidadUsuarios; i++)
            {
                if (usuariosCorreos[i].Equals(
                    correo, StringComparison.OrdinalIgnoreCase))
                {
                    pos = i;
                    break;
                }
            }
            if (pos == -1)
            {
                Console.WriteLine("Correo no encontrado.");
                Console.ReadKey();
                return false;
            }
            string codigo = new Random().Next(100000, 1000000).ToString();

            if (!Correos.EnviarCodigoRecuperacion(correo, codigo))
            {
                Console.WriteLine("No se pudo enviar el código.");
                Console.ReadKey();
                return false;
            }
            DateTime vencimiento = DateTime.Now.AddMinutes(10);

            Console.WriteLine("\nSe envió un código de 6 dígitos a Gmail.");
            Console.WriteLine("El código vence en 10 minutos.");

            for (int intento = 1; intento <= 3; intento++)
            {
                string ingresado = Leer("\nCódigo de verificación: ");

                if (DateTime.Now > vencimiento)
                {
                    Console.WriteLine("El código ha vencido.");
                    Console.ReadKey();
                    return false;
                }

                if (ingresado == codigo)
                {
                    Console.WriteLine("\nCódigo verificado correctamente.");
                    string nueva = LeerHasta(
                        "Nueva contraseña: ", Validaciones.ValidarContrasena);

                    string confirmar = Leer("Confirmar contraseña: ");

                    while (confirmar != nueva)
                    {
                        Console.WriteLine("Las contraseñas no coinciden.");
                        confirmar = Leer("Confirmar contraseña: ");
                    }

                    usuariosContrasenas[pos] = nueva;
                    intentosFallidos[pos] = 0;
                    usuariosBloqueados[pos] = false;
                    Persistencia.GuardarUsuarios();
                    Console.WriteLine("\nCONTRASEÑA ACTUALIZADA.");
                    Console.WriteLine(
                        "Regresará al menú de acceso para iniciar sesión.");

                    Console.ReadKey();
                    return true;
                }
                Console.WriteLine(
                    "Código incorrecto. Intento " + intento + " de 3.");
            }
            Console.WriteLine("\nSe agotaron los intentos de verificación.");
            Console.ReadKey();
            return false;
        }
        internal static void AccesoUsuario()
        {
            string op = "";
            while (op != "3")
            {
                Console.Clear();
                Titulo("ACCESO DE USUARIO");
                Console.WriteLine("1. Iniciar sesión\n2. Registrarme\n3. Volver\n");
                op = Leer("Opción: ");

                if (op == "1")
                {
                    if (IniciarSesionUsuario())
                    {
                        MenuUsuarioRegistrado();
                        usuarioActual = -1;
                    }
                }
                else if (op == "2") RegistrarUsuario();
                else if (op != "3")
                {
                    Console.WriteLine("Opción inválida.");
                    Console.ReadKey();
                }
            }
        }
        static void MenuUsuarioRegistrado()
        {
            string op = "";
            while (op != "5")
            {
                Console.Clear();
                Titulo("PANEL DEL USUARIO");
                if (usuarioActual >= 0) Console.WriteLine("Usuario: " + usuariosNombres[usuarioActual]);
                Seccion("OPCIONES DEL USUARIO");
                Console.WriteLine("1. Registrar expediente\n2. Mis expedientes\n3. Revisar estado\n4. Mi cuenta\n5. Cerrar sesión\n");
                op = Leer("Opción: ");
                if (op == "1") Expedientes.RegistrarExpediente();
                else if (op == "2") MostrarMisExpedientes();
                else if (op == "3") RevisarEstadoUsuario();
                else if (op == "4") MostrarCuentaUsuario();
                else if (op != "5") { Console.WriteLine("Opción inválida."); Console.ReadKey(); }
            }
        }
        static void MostrarCuentaUsuario()
        {
            string op = "";
            while (op != "3")
            {
                Console.Clear();
                Titulo("MI CUENTA");
                Console.WriteLine($"Nombre   : {usuariosNombres[usuarioActual]}\nDNI      : {usuariosDnis[usuarioActual]}");
                Console.WriteLine($"Teléfono : {usuariosTelefonos[usuarioActual]}\nCorreo   : {usuariosCorreos[usuarioActual]}");
                Seccion("OPCIONES DE CUENTA");
                Console.WriteLine("1. Modificar teléfono\n2. Modificar contraseña\n3. Volver\n");
                op = Leer("Opción: ");
                if (op == "1") ModificarTelefonoUsuario();
                else if (op == "2") ModificarContrasenaUsuario();
                else if (op != "3") { Console.WriteLine("Opción inválida."); Console.ReadKey(); }
            }
        }
        static void ModificarTelefonoUsuario()
        {
            Console.Clear();
            Console.WriteLine("Teléfono actual: " + usuariosTelefonos[usuarioActual]);
            usuariosTelefonos[usuarioActual] = LeerHasta("Nuevo teléfono: ", Validaciones.ValidarTelefono);
            Persistencia.GuardarUsuarios();
            Console.WriteLine("Modificado.");
            Console.ReadKey();
        }
        static void ModificarContrasenaUsuario()
        {
            Console.Clear();
            if (Leer("Contraseña actual: ") != usuariosContrasenas[usuarioActual])
            {
                Console.WriteLine("Incorrecta.");
                Console.ReadKey();
                return;
            }
            string nueva = LeerHasta("Nueva: ", Validaciones.ValidarContrasena);
            string conf = Leer("Confirmar: ");
            while (conf != nueva) { Console.WriteLine("No coinciden."); conf = Leer("Confirmar: "); }
            usuariosContrasenas[usuarioActual] = nueva;
            Persistencia.GuardarUsuarios();
            Console.WriteLine("Modificada.");
            Console.ReadKey();
        }

        static void MostrarMisExpedientes()
        {
            Console.Clear();
            Titulo("MIS EXPEDIENTES");
            int total = 0;
            void MostrarTotal() { Console.WriteLine("Total de expedientes: " + total); }
            for (int i = 0; i < TotalExpedientes; i++)
                if (usuarioActual >= 0 && propietariosDni[i] == usuariosDnis[usuarioActual])
                {
                    total++;
                    Console.WriteLine($"{codigos[i]} | {tiposDocumento[i]} | {especialidades[i]} | {fechasRegistro[i]} {MostrarHora12(horasRegistro[i])} | {estados[i]}");
                }
            if (total == 0) Console.WriteLine("Aún no tiene expedientes.");
            MostrarTotal();
            Console.ReadKey();
        }

        static void RevisarEstadoUsuario()
        {
            Console.Clear();
            string codigo = Leer("Código: ").ToUpper();
            int pos = -1;
            for (int i = 0; i < cantidad; i++)
                if (usuarioActual >= 0 && codigos[i] == codigo && propietariosDni[i] == usuariosDnis[usuarioActual]) pos = i;

            if (pos == -1) Console.WriteLine("No existe o no es suyo.");
            else
            {
                Seccion("RESULTADO DE LA CONSULTA");
                Console.WriteLine($"Código       : {codigos[pos]}\nDocumento    : {tiposDocumento[pos]}\nEspecialidad : {especialidades[pos]}");
                Console.WriteLine($"Fecha        : {fechasRegistro[pos]}\nHora         : {MostrarHora12(horasRegistro[pos])}");
                Console.WriteLine($"Estado       : {estados[pos]}\nJuez         : {jueces[pos]}");
            }
            Console.ReadKey();
        }
    }
}
