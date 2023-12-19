using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.Cookies;

using AppCore.Models;
using AppCore.Filters;
using AppCore.Models.ViewModels;
using AppCore.App.Repositories;
using AppCore.Data;

namespace AppCore.App.Controllers;

[AuthorizeAdmin]
public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private ProductRepository _productRepository;
    private CategoryRepository _categoryRepository;

    private readonly ApplicationDbContext _dbContext;

    public ProductController(ILogger<ProductController> logger, ProductRepository productRepository , CategoryRepository categoryRepository, ApplicationDbContext dbContext) {
        _logger = logger;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
    }


    [Route("Admin/Product")]
    public IActionResult Index() {
        string message = TempData["Message"] as string;
        ViewBag.Message = message;
        var query = from category in _dbContext.Categories
                    join product in _dbContext.Products on category.CategoryId equals product.CategoryId
                    select new
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Image = product.Image,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        Active = product.Active,
                        CategoryName = category.Name,
                    };

        var products = query.ToList();
        // IEnumerable<Product> products  = await _productRepository.GetAllProductsAsync();
        return View("~/Views/Admin/Product/Index.cshtml", products);
    }
   
    [Route("Admin/Product/Create")]
    public async Task<IActionResult> Create() {
        string message = TempData["Message"] as string;
        ViewBag.Message = message;
        ViewData["categories"] = await _categoryRepository.GetAllCategoriesAsync();
        return View("~/Views/Admin/Product/Create.cshtml");
    }

    public async Task<IActionResult> Store(ProductRequest productRequest) {
        await _productRepository.AddProductAsync(new Product {
            Name = productRequest.Name,
            Image = productRequest.Image,
            Description = productRequest.Description,
            Active = productRequest.Active,
            CategoryId = productRequest.CategoryId, 
            Price = productRequest.Price,
            Quantity = productRequest.Quantity,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        TempData["Message"] = "Tạo thành công";
        return RedirectToAction("Create", "Product");
    }



    [Route("Admin/Product/Edit/{productId}")]
    public async Task<IActionResult> Edit(int productId) {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if(product != null) {
            Console.WriteLine("có");
            ViewData["Product"] = product;
            ViewData["categories"] = await _categoryRepository.GetAllCategoriesAsync();
            return View("~/Views/Admin/Product/Edit.cshtml");
        }
        return View("~/Views/Admin/404.cshtml");
    }

    [Route("Admin/Product/Update/{productId}")]
    public async Task<IActionResult> Update(int productId, ProductRequest productRequest) {
        await _productRepository.UpdateProductAsync(new Product {
            ProductId = productId,
            CategoryId = productRequest.CategoryId,
            Name = productRequest.Name,
            Image = productRequest.Image,
            Description = productRequest.Description,
            Active = productRequest.Active,
            Price = productRequest.Price,
            Quantity = productRequest.Quantity,
            UpdatedAt = DateTime.Now
        });
        TempData["Message"] = "Sửa thành công";
        return RedirectToAction("Index");
    }

    [Route("Admin/Product/Delete/{productId}")]
    public async Task<IActionResult> Delete(int productId) {
        Console.WriteLine(productId);
        var result = await _productRepository.DeleteProductAsync(productId);
        if(result) {
            return Json(new { success = true, message = "Xóa thành công." });
        }
        return Json(new { success = false, message = "Xóa ko thành công." });
    }
}
