using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services.Interfaces
{
    public interface ICustomerGUIService
    {
        public void ListOrderHistory();
        public void ViewOrderDetails();
        public void EditProfile();

    }
}
