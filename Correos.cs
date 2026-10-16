using System;
using System.Net;
using System.Net.Mail;
using static Avances.Program;

namespace Avances
{
    // SERVICIOS DEL SISTEMA: CORREO Y RECUPERACIÓN DE CONTRASEÑA (Gmail SMTP)
    // La cuenta y la clave de aplicación NO están en el código: se leen de variables
    // de entorno de Windows (MESA_CORREO y MESA_CLAVE_APP). Ver INSTRUCCIONES.txt.
    internal static class Correos
    {
        static string ObtenerCorreoSistema()
        {
            string c = Environment.GetEnvironmentVariable("MESA_CORREO");
            return string.IsNullOrWhiteSpace(c) ? "mesadepartes.csharp@gmail.com" : c.Trim();
        }

        // Gmail muestra la clave con espacios; se quitan para evitar errores.
        static string ObtenerClave()
        {
            string c = Environment.GetEnvironmentVariable("MESA_CLAVE_APP");
            return string.IsNullOrWhiteSpace(c) ? "" : c.Replace(" ", "").Trim();
        }

        static SmtpClient CrearSmtp(string correoSistema, string clave)
        {
            return new SmtpClient("smtp.gmail.com", 587) { EnableSsl = true, Credentials = new NetworkCredential(correoSistema, clave) };
        }

        internal static bool EnviarCodigoRecuperacion(string destino, string codigo)
        {
            string correoSistema = ObtenerCorreoSistema(), clave = ObtenerClave();
            if (clave == "")
            {
                Console.WriteLine("ERROR: Falta configurar la variable de entorno MESA_CLAVE_APP (ver INSTRUCCIONES.txt).");
                return false;
            }
            try
            {
                using (var msg = new MailMessage(new MailAddress(correoSistema, "Mesa de Partes Digital"), new MailAddress(destino))
                {
                    Subject = "Código de recuperación - Mesa de Partes Digital",
                    Body = $"Estimado(a):\n\nSu código es: {codigo}\nVálido 10 minutos.\n\nAtentamente,\nMesa de Partes Digital"
                })
                using (var smtp = CrearSmtp(correoSistema, clave))
                {
                    smtp.Send(msg);
                    Console.WriteLine("Código enviado.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
        internal static bool EnviarCorreoExpediente(int pos, string rutaPDF, string estado)
        {
            string correoSistema = ObtenerCorreoSistema(), clave = ObtenerClave();
            if (clave == "")
            {
                Console.WriteLine("AVISO: PDF generado pero no se envió el correo: falta la variable MESA_CLAVE_APP (ver INSTRUCCIONES.txt).");
                return false;
            }
            try
            {
                string asunto = estado == "REGISTRADO" ? $"Registro exitoso del expediente {codigos[pos]}" :
                                estado == "EN REVISIÓN" ? $"Actualización del expediente {codigos[pos]} - En revisión" :
                                $"Actualización del expediente {codigos[pos]} - Atendido";
                string cuerpo = estado == "REGISTRADO" ?
                    $"Estimada(o) {nombres[pos]}:\n\nSe confirma el registro de su expediente.\nCódigo: {codigos[pos]}\nEstado: REGISTRADO\n\nAtentamente,\nMesa de Partes Digital" :
                    $"Estimada(o) {nombres[pos]}:\n\nSu expediente {codigos[pos]} está {estado}.\nAdjuntamos actualización.\n\nAtentamente,\nMesa de Partes Digital";

                using (var msg = new MailMessage(new MailAddress(correoSistema, "Mesa de Partes Digital"), new MailAddress(correos[pos]))
                { Subject = asunto, Body = cuerpo })
                {
                    msg.Attachments.Add(new Attachment(rutaPDF));
                    using (var smtp = CrearSmtp(correoSistema, clave))
                        smtp.Send(msg);
                }
                Console.WriteLine("Correo enviado a: " + correos[pos]);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AVISO: PDF generado pero correo falló. " + ex.Message);
                return false;
            }
        }
    }
}
