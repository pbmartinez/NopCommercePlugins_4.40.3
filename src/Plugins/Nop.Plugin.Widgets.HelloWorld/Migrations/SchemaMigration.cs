using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.HelloWorld.Domains;

namespace Nop.Plugin.Widgets.HelloWorld.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/07/21 08:40:55:1687541", "Nop.Plugin.Widgets.HelloWorld schema")]
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
            _migrationManager.BuildTable<Presentacion>(Create);
        }
    }
}
