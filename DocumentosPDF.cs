using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.IO;
using static Avances.Program;

namespace Avances
{
    // TEMA 5: DOCUMENTOS PDF BINARIOS CON PDFSHARP
    internal static class DocumentosPDF
    {
        static string CarpetaPDF()
        {
            string c = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDF_Generados");
            if (!Directory.Exists(c)) Directory.CreateDirectory(c);
            return c;
        }

        static string RutaPlantilla(string archivo)
        {
            string r = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", archivo);
            if (File.Exists(r)) return r;
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Referencias_Diseno", archivo));
        }

        static void EscribirDato(XGraphics g, string texto, double x, double y, double ancho, double tam, bool bold)
        {
            if (texto == null) texto = "";
            g.DrawString(texto, new XFont("Arial", tam, bold ? XFontStyle.Bold : XFontStyle.Regular), XBrushes.Black, new XRect(x, y, ancho, 16), XStringFormats.CenterLeft);
        }

        static void ColocarPlantilla(XGraphics g, string archivo)
        {
            string r = RutaPlantilla(archivo);
            if (!File.Exists(r)) throw new Exception("Plantilla no encontrada: " + r);
            using (var img = XImage.FromFile(r))
                g.DrawImage(img, 0, 0, 595, 842);
        }
        static void EscribirDatosUsuario(XGraphics g, int p, string est)
        {
            double x = est == "ATENDIDO" ? 242 : 232;
            double y = est == "REGISTRADO" ? 299 : est == "EN REVISIÓN" ? 310 : 329;
            double s = est == "REGISTRADO" ? 25.5 : 22.5;
            EscribirDato(g, nombres[p], x, est == "REGISTRADO" ? y + 5 : y, 310, 10, false);
            EscribirDato(g, dnis[p], x, y + s, 310, 10, false);
            EscribirDato(g, telefonos[p], x, y + s * 2, 310, 10, false);
            EscribirDato(g, correos[p], x, y + s * 3, 310, 9.4, false);
        }

