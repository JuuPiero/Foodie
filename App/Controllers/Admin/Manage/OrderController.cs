using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AppCore.Filters;
using AppCore.Models;
using AppCore.Models.ViewModels;
using AppCore.App.Repositories;

namespace AppCore.Controllers;


[AuthorizeAdmin]
public class OrderController : Controller {
    private readonly ILogger<OrderController> _logger;
    private OrderRepository _orderRepository;
    public OrderController(ILogger<OrderController> logger, OrderRepository orderRepository) {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    [Route("Admin/Order")]
    public async Task<IActionResult> Index() {
        var orders = await _orderRepository.GetAllOrdersAsync();
        return View("~/Views/Admin/Order/Index.cshtml", orders);
    }

    [Route("Admin/Order/Detail/{orderId}")]
    public IActionResult Detail() {
        return View("~/Views/Admin/Order/Detail.cshtml");
    }

    // [Route("Admin/Order")]
    // public async Task<IActionResult> Store() {
    //     await _categoryRepository.AddCategoryAsync(new Category {
    //         Name = categoryRequest.Name,
    //         Image = categoryRequest.Image,
    //         Description = categoryRequest.Description,
    //         Active = categoryRequest.Active,
    //         CreatedAt = DateTime.Now,
    //         UpdatedAt = DateTime.Now
    //     });
    //     return RedirectToAction("Create");
    // }
}
