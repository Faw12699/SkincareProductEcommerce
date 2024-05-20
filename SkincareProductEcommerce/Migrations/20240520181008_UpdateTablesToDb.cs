using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkincareProductEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Cleansers & Lotions");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Face Masks");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Face Serums");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[] { 4, 2, "Creams and Emulsions" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ", "Purifying Light Foam" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ", "Hydra-Mask" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[] { 2, "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ", "Anti-Age Defence Mask" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[] { 1, "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ", "Micellar Water Face & Eyes" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[] { 3, "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ", "Anti-Age Booster Serum" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "History");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Sci-fi");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Action");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "A lightweight serum that hydrates and plumps skin", "Hydrating Serum" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Gentle cleanser for acne-prone skin", "Acne Cleanser " });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[] { 1, "Brightening cream enriched with Vitamin C", "Vitamin C Cream" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[] { 3, "Anti-aging serum with retinol", "Retinol Serum" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[] { 2, "Broad-spectrum sunscreen with SPF", "SPF 50 Sunscreen" });
        }
    }
}
