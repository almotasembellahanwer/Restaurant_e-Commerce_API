using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.IOrdersService
{
    public interface IOrdersDeleterService
    {
        void DeleteOrder(int orderID);
    }
}
