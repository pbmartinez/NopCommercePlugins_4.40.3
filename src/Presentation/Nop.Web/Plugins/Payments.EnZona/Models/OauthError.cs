namespace Nop.Plugin.Payments.EnZona.Models
{
    public class OauthError
    {
        public string ErrorDescription { get; set; }
        public string Error { get; set; }

        public override string ToString() => $"{Error}: {ErrorDescription}";
    }
}
