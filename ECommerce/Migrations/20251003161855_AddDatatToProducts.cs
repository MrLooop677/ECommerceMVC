using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddDatatToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
               name: "Discount",
               table: "Products",
               type: "decimal(18,2)",
               nullable: false,
               defaultValue: 0m);
            migrationBuilder.Sql("insert into Products (name, description, status, price, quantity, rate, mainImg, categoryId, brandId, Discount) values ('iphone11', 'Donec semper sapien a libero. Nam dui. Proin leo odio, porttitor id, consequat in, consequat ut, nulla. Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi.', 1, 791.81, 9601, 0.1, '5.png', 6, 1, 55);insert into Products (name, description, status, price, quantity, rate, mainImg, categoryId, brandId, Discount) values ('iphone11 pro', 'Suspendisse ornare consequat lectus. In est risus, auctor sed, tristique in, tempus sit amet, sem. Fusce consequat. Nulla nisl. Nunc nisl. Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi, in porttitor pede justo eu massa. Donec dapibus. Duis at velit eu est congue elementum. In hac habitasse platea dictumst.', 1, 481.78, 8279, 1.2, '1.png', 6, 1, 6);insert into Products (name, description, status, price, quantity, rate, mainImg, categoryId, brandId, Discount) values ('iphone11 pro max', 'Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh. In quis justo. Maecenas rhoncus aliquam lacus. Morbi quis tortor id nulla ultrices aliquet.', 1, 539.99, 7157, 0.3, '4.png', 6, 1, 10);insert into Products (name, description, status, price, quantity, rate, mainImg, categoryId, brandId, Discount) values ('iphone12', 'In congue. Etiam justo. Etiam pretium iaculis justo. In hac habitasse platea dictumst. Etiam faucibus cursus urna. Ut tellus. Nulla ut erat id mauris vulputate elementum.', 1, 731.23, 15973, 2.8, '2.png', 6, 1, 76);insert into Products (name, description, status, price, quantity, rate, mainImg, categoryId, brandId, Discount) values ('iphone12 pro', 'Nunc nisl. Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi, in porttitor pede justo eu massa.', 1, 136.98, 5741, 0.5, '4.png', 6, 1, 50);");
           

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Products");
            migrationBuilder.Sql("DELETE FROM Products");
        }

    }
}
