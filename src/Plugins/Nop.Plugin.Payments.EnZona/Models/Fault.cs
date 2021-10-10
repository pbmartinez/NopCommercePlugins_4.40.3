namespace Nop.Plugin.Payments.EnZona.Models
{
    public class Fault
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public override string ToString() => $"{Code}: {Message}";
    }
}
