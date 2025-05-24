using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.QueryHandler;
using CommunicationFunction.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.Templates
{
    public class ProcessTemplateQueryHandler : IProcessTemplateQueryHandler
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly AppSettings _appSettings;
        private readonly Dictionary<string, string> _templateCache = new();
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        public ProcessTemplateQueryHandler(ITemplateRepository templateRepository, AppSettings appSettings)
        {
            _templateRepository = templateRepository;
            _appSettings = appSettings;
        }

        public async Task<string> HandleAsync(ProcessTemplateQuery query)
        {
            var template = await GetTemplateAsync(query.TemplateId, query.TemplateType);
            return ProcessTemplate(template, query.Data);
        }

        private async Task<string> GetTemplateAsync(string templateId, string templateType)
        {
            var cacheKey = $"{templateType}:{templateId}";

            if (_appSettings.Templates.CacheTemplates)
            {
                await _cacheLock.WaitAsync();
                try
                {
                    if (_templateCache.TryGetValue(cacheKey, out var cachedTemplate))
                    {
                        return cachedTemplate;
                    }

                    var template = await _templateRepository.GetTemplateAsync(templateId, templateType);
                    if (template != null)
                    {
                        _templateCache[cacheKey] = template;
                        return template;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
            }

            var templateFromRepo = await _templateRepository.GetTemplateAsync(templateId, templateType);
            if (templateFromRepo == null)
            {
                throw new ArgumentException($"Template '{templateId}' of type '{templateType}' not found");
            }

            return templateFromRepo;
        }

        private string ProcessTemplate(string template, Dictionary<string, object> data)
        {
            var result = template;
            foreach (var kvp in data)
            {
                var placeholder = "{{" + kvp.Key + "}}";
                var value = kvp.Value?.ToString() ?? string.Empty;
                result = result.Replace(placeholder, value);
            }
            return result;
        }
    }
}
