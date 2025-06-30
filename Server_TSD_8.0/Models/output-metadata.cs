using System.Collections.Generic;

namespace Server_TSD.Models
{
    public class output_metadata
    {
        public List<Elements> elements { get; set; }
    }

    public class Elements
    {
        public string type { get; set; }
        public string[] filters { get; set; }
        public string[] attributes { get; set; }
        public string versionCode { get; set; }
        public string versionName { get; set; }
        public string outputFile { get; set; }
    }
}
