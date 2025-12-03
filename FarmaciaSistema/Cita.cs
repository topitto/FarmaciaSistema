// En FarmaciaSistema.Domain/Cita.cs
using System;

namespace FarmaciaSistema.Domain
{
    public class Cita
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Sintomas { get; set; }
        public string Receta { get; set; } // Aquí va el texto de los medicamentos recetados

        // Relación con el Cliente (Paciente)
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
