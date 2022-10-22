using CPEFinalProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.ViewModels
{
    public class ScrollerOption
    {
        public ScrollerOption(string label, Type destinationType, ApplicationStateFlagsEnum destinationStateFlagEnum)
        {
            Label = label;
            DestinationType = destinationType;
            DestinationStateFlagEnum = destinationStateFlagEnum;
        }

        public string Label { get; set; }
        public Type DestinationType { get; set; }
        public ApplicationStateFlagsEnum DestinationStateFlagEnum { get; set; }

    }
}
