using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taste.DataAccess.Data.Reposiritory.IReposirtory;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork ;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            List<OrderDetailsVM> orderListsVM = new List<OrderDetailsVM>();
            IEnumerable<OrderHeader> OrderHeadersList;

            if (User.IsInRole(SD.CustomerRole))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                OrderHeadersList = _unitOfWork.OrderHeader.GetAll(u => u.UserId == claim.Value, null, "ApplicationUser");
            }
            else
            {
                OrderHeadersList = _unitOfWork.OrderHeader.GetAll(null , null, "ApplicationUser");

            }

            foreach (OrderHeader item in OrderHeadersList)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.Id == item.Id).ToList()
                };
            orderListsVM.Add(individual);
            }
            return Json(new { data = orderListsVM});
        }
    }
}