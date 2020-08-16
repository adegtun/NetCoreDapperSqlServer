using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestWebApp.Models;
using TestWebApp.Utility;

namespace TestWebApp.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;
        private MyConnection con;
        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
            con = new MyConnection(_configuration);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Employee contact)
        {
            IDbConnection dbcon = con.GetConnection();
            //ResponseModel model = new ResponseModel();
            try
            {
                //dbcon.Open();
                var result = await dbcon.ExecuteAsync("INSERT INTO Employee (FirstName, LastName, Phone, Email) VALUES" +
                    "(@FirstName, @LastName, @Phone, @Email)", contact).ConfigureAwait(false);                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists see your system administrator.");
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            return RedirectToAction(nameof(Index));
            //return View();
        }
        public async Task<IActionResult> Index()
        {
            string conString = _configuration.GetValue<string>("DefaultConnection");
            List<Employee> lstContacts = new List<Employee>();
            string query = "select * from Employee";
            IDbConnection dbcon = con.GetConnection();
            try
            {
                var rs = await dbcon.QueryAsync<Employee>(query).ConfigureAwait(false);
                lstContacts = rs.ToList();
            }
            catch (Exception ex)
            {
                //_logger.Error(ex.ToString());
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            return View(lstContacts);
            //return View();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int ID)
        {
            IDbConnection dbcon = con.GetConnection();
            Employee employee = null;
            try
            {
                var rs = await dbcon.QueryAsync<Employee>("SELECT * from Employee where Id = @Id ", new { Id = ID }).ConfigureAwait(false);
                employee = rs.FirstOrDefault();
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An Error occurred");
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int ID, Employee employee)
        {
            int rs = 0;
            if (ID != employee.ID)
            {
                return NotFound();
            }
            IDbConnection dbcon = con.GetConnection();
            try
            {
                string sql = "UPDATE Employee SET FirstName = @FirstName, LastName = @LastName, Phone = @Phone," +
                    "Email = @Email WHERE id = @ID";
                rs = await dbcon.ExecuteAsync(sql, employee).ConfigureAwait(false);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists, " +
                "see your system administrator.");
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            if (rs == 1)
                return RedirectToAction(nameof(Index));
            else
                return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int ID)
        {
            IDbConnection dbcon = con.GetConnection();
            Employee employee = null;
            try
            {
                var rs = await dbcon.QueryAsync<Employee>("SELECT * from Employee where Id = @Id ", new { Id = ID }).ConfigureAwait(false);
                employee = rs.FirstOrDefault();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An Error occurred");
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int ID)
        {
            IDbConnection dbcon = con.GetConnection();
            Employee employee = null;
            try
            {
                var rs = await dbcon.QueryAsync<Employee>("SELECT * from Employee where Id = @Id ", new { Id = ID }).ConfigureAwait(false);
                employee = rs.FirstOrDefault();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An Error occurred");
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ID)
        {
            int result = 0;
            IDbConnection dbcon = con.GetConnection();
            Employee employee = null;

            try
            {
                var rs = await dbcon.QueryAsync<Employee>("SELECT * from Employee where Id = @Id ", new { Id = ID }).ConfigureAwait(false);
                employee = rs.FirstOrDefault();
                if (employee != null)
                {
                    result = await dbcon.ExecuteAsync("Delete from Employee where Id = @Id", new { Id = ID }).ConfigureAwait(false);
                } 
                
            }            
            catch (Exception /* ex */)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists, " +
                "see your system administrator.");
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            if (result == 1)
                return RedirectToAction(nameof(Index));
            else
                return View(employee);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}