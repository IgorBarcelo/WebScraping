using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping
{
    public class Product
    {
        //Classe onde armazena ID, código, nome, nome científico e grupo dos produtos
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameScientific { get; set; }
        public string ProductGroup { get; set; }

        public Product(int id, string code, string name, string nameScientific, string product_Group)
        {
            //método construtor
            Id = id;
            Code = code;
            Name = name;
            NameScientific = nameScientific;
            ProductGroup = product_Group;
        }

    }

}