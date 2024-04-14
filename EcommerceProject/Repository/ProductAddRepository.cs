using EcommerceProject.Models;
using EcommerceProject.Database;
using EcommerceProject.Repository;
using Microsoft.EntityFrameworkCore;
namespace EcommerceProject.Repository
{
    public class ProductAddRepository
    {
        private readonly EcommerceContext _ecommerce;

        public ProductAddRepository(EcommerceContext ecommerce)
        {
            _ecommerce = ecommerce;
        }

        public async Task<int> AddProducts(ProductsModel model)
        {
            var addProducts = new Products
            {
                ProductName = model.ProductName,
                ProductColor = model.ProductColor,
                Price = model.Price,
                ProductId=model.ProductId,
                SpecialTagId = model.SpecialTagId,
                ImageUrl=model.ImageUrl,
                IsAvailable=model.IsAvailable

            };
            await _ecommerce.AddAsync(addProducts);
            await _ecommerce.SaveChangesAsync();    
            return addProducts.Id;
        }
        public async Task<List<ProductsModel>> ShowAllProducts()
        {
         var model=await _ecommerce.Products.Select(x=> new ProductsModel
         {
             Id=x.Id,
             ProductName=x.ProductName,
             ProductColor=x.ProductColor,
             Price=x.Price,
             IsAvailable=x.IsAvailable,
             ProductId=x.ProductId,
             SpecialTagId=x.SpecialTagId,
             ImageUrl=x.ImageUrl,
             SpecialTag=x.SpecialTag.name,
            ProductType=x.ProductType.ProductType,
           
          }).ToListAsync();
            return model;
        }
        public async Task<ProductsModel> showSingleProducts(int id)
        {
            var data = await _ecommerce.Products.Where(x => x.Id == id)
                .Select(p => new ProductsModel
                {
                    Id=p.Id,
                    ProductName=p.ProductName,
                    ProductColor=p.ProductColor,
                    Price = p.Price,
                    IsAvailable=p.IsAvailable,
                    ImageUrl = p.ImageUrl,
                    ProductType= p.ProductType.ProductType,
                    SpecialTag=p.SpecialTag.name,
                    SpecialTagId=p.SpecialTagId,
                    ProductId= p.ProductId,

                }).FirstOrDefaultAsync();
            return data;
            

                            
        }

        public async Task<ProductsModel> EditProducts(int id,ProductsModel model)
        {
            var data = await _ecommerce.Products.FindAsync(id);
            if (data != null)
            {
                data.ProductName = model.ProductName;
                data.ProductColor = model.ProductColor;
                data.Price = model.Price;
                data.IsAvailable = model.IsAvailable;
                data.ProductId = model.ProductId;
                data.SpecialTagId = model.SpecialTagId;
                data.ImageUrl = model.ImageUrl;

                await _ecommerce.SaveChangesAsync();
                return model;
            }
            return model;

            
        }
        public async Task DeleteProducts(int id)
        {
            var data = await _ecommerce.Products.FindAsync(id);
            if(data != null)
            {
                 _ecommerce.Products.Remove(data);
                await _ecommerce.SaveChangesAsync();
            }
        }
        
        public async Task<ProductsModel> SearchProduct( ProductsModel model)
        {
            var data= await _ecommerce.Products.FirstOrDefaultAsync(c=>c.ProductName == model.ProductName);
            if (data != null)
            {
                var Productmodel = new ProductsModel
                {
                    ProductName = data.ProductName,
                };
                return Productmodel;
            }
            return null;
            
        }
        public async Task<List<ProductsModel>>  SearchByAmount(decimal LowAmount,decimal HighAmount)
        {
            var data=await _ecommerce.Products.Include(c=>c.ProductType).Include(x=>x.SpecialTag).
                Where(x=>x.Price>=LowAmount&& x.Price<=HighAmount).ToListAsync();

            var products = data.Select(x=> new ProductsModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                ProductColor = x.ProductColor,
                Price = x.Price,
                IsAvailable = x.IsAvailable,
                ProductId = x.ProductId,
                SpecialTagId = x.SpecialTagId,
                ImageUrl = x.ImageUrl,
                SpecialTag = x.SpecialTag.name,
                ProductType = x.ProductType.ProductType,
            }).ToList();
            return products;
        }
    }
}
