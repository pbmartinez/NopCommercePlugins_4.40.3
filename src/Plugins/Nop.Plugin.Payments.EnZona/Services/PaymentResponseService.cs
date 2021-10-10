using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Payments.EnZona.Models;

namespace Nop.Plugin.Payments.EnZona.Services
{
    public interface IPaymentResponseService
    {
        Task<int> InsertAsync(PaymentResponse item);

        Task<PaymentResponse> GetByTransaccionUuid(string transacctioUuid);
        /// <summary>
        /// Get all PaymentResponses ordered by InvoiceNumber
        /// </summary>
        /// <returns></returns>
        Task<List<PaymentResponse>> GetAllAsync();
    }
    public class PaymentResponseService : IPaymentResponseService
    {
        private readonly IRepository<PaymentResponse> _paymentResponseRepository;

        public PaymentResponseService(IRepository<PaymentResponse> paymentResponseRepository)
        {
            _paymentResponseRepository = paymentResponseRepository;
        }

        public async Task<int> InsertAsync(PaymentResponse item)
        {
            await _paymentResponseRepository.InsertAsync(item);
            var id = await _paymentResponseRepository.Table.MaxAsync(a => a.Id);
            return id;
        }

        public async Task<PaymentResponse> GetByTransaccionUuid(string transacctioUuid)
        {
            return await _paymentResponseRepository.Table.FirstOrDefaultAsync(p => p.TransactionUuid == transacctioUuid);
        }

        public async Task<List<PaymentResponse>> GetAllAsync()
        {
            return await _paymentResponseRepository.Table.OrderByDescending(a => a.InvoiceNumber).ToListAsync();
        }
    }
}
