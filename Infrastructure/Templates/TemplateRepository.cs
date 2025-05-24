using CommunicationFunction.Core.Configuration;
using CommunicationFunction.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Infrastructure.Templates
{
    /// <summary>
    /// Repository for managing email/notification templates (in-memory, file, or database).
    /// </summary>
    public class TemplateRepository : ITemplateRepository
    {
        private readonly AppSettings _appSettings;
        private readonly Dictionary<string, string> _inMemoryTemplates = new();

        public TemplateRepository(AppSettings appSettings)
        {
            _appSettings = appSettings;
            LoadDefaultTemplates();
        }

        /// <summary>
        /// Gets a template by id and type.
        /// </summary>
        public async Task<string?> GetTemplateAsync(string templateId, string templateType)
        {
            var key = $"{templateType}:{templateId}";

            switch (_appSettings.Templates.TemplateStorageType.ToLower())
            {
                case "inmemory":
                    return _inMemoryTemplates.TryGetValue(key, out var template) ? template : null;

                case "file":
                    return await GetTemplateFromFileAsync(templateId, templateType);

                default:
                    return _inMemoryTemplates.TryGetValue(key, out var defaultTemplate) ? defaultTemplate : null;
            }
        }

        /// <summary>
        /// Saves or updates a template.
        /// </summary>
        public async Task SaveTemplateAsync(string templateId, string templateType, string content)
        {
            var key = $"{templateType}:{templateId}";

            switch (_appSettings.Templates.TemplateStorageType.ToLower())
            {
                case "inmemory":
                    _inMemoryTemplates[key] = content;
                    break;

                case "file":
                    await SaveTemplateToFileAsync(templateId, templateType, content);
                    break;
            }
        }

        /// <summary>
        /// Checks if a template exists.
        /// </summary>
        public async Task<bool> TemplateExistsAsync(string templateId, string templateType)
        {
            var template = await GetTemplateAsync(templateId, templateType);
            return !string.IsNullOrEmpty(template);
        }

        private void LoadDefaultTemplates()
        {
            // Email templates
            _inMemoryTemplates["Email:welcome"] = @"
                <html>
                    <body>
                        <h1>Welcome {{name}}!</h1>
                        <p>Thank you for joining us on {{date}}.</p>
                        <p>Best regards,<br/>{{senderName}}</p>
                    </body>
                </html>";

            _inMemoryTemplates["Email:notification"] = @"
                <html>
                    <body>
                        <h2>{{title}}</h2>
                        <p>{{message}}</p>
                        <p>Time: {{timestamp}}</p>
                        <p>Best regards,<br/>{{senderName}}</p>
                    </body>
                </html>";

            // Notification templates (format: title|body)
            _inMemoryTemplates["Notification:alert"] = "Alert: {{alertType}}|{{message}} - {{timestamp}}";
            _inMemoryTemplates["Notification:reminder"] = "Reminder|Don't forget: {{task}} is due {{dueDate}}";
            _inMemoryTemplates["Notification:welcome"] = "Welcome {{name}}!|Thank you for joining us. Get started now!";
        }

        private async Task<string?> GetTemplateFromFileAsync(string templateId, string templateType)
        {
            var fileName = $"{templateType}_{templateId}.txt";
            var filePath = Path.Combine(_appSettings.Templates.TemplateBasePath, fileName);

            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }

            return null;
        }

        private async Task SaveTemplateToFileAsync(string templateId, string templateType, string content)
        {
            var fileName = $"{templateType}_{templateId}.txt";
            var filePath = Path.Combine(_appSettings.Templates.TemplateBasePath, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, content);
        }
    }
}
