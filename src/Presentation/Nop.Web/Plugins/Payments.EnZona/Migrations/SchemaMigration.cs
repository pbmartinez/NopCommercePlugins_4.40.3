using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Payments.EnZona.Models;

namespace Nop.Plugin.Payments.EnZona.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/09/01 08:40:55:1687541", "Nop.Plugin.Payments.EnZona schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<PaymentResponse>(Create);
        }
    }
}
