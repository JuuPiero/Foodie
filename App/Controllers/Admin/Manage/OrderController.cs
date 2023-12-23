using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AppCore.Filters;
using AppCore.Models;
using AppCore.Models.ViewModels;
using AppCore.App.Repositories;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
namespace AppCore.Controllers;


[AuthorizeAdmin]
public class OrderController : Controller {
    private readonly ILogger<OrderController> _logger;
    private OrderRepository _orderRepository;

     private readonly ApplicationDbContext _dbContext;
    public OrderController(ILogger<OrderController> logger, OrderRepository orderRepository, ApplicationDbContext dbContext) {
        _logger = logger;
        _orderRepository = orderRepository;
        _dbContext = dbContext;
    }

    [Route("Admin/Order")]
    public async Task<IActionResult> Index() {
        var orders = await _orderRepository.GetAllOrdersAsync();
        return View("~/Views/Admin/Order/Index.cshtml", orders);
    }

    [Route("Admin/Order/Detail/{orderId}")]
    public async Task<IActionResult> Detail(int orderId) {
        var orderItems = from orderItem in _dbContext.OrderItems
                        join product in _dbContext.Products on orderItem.ProductId equals product.ProductId
                        // join order in _dbContext.Orders on orderItem.OrderId equals order.OrderId
                        where orderItem.OrderId == orderId 
                        select new OrderItemViewModel {
                            // OrderId = order.OrderId,
                            ProductId = orderItem.ProductId,
                            ProductName = product.Name,
                            Image = product.Image,
                            Price = product.Price,
                            Quantity = orderItem.Quantity
                        };
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
       
        ViewData["OrderItems"] = await orderItems.ToListAsync();
        ViewData["Amount"] = order.TotalAmount;
        ViewData["orderId"] = orderId;

        return View("~/Views/Admin/Order/Detail.cshtml");
    }
}
