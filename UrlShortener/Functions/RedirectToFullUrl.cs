using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Models.Dto;
using UrlShortener.Models.Entities;
using UrlShortener.Repository;

namespace UrlShortener.Functions
{
    public class RedirectToFullUrl
    {
        private readonly ITableStorageRepository<UrlCodeTableEntity> _tableStorageRepository;
        private readonly ICodeGenerator _codeGenerator;

        public RedirectToFullUrl(ITableStorageRepository<UrlCodeTableEntity> tableStorageRepository, ICodeGenerator codeGenerator)
        {
            _tableStorageRepository = tableStorageRepository;
            _codeGenerator = codeGenerator;
        }

        [FunctionName(nameof(RedirectToFullUrl))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{code}")] HttpRequest req, string code,
            ILogger log)
        {
            try
            {
                log.LogInformation($"{nameof(RedirectToFullUrl)} started");

                log.LogInformation($"Code: {code}");

                var entity = await _tableStorageRepository.GetAsync(partitionKey: _codeGenerator.GetShortCode(code), rowKey: code);

                if (entity == null)
                {
                    log.LogInformation($"No entity found for code: {code}");
                    return new NotFoundObjectResult(new ErrorResponse("No record found"));
                }

                log.LogInformation($"Redirecting to: {entity.FullUrl} for code: {code}");

                return new RedirectResult(entity.FullUrl);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong. Exception message: {ex.Message}");
                return new InternalServerErrorResult();
            }
            finally
            {
                log.LogInformation($"{nameof(RedirectToFullUrl)} finished");
            }
        }
    }
}
