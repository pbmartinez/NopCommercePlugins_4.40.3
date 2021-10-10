using Nop.Core;
using Nop.Plugin.Widgets.HelloWorld.Validators;

namespace Nop.Plugin.Widgets.HelloWorld.Domains
{
    //[Validator(typeof(PresentacionValidator))]
    public partial class Presentacion : BaseEntity
    {
        public string Mensaje { get; set; }
    }
}
