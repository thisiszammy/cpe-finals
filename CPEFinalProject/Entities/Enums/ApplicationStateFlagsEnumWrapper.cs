using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.Enums
{
    public class ApplicationStateFlagsEnumWrapper
    {
        ApplicationStateFlagsEnum pageEnum;

        public ApplicationStateFlagsEnumWrapper(ApplicationStateFlagsEnum pageEnum)
        {
            this.pageEnum = pageEnum;
        }

        public ApplicationStateFlagsEnum PageEnum { get => pageEnum; set => pageEnum = value; }

        public override string ToString()
        {
            DescriptionAttribute attribute = PageEnum.GetType()
            .GetField(PageEnum.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() as DescriptionAttribute;

            return attribute.Description;
        }

    }
}
