using ElasticSearch.API.Models;
using Nest;

namespace ElasticSearch.API.DTOs
{
    public record ProductDto(string Id, string Name, decimal Price, int Stock, ProductFeatureDto? Feature)
    {
    }
}
