using System.Linq;
using bookpage.business.Abstract;
using bookpage.entity;
using bookpage.webui.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace bookpage.webui.Controllers
{
    public class AdminController:Controller
    {
        private IProductServices _productServices;
        private ICategoryServices _categoryServices;
        public AdminController(IProductServices productServices,ICategoryServices categoryServices)
        {
            this._productServices=productServices;
            this._categoryServices=categoryServices;
        }
        public IActionResult ProductList()
        {
            return View(new ProductListViewModel()
            {
                Products=_productServices.GetAll(),
            });
        }
        [HttpGet]
        public IActionResult ProductCreate()
        {//ProductCreate içindeki bilgileri getirdim.
            return View();
        }
        [HttpPost]
        public IActionResult ProductCreate(ProductModel model)
        {
            var entity=new Product()
            {
                Name=model.Name,
                Url=model.Url,
                Author=model.Author,
                Pages=model.Pages,
                Description=model.Description,
                ImageUrl=model.ImageUrl,
            };
            _productServices.Create(entity);
            var msg= new AlertMessage()
            {
              Message=$"{entity.Name} isimli ürün eklendi",
              AlertType="success"
            };
            TempData["message"]=JsonConvert.SerializeObject(msg); 
            return RedirectToAction("ProductList");
        }

        [HttpGet]
        public IActionResult ProductEdit(int? id)//gelen idye göre sorgular gelen idye göre bilgiyi göstericezb
        {
            if(id==null)
            {
                return NotFound();
            }
            var entity=_productServices.GetByIdWithCategories((int)id);//bunla gittim ürünü buldum entity içine atadım
            var model=new ProductModel()//buradada entity içinden modele atıyorum
            {
                ProductId=entity.ProductId,
                Name=entity.Name,
                Url=entity.Url,
                Author=entity.Author,
                Pages=entity.Pages,
                Description=entity.Description,
                ImageUrl=entity.ImageUrl,
                categories=entity.ProductCategories.Select(i=>i.Categories).ToList()//seçilmiş olan ürünle ilişkili kategorileri bir listeye çevirip categories içine attım
            };
            //categorieste sadece o idye ait ürünü aldım şimdi ise tüm kategori bilgilerini alacağım
            ViewBag.Category=_categoryServices.GetAll();
            return View(model);
        }

        [HttpPost]
        public IActionResult ProductEdit(ProductModel model,int [] CategoryIds)
        {
            var entity=_productServices.GetById(model.ProductId);//modelin productıdsini buldum getirdim eski modeldekiyle yenisini değiştiricem
            if(entity==null)
            {
                return NotFound();
            }
            entity.Name=model.Name;
            entity.Author=model.Author;
            entity.Url=model.Url;
            entity.Pages=model.Pages;
            entity.Description=model.Description;
            entity.ImageUrl=model.ImageUrl;

            _productServices.Update(entity,CategoryIds);
            //
            var msg= new AlertMessage()
            {
              Message=$"{entity.Name} isimli ürün güncellendi",
              AlertType="primary"
            };
            TempData["message"]=JsonConvert.SerializeObject(msg); 
            
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public IActionResult ProductDelete(int productId)
        {
            var entity=_productServices.GetById(productId);
            if(entity!=null)
            {
                _productServices.Delete(entity);
            }           

            var msg= new AlertMessage()
            {
              Message=$"{entity.Name} isimli ürün silindi",
              AlertType="danger"
            };
            TempData["message"]=JsonConvert.SerializeObject(msg); 

            return RedirectToAction("ProductList");         
        }

//-----------------CATEGORY-----------------
         
        public IActionResult CategoryList()
        {
            return View(new CategoryListViewModel()
            {
                Categories=_categoryServices.GetAll(),
            });
        }


        public IActionResult CategoryCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {
            var entity=new Category()
            {
                Name=model.Name,
                Url=model.Url,
            };
            _categoryServices.Create(entity);
            var msg= new AlertMessage()
            {
              Message=$"{entity.Name} isimli kategori eklendi",
              AlertType="success"
            };
            TempData["message"]=JsonConvert.SerializeObject(msg); 
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public IActionResult CategoryEdit(int? id)//gelen idye göre sorgular gelen idye göre bilgiyi göstericezb
        {
            if(id==null)
            {
                return NotFound();
            }
            var entity=_categoryServices.GetByIdWithProducts((int)id);//ile entity bize gelir onuda sayfaya göndeririz
            var model=new CategoryModel()//buradada entity içinden modele atıyorum
            {
                CategoryId=entity.CategoryId,
                Name=entity.Name,
                Url=entity.Url,
                Products= entity.ProductCategories.Select(p=>p.products).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {
            //var entity=_categoryServices.GetById(model.CategoryId);
            var entity=_categoryServices.GetById(model.CategoryId);
            if(entity==null)
            {
                return NotFound();
            }
            entity.Name=model.Name;
            entity.Url=model.Url;

            _categoryServices.Update(entity);
            
            var msg= new AlertMessage()
            {
              Message=$"{entity.Name} isimli kategori güncellendi",
              AlertType="primary"
            };
            TempData["message"]=JsonConvert.SerializeObject(msg); 
            
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult CategoryDelete(int categoryId)
        {
            var entity=_categoryServices.GetById(categoryId);
            if(entity!=null)
            {
                _categoryServices.Delete(entity);
            }           

            var msg= new AlertMessage()
            {
              Message=$"{entity.Name} isimli kategori silindi",
              AlertType="danger"
            };
            TempData["message"]=JsonConvert.SerializeObject(msg); 

            return RedirectToAction("CategoryList");         
        }
        
        [HttpPost]
        public IActionResult DeleteFromCategory(int productId,int categoryId)
        {
            _categoryServices.DeleteFromCategory(productId,categoryId);
            return Redirect("/admin/categories/"+categoryId);         
        }

    }
}