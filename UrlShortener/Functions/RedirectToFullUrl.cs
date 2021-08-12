using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
                log.LogInformation($"Start processing code: {code}");
                
                var entity = await _tableStorageRepository.GetAsync(_codeGenerator.GetShortCode(code), code);

                if (entity == null)
                {
                    log.LogInformation($"No entity found for code: {code}");
                    return new NotFoundObjectResult(new {message = "No record found"});
                }
                
                return new RedirectResult(entity.FullUrl);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong. Exception message: {ex.Message}");
                throw;
            }
            finally
            {
                log.LogInformation($"Finished processing code: {code}");
            }
        }
    }
}
