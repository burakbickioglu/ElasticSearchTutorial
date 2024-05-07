using ElasticSearch.API.DTOs;
using Nest;

namespace ElasticSearch.API.Models
{
    public class Product
    {
        [PropertyName("_id")]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public ProductFeature? Feature { get; set; }

        public ProductDto CreateProductDto()
        {
            if (Feature == null)
            {
                return new ProductDto(Id, Name, Price, Stock, null);
            }
            return new ProductDto(Id, Name, Price, Stock, new ProductFeatureDto(Feature.Width, Feature.Height, Feature.Color.ToString()));
        }
    }
}