        static void EscribirDatosExpediente(XGraphics g, int p, string est)
        {
            double x = est == "ATENDIDO" ? 242 : 232;
            double y = est == "REGISTRADO" ? 426 : est == "EN REVISIÓN" ? 433 : 453;
            double s = 20.4;
            EscribirDato(g, tiposDocumento[p], x, y, 310, 9.8, false);
            EscribirDato(g, especialidades[p], x, y + s, 310, 9.8, false);
            EscribirDato(g, Path.GetFileName(documentos[p]), x, y + s * 2, 310, 9.2, false);
            EscribirDato(g, descripcionDocumento[p], x, y + s * 3, 310, 9.2, false);
            EscribirDato(g, fechasRegistro[p], x, y + s * 4, 310, 9.6, false);
            EscribirDato(g, MostrarHora12(horasRegistro[p]), x, y + s * 5, 310, 9.6, false);

            if (est == "EN REVISIÓN")
            {
                EscribirDato(g, est, x + 15, y + s * 6 + 5, 130, 9.3, true);
                EscribirDato(g, jueces[p], x, y + s * 7 + 5, 310, 9.3, false);
            }
            else if (est == "ATENDIDO")
            {
                g.DrawString(est, new XFont("Arial", 9.3, XFontStyle.Bold), XBrushes.Black, new XRect(252, y + s * 6, 107, 16), XStringFormats.Center);
                EscribirDato(g, jueces[p], x, y + s * 7 + 7, 310, 9.3, false);
            }
            else if (est == "REGISTRADO")
            {
                g.DrawString(est, new XFont("Arial", 9.3, XFontStyle.Bold), XBrushes.Black, new XRect(245, y + s * 6 + 5, 126, 16), XStringFormats.Center);
                EscribirDato(g, jueces[p], x, y + s * 7 + 5, 310, 9.3, false);
            }
            else
            {
                EscribirDato(g, est, x, y + s * 6, 130, 9.3, true);
                EscribirDato(g, jueces[p], x, y + s * 7, 310, 9.3, false);
            }
        }
        static void EscribirMediosProbatorios(XGraphics g, int p, string est)
        {
            double y0 = est == "REGISTRADO" ? 660 : est == "EN REVISIÓN" ? 661 : est == "ATENDIDO" ? 686 : 646;
            double s = 20.8;
            if (cantidadMedios[p] == 0)
            {
                EscribirDato(g, "Sin medios probatorios adicionales", 110, y0, 420, 8.6, false);
                return;
            }
            for (int i = 0; i < cantidadMedios[p] && i < 3; i++)
            {
                double y = y0 + s * i;
                string desc = descripcionMedios[p, i] ?? "";
                if (desc.Length > 42) desc = desc.Substring(0, 42) + "...";
                var f = new XFont("Arial", 8.2, XFontStyle.Regular);

                if (est == "EN REVISIÓN")
                {
                    g.DrawString((i + 1).ToString(), f, XBrushes.Black, new XRect(47, y, 61, 16), XStringFormats.Center);
                    g.DrawString(tiposMedios[p, i], f, XBrushes.Black, new XRect(108, y, 104, 16), XStringFormats.Center);
                    g.DrawString(Path.GetFileName(archivosMedios[p, i]), f, XBrushes.Black, new XRect(212, y, 177, 16), XStringFormats.Center);
                    g.DrawString(desc, f, XBrushes.Black, new XRect(389, y, 185, 16), XStringFormats.Center);
                }
                else if (est == "ATENDIDO")
                {
                    g.DrawString((i + 1).ToString(), f, XBrushes.Black, new XRect(70, y, 29, 16), XStringFormats.Center);
                    g.DrawString(tiposMedios[p, i], f, XBrushes.Black, new XRect(99, y, 119, 16), XStringFormats.Center);
                    g.DrawString(Path.GetFileName(archivosMedios[p, i]), f, XBrushes.Black, new XRect(218, y, 172, 16), XStringFormats.Center);
                    g.DrawString(desc, f, XBrushes.Black, new XRect(390, y, 189, 16), XStringFormats.Center);
                }
                else
                {
                    g.DrawString((i + 1).ToString(), f, XBrushes.Black, new XRect(78, y, 29, 16), XStringFormats.Center);
                    g.DrawString(tiposMedios[p, i], f, XBrushes.Black, new XRect(107, y, 115, 16), XStringFormats.Center);
                    g.DrawString(Path.GetFileName(archivosMedios[p, i]), f, XBrushes.Black, new XRect(208, y, 182, 16), XStringFormats.Center);
                    g.DrawString(desc, f, XBrushes.Black, new XRect(390, y, 216, 16), XStringFormats.Center);
                }
            }
        }
        internal static string GenerarPDFRegistro(int p) => CrearPDF(p, "REGISTRADO", "01_REGISTRADO_VINO_A4.png", "CONSTANCIA_REGISTRO_");
        internal static string GenerarPDFRevision(int p) => CrearPDF(p, "EN REVISIÓN", "02_EN_REVISION_AZUL_A4.png", "ACTUALIZACION_REVISION_");
        internal static string GenerarPDFAtendido(int p) => CrearPDF(p, "ATENDIDO", "03_ATENDIDO_VERDE_A4.png", "ACTUALIZACION_ATENDIDO_");

