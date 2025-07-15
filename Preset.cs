using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace mpegui
{
    public class Preset
    {
        public FileConversionInfo ConversionInfo { get; set; }
        public List<string> ToCopy { get; set; }

        public Preset() { }

        public Preset(FileConversionInfo f)
        {
            List<string> exclusions = new List<string>()
            {
                "Filename",
                "TrimStart",
                "TrimEnd",
                "TrimUseDuration",
                "OverwriteExisting",
            };
            ConversionInfo = f;
            IEnumerable<PropertyInfo> props = typeof(FileConversionInfo)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite &&
                // default exclusions
                !exclusions.Contains(p.Name));

            ToCopy = props.Select(p => p.Name).ToList();
        }
    }
}
