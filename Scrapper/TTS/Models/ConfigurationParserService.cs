using Microsoft.Extensions.Configuration;
using System.Drawing;
using TTS.Interfaces;

namespace TTS.Models
{
    public class ConfigurationParserService
    {
        IConfiguration _configuration;
        public ConfigurationParserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IModelOption? ConfigureOptions(string url)
        {
            IModelOption options = null;
            foreach(string key in WebsiteCatalog.Catalog.Keys)
            {
                if(url.ToLower().Contains(key.ToLower()))                    
                {
                    Type myType = WebsiteCatalog.Catalog[key];
                    options = Activator.CreateInstance(myType) as IModelOption;
                    _configuration.GetSection(options?.InstanceSectionName).Bind(options);
                }
            }
            return options;
        }
    }
}