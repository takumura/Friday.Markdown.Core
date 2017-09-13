using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft​.Extensions​.Primitives;

namespace Takumura.Friday.Markdown.Core
{
    public class DocumentManager
    {
        readonly IFileProvider _fileProvider;
        readonly ILogger _logger;
        readonly IOptions<DocumentServiceOptions> _options;
        MarkdownPipeline _pipeline;

        public DocumentManager(
            IFileProvider fileProvider,
            ILogger<DocumentManager> logger,
            IOptions<DocumentServiceOptions> options)
        {
            this._fileProvider = fileProvider;
            this._logger = logger;
            this._options = options;

            _pipeline = new MarkdownPipelineBuilder()
                .UsePragmaLines()
                .UseDiagrams()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter()
                .Build();
        }

        public async Task<Dictionary<string, DocumentBase>> SetupDictionaryAsync() => await AddToDictionaryAsync("", new Dictionary<string, DocumentBase>());

        private async Task<Dictionary<string, DocumentBase>> AddToDictionaryAsync(string basePath, Dictionary<string, DocumentBase> dic)
        {
            //_logger.LogInformation($"BasePath: {basePath}");
            var contents = _fileProvider.GetDirectoryContents(basePath);
            //_logger.LogInformation($"Contents Count: {contents.Count()}");

            try
            {
                foreach (var item in contents)
                {
                    //_logger.LogInformation($"IsDirectory: {item.IsDirectory.ToString()}");
                    if (!item.IsDirectory)
                    {
                        if (!item.Name.StartsWith("_") && item.Name.EndsWith(".md"))
                        {
                            //_logger.LogInformation($"GetDocumentFromMetadataAsync start.");
                            var doc = await GetDocumentFromMetadataAsync(item, contents, basePath);
                            //_logger.LogInformation($"GetDocumentFromMetadataAsync end.");

                            //_logger.LogInformation($"GetMarkdownDocumentAsync start.");
                            doc.Description = await GetMarkdownDocumentAsync(item);
                            //_logger.LogInformation($"GetMarkdownDocumentAsync end.");

                            dic.Add(GetKey(basePath, item.Name.Replace(".md", "")).Replace("\\", "/"), doc);
                        }
                    }
                    else
                    {
                        var subPath = GetKey(basePath, item.Name);
                        // _logger.LogInformation($"SubPath: {subPath}");
                        await AddToDictionaryAsync(subPath, dic);
                    }
                }

                return dic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        private string GetKey(string basePath, string itemName) =>
            string.IsNullOrWhiteSpace(basePath) ? itemName : basePath + "\\" + itemName;

        private async Task<DocumentBase> GetDocumentFromMetadataAsync(IFileInfo item, IDirectoryContents contents, string basePath)
        {
            DocumentBase doc;
            var fileName = item.Name.Replace(".md", ".metadata.json");
            // _logger.LogInformation($"fileName: {fileName}");

            var metadataFile = contents.Where(x => x.Name == fileName).FirstOrDefault();
            if (metadataFile != null)
            {
                using (var stream = metadataFile.CreateReadStream())
                using (var sr = new StreamReader(stream))
                {
                    var content = await sr.ReadToEndAsync();
                    doc = this._options.Value.Factory.Create(content);
                }
                doc.FileName = item.Name;
                doc.MetadataMissing = false;
            }
            else
            {
                doc = this._options.Value.Factory.Create();
                doc.Title = item.Name;
                doc.FileName = item.Name;
                doc.MetadataMissing = true;
            }

            doc.Path = basePath.Replace("\\", "/");
            return doc;
        }
        private async Task<string> GetMarkdownDocumentAsync(IFileInfo item)
        {
            string description = string.Empty;
            // _logger.LogInformation($"Title: {doc.Title}, Path: {item.PhysicalPath}");
            using (var stream = item.CreateReadStream())
            using (var sr = new StreamReader(stream))
            {
                var content = await sr.ReadToEndAsync();
                description = Markdig.Markdown.ToHtml(content, _pipeline);
            }

            return description;
        }


        public IChangeToken GetChangeToken(string filter = "**") =>
            _fileProvider.Watch(filter);
    }
}
