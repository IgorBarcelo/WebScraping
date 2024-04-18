using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using MySql.Data.MySqlClient;
using Dapper;
using System.Runtime.InteropServices;

namespace WebScraping
{
    public partial class Form1 : Form
    {
        private readonly DatabaseHelper dbHelper;
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=WebScraping;Uid=root;Pwd=1234;";
        private readonly ProductRepository productRepository;
        
        public Form1()
        {
            InitializeComponent();

            // Verificar se o banco de dados e as tabelas existem
            if (!DatabaseHelper.CheckExistenceofDatabaseandTables())
            {
                // Se não existirem, criar o banco de dados e as tabelas
                DatabaseHelper.CreateDatabaseAndTables();
            }

            dbHelper = new DatabaseHelper(connectionString); // Instancia a classe DatabaseHelper
            productRepository = new ProductRepository(connectionString); // Instancia a classe ProductRepository
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Assim que o formulário for carregado, vincula o evento SelectionChanged do DataGridView1
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            FillDataGridView(); // Método para preencher o DataGridView1

        }


        #region DataGridView
        // Método para preencher o DataGridView com os produtos recuperados do banco de dados
        private async void FillDataGridView()
        {
            List<Product> products = await productRepository.GetProductsFromDatabase();
            dataGridView1.DataSource = products;

            // Ajuste automático das colunas
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);


            // Renomeia as colunas conforme necessário
            dataGridView1.Columns["Id"].HeaderText = "ID";
            dataGridView1.Columns["Code"].HeaderText = "Código";
            dataGridView1.Columns["Name"].HeaderText = "Nome";
            dataGridView1.Columns["NameScientific"].HeaderText = "Nome Científico";
            dataGridView1.Columns["ProductGroup"].HeaderText = "Grupo";
        }

        private async void FillDataGridViewSpec(int productId)
        {
            List<ProductSpec> productSpecs = await productRepository.GetProductSpecsFromDatabase(productId);


            if (productSpecs != null && productSpecs.Any())
            {
                // Atualiza o controle de interface do usuário na thread principal
                dataGridView2.BeginInvoke((MethodInvoker)delegate
                {
                    // Define a fonte de dados antes de renomear as colunas
                    dataGridView2.DataSource = productSpecs;

                    // Renomeia as colunas conforme necessário
                    dataGridView2.Columns["Id_Spec"].HeaderText = "ID";
                    dataGridView2.Columns["Component"].HeaderText = "Componente";
                    dataGridView2.Columns["Units"].HeaderText = "Unidades";
                    dataGridView2.Columns["ValuePer"].HeaderText = "Valor por";
                    dataGridView2.Columns["StandardDeviation"].HeaderText = "Desvio Padrão";
                    dataGridView2.Columns["MinimumValue"].HeaderText = "Valor Mínimo";
                    dataGridView2.Columns["MaximumValue"].HeaderText = "Valor Máximo";
                    dataGridView2.Columns["NdataUsed"].HeaderText = "Dados Utilizados";
                    dataGridView2.Columns["Ref"].HeaderText = "Referência";
                    dataGridView2.Columns["DataType"].HeaderText = "Tipo de Dados";

                    // Oculta coluna do dataGridView2
                    dataGridView2.Columns["Id"].Visible = false;

                    // Ajusta automaticamente as colunas
                    dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                });
            }
            else
            {
                MessageBox.Show("Nenhuma especificação de produto encontrada.");
            }
        }
        private async void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Verifica se uma linha está selecionada no DataGridView1
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtém o ID do produto selecionado
                int productId = ((Product)dataGridView1.SelectedRows[0].DataBoundItem).Id;

