using EcommerceProject.Database;
using EcommerceProject.Models;
using Microsoft.EntityFrameworkCore;
namespace EcommerceProject.Repository
{
    public class ProductRepository
    {
        private readonly EcommerceContext _ecommerceContext;

        public ProductRepository(EcommerceContext ecommerceContext)
        {
            _ecommerceContext = ecommerceContext;
        }

        public async Task<int> CreateProducts(ProductTypesModel model)
        {
            var addProduct = new ProductTypes
            {
                ProductType=model.ProductType,
            };
            await _ecommerceContext.AddAsync(addProduct);
            await _ecommerceContext.SaveChangesAsync();
            return addProduct.Id;
        }

        public async Task<List<ProductTypesModel>> ShowAllProducts()
        {
            var Product =new List<ProductTypesModel>();
            var data = await _ecommerceContext.ProductTypes.ToListAsync();
            if(data?.Any()==true)
            {
                foreach(var product in data)
                {
                    Product.Add(new ProductTypesModel
                    {
                        Id = product.Id,
                        ProductType = product.ProductType,
                    });
                }
            }
            return Product;

        }

        public async Task<ProductTypesModel> ShowSingleProduct(int id)
        {
            var data= await _ecommerceContext.ProductTypes.FindAsync(id);
            if (data != null)
            {
                var model = new ProductTypesModel
                {
                    ProductType = data.ProductType,
                };
                return model;
            }
            return null;
        }
        public async Task<ProductTypesModel> EditProduct(int id,ProductTypesModel model)
        {
            var data=await _ecommerceContext.ProductTypes.FindAsync(id);
            if (data != null)
            {
                data.ProductType = model.ProductType;

                await _ecommerceContext.SaveChangesAsync();
                return model;

            };
            return null;
            
        }
        public async Task DeleteProduct(int id)
        {
            var data = await _ecommerceContext.ProductTypes.FindAsync(id);
            if(data != null)
            {
                _ecommerceContext.ProductTypes.Remove(data);
                await _ecommerceContext.SaveChangesAsync();
            }
        }

        public async Task<List<ProductTypesModel>> GetProductTypes()
        {
            var Products = await _ecommerceContext.ProductTypes.ToListAsync();
            var Model = new List<ProductTypesModel>();

            foreach (var item in Products)
            {
                Model.Add(new ProductTypesModel
                {
                    Id = item.Id,
                    ProductType = item.ProductType,
                });
            }
            return Model;
        }
            
        
    }
}
