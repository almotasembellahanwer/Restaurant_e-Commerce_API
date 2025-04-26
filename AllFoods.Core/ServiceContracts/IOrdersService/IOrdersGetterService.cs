using AllFoods.Core.DTO.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.IOrdersService
{
    public interface IOrdersGetterService
    {
        Task<List<OrderDTO>> GetAllOrders();
        Task<OrderDTO> GetOrderByID(int id);
    }
}
