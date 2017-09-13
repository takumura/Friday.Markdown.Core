namespace Takumura.Friday.Markdown.Core
{
    public class DocumentServiceOptions
    {
        public bool WatchFileChange { get; set; }
        public DocumentFactoryBase Factory { get; set; }
    }
}
