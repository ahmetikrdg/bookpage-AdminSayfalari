using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using bookpage.business.Abstract;
using bookpage.business.Concrate;
using bookpage.data.Abstract;
using bookpage.data.Concrate.EfCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace bookpage.webui
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {//serisleri proje içine dahil ederiz
            services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>();//ICategryRepository çağırıldığı zaman EfCoreCategory repository gelsin
            services.AddScoped<ICategoryServices,CategoryManager>();
            services.AddScoped<IProductRepository,EfCoreProductRepository>();//mysql kullanmak istiyosam onu yazarım
            services.AddScoped<IProductServices,ProductManager>();
            services.AddControllersWithViews();//mvc yapısını kullandım controlleri kullanacağım dedim
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();//wwwroot altındaki klasörler açılır
            app.UseStaticFiles(new StaticFileOptions{
                FileProvider=new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),"node_modules")),
                    RequestPath="/modules"
               
            });
            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
           

            app.UseEndpoints(endpoints =>
            {
                
               
    //--------------PRODUCTS
                
                //burada çok işime yarayacak bir hata yaptım productedit öndeydi ve çalışmadı bende listi başa çektim neden çalışmadı çünkü ilk olarak productsa girmeliyim daha sonra isteğe göre id verilir ve edite girer direk edite gitmez
                endpoints.MapControllerRoute(
                    name:"adminproductlist",
                    pattern:"admin/products",//admin products dediğimizde gitmek istediğim yer
                    defaults:new {Controller="Admin",action="ProductList"}
                );

                endpoints.MapControllerRoute(
                    name:"adminproductedit",
                    pattern:"admin/products/{id?}", //id verilirse ProductEdite git
                    defaults:new {Controller="Admin",action="ProductEdit"}
                );

                 endpoints.MapControllerRoute(
                    name:"adminproductcreate",
                    pattern:"admin/products/create",//admin products dediğimizde gitmek istediğim yer
                    defaults:new {Controller="Admin",action="ProductCreate"}
                );


                //---------- CATEGORİES
                 endpoints.MapControllerRoute(
                    name:"admincategorylist",
                    pattern:"admin/categories",
                    defaults:new {Controller="Admin",action="CategoryList"}
                );
                

                endpoints.MapControllerRoute(
                    name:"admincategorycreate",
                    pattern:"admin/categories/create",
                    defaults:new {Controller="Admin",action="CategoryCreate"}
                );

                endpoints.MapControllerRoute(
                    name:"admincategoryedit",
                    pattern:"admin/categories/{id?}",
                    defaults:new {Controller="Admin",action="CategoryEdit"}
                );
                //----------------
                 endpoints.MapControllerRoute(
                    name:"search",
                    pattern:"search",
                    defaults:new {Controller="Book",action="search"}
                );

                 endpoints.MapControllerRoute(
                    name:"productdetails",
                    pattern:"{Url}",
                    defaults:new {Controller="Book",action="details"}
                );

                endpoints.MapControllerRoute(
                    name:"products",
                    pattern:"products/{category?}",//Güncelledim. Kullanıcı category değeri verirse o kategori bilgileri gelsin--pattern:"products kullanıcı productsu çağırırsa
                    defaults:new {Controller="Book",action="list"}//bura gelsin
                );

                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Book}/{action=Index}/{id?}"
                //controller=home dedim yani sen birşey çağırmasan bile ilk olarak home çıkar karşına actionu ise ındex
                );
            });
        }
    }
}
