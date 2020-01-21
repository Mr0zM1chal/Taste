using System;
using System.Collections.Generic;
using System.Text;
using Taste.DataAccess.Data.Reposiritory.IReposirtory;
using Taste.Models;

namespace Taste.DataAccess.Data.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetails);
    }
}
