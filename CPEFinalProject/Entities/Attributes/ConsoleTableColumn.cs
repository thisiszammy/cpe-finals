using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.Attributes
{
    public class ConsoleTableColumn : Attribute
    {
        public string displayName { get; set; }
        public int order { get; set; }

        public ConsoleTableColumn(string displayName, int order)
        {
            this.displayName = displayName;
            this.order = order;
        }
    }
}
