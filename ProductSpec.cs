using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping
{
    public class ProductSpec
    {
        //Classe onde armazena ID, código, componente, unidades, valor por 100g, desvio padrao, valor minimo, valor máximo, n de dados, ref, tipo de dados
        public int Id_Spec { get; set; }
        public int Id { get; set; }
        public string Component { get; set; }
        public string Units { get; set; }
        public float? ValuePer { get; set; }
        public float? StandardDeviation { get; set; }
        public float? MinimumValue { get; set; }
        public float? MaximumValue { get; set; }
        public float? NdataUsed { get; set; }
        public float? Ref { get; set; }
        public string DataType { get; set; }

        public ProductSpec(int id_spec, int id, string component, string units, float? valuePer, float? standardDeviation, 
            float? minimumValue, float? maximumValue, float? ndataUsed, float? @ref, string dataType)
        {
            //método construtor
            Id_Spec = id_spec;
            Id = id;
            Component = component;
            Units = units;
            ValuePer = valuePer;
            StandardDeviation = standardDeviation;
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
            NdataUsed = ndataUsed;
            Ref = @ref;
            DataType = dataType;
        }
    }
}
