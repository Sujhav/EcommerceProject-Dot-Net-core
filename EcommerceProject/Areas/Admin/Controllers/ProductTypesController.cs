using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.Repository;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductTypesController : Controller
    {
        private readonly ProductRepository _productRepository;
        public ProductTypesController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public  async Task< IActionResult> Index()
        {
            var data=await _productRepository.ShowAllProducts();
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductTypesModel model)
        {
            if(ModelState.IsValid)
            {
                int id = await _productRepository.CreateProducts(model);
                if (id > 0)
                {
                    TempData["save"] = "Product type has benn successfully saved";
                    return RedirectToAction("Index");
                }

            }
           
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var data= await _productRepository.ShowSingleProduct(id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(int id , ProductTypesModel model)
        {
            if (ModelState.IsValid)
            {
                var data=await _productRepository.EditProduct(id, model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await _productRepository.ShowSingleProduct(id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
          await _productRepository.DeleteProduct(id);
            return RedirectToAction("Index");
          
        }


    }
}
