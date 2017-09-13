using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Takumura.Friday.Markdown.Core
{
    public class FridayMiddleware
    {
        readonly RequestDelegate _next;
        readonly DocumentService _service;
        readonly ILogger _logger;
        readonly IOptions<DocumentServiceOptions> _options;

        public FridayMiddleware(RequestDelegate next,
            DocumentService handler,
            ILogger<FridayMiddleware> logger,
            IOptions<DocumentServiceOptions> options)
        {
            this._next = next;
            this._service = handler;
            this._logger = logger;
            this._options = options;

            //_logger.LogInformation($"watchFileChange: {this._options.Value.WatchFileChange}");
            if (this._options.Value.WatchFileChange)
            {
                SubscribeFileWatch().ConfigureAwait(false);
            }
        }
        private async Task SubscribeFileWatch()
        {
            //_logger.LogInformation($"File watch start.");
            //var watch = _service.WatchDocumentAsync();
            //_logger.LogInformation($"File watch end.");
            //
            //watch.ToObservable().Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(
            //    value => { },
            //    error => _logger.LogInformation($"OnError = {error.Message}"),
            //    () =>
            //    {
            //        _logger.LogInformation("Reset Dictionary start");
            //        var setupTask = _service.SetupDictionaryAsync();
            //        _logger.LogInformation("Reset Dictionary end");

            //        _logger.LogInformation($"File watch start.");
            //        var fileWatchAgainTask = SubscribeFileWatch();
            //        _logger.LogInformation($"File watch end.");

            //        await Task.WhenAll(setupTask, fileWatchAgainTask);
            //    });

            _logger.LogInformation($"File watch start.");
            await _service.WatchDocumentAsync();

            while (true)
            {
                _logger.LogInformation($"File watch re-start.");
                var fileWatchAgainTask = _service.WatchDocumentAsync();
                _logger.LogInformation("Reset Dictionary");
                var setupTask = _service.SetupDictionaryAsync();

                await Task.WhenAll(setupTask, fileWatchAgainTask);
            }
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
        }
    }
}
