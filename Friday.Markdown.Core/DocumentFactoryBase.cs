namespace Takumura.Friday.Markdown.Core
{
    public abstract class DocumentFactoryBase
    {
        public DocumentBase Create() => CreateDocument();
        public DocumentBase Create(string doc) => CreateDocument(doc);

        protected abstract DocumentBase CreateDocument();
        protected abstract DocumentBase CreateDocument(string doc);
    }
}
