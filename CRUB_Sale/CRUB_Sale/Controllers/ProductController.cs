using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRUB_Sale.Models;

namespace CRUB_Sale.Controllers
{
    public class ProductController : Controller
    {
        private SaledbDataContext dt = new SaledbDataContext();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductView(string kw)
        {
            var search = dt.products.Select(s=>s);

            if (!String.IsNullOrEmpty(kw))
            {
                search = search.Where(s => s.name== String.Format("%%s%%", kw));
            }

            return View(search);
        }
        public ActionResult Details(int id)
        {
            product p = dt.products.FirstOrDefault(s => s.id == id);
            return View(p);
        }
        public ActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(dt.categories, "id", "name");


            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, product pro)
        {

            int p = dt.products.Select(s => s.id).Max();



            try
            {
                if (pro.ImageUpload != null)
                {
                    //upload ảnh sp
                    string filename = Path.GetFileNameWithoutExtension(pro.ImageUpload.FileName);
                    string extention = Path.GetExtension(pro.ImageUpload.FileName);
                    filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhh")) + extention;
                    pro.image = filename;
                    pro.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));


                    pro.category_id = int.Parse(form["DanhMuc"]);
                    pro.id = p;
                    pro.id++;
                    pro.created_date = DateTime.Now;
                    dt.products.InsertOnSubmit(pro);
                    dt.SubmitChanges();
                   
                }

                
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("ProductView");
        }
        public ActionResult Edit(int id)
        {
            var p = dt.products.Where(s => s.id == id).FirstOrDefault();
            ViewData["DanhMuc"] = new SelectList(dt.categories, "id", "name");
          
            return View(p);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection form, int id)
        {
            var p = dt.products.Where(s => s.id == id).FirstOrDefault();

            p.name = form["name"];
        
            p.category_id = int.Parse(form["DanhMuc"]);
            p.image = form["image"];
            p.price = int.Parse(form["price"]);
            p.description = form["description"];
            p.manufacturer = form["manufacturer"];
            p.created_date = DateTime.Now;
            UpdateModel(p);

            return RedirectToAction("ProductView");
        }
      
        public ActionResult Delete(int id)
        {
            var p = dt.products.FirstOrDefault(s => s.id == id);

            return View(p);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection form)
        {
            //Tạm thời xóa Order_detail khi tt xóa product để tránh xung đột khóa ngoại
            while (dt.order_details.Any(u => u.product_id == id))
            {
                var o = dt.order_details.FirstOrDefault(s => s.product_id == id);
                var t = o.id;
                var or = dt.order_details.FirstOrDefault(s => s.id == t);
                dt.order_details.DeleteOnSubmit(or);
                dt.SubmitChanges();
                

            }

            var p = dt.products.FirstOrDefault(s => s.id == id);

            dt.products.DeleteOnSubmit(p);

            dt.SubmitChanges();

            return RedirectToAction("ProductView");

        }
    }
}