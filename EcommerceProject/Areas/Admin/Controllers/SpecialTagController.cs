using EcommerceProject.Models;
using EcommerceProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SpecialTagController : Controller
    {
        private readonly SpecialTagNameRepository _specialTagRepository;
        public SpecialTagController(SpecialTagNameRepository specialTagNameRepository)
        {
            _specialTagRepository = specialTagNameRepository;
        }
        public async  Task<IActionResult> ShowAllSpecialTag()
        {
            var data=await _specialTagRepository.ShowAllSpecialTag();
            return View(data);  
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SpecialTagModel model)
        {
            if (ModelState.IsValid)
            {
                int id = await _specialTagRepository.AddSpecialTag(model);
                if (id > 0)
                {
                    return RedirectToAction("ShowAllSpecialTag");
                }

            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditSpecialTag(int id)
        {
            var data = await _specialTagRepository.ShowSingleSpecialTag(id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> EditSpecialTag(int id, SpecialTagModel model)
        {
            if (ModelState.IsValid)
            {
                var data = await _specialTagRepository.EditSpecialName(id, model);
                return RedirectToAction("ShowAllSpecialTag");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await _specialTagRepository.ShowSingleSpecialTag(id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _specialTagRepository.DeleteSpecialName(id);
            return RedirectToAction("ShowAllSpecialTag");

        }
    }
}
