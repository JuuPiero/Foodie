using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AppCore.Models;
using AppCore.Models.ViewModels;
using AppCore.Filters;
using AppCore.App.Repositories;
using AppCore.App.Wrapper;
using AppCore.Extensions;
using AppCore.Data;

namespace AppCore.Controllers;

public class CartController : Controller {
    private readonly ILogger<CartController> _logger;
    private PaymentMethodRepository _paymentMethodRepository;
    private readonly ApplicationDbContext _dbContext;
    public CartController(ILogger<CartController> logger, ApplicationDbContext dbContext , PaymentMethodRepository paymentMethodRepository) {
        _logger = logger;
        _paymentMethodRepository = paymentMethodRepository;
        _dbContext = dbContext;
    }
    
    public async Task<IActionResult> Index() {
        var cart = HttpContext.Session.GetObject<Cart>("Cart");
        if(cart == null) {
            HttpContext.Session.SetObject<Cart>("Cart", new Cart());
            cart = HttpContext.Session.GetObject<Cart>("Cart");
        }
        if(cart.OrderItems.Count > 0) {
            var query = from orderItem in cart.OrderItems
                        join product in _dbContext.Products on orderItem.ProductId equals product.ProductId
                        select new OrderItemViewModel {
                            ProductId = orderItem.ProductId,
                            ProductName = product.Name,
                            Image = product.Image,
                            Price = product.Price,
                            Quantity = orderItem.Quantity,
                        };
            // var orderItems = query.ToList();
            var orderItems = await Task.Run(() => query.ToList());
            ViewData["OrderItems"] = orderItems;
        }
        else {
            ViewData["OrderItems"] = new List<OrderItemViewModel>();
        }
        // ViewData["PaymentMethods"] = await _paymentMethodRepository.GetAllPaymentMethodsActiveAsync();
       
        return View();
    }

    [Route("Cart/Add/{productId}/{price}/{quantity}")]
    public IActionResult Add(int productId, decimal price, int quantity = 1) {
       var cart = HttpContext.Session.GetObject<Cart>("Cart");
        cart.AddItem(productId, price, quantity);
        HttpContext.Session.SetObject<Cart>("Cart", cart);
        return Json(new { success = true, message = "thêm vào giỏ thành công." });
    }

    [Route("Cart/Remove/{productId}")]
    public IActionResult Delete(int productId) {
        var cart = HttpContext.Session.GetObject<Cart>("Cart");
        cart.RemoveItem(productId);
        HttpContext.Session.SetObject<Cart>("Cart", cart);
        return Json(new { success = true, message = "xóa item thành công." });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
