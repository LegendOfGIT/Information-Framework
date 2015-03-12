using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationFramework.Models
{
    public class InformationProperty
    {
        public const string Name = "Name";
        public const string Type = "Type";

        public InformationProperty()
        {
            ID =
            Label =
                string.Empty
            ;
        }

        public string ID { get; set; }
        public string Label { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
