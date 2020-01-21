using System;
using System.Collections.Generic;
using System.Text;
using Taste.DataAccess.Data.Repository.IRepository;

namespace Taste.DataAccess.Data.Reposiritory.IReposirtory
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }

        IFoodTypeRepository FoodType{ get; }

        IMenuItemRepository MenuItem { get; }

        IApplicationUserRepository ApplicationUser { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        void Save();
    }
}
