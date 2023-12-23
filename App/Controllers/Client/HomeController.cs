using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AppCore.Models;
using AppCore.Filters;
using AppCore.App.Repositories;
using AppCore.App.Wrapper;
using AppCore.Extensions;

namespace AppCore.Controllers;

public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private ProductRepository _productRepository;
    private CategoryRepository _categoryRepository;
    public HomeController(ILogger<HomeController> logger, ProductRepository productRepository , CategoryRepository categoryRepository) {
        _logger = logger;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<IActionResult> Index() {
        var cart = HttpContext.Session.GetObject<Cart>("Cart");
        if(cart == null) {
            HttpContext.Session.SetObject<Cart>("Cart", new Cart());
            cart = HttpContext.Session.GetObject<Cart>("Cart");
        }
        IEnumerable<Product> products  = await _productRepository.GetAllProductsAsync();
        IEnumerable<Category> categories  = await _categoryRepository.GetAllCategoriesActiveAsync();
        ViewData["Categories"] = categories;
        ViewData["Products"] = products;
        return View();
    }

    // [AuthorizeUser]
    public IActionResult Privacy() {
        return View();
    }

    [AuthorizeUser]
    public IActionResult About() {
         return View("~/Views/Home/About.cshtml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
