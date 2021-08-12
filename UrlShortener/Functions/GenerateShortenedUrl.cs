using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrlShortener.Models.Dto;
using UrlShortener.Models.Entities;
using UrlShortener.Repository;

namespace UrlShortener.Functions
{
    public class GenerateShortenedUrl
    {
        private readonly ICodeGenerator _codeGenerator;
        private readonly ITableStorageRepository<UrlCodeTableEntity> _tableStorageRepository;

        public GenerateShortenedUrl(ICodeGenerator codeGenerator, ITableStorageRepository<UrlCodeTableEntity> tableStorageRepository)
        {
            _codeGenerator = codeGenerator;
            _tableStorageRepository = tableStorageRepository;
        }

        [FunctionName(nameof(GenerateShortenedUrl))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "shorten")] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var shortenUrlRequest = JsonConvert.DeserializeObject<UrlShortenRequest>(requestBody);

            var code = await TryInsert(shortenUrlRequest.Url);

            var shortenedUrl = $"{req.Scheme}://{req.Host}/{code}";

            return new OkObjectResult(new UrlShortenResponse
            {
                ShortUrl = shortenedUrl,
                Code = code
            });
        }

        private async Task<string> TryInsert(string fullUrl)
        {
            bool uniqueCode;
            string code = null;

            do
            {
                try
                {
                    code = _codeGenerator.Generate();

                    var entity = new UrlCodeTableEntity
                    {
                        PartitionKey = _codeGenerator.GetShortCode(code),
                        RowKey = code,
                        FullUrl = fullUrl
                    };

                    await _tableStorageRepository.InsertAsync(entity);

                    uniqueCode = true;
                }
                catch (Exception)
                {
                    uniqueCode = false;
                }
            } while (!uniqueCode);

            return code;
        }
    }
}
