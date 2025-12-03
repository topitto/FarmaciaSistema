using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Diagnostics; // Para abrir el PDF automáticamente

namespace FarmaciaSistema.Desktop.Services
{
    public class RecetaPdfService
    {
        public void GenerarReceta(string nombrePaciente, string sintomas, string recetaTexto, string fecha)
        {
            // Definimos el nombre del archivo (ej. Receta_JuanPerez_20251030.pdf)
            string nombreArchivo = $"Receta_{nombrePaciente.Replace(" ", "")}_{DateTime.Now:yyyyMMddHHmm}.pdf";
            string rutaCompleta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nombreArchivo);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // --- ENCABEZADO ---
                    page.Header()
                        .Text(text =>
                        {
                            text.Span("Farmacia Sistema\n").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                            text.Span("Dr. Leonel (Cédula Prof. 123456)\n").FontSize(10);
                            text.Span($"Fecha: {fecha}").FontSize(10);
                        });

                    // --- CONTENIDO ---
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text($"Paciente: {nombrePaciente}").FontSize(14).Bold();

                            x.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2); // Línea separadora

                            x.Item().Text("Diagnóstico / Síntomas:").Bold();
                            x.Item().Text(sintomas);

                            x.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                            x.Item().Text("Tratamiento / Receta:").Bold();
                            x.Item().Text(recetaTexto);
                        });

                    // --- PIE DE PÁGINA ---
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Gracias por su preferencia. ").FontSize(10);
                            x.Span("Tel: 555-123-4567");
                        });
                });
            })
            .GeneratePdf(rutaCompleta);

            // Intentar abrir el PDF automáticamente
            try
            {
                Process.Start(new ProcessStartInfo(rutaCompleta) { UseShellExecute = true });
            }
            catch { /* Ignorar si no se puede abrir */ }
        }
    }
}
