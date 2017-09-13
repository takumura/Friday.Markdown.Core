using Microsoft.AspNetCore.Builder;

namespace Takumura.Friday.Markdown.Core
{
    public static class FridayMiddlwareExtentions
    {
        public static IApplicationBuilder UseFriday(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FridayMiddleware>();
        }
    }
}
