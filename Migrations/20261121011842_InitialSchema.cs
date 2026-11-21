using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_stocks");

            migrationBuilder.DropColumn(
                name: "manufacturer",
                table: "products");

            migrationBuilder.AlterColumn<decimal>(
                name: "cost",
                table: "products",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "household_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "low_stock_threshold",
                table: "products",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "quantity",
                table: "products",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "reorder_quantity",
                table: "products",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "products",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "processed_restocks",
                columns: table => new
                {
                    shopping_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processed_restocks", x => x.shopping_item_id);
                    table.ForeignKey(
                        name: "fk_processed_restocks_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_household_id_name",
                table: "products",
                columns: new[] { "household_id", "name" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_products_cost_non_negative",
                table: "products",
                sql: "cost >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_products_quantity_non_negative",
                table: "products",
                sql: "quantity >= 0");

            migrationBuilder.CreateIndex(
                name: "ix_processed_restocks_product_id",
                table: "processed_restocks",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "processed_restocks");

            migrationBuilder.DropIndex(
                name: "ix_products_household_id_name",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_products_cost_non_negative",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_products_quantity_non_negative",
                table: "products");

            migrationBuilder.DropColumn(
                name: "household_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "low_stock_threshold",
                table: "products");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "products");

            migrationBuilder.DropColumn(
                name: "reorder_quantity",
                table: "products");

            migrationBuilder.DropColumn(
                name: "unit",
                table: "products");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "products");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "products");

            migrationBuilder.AlterColumn<float>(
                name: "cost",
                table: "products",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "manufacturer",
                table: "products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "product_stocks",
                columns: table => new
                {
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_stocks", x => new { x.household_id, x.product_id });
                    table.ForeignKey(
                        name: "fk_product_stocks_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_stocks_household_id",
                table: "product_stocks",
                column: "household_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_stocks_product_id",
                table: "product_stocks",
                column: "product_id");
        }
    }
}
