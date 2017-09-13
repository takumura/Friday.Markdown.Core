using System;

namespace Takumura.Friday.Markdown.Core
{
    public class DocumentBase : IDocument
    {
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedByUser { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public bool MetadataMissing { get; set; }
    }
}
