using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.Enums
{
    public enum ApplicationStateFlagsEnum
    {
        START,
        AUTHENTICATE,
        MENU_GENERAL,
        MENU_MANAGEMENT,
        MENU_CUSTOMER,
        LIST,
        CREATE,
        EDIT,
        DELETE,
        BACK,
        REPORTS,
        CUST_ORDER_HIST,
        CUST_ORDER_DET,
        CUST_PROFILE,
        EXIT
    }
}
