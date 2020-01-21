using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Reposiritory.IReposirtory;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public OrderDetailsCart detailsCartVM { get; set; }
        public IActionResult OnGet()
        {
            detailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            detailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                detailsCartVM.listCart = cart.ToList();
            }

            foreach (var cartList in detailsCartVM.listCart)
            {
                cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                detailsCartVM.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
            }
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            detailsCartVM.OrderHeader.PickUpName = applicationUser.FullName;
            detailsCartVM.OrderHeader.PickUpTime = DateTime.Now;
            detailsCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            
            return Page();
        }

        public IActionResult OnPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailsCartVM.listCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList();

            detailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatus;
            detailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            detailsCartVM.OrderHeader.UserId = claim.Value;
            detailsCartVM.OrderHeader.Status = SD.PaymentStatus;

            detailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(detailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + " " + detailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _unitOfWork.OrderHeader.Add(detailsCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in detailsCartVM.listCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = detailsCartVM.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                detailsCartVM.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price);
                _unitOfWork.OrderDetails.Add(orderDetails);

            }
            _unitOfWork.ShoppingCart.RemoveRange(detailsCartVM.listCart);
            HttpContext.Session.SetInt32(SD.ShopingCart, 0);
            _unitOfWork.Save();
            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = detailsCartVM.OrderHeader.Id});
        }
    }
}