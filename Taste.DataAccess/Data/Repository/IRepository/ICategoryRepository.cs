using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Taste.DataAccess.Data.Reposiritory.IReposirtory;
using Taste.Models;

namespace Taste.DataAccess.Data.Reposiritory
{
    public interface ICategoryRepository : IRepository<Category>
    {
         IEnumerable<SelectListItem> GetCategoryListForDropDown();

        void Update(Category category);

    }
}
