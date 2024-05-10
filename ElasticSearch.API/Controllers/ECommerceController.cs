using ElasticSearch.API.Models.ECommerceModel;
using ElasticSearch.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _repository;

        public ECommerceController(ECommerceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName) //Eddie
        {
            return Ok(await _repository.TermQuery(customerFirstName));
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstNameList) //Eddie Underwood
        {
            return Ok(await _repository.TermsQuery(customerFirstNameList));
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string CustomerFullName) //Edd
        {
            return Ok(await _repository.PrefixQuery(CustomerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double FromPrice, double ToPrice) //100, 200
        {
            return Ok(await _repository.RangeQuery(FromPrice, ToPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery(int page = 1, int pageSize = 3)
        {
            return Ok(await _repository.MatchAllQuery(page, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string CustomerFullName) //R*
        {
            return Ok(await _repository.WildCardQuery(CustomerFullName));
        }
        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerName) //Diane, Deane
        {
            return Ok(await _repository.FuzzyQuery(customerName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchQueryFullText(string categoryName) //Women's
        {
            return Ok(await _repository.MatchQueryFullText(categoryName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixQueryFullText(string customerFullName) //Wilhemina St. Massey, Wilhemina St. Mas
        {
            return Ok(await _repository.MatchBoolPrefixQueryFullText(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchPhraseQueryFullText(string customerFullName) //Wilhemina St. Massey
        {
            return Ok(await _repository.MatchPhraseQueryFullText(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> CompaundQueryExampleOne(string cityName, double taxFulTotalPrice, string categoryName, string menufacturer) //New York, 100, Women's Clothing, Tigress Enterprises
        {
            return Ok(await _repository.CompaundQueryExampleOne(cityName, taxFulTotalPrice, categoryName, menufacturer));
        }

        [HttpGet]
        public async Task<IActionResult> CompaundQueryExampleTwo(string customerFullName) // Jason Bowers
        {
            return Ok(await _repository.CompaundQueryExampleTwo(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> MultiMatchQueryFullText(string name) //George
        {
            return Ok(await _repository.MultiMatchQueryFullText(name));
        }

    }
}