        static string CrearPDF(int p, string est, string plantilla, string prefijo)
        {
            try
            {
                var pdf = new PdfDocument();
                var pag = pdf.AddPage();
                pag.Size = PdfSharp.PageSize.A4;
                var g = XGraphics.FromPdfPage(pag);
                ColocarPlantilla(g, plantilla);

                string fechaDoc = est == "REGISTRADO"
                    ? fechasRegistro[p] + "   " + MostrarHora12(horasRegistro[p])
                    : DateTime.Now.ToString("dd/MM/yyyy") + "   " + DateTime.Now.ToString("hh:mm tt").Replace("AM", "a. m.").Replace("PM", "p. m.");

                if (est == "EN REVISIÓN")
                {
                    g.DrawString(codigos[p], new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(430, 205, 138, 16), XStringFormats.Center);
                    g.DrawString(fechaDoc, new XFont("Arial", 8.2), XBrushes.Black, new XRect(430, 254, 138, 16), XStringFormats.Center);
                }
                else if (est == "ATENDIDO")
                {
                    g.DrawString(codigos[p], new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(430, 207, 138, 16), XStringFormats.Center);
                    g.DrawString(fechaDoc, new XFont("Arial", 8.2), XBrushes.Black, new XRect(430, 258, 138, 16), XStringFormats.Center);
                }
                else
                {
                    g.DrawString(codigos[p], new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(430, 194, 138, 16), XStringFormats.Center);
                    g.DrawString(fechaDoc, new XFont("Arial", 8.2), XBrushes.Black, new XRect(430, 239, 138, 16), XStringFormats.Center);
                }

                EscribirDatosUsuario(g, p, est);
                EscribirDatosExpediente(g, p, est);
                EscribirMediosProbatorios(g, p, est);

                if (est == "EN REVISIÓN")
                {
                    g.DrawRectangle(XBrushes.White, 20, 705, 555, 78);
                    g.DrawRectangle(new XSolidBrush(XColor.FromArgb(232, 244, 255)), 68, 711, 500, 58);
                    g.DrawRectangle(new XPen(XColor.FromArgb(180, 210, 235), 0.6), 68, 711, 500, 58);
                    var fa = new XFont("Arial", 11);
                    g.DrawString("Su documentación presentada está siendo revisada por el órgano", fa, XBrushes.Black, new XRect(83, 717, 470, 16), XStringFormats.CenterLeft);
                    g.DrawString("jurisdiccional correspondiente. En caso de requerirse información", fa, XBrushes.Black, new XRect(83, 734, 470, 16), XStringFormats.CenterLeft);
                    g.DrawString("adicional, se le comunicará oportunamente a través de este medio.", fa, XBrushes.Black, new XRect(83, 751, 470, 16), XStringFormats.CenterLeft);
                    var pen = new XPen(XColor.FromArgb(40, 110, 190), 2);
                    g.DrawRectangle(pen, 505, 724, 22, 28);
                    g.DrawLine(pen, 510, 733, 521, 733);
                    g.DrawLine(pen, 510, 739, 521, 739);
                    g.DrawLine(pen, 509, 745, 514, 750);
                    g.DrawLine(pen, 514, 750, 525, 738);
                }
                else if (est == "ATENDIDO")
                {
                    g.DrawRectangle(XBrushes.White, 20, 705, 555, 78);
                    g.DrawRectangle(new XSolidBrush(XColor.FromArgb(235, 248, 238)), 68, 711, 500, 58);
                    g.DrawRectangle(new XPen(XColor.FromArgb(155, 205, 165), 0.6), 68, 711, 500, 58);
                    var fv = new XFont("Arial", 11);
                    g.DrawString("La atención correspondiente a su expediente ha sido registrada", fv, XBrushes.Black, new XRect(83, 717, 400, 16), XStringFormats.CenterLeft);
                    g.DrawString("en el sistema. Puede consultar la información actualizada", fv, XBrushes.Black, new XRect(83, 734, 400, 16), XStringFormats.CenterLeft);
                    g.DrawString("ingresando a la Mesa de Partes Digital.", fv, XBrushes.Black, new XRect(83, 751, 400, 16), XStringFormats.CenterLeft);
                    var pen = new XPen(XColor.FromArgb(35, 130, 65), 2);
                    g.DrawRectangle(pen, 505, 724, 22, 28);
                    g.DrawLine(pen, 510, 733, 521, 733);
                    g.DrawLine(pen, 510, 739, 521, 739);
                    g.DrawLine(pen, 509, 745, 514, 750);
                    g.DrawLine(pen, 514, 750, 525, 738);
                }
                else if (est == "REGISTRADO")
                    g.DrawRectangle(XBrushes.White, 20, 753, 555, 27);

                string codCom = (est == "REGISTRADO" ? "CR-" : est == "EN REVISIÓN" ? "AR-" : "AA-") + codigos[p] + "-" + DateTime.Now.Year;
                g.DrawRectangle(XBrushes.White, 20, 780, 555, 55);
                EscribirDato(g, codCom, 30, 795, 250, 10, true);

                var fechaPie = est == "REGISTRADO" ? ObtenerFechaHora(fechasRegistro[p], horasRegistro[p]) : DateTime.Now;
                if (fechaPie == DateTime.MaxValue) fechaPie = DateTime.Now;
                string[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
                EscribirDato(g, "Lima, " + fechaPie.Day + " de " + meses[fechaPie.Month - 1] + " de " + fechaPie.Year, 400, 795, 175, 9.5, false);

                string ruta = Path.Combine(CarpetaPDF(), prefijo + codigos[p] + ".pdf");
                pdf.Save(ruta);
                pdf.Close();
                return ruta;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR PDF: " + ex.Message);
                return "";
            }
        }
    }
}
