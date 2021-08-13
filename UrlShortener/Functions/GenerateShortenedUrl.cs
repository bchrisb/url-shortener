using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "shorten")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation($"{nameof(GenerateShortenedUrl)} started");

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var shortenUrlRequest = JsonConvert.DeserializeObject<UrlShortenRequest>(requestBody);

                if (!Uri.TryCreate(shortenUrlRequest.Url, UriKind.Absolute, out var fullUrl))
                {
                    log.LogWarning("Request contained an invalid URL. Unable to shorten.");
                    return new BadRequestObjectResult(new ErrorResponse("Invalid URL. Unable to shorten"));
                }

                var code = await TryInsert(shortenUrlRequest.Url, log);

                var shortUrl = $"{req.Scheme}://{req.Host}/{code}";

                log.LogInformation($"Short URL: {shortUrl}");

                return new OkObjectResult(new UrlShortenResponse
                {
                    ShortUrl = shortUrl,
                    Code = code
                });
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong. Exception message: {ex.Message}");
                return new InternalServerErrorResult();
            }
            finally
            {
                log.LogInformation($"{nameof(GenerateShortenedUrl)} finished");
            }
        }

        private async Task<string> TryInsert(string fullUrl, ILogger log)
        {
            var uniqueCode = false;
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

                    log.LogInformation($"Successfully created entity: {entity.ToString()}");

                    uniqueCode = true;
                }
                catch (StorageException ex)
                {
                    if (ex.Message == "Conflict")
                    {
                        log.LogWarning("Code has been generated before and is present in table");
                    }
                }
            } while (!uniqueCode);

            return code;
        }
    }
}
