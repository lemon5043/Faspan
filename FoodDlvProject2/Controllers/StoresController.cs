using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodDlvProject2.EFModels;
using Microsoft.AspNetCore.Http;
using FoodDlvProject2.Models.ViewModels;

namespace FoodDlvProject2.Controllers
{
	public class StoresController : Controller
	{
		private readonly AppDbContext _context;

		public StoresController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Stores
		public async Task<IActionResult> Index()
		{


			var foodDeliveryContext = _context.Stores.Include(s => s.StorePrincipal).Include(p => p.Products);
			return View(await foodDeliveryContext.ToListAsync());
		}

		// GET: Stores/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Stores == null)
			{
				return NotFound();
			}

			var store = await _context.Stores
				.Include(s => s.StorePrincipal).Include(x => x.StoreBusinessHours).Include(x => x.Products).Include(x=>x.StoresCategoriesLists).ThenInclude(x=>x.Category)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (store == null)
			{
				return NotFound();
			}
			ViewBag.Apikey = _context.Apis.Where(x => x.Id == 1).FirstOrDefault().Apikey;
			return View(store);
		}

		// GET: Stores/Create
		public IActionResult Create()
		{




			ViewBag.Apikey = _context.Apis.Where(x=>x.Id==1).FirstOrDefault().Apikey;
			ViewData["StorePrincipalId"] = new SelectList(_context.StorePrincipals, "Id", "Account");
			return View();
		}

		// POST: Stores/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,StorePrincipalId,StoreName,Address,ContactNumber,Photo,Latitude,Longitude")] StoreCreateVM storeCreateVM)
		{
			if (ModelState.IsValid)
			{

				//if (myimg != null)
				//{
				//    using (var ms = new MemoryStream())
				//    {
				//        myimg.CopyTo(ms);
				//        store.Photo = ms.ToArray();
				//    }
				//}
				Store store = new Store
				{
					Id = storeCreateVM.Id,
					StorePrincipalId = storeCreateVM.StorePrincipalId,
					StoreName = storeCreateVM.StoreName,
					Address = storeCreateVM.Address,
					ContactNumber = storeCreateVM.ContactNumber,
					Latitude = storeCreateVM.Latitude,
					Longitude = storeCreateVM.Longitude,
				};

				if (storeCreateVM.Photo != null)
				{
					var now = DateTime.Now;
					var fileName = $"{storeCreateVM.StoreName}{now.Year}{now.Month}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}{storeCreateVM.Address}.jpg";
					var filePath = Path.Combine(
				   Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Stores/",
				   fileName);
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await storeCreateVM.Photo.CopyToAsync(stream);
					}
					store.Photo = fileName;
				}

				_context.Add(store);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["StorePrincipalId"] = new SelectList(_context.StorePrincipals, "Id", "Account", storeCreateVM.StorePrincipalId);
			return View(storeCreateVM);
		}

		// GET: Stores/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Stores == null)
			{
				return NotFound();
			}

			var store = await _context.Stores.FindAsync(id);
			if (store == null)
			{
				return NotFound();
			}
			var storeEditVM = new StoreEditVM
			{
				Id = store.Id,
				StorePrincipalId = store.StorePrincipalId,
				StoreName = store.StoreName,
				Address = store.Address,
				ContactNumber = store.ContactNumber,
				Latitude = store.Latitude,
				Longitude = store.Longitude,
			};


