using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WebScraping
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static bool CheckExistenceofDatabaseandTables()
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=1234;";

            using MySqlConnection connection = new(connectionString);
            try
            {
                connection.Open();

                // Verificar se o banco de dados existe
                MySqlCommand command = new("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'WebScraping'", connection);
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    return false; // Banco de dados não existe
                }

                // Verificar se as tabelas existem
                command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'WebScraping' AND table_name IN ('product', 'productspec')";
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count == 2; // Verificar se todas as tabelas existem
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao verificar a existência do banco de dados e tabelas: " + ex.Message);
                return false;
            }
        }

        public static void CreateDatabaseAndTables()
        {

            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=1234;";

            // Criar a conexão com o MySQL
            using MySqlConnection connection = new(connectionString);
            connection.Open();

            // Criar o comando para criar o banco de dados
            string createDatabaseQuery = "CREATE DATABASE IF NOT EXISTS WebScraping;";
            MySqlCommand createDatabaseCommand = new(createDatabaseQuery, connection);
            createDatabaseCommand.ExecuteNonQuery();

            // Alterar a conexão para utilizar o banco de dados recém-criado
            connection.ChangeDatabase("WebScraping");

            // Criar o comando para criar a tabela Product
            string createProductTableQuery = @"
            CREATE TABLE IF NOT EXISTS Product (
                id int NOT NULL,
                code varchar(50) DEFAULT NULL,
                name varchar(255) DEFAULT NULL,
                nameScientific varchar(100) DEFAULT NULL,
                product_group varchar(50) DEFAULT NULL
            );";
            MySqlCommand createProductTableCommand = new(createProductTableQuery, connection);
            createProductTableCommand.ExecuteNonQuery();

            // Criar o comando para criar a tabela ProductSpec
            string createProductSpecTableQuery = @"
            CREATE TABLE IF NOT EXISTS ProductSpec (
                id_spec INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                id INT NOT NULL,
                component VARCHAR(30) DEFAULT NULL,
                units VARCHAR(5) DEFAULT NULL,
                valuePer FLOAT(8,2) DEFAULT NULL,
                standardDeviation FLOAT(8,2) DEFAULT NULL,
                minimumValue FLOAT(8,2) DEFAULT NULL,
                maximumValue FLOAT(8,2) DEFAULT NULL,
                ndataUsed FLOAT(8,2) DEFAULT NULL,
                ref FLOAT DEFAULT NULL,
                dataType VARCHAR(10) DEFAULT NULL
            );";
            MySqlCommand createProductSpecTableCommand = new(createProductSpecTableQuery, connection);
            createProductSpecTableCommand.ExecuteNonQuery();
            MessageBox.Show("Banco de dados e tabelas criados com sucesso!");
        }

        public async Task InsertDataAsync(List<Product> products, List<ProductSpec> allProductSpecs)
        {
            // Abre uma conexão com o banco de dados utilizando um objeto MySqlConnection
            using MySqlConnection dbConnection = new(connectionString);
            // Abre a conexão com o banco de dados
            await dbConnection.OpenAsync();

            // Exclui todos os dados da tabela 'product'
            string deleteProductSql = "DELETE FROM product";
            await dbConnection.ExecuteAsync(deleteProductSql);

            // Exclui todos os dados da tabela 'productspec'
            string deleteProductSpecSql = "DELETE FROM productspec";
            await dbConnection.ExecuteAsync(deleteProductSpecSql);

            // Loop sobre cada produto na lista de produtos
            foreach (var product in products)
            {
                // Cria uma string SQL para inserir um novo produto na tabela 'product'
                string sql = "INSERT INTO product (Id, Code, Name, NameScientific, Product_Group) VALUES (@Id, @Code, @Name, @NameScientific, @ProductGroup)";

                // Executa a consulta SQL para inserir o produto na tabela 'product'
                await dbConnection.ExecuteAsync(sql, product);
            }

            // Loop sobre cada ProductSpec na lista de ProductSpecs
            foreach (var allProductSpec in allProductSpecs)
            {
                // Cria uma string SQL para inserir uma nova ProductSpec na tabela 'productspec'
                string sql = "INSERT INTO productspec (Id, Component, Units, ValuePer, StandardDeviation, MinimumValue, MaximumValue, NdataUsed, Ref, " +
                    "DataType) VALUES (@Id, @Component, @Units, @ValuePer, @StandardDeviation, @MinimumValue, @MaximumValue, @NdataUsed, @Ref, @DataType)";

                // Executa a consulta SQL para inserir a ProductSpec na tabela 'productspec'
                await dbConnection.ExecuteAsync(sql, allProductSpec);
            }
        }
    }

}