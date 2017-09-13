using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Takumura.Friday.Markdown.Core
{
    public class DocumentService
    {
        readonly DocumentManager _manager;
        readonly ILogger _logger;
        Dictionary<string, DocumentBase> Docs { get; set; }

        public DocumentService(DocumentManager manager, ILogger<DocumentService> logger)
        {
            this._manager = manager;
            this._logger = logger;
            //_logger.LogInformation($"DocumentService:Constructor - SetupDictionaryAsync start.");
            SetupDictionaryAsync().ConfigureAwait(false);
            //_logger.LogInformation($"DocumentService:Constructor - SetupDictionaryAsync end.");
        }
        public virtual async Task SetupDictionaryAsync()
        {
            //_logger.LogInformation($"SetupDictionaryAsync start.");
            Docs = await _manager.SetupDictionaryAsync();
            //_logger.LogInformation($"SetupDictionaryAsync end.");
        }

        public virtual Task WatchDocumentAsync()
        {
            var token = _manager.GetChangeToken();
            var tcs = new TaskCompletionSource<object>();
            token.RegisterChangeCallback(state =>
                ((TaskCompletionSource<object>)state).TrySetResult(null), tcs);
            return tcs.Task;
        }

        public virtual DocumentBase Get(string docPath) => Docs.FirstOrDefault(x => x.Key == docPath).Value;

        public virtual IEnumerable<DocumentBase> List(string docPath)
        {
            if (string.IsNullOrWhiteSpace(docPath))
            {
                return Docs.Select(x => x.Value).ToList();
            }

            return Docs.Where(x => x.Key.Contains(docPath))
                .Select(x => x.Value)
                .ToList();
        }
    }
}
