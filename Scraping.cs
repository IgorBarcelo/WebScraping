using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping
{
    internal class Scraping
    {
        public static async Task<List<Product>> ScrapeProductsAsync()
        {
            List<Product> products = new();

            // URL da página que lista os produtos
            string baseUrl = "https://www.tbca.net.br/base-dados/composicao_estatistica.php?pagina={0}&atuald=1#";

            int pageNumber = 1; // Número da página inicial

            int productNumber = 0;

            int prodId = 1;

            while (true)
            {
                // Construir o URL da página atual
                string url = string.Format(baseUrl, pageNumber);

                // Realizar a requisição HTTP para obter o conteúdo da página
                HttpClient httpClient = new();
                string html = await httpClient.GetStringAsync(url);

                // Criar um documento HtmlAgilityPack a partir do HTML obtido
                HtmlAgilityPack.HtmlDocument doc = new();
                doc.LoadHtml(html);

                // Identificar os elementos que contêm os dados dos produtos usando XPath
                var productNodes = doc.DocumentNode.SelectNodes("/html/body/div/main/div/table/tbody/tr");

                // Verificar se foram encontrados produtos
                if (productNodes != null)
                {
                    // Iterar sobre os elementos dos produtos e extrair os dados relevantes
                    foreach (var productNode in productNodes)
                    {
                        // Extrair o código, nome, nome científico e grupo
                        int productId = prodId;
                        string productCode = productNode.SelectSingleNode("td[1]").InnerText;
                        string productName = productNode.SelectSingleNode("td[2]").InnerText;
                        string productNameScientific = productNode.SelectSingleNode("td[3]").InnerText;
                        string productGroup = productNode.SelectSingleNode("td[4]").InnerText;

                        // Constrói o URL dos detalhes do produto usando o código
                        string productDetailsUrl = $"https://www.tbca.net.br/base-dados/int_composicao_estatistica.php?cod_produto={productCode}";

                        // Criar uma instância de Product e adicionar à lista
                        products.Add(new Product(productId, productCode, productName, productNameScientific, productGroup));
                        Debug.WriteLine($"produto {productNumber} cadastrado");
                        productNumber++;
                        prodId++;
                    }

                }
                else
                {
                    // Se não houver mais produtos, sair do loop
                    break;
                }

                // Avançar para a proxima página
                pageNumber++;
            }

            return products;
        }
        public static async Task<Dictionary<int, List<ProductSpec>>> ScrapeProductDetailsAsync(List<Product> products)
        {
            Dictionary<int, List<ProductSpec>> productSpecsDict = new();

            int productNumber = 0;

            foreach (var product in products)
            {
                // Construir o URL dos detalhes do produto usando o código
                string productDetailsUrl = $"https://www.tbca.net.br/base-dados/int_composicao_estatistica.php?cod_produto={product.Code}";

                // Realizar a requisição HTTP para obter o conteúdo da página de detalhes
                HttpClient httpClient = new();
                string html = await httpClient.GetStringAsync(productDetailsUrl);

                // Criar um documento HtmlAgilityPack a partir do HTML obtido
                HtmlAgilityPack.HtmlDocument doc = new();
                doc.LoadHtml(html);

                // Identificar os elementos que contêm os dados dos produtos usando XPath
                var productNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tabela1\"]/tbody/tr");

                if (productNodes != null)
                {
                    List<ProductSpec> productSpecs = new();

                    // Iterar sobre os elementos dos produtos e extrair os dados relevantes
                    foreach (var productNode in productNodes)
                    {
                        // Extrair os dados dos detalhes do produto usando XPath
                        string? component = productNode.SelectSingleNode("td[1]").InnerText;
                        string? units = productNode.SelectSingleNode("td[2]").InnerText;
                        float? valuePer = float.TryParse(productNode.SelectSingleNode("td[3]").InnerText, out float result) ? result : (float?)null;
                        float? standardDeviation = float.TryParse(productNode.SelectSingleNode("td[4]").InnerText, out result) ? result : (float?)null;
                        float? minimumValue = float.TryParse(productNode.SelectSingleNode("td[5]").InnerText, out result) ? result : (float?)null;
                        float? maximumValue = float.TryParse(productNode.SelectSingleNode("td[6]").InnerText, out result) ? result : (float?)null;
                        float? ndataUsed = float.TryParse(productNode.SelectSingleNode("td[7]").InnerText, out result) ? result : (float?)null;
                        float? @ref = float.TryParse(productNode.SelectSingleNode("td[8]").InnerText, out result) ? result : (float?)null;
                        string? dataType = productNode.SelectSingleNode("td[9]").InnerText;

                        // Cria uma instância de ProductSpec com os detalhes do produto e adiciona à lista
                        productSpecs.Add(new ProductSpec(0, product.Id, component, units, valuePer, standardDeviation, minimumValue, maximumValue, 
                            ndataUsed, @ref, dataType));
                        Debug.WriteLine($"composição {productNumber} cadastrada");
                        productNumber++;
                    }

                    // Adicionar os ProductSpecs do produto atual ao dicionário
                    productSpecsDict.Add(product.Id, productSpecs);

                }
            }

            return productSpecsDict;
        }
    }
}