                // Chama o método para preencher o DataGridView2 com as especificações do produto com base no ID do produto selecionado
                await Task.Run(() => FillDataGridViewSpec(productId)); // Executa FillDataGridViewSpec em uma thread separada
            }
        }
        #endregion
        #region BntGrid
        // Evento para buscar um produto pelo campo "Name"
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string name = txtSearch.Text;
            List<Product> products = await productRepository.GetProductByName(name);
            if (products != null && products.Any())
            {
                // Se encontrado, exibe os dados do produto
                dataGridView1.DataSource = products;
            }
            else
            {
                MessageBox.Show("Produto não encontrado.");
            }
        }

        // Evento para atualizar um produto
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow) // Ignora a última linha em branco
                    {
                        // Obtém o produto editado na linha
                        Product editedProduct = (Product)row.DataBoundItem;

                        // Atualiza o produto no banco de dados
                        await productRepository.UpdateProduct(editedProduct);
                    }
                }

                // Atualiza o DataGridView para refletir as mudanças
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar os produtos: " + ex.Message);
            }
        }

        //Evento para deletar um produto
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            // Verifica se uma linha está selecionada no DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtém o ID do produto selecionado
                int productId = ((Product)dataGridView1.SelectedRows[0].DataBoundItem).Id;

                // Deleta o produto do banco de dados
                await productRepository.DeleteProduct(productId);

                // Atualiza o DataGridView após deletar o produto
                FillDataGridView();
            }
            else
            {
                MessageBox.Show("Selecione um produto para deletar.");
            }
        }
        private async void btnUpdateSpec_Click_1(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (!row.IsNewRow) // Ignora a última linha em branco
                    {
                        // Obtém o produto editado na linha
                        ProductSpec editedProductSpec = (ProductSpec)row.DataBoundItem;

                        // Atualiza o produto no banco de dados
                        await productRepository.UpdateProductSpec(editedProductSpec);

                        // Obtém o productId do ProductSpec atualizado
                        int productId = editedProductSpec.Id;

                        // Atualiza o DataGridView2 com as especificações atualizadas
                        FillDataGridViewSpec(productId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar os produtos: " + ex.Message);
            }
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            // Verifica se uma linha está selecionada no DataGridView
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // Obtém a especificação do produto selecionado
                ProductSpec selectedProductSpec = (ProductSpec)dataGridView2.SelectedRows[0].DataBoundItem;

                // Obtém o ID do produto associado à especificação
                int productId = selectedProductSpec.Id;

                // Deleta a especificação do produto do banco de dados
                await productRepository.DeleteProductSpec(selectedProductSpec.Id_Spec);

                // Atualiza o DataGridView2 com as especificações atualizadas
                FillDataGridViewSpec(productId);
            }
            else
            {
                MessageBox.Show("Selecione um produto para deletar.");
            }
        }
        #endregion
        #region BtnTools
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Realizar o scraping dos produtos
                List<Product> products = await Scraping.ScrapeProductsAsync();

                // Realizar o scraping das especificações dos produtos
                Dictionary<int, List<ProductSpec>> productSpecsDict = await Scraping.ScrapeProductDetailsAsync(products);

                // Criar uma lista de todos os ProductSpecs
                List<ProductSpec> allProductSpecs = productSpecsDict.Values.SelectMany(specList => specList).ToList();

                // Chama o método para inserir os produtos no banco de dados
                await dbHelper.InsertDataAsync(products, allProductSpecs);

                MessageBox.Show("Dados inseridos com sucesso no banco de dados.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inserir dados no banco de dados: " + ex.Message);
            }
        }

        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            DatabaseHelper.CreateDatabaseAndTables();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMax.Visible = false;
            btnRes.Visible = true;
        }

        private void btnRes_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnRes.Visible = false;
            btnMax.Visible = true;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int IParam);

        private void barTittle_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("*Alimento* " +
                "\nBuscar: Digite no campo ao lado do botão e clique em buscar" +
                "\nAlterar: Clique três vezes no campo que deseja alterar e digite o novo valor e precione enter." +
                "\nDeletar: Clique no campo ao lado esquerdo da coluna ID, ele selecionará a linha do intem, após isso clique em Deletar." +
                "" +
                "\n\n*Composição* " +
                "\nAlterar: Clique três vezes no campo que deseja alterar e digite o novo valor e precione enter." +
                "\nDeletar: Clique no campo ao lado esquerdo da coluna ID, ele selecionará a linha do intem, após isso clique em Deletar.");
        }
        #endregion
    }
}
