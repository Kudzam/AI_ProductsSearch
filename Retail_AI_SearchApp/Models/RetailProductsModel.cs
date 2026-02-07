using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail_AI_SearchApp.Models
{
    internal class RetailProductsModel
    {
        [VectorStoreKey]
        public int ID { get; set; }

        [VectorStoreData]
        public string ProductType { get; set; }

        [VectorStoreData]
        public string ProductLabel { get; set; }

        [VectorStoreData]
        public string ProductDescription { get; set; }

        [VectorStoreVector(Dimensions:384, DistanceFunction = DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }

    }
}
