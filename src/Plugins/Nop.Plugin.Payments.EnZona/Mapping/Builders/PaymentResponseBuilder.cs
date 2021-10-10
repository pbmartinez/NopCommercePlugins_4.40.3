using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.EnZona.Models;

namespace Nop.Plugin.Payments.EnZona.Mapping.Builders
{
    public class PaymentResponseBuilder : NopEntityBuilder<PaymentResponse>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            //map the primary key (not necessary if it is Id field)
            table
                .WithColumn(nameof(PaymentResponse.Id)).AsInt32().PrimaryKey().Identity()

            .WithColumn(nameof(PaymentResponse.CreatedAt)).AsDateTime()
            .WithColumn(nameof(PaymentResponse.UpdatedAt)).AsDateTime()
            .WithColumn(nameof(PaymentResponse.StatusCode)).AsInt32()
            .WithColumn(nameof(PaymentResponse.Commission)).AsDouble();
        }
    }
}
