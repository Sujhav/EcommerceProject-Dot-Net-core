using EcommerceProject.Database;
using EcommerceProject.Models;
using EcommerceProject.Repository;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
namespace EcommerceProject.Repository
{
    public class SpecialTagNameRepository
    {
        private readonly EcommerceContext _context;
        public SpecialTagNameRepository(EcommerceContext ecommerceContext)
        {
            _context = ecommerceContext;
        }

        public async Task<int> AddSpecialTag(SpecialTagModel model)
        {
            var data = new SpecialTag
            {
                name = model.name
            };
            await _context.AddAsync(data);
            await _context.SaveChangesAsync();
            return data.id;
        }

        public async Task<List< SpecialTagModel>> ShowAllSpecialTag()
        {
            var SpecialName=new List<SpecialTagModel>();
            var data= await _context.SpecialTag.ToListAsync();
            if(data != null)
            {
                foreach(var item in data)
                {
                    SpecialName.Add(new SpecialTagModel
                    {
                        id = item.id,
                        name = item.name,
                    });
                }
                return SpecialName;
            }
            return null;
        }

        public async Task<SpecialTagModel> ShowSingleSpecialTag(int id)
        {
            var data=await _context.SpecialTag.FindAsync(id);
            if(data != null)
            {
                var model = new SpecialTagModel
                {
                    id = data.id,
                    name = data.name,
                };
                return model;
            }
            return null ;
        }

        public async Task<object> EditSpecialName(int id, SpecialTagModel model)
        {
            var data = await _context.SpecialTag.FindAsync(id);
            if(data != null)
            {
                data.name = model.name;
                await _context.SaveChangesAsync();
                
            }
            return null;;
           
        }
        public async Task DeleteSpecialName(int id)
        {
            var data = await _context.SpecialTag.FindAsync(id);
            if(data!= null) {
                 _context.SpecialTag.Remove(data);
                await _context.SaveChangesAsync();
            }
        }
        public async Task <List<SpecialTagModel>> GetSpecialTag()
        {
            var data = await _context.SpecialTag.Select(x => new SpecialTagModel
            {
                id = x.id,
                name = x.name,
            }).ToListAsync();
            return data;
        }
    }
}
