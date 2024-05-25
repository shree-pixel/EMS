using EMS.Data;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace EMS.Controllers
{
    public class EMSController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EMSController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Homepage(string searchInput = "", string filterDropdown = "", string filterStatus = "")
        {
            var model = GetEmployeeDetails(); // Retrieve the employee details from your data source

            if (!string.IsNullOrEmpty(searchInput))
            {
                searchInput = searchInput.ToLower(); // Convert search input to lowercase

                switch (filterDropdown)
                {
                    case "empno":
                        model = model.Where(x => x.Empno.ToLower().Contains(searchInput));
                        break;
                    case "name":
                        model = model.Where(x => x.Name.ToLower().Contains(searchInput));
                        break;
                    case "designation":
                        model = model.Where(x => x.Designation.ToLower().Contains(searchInput));
                        break;
                    case "email":
                        model = model.Where(x => x.Email.ToLower().Contains(searchInput));
                        break;
                    case "phonenumber":
                        model = model.Where(x => x.PhoneNumber.ToLower().Contains(searchInput));
                        break;
                }
            }
            ViewData["FilterDropdownOptions"] = new List<SelectListItem>
    {
        new SelectListItem { Value = "empno", Text = "Employee Number", Selected = filterDropdown == "empno" },
        new SelectListItem { Value = "name", Text = "Name", Selected = filterDropdown == "name" },
        new SelectListItem { Value = "designation", Text = "Designation", Selected = filterDropdown == "designation" },
        new SelectListItem { Value = "email", Text = "Email", Selected = filterDropdown == "email" },
        new SelectListItem { Value = "phonenumber", Text = "Phone Number", Selected = filterDropdown == "phonenumber" }
    };

            // Populate filter status options
            ViewData["FilterStatusOptions"] = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "All", Selected = filterStatus == "" },
        new SelectListItem { Value = "active", Text = "Active", Selected = filterStatus == "active" },
        new SelectListItem { Value = "inactive", Text = "Inactive", Selected = filterStatus == "inactive" }
    };

            if (!string.IsNullOrEmpty(filterStatus))
            {
                filterStatus = filterStatus.ToLower(); // Convert filter status to lowercase

                if (filterStatus == "active")
                {
                    model = model.Where(x => x.Status);
                }
                else if (filterStatus == "inactive")
                {
                    model = model.Where(x => !x.Status);
                }
            }
            List<DetailsModel> model1 = model.Where(row => row.Deleteflag == 0).ToList();
            var sno = 1;
            foreach (var employee in model1)
            {
                employee.Sno = sno++;
            }

            model1 = model1.OrderByDescending(e => e.UpdatedDate).ToList();
            return View(model1);
        }



        // GET: Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DetailsModel obj)
        {
            var obj1 = _db.Details.FirstOrDefault(d => d.Email == obj.Email);
            if (obj1 != null)
            {
                TempData["Email"] = "Email already exist ";
            }
            var obj2 = _db.Details.FirstOrDefault(d => d.PhoneNumber == obj.PhoneNumber);
            if (obj2 != null)
            {
                TempData["PhoneNumber"] = "Phone Number already exist ";
            }
            if (obj1 == null && obj2 == null)
            {
                ModelState.Remove(nameof(obj.Address));
                obj.Address = obj.Address1 + " , " + obj.Address2 + " , " + obj.City + " , " + obj.Address1 + " , " + obj.pincode.ToString();

                if (ModelState.IsValid)
                {
                    _db.Details.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Homepage"); // Redirect to Homepage action
                }
            }
            return View(obj);
        }
        public IActionResult ViewDetails(int? id)
        {
            if (id != null)
            {
                var obj = _db.Details.FirstOrDefault(d => d.Id == id);
                if (obj != null)
                    return View(obj);
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {
                var obj = _db.Details.Find(id);
                if (obj != null)
                    return View(obj);
            }
            return View();
        }
        public IActionResult Check()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Check(DetailsModel obj1)
        {

            var obj = _db.Details.FirstOrDefault(d => d.Empno == obj1.Empno);

            if (obj != null)
            {
                if (obj.PhoneNumber == obj.PhoneNumber)
                    return RedirectToAction("ViewDetails", obj);
                else
                    TempData["Error"] = " Phone number doesn't match with given Employee Number";
            }
            else
            {
                TempData["Error"] = "Credentials do not match";
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DetailsModel obj)
        {
            var obj1 = _db.Details.FirstOrDefault(d => d.Email == obj.Email && d.Id != obj.Id);
            if (obj1 != null)
            {
                TempData["Email"] = "Email already exist ";
            }
            var obj2 = _db.Details.FirstOrDefault(d => d.PhoneNumber == obj.PhoneNumber && d.Id != obj.Id);
            if (obj2 != null)
            {
                TempData["PhoneNumber"] = "Phone Number already exist ";
            }
            if (obj1 == null && obj2 == null)
            {
                var obj3 = _db.Details.Find(obj.Id);
                obj.Address = obj.Address1 + " , " + obj.Address2 + " , " + obj.City + " , " + obj.Address1 + " , " + obj.pincode.ToString();
                obj3.UpdatedDate = DateTime.Now;
                obj3.Email = obj.Email;
                obj3.PhoneNumber = obj.PhoneNumber;
                obj3.Address1 = obj.Address1;
                obj3.Address = obj.Address;
                obj3.Address2 = obj.Address2;
                obj3.City = obj.City;
                obj3.State = obj.State;
                obj3.pincode = obj.pincode;

                _db.SaveChanges();
                return RedirectToAction("Homepage");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            var obj = _db.Details.Find(id);
            obj.Deleteflag = 1;
            _db.SaveChanges();
            return RedirectToAction("Homepage");
        }
        public IActionResult PermanentDelete(int? id)
        {
            var obj = _db.Details.Find(id);
            _db.Details.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
        private IEnumerable<DetailsModel> GetEmployeeDetails()
        {
            var employeeDetails = _db.Details.ToList();
            var sno = 1;

            foreach (var employee in employeeDetails)
            {
                employee.Sno = sno++;
            }

            return employeeDetails.OrderByDescending(e => e.UpdatedDate);
        }
        public IActionResult EmailCheck(string Email, int? Id)
        {
            if (Id == null)
            {
                var data = _db.Details.Where(e => e.Email == Email).SingleOrDefault();
                if (data != null)
                {
                    return Json($"Email already exist !!!");
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                var data = _db.Details.Where(e => e.Email == Email && e.Id != Id).SingleOrDefault();
                if (data != null)
                {
                    return Json($"Email already exist !!!");
                }
                else
                {
                    return Json(true);
                }
            }
        }
        public IActionResult PhoneNumberCheck(string PhoneNumber, int? Id)
        {
            if (Id == null)
            {
                var data = _db.Details.Where(e => e.PhoneNumber == PhoneNumber).SingleOrDefault();
                if (data != null)
                {
                    return Json($"Phone Number already exist !!!");
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                var data = _db.Details.Where(e => e.PhoneNumber == PhoneNumber && e.Id != Id).SingleOrDefault();
                if (data != null)
                {
                    return Json($"Phone Number already exist !!!");
                }
                else
                {
                    return Json(true);
                }
            }
        }
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var data = _db.Details.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");


                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "DOB";
                worksheet.Cells[1, 3].Value = "Gender";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Phone Number";
                worksheet.Cells[1, 6].Value = "Address";
                worksheet.Cells[1, 7].Value = "State";
                worksheet.Cells[1, 8].Value = "EmpNo";
                worksheet.Cells[1, 9].Value = "Designation";
                worksheet.Cells[1, 10].Value = "DOJ";
                worksheet.Cells[1, 11].Value = "Status";

                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Name;
                    worksheet.Cells[i + 2, 2].Value = data[i].DOB.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 3].Value = data[i].Gender;
                    worksheet.Cells[i + 2, 4].Value = data[i].Email;
                    worksheet.Cells[i + 2, 5].Value = data[i].PhoneNumber;
                    worksheet.Cells[i + 2, 6].Value = data[i].Address;
                    worksheet.Cells[i + 2, 7].Value = data[i].State;
                    worksheet.Cells[i + 2, 8].Value = data[i].Empno;
                    worksheet.Cells[i + 2, 9].Value = data[i].Designation;
                    worksheet.Cells[i + 2, 10].Value = data[i].DOJ.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 11].Value = data[i].Status;

                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                var fileName = $"Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public IActionResult RecycleBin()
        {
            var model = GetEmployeeDetails();
            List<DetailsModel> model1 = model.Where(row => row.Deleteflag == 1).ToList();
            var sno = 1;
            foreach (var employee in model1)
            {
                employee.Sno = sno++;
            }

            model1 = model1.OrderByDescending(e => e.UpdatedDate).ToList();
            return View(model1);
        }
        public IActionResult Undo(int? id)
        {
            var obj = _db.Details.Find(id);
            obj.Deleteflag = 0;
            _db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
    }
}
