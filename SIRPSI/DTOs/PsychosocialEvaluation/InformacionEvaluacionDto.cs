namespace SIRPSI.DTOs.PsychosocialEvaluation
{
    public class InformacionEvaluacionDto
    {
        public string? Id { get; set; } = null!;
        public bool? Finalizado { get; set; }
        public DateTime FechaInicio { get; set; }
        public string? NamePsicologo { get; set; }
        public string? documentoPsicologo { get; set; }
        public string? telefono { get; set; }
        public string? documentoEmpresa { get; set; }
    }
}
