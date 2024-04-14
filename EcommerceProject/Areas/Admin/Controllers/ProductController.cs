using EcommerceProject.Database;
using EcommerceProject.Models;
using EcommerceProject.Repository;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcommerceProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        
        private readonly ProductAddRepository _ProductAddrepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;
        private readonly ProductRepository _productRepository;
        private readonly SpecialTagNameRepository _specialTagNameRepository;

        public ProductController(ProductAddRepository productAddRepository, 
            IWebHostEnvironment webHostEnvironment
            ,ILogger<ProductController> logger
            ,ProductRepository productRepository,
            SpecialTagNameRepository specialTagNameRepository
            )
        {
            _ProductAddrepository = productAddRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _productRepository = productRepository;
            _specialTagNameRepository = specialTagNameRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Create( bool isfound=false)
        {
            ViewBag.IsFound=isfound;
            ViewBag.Product= new SelectList(await _productRepository.GetProductTypes(), "Id", "ProductType");
            ViewBag.SpecialTag = new SelectList(await _specialTagNameRepository.GetSpecialTag(), "id", "name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductsModel products)
        {
           if(ModelState.IsValid)
            {          
                var data= await _ProductAddrepository.SearchProduct(products);
                if (data != null)
                {
                    ViewBag.message = "the product already exists .choose a new product ";
                    ViewBag.Product = new SelectList(await _productRepository.GetProductTypes(), "Id", "ProductType");
                    ViewBag.SpecialTag = new SelectList(await _specialTagNameRepository.GetSpecialTag(), "id", "name");
                    return View(products);
                
                }

                if(products.Image!=null)
                {
                    string folder = "Product/Images/";                                   
                   products.ImageUrl = await UploadImage(folder, products.Image);                
                }
                else
                {
                    products.ImageUrl = "/Product/Images/NOIMAGE.jpg";

                }

                int id = await _ProductAddrepository.AddProducts(products);
                if (id > 0)
                {
                    return RedirectToAction("Create");
                }
            }

            ViewBag.Product = new SelectList(await _productRepository.GetProductTypes(), "Id", "ProductType");
            ViewBag.SpecialTag = new SelectList(await _specialTagNameRepository.GetSpecialTag(), "id", "name");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ShowAllProduts()
        {
            var data=await _ProductAddrepository.ShowAllProducts();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> ShowAllProduts(decimal? LowAmount,decimal? HighAmount)
        {
            if (LowAmount.HasValue && HighAmount.HasValue)
            {
                var data = await _ProductAddrepository.SearchByAmount(LowAmount.Value, HighAmount.Value);
                return View(data);
            }

            return RedirectToAction("ShowAllProduts");

        }


        [HttpGet]
        public async Task<IActionResult> EditProducts(int id,bool isSuccess=false)
        {
            ViewBag.IsFound = isSuccess;
            ViewBag.Product = new SelectList(await _productRepository.GetProductTypes(), "Id", "ProductType");
            ViewBag.SpecialTag = new SelectList(await _specialTagNameRepository.GetSpecialTag(), "id", "name");
            var data = await _ProductAddrepository.showSingleProducts(id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> EditProducts(int id,ProductsModel model)
        {
            if(ModelState.IsValid)
            {
                var Search = await _ProductAddrepository.SearchProduct(model);
                if(Search != null)
                {
                    ViewBag.message = "the poduct already exists";
                    ViewBag.Product = new SelectList(await _productRepository.GetProductTypes(), "Id", "ProductType");
                    ViewBag.SpecialTag = new SelectList(await _specialTagNameRepository.GetSpecialTag(), "id", "name");
                    model.ImageUrl = (await _ProductAddrepository.showSingleProducts(id))?.ImageUrl;
                    return View(model);
                }
                var existingImagePath = await _ProductAddrepository.showSingleProducts(id);
                if (model.Image!=null)
                {
                    string folder = "Product/Images/";
                   
                    model.ImageUrl= await UploadImage(folder,model.Image);
                   

                    if (!string.IsNullOrEmpty(existingImagePath.ImageUrl) && !existingImagePath.ImageUrl.EndsWith("NOIMAGE.jpg"))
                    {
                        string ExistingImageUrl = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(ExistingImageUrl))
                        {
                            System.IO.File.Delete(ExistingImageUrl);
                        }
                    }   
                }
                else
                {
                    model.ImageUrl = "/Product/Images/NOIMAGE.jpg";
                   
                }
                await _ProductAddrepository.EditProducts(id, model);
                return RedirectToAction("ShowAllProduts", "Product");
            }
            ViewBag.Product = new SelectList(await _productRepository.GetProductTypes(), "Id", "ProductType");
            ViewBag.SpecialTag = new SelectList(await _specialTagNameRepository.GetSpecialTag(), "id", "name");
            model.ImageUrl = (await _ProductAddrepository.showSingleProducts(id))?.ImageUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProducts(int id)
        {
            await _ProductAddrepository.DeleteProducts(id);
            return RedirectToAction("ShowAllProduts", "Product");
        }

        public async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;
             string ServerFolder = Path.Combine( _webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(ServerFolder, FileMode.Create));
            return "/"+folderPath;


    }
    }



    
}
