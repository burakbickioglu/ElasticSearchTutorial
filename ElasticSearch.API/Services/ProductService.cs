using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repositories;
using System.Collections.Immutable;
using System.Net;
namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _productRepository.SaveAsync(request.CreateProduct());
            if (response == null)
                return ResponseDto<ProductDto>.Fail("Kayıt esnasında bir hata meydana geldi", System.Net.HttpStatusCode.InternalServerError);

            return ResponseDto<ProductDto>.Success(response.CreateProductDto(), HttpStatusCode.OK);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productListDto = products.Select(x => new ProductDto(x.Id, x.Name, x.Price, x.Stock, x.Feature != null ? new ProductFeatureDto(x.Feature.Width, x.Feature.Height, x.Feature.Color.ToString()) : null)).ToList();

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün bulunamadı", HttpStatusCode.NotFound);
            }
            return ResponseDto<ProductDto>.Success(product.CreateProductDto(), HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _productRepository.UpdateAsync(updateProduct);
            if (!response)
                return ResponseDto<bool>.Fail("Güncelleme Esnasında Bir hata meydana geldi", HttpStatusCode.InternalServerError);

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }
        public async Task<ResponseDto<bool>> DeleteAsync(string Id)
        {
            var deleteResponse = await _productRepository.DeleteAsync(Id);
            if(!deleteResponse.IsValid && deleteResponse.Result == Nest.Result.NotFound)
                return ResponseDto<bool>.Fail("Silmeye çalıştığınız ürün bulunamamıştır.", HttpStatusCode.NotFound);

            if (!deleteResponse.IsValid)
            {
                _logger.LogError(deleteResponse.OriginalException, deleteResponse.ServerError.ToString());
                return ResponseDto<bool>.Fail("Silme Esnasında Bir hata meydana geldi", HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }
    }
}
