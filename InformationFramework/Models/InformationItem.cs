using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationFramework.Presentation.Objects;

namespace InformationFramework.Models
{
    public class InformationItem 
    {
        public InformationItem()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public InformationItem Parent { get; set; }
        public IEnumerable<InformationItem> Items { get; set; }
        public IEnumerable<InformationProperty> Properties { get; set; }
        public PresentationObject PresentationObject { get; set; }


    }
    public static class InformationItemExtensions
    {
        public static InformationItem GetInformationItem(this IEnumerable<InformationItem> items, PresentationObject presentationobject)
        {
            var response = default(InformationItem);

            response =
                presentationobject != null && items != null ?
                items.FirstOrDefault(item => { return item.PresentationObject == presentationobject; }) :
                response
            ;

            return response;
        }
    }
}
