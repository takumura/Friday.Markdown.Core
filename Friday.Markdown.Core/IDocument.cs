using System;

namespace Takumura.Friday.Markdown.Core
{
    public interface IDocument
    {
        string Title { get; set; }
        DateTime DateCreated { get; set; }
        string CreatedByUser { get; set; }
        DateTime? DateUpdated { get; set; }
        string UpdatedByUser { get; set; }
        string Description { get; set; }
        string Path { get; set; }
        string FileName { get; set; }
        bool MetadataMissing { get; set; }
    }
}