			ViewBag.Apikey = _context.Apis.Where(x => x.Id == 1).FirstOrDefault().Apikey;
			ViewData["StorePrincipalId"] = new SelectList(_context.StorePrincipals, "Id", "Account", store.StorePrincipalId);
			return View(storeEditVM);
		}

		// POST: Stores/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,StorePrincipalId,StoreName,Address,ContactNumber,Photo,Latitude,Longitude")] StoreEditVM storeEditVM)
		{
			if (id != storeEditVM.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var store = _context.Stores.FirstOrDefault(x => x.Id == id);

					store.StorePrincipalId = storeEditVM.StorePrincipalId;
					store.StoreName = storeEditVM.StoreName;
					store.Address = storeEditVM.Address;
					store.ContactNumber = storeEditVM.ContactNumber;
					store.Latitude = storeEditVM.Latitude;
					store.Longitude = storeEditVM.Longitude;

					// IFormFile 的值就算沒照片也不會是 null，會直接報錯
					//就算有照片，167 行的 store.Photo 也沒被定義，一樣無法修改
					if (storeEditVM.Photo != null)
					{
						// 刪除舊圖片
						if (store.Photo != null)
						{
							var oldFilePath = Path.Combine(
								Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Stores/",
								store.Photo);

							if (System.IO.File.Exists(oldFilePath))
							{
								System.IO.File.Delete(oldFilePath);
							}
						}
						

						var now = DateTime.Now;
						var fileName = $"{storeEditVM.StoreName}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}{storeEditVM.Address}.jpg";
						var filePath = Path.Combine(
							Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Stores/",
							fileName);
						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await storeEditVM.Photo.CopyToAsync(stream);
						}
						store.Photo = fileName;
					}


					_context.Update(store);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!StoreExists(storeEditVM.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["StorePrincipalId"] = new SelectList(_context.StorePrincipals, "Id", "Account", storeEditVM.StorePrincipalId);
			return View(storeEditVM);
		}

		// GET: Stores/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Stores == null)
			{
				return NotFound();
			}

			var store = await _context.Stores
				.Include(s => s.StorePrincipal)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (store == null)
			{
				return NotFound();
			}

			return View(store);
		}

		// POST: Stores/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Stores == null)
			{
				return Problem("Entity set 'FoodDeliveryContext.Stores'  is null.");
			}
			var store = await _context.Stores.FindAsync(id);


			if (store != null)
			{
				_context.Stores.Remove(store);
			}

			var oldFilePath = Path.Combine(
	Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Stores/",
	store.Photo);

			if (System.IO.File.Exists(oldFilePath))
			{
				System.IO.File.Delete(oldFilePath);
			}




			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool StoreExists(int id)
		{
			return _context.Stores.Any(e => e.Id == id);
		}






		public async Task<IActionResult> DetailsP(long? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Store)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}













		// GET: Products/Create
		public IActionResult CreateP(int id)
		{

			var store = _context.Stores.Find(id);


			ViewBag.Name = store.StoreName;
			ViewBag.StoreId = store.Id;
			//ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateP([Bind("StoreId,ProductName,Photo,ProductContent,Status,UnitPrice")] ProductCreateVM productCreateVM/*, IFormFile? myimg*/)
		{



			if (ModelState.IsValid)
			{

				//if (myimg != null)
				//{
				//	using (var ms = new MemoryStream())
				//	{
				//		myimg.CopyTo(ms);
				//		product.Photo = ms.ToArray();
				//	}
				//}



				Product product = new Product
				{
					Id = productCreateVM.Id,
					StoreId = productCreateVM.StoreId,
					ProductName = productCreateVM.ProductName,
					ProductContent = productCreateVM.ProductContent,
					Status = productCreateVM.Status,
					UnitPrice = productCreateVM.UnitPrice,
				};

				if (productCreateVM.Photo != null)
				{
					var now = DateTime.Now;
					var fileName = $"{productCreateVM.ProductName}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}.jpg";
					var filePath = Path.Combine(
				   Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
				   fileName);
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await productCreateVM.Photo.CopyToAsync(stream);
					}
					product.Photo = fileName;
				}



				_context.Add(product);
				await _context.SaveChangesAsync();
				//return RedirectToAction(nameof(Index));

				return Redirect($"~/Stores/Details/{product.StoreId}");


			}
			ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName", productCreateVM.StoreId);
			return View(productCreateVM);
		}

		// GET: Products/Edit/5
		public async Task<IActionResult> EditP(long? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}


			var productEditVM = new ProductEditVM
			{
				Id = product.Id,
				StoreId = product.StoreId,
				ProductName = product.ProductName,
				ProductContent = product.ProductContent,
				Status = product.Status,
				UnitPrice = product.UnitPrice,

			};


			ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName", product.StoreId);
			return View(productEditVM);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditP(long id, [Bind("Id,StoreId,ProductName,Photo,ProductContent,Status,UnitPrice")] ProductEditVM productEditVM)
		{
			if (id != productEditVM.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var product = _context.Products.FirstOrDefault(x => x.Id == id);

					product.StoreId = productEditVM.StoreId;
					product.ProductName = productEditVM.ProductName;
					product.ProductContent = productEditVM.ProductContent;
					product.Status = productEditVM.Status;
					product.UnitPrice = productEditVM.UnitPrice;

					if (productEditVM.Photo != null)
					{
						// 刪除舊圖片
						if (product.Photo != null)
						{
                            var oldFilePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
                            product.Photo);

                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }

                        }
					




                            var now = DateTime.Now;
						var fileName = $"{productEditVM.ProductName}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}.jpg";
						var filePath = Path.Combine(
							Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
							fileName);
						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await productEditVM.Photo.CopyToAsync(stream);
						}
						product.Photo = fileName;
					}












					_context.Update(product);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductExists(productEditVM.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return Redirect($"~/Stores/Details/{productEditVM.StoreId}");
			}
			ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName", productEditVM.StoreId);
			return View(productEditVM);
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> DeleteP(long? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Store)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("DeleteP")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmedP(long id)
		{
			if (_context.Products == null)
			{
				return Problem("Entity set 'AppDbContext.Products'  is null.");
			}
			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				_context.Products.Remove(product);
			}
			var oldFilePath = Path.Combine(
Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
product.Photo);

			if (System.IO.File.Exists(oldFilePath))
			{
				System.IO.File.Delete(oldFilePath);
			}




			await _context.SaveChangesAsync();
			return Redirect($"~/Stores/Details/{product.StoreId}");
		}

		private bool ProductExists(long id)
		{
			return _context.Products.Any(e => e.Id == id);
		}

	}
}
