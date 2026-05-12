namespace ClickAndGoApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;

public class ProductController : Controller
{
    private readonly ProductDAL _productDal;
    private readonly CategoryDAL _categoryDal;

    public ProductController(ProductDAL productDal, CategoryDAL categoryDal)
    {
        _productDal = productDal;
        _categoryDal = categoryDal;
    }

    public IActionResult Browse(string q, int? categoryId)
    {
        var vm = new BrowseViewModel
        {
            Products = _productDal.GetFiltered(q, categoryId),
            Categories = _categoryDal.GetAll(),
            SearchQuery = q,
            SelectedCategoryId = categoryId
        };

        return View(vm);
    }

    public IActionResult Details(int id)
    {
        var vm = _productDal.GetById(id);
        if (vm == null) return RedirectToAction("Browse");
        return View(vm);
    }
}
