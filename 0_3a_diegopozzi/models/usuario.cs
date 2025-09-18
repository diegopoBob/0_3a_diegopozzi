namespace _0_3a_diegopozzi.models
{
    public class Usuario
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = "";
        public string? CodigoVerificacion { get; set; }
        public bool Verificado { get; set; } = false;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}