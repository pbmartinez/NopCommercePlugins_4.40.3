using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.HelloWorld.Domains;

namespace Nop.Plugin.Widgets.HelloWorld.Mapping.Builders
{
    public class PresentacionBuilder : NopEntityBuilder<Presentacion>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Presentacion.Id)).AsString().PrimaryKey()
                .WithColumn(nameof(Presentacion.Mensaje)).AsString().NotNullable();
        }

        #endregion
    }
}