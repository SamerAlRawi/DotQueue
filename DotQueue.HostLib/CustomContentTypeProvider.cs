using Microsoft.Owin.StaticFiles.ContentTypes;

namespace DotQueue.HostLib
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            //Mappings.Add(".js", "text/javascript");
        }
    }
}