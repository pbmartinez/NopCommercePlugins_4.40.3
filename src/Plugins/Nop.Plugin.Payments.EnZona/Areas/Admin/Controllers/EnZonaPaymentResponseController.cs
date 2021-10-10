using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.EnZona.Models;
using Nop.Plugin.Payments.EnZona.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.EnZona.Areas.Admin.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin] //confirms access to the admin panel
    [Area(AreaNames.Admin)] //specifies the area containing a controller or action
    public class EnZonaPaymentResponseController : BaseController
    {
        private readonly IPaymentResponseService _paymentResponseService;

        public EnZonaPaymentResponseController(IPaymentResponseService paymentResponseService)
        {
            _paymentResponseService = paymentResponseService;
        }

        [HttpGet]
        public IActionResult List()
        {
            var paymentResponseSearchModel = new PaymentResponseSearchModel
            {
                AvailablePageSizes = "10"
            };
            return View(paymentResponseSearchModel);
        }

        [HttpPost]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(new DataTablesModel { Data = await _paymentResponseService.GetAllAsync() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
