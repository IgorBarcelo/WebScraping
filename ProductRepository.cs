using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Windows.Forms;
using System.Xml.Linq;
using Dapper;

namespace WebScraping
{
    public class ProductRepository
    {
        private readonly string connectionString;

        public ProductRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Método para recuperar os dados do banco de dados
        public async Task<List<Product>> GetProductsFromDatabase()
        {
            List<Product> products = new();

            // Conectar ao banco de dados e executar a consulta SQL para selecionar os produtos
            using (MySqlConnection dbConnection = new(connectionString))
            {
                await dbConnection.OpenAsync();
                string sql = "SELECT * FROM product";
                var productEnumerable = await dbConnection.QueryAsync<Product>(sql);
                products = productEnumerable.ToList();
            }

            return products;
        }

        public async Task<List<ProductSpec>> GetProductSpecsFromDatabase(int productId)
        {
            List<ProductSpec> productSpecs = new();

            // Conectar ao banco de dados e executar a consulta SQL para selecionar as especificações dos produtos
            using (MySqlConnection dbConnection = new(connectionString))
            {
                await dbConnection.OpenAsync();
                string sql = "SELECT * FROM productspec WHERE Id = @ProductId";
                var productSpecEnumerable = await dbConnection.QueryAsync<ProductSpec>(sql, new { ProductId = productId });
                productSpecs = productSpecEnumerable.ToList();
            }

            return productSpecs;
        }

        // Método para buscar um produto pelo campo "Name"
        public async Task<List<Product>> GetProductByName(string name)
        {
            using MySqlConnection dbConnection = new(connectionString);
            await dbConnection.OpenAsync();
            string sql = "SELECT * FROM product WHERE Name LIKE CONCAT(@Name, '%')";
            return (await dbConnection.QueryAsync<Product>(sql, new { Name = name })).ToList();
        }

        // Método para atualizar um produto no banco de dados
        public async Task UpdateProduct(Product product)
        {
            using MySqlConnection dbConnection = new(connectionString);
            await dbConnection.OpenAsync();
            string sql = "UPDATE product SET Code = @Code, Name = @Name, NameScientific = @NameScientific, Product_Group = @ProductGroup WHERE Id = @Id";
            await dbConnection.ExecuteAsync(sql, product);
        }

        // Método para deletar um produto do banco de dados
        public async Task DeleteProduct(int id)
        {
            using MySqlConnection dbConnection = new(connectionString);
            await dbConnection.OpenAsync();
            string sql = "DELETE FROM product WHERE Id = @Id";
            await dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        // Método para atualizar um produto no banco de dados
        public async Task UpdateProductSpec(ProductSpec productspec)
        {
            using MySqlConnection dbConnection = new(connectionString);
            await dbConnection.OpenAsync();
            string sql = "UPDATE productspec SET Id = @Id, Component = @Component, Units = @Units, ValuePer = @ValuePer, StandardDeviation = @StandardDeviation, " +
                "MinimumValue = @MinimumValue, MaximumValue = @MaximumValue, NdataUsed = @NdataUsed, Ref = @Ref, DataType = @DataType WHERE Id_spec = @Id_Spec";
            await dbConnection.ExecuteAsync(sql, productspec);
        }

        // Método para deletar um produto do banco de dados
        public async Task DeleteProductSpec(int id_spec)
        {
            using MySqlConnection dbConnection = new(connectionString);
            await dbConnection.OpenAsync();
            string sql = "DELETE FROM productspec WHERE Id_spec = @Id_Spec";
            await dbConnection.ExecuteAsync(sql, new { Id_spec = id_spec });
        }


    }
}
