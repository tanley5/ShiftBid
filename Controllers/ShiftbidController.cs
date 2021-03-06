using System;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

using Hangfire;
using Hangfire.SqlServer;

using Shiftbid.Models;
using Shiftbid.Models.ViewModels;
using Shiftbid.Data;
using Shiftbid.Background;


namespace Shiftbid.Controllers
{
    public class ShiftbidController : Controller
    {
        private readonly ApplicationDbContext context;
        private IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShiftbidController(ApplicationDbContext ctx, IConfiguration _configuration, IHttpContextAccessor httpContextAccessor)
        {
            context = ctx;
            Configuration = _configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            ShiftbidIndexViewModel ShiftbidIndexVM = new ShiftbidIndexViewModel();

            var NewReports = await context.Reports.Where(r => r.Status == Status.New).ToListAsync();
            var WorkingReports = await context.Reports.Where(r => r.Status == Status.Working).ToListAsync();
            var CompletedReport = await context.Reports.Where(r => r.Status == Status.Complete).ToListAsync();

            ShiftbidIndexVM.NewReport = NewReports;
            ShiftbidIndexVM.WorkingReport = WorkingReports;
            ShiftbidIndexVM.CompletedReport = CompletedReport;

            return View(ShiftbidIndexVM);
        }

        [HttpGet]
        public IActionResult CreateReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Responses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var report = await context.Reports.FirstOrDefaultAsync(r => r.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }
            report.Status = Status.Working;
            context.SaveChanges();

            var shift = context.Shifts.Where(s => s.Report == report && s.Email == null).Select(s => new SelectListItem()
            {
                Value = s.ShiftID.ToString(),
                Text = s.ShiftName
            }).ToList();

            if (shift == null)
            {
                return NotFound();
            }
            ResponsesViewModel responsesViewModel = new ResponsesViewModel();
            responsesViewModel.Shifts = shift;

            BackgroundJob.Enqueue<Background.Background>((bg) => bg.ProcessBGJob(report.ReportID));

            return View(responsesViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Responses(ResponsesViewModel vm)
        {

            if (vm.EmailAddress != null && vm.Shifts != null)
            {
                var selected = Int64.Parse(Request.Form["Shifts"]);
                Shift selectedShift = context.Shifts.FirstOrDefault(s => s.ShiftID == selected);

                selectedShift.Email = vm.EmailAddress;
                Seniority currentSeniority = context.Seniorities.FirstOrDefault(s => s.AgentEmail == vm.EmailAddress);
                currentSeniority.SeniorityState = SeniorityState.Received;
                context.SaveChanges();

                return Content("Success");
            }
            return Content("Fail");
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(ReportViewModel vm)
        {
            //Create Seniority object with report object added
            if (vm.SeniorityFile != null && vm.ShiftFile != null)
            {
                // Create and add report object to DB
                var datetime = DateTime.Now;
                var report = new Report { ReportName = vm.ReportName, DateTimeCreated = datetime, Status = Status.New };
                var dbReport = context.Reports.Add(report);
                context.SaveChanges();
                // Get Report Object from Context
                report = context.Reports.Where(r => r.ReportName == vm.ReportName).Single();

                //get path
                var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", report.ReportName);

                //create directory "Uploads" if it doesn't exists
                if (!Directory.Exists(MainPath))
                {
                    Directory.CreateDirectory(MainPath);
                }

                DataTable SeniorityDt = DtCreator(MainPath, vm, "Seniority");
                DataTable ShiftDt = DtCreator(MainPath, vm, "Shift");
                Directory.Delete(MainPath, true);
                foreach (DataRow row in SeniorityDt.Rows.Cast<DataRow>().Skip(1))
                {
                    Seniority sn = new Seniority();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i < 1)
                        {
                            sn.SeniorityNumber = Convert.ToInt32(row.ItemArray[i]);
                        }
                        else if (i == 1)
                        {
                            sn.AgentName = (string)row.ItemArray[i];
                        }
                        else
                        {
                            sn.AgentEmail = (string)row.ItemArray[i];
                        }
                    }
                    sn.Report = report;
                    sn.ReportID = report.ReportID;
                    sn.SeniorityState = SeniorityState.New;
                    context.Seniorities.Add(sn);
                }
                await context.SaveChangesAsync();

                foreach (DataRow row in ShiftDt.Rows.Cast<DataRow>().Skip(1))
                {
                    Shift sh = new Shift();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i < 1)
                        {
                            sh.ShiftName = (string)row.ItemArray[i];
                        }
                    }
                    sh.Report = report;
                    sh.ReportID = report.ReportID;
                    context.Shifts.Add(sh);
                }
                await context.SaveChangesAsync();

                return RedirectPermanent("~/Shiftbid/Index");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var report = await context.Reports.FirstOrDefaultAsync(r => r.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await context.Reports.FindAsync(id);
            context.Reports.Remove(report);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public DataTable DtCreator(string MainPath, ReportViewModel vm, string Model)
        {
            if (Model.Equals("Seniority"))
            {
                var filePath = Path.Combine(MainPath, vm.SeniorityFile.FileName);
                //var filePath = vm.SeniorityFile.FileName;
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    vm.SeniorityFile.CopyTo(stream);
                }

                string conString = this.Configuration.GetConnectionString("ExcelConString");

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {

                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                return dt;
            }
            else
            {
                var filePath = Path.Combine(MainPath, vm.ShiftFile.FileName);
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    vm.ShiftFile.CopyTo(stream);
                }

                string conString = this.Configuration.GetConnectionString("ExcelConString");

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {

                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                return dt;
            }
        }

        public IEnumerable<Seniority> GetSeniorities(Report r)
        {
            var seniorities = context.Seniorities.Where(s => s.ReportID == r.ReportID).ToList();
            return seniorities;
        }
        public IEnumerable<Shift> GetShifts(Report r)
        {
            var shifts = context.Shifts.Where(sh => sh.ReportID == r.ReportID).ToList();
            return shifts;
        }
        public IEnumerable<Seniority> GetNotAssignedSeniorities(Report r)
        {
            // Get Shifts and Seniorities With the Same Report ID
            var AllShifts = GetShifts(r);
            var AllSeniorities = GetSeniorities(r);
            // Remove seniorities where their email is in shifts
            var UnassignedSeniorities = AllSeniorities.Where(sen => !AllShifts.Any(sh => sh.Email == sen.AgentEmail));
            //var AssignedSeniorities = AllSeniorities.Where(sen => AllShifts.Any(sh => sh.Email == sen.AgentEmail));
            //Console.WriteLine(UnassignedSeniorities.First().SeniorityNumber);
            return UnassignedSeniorities;
        }

        public async Task BackgroundSeniorityAsync(int rId)
        {
            var report = await context.Reports.FirstOrDefaultAsync(r => r.ReportID == rId);
            while (report.Status != Status.Complete)
            {
                //var Seniorities = context.Seniorities.Where(sen => sen.ReportID == a.ReportID);
                var Seniorities = context.Seniorities.Where(sen => sen.SeniorityState != SeniorityState.Received && sen.Report == report);
                // Check if any of the seniority stat is "sent". If there is, we skip this round
                if (Seniorities.Any(sen => sen.SeniorityState == SeniorityState.Sent))
                {
                    Console.WriteLine("Still Waiting Reply");
                    Task.Delay(10000).Wait();
                }
                // if all Seniorities are null, change report status to complete
                else if (Seniorities == null)
                {
                    report.Status = Status.Complete;
                    context.SaveChanges(); ;
                }
                // If none of the seniority state is "sent" (All state is new), we get the first seniority of the list and send an email to them with the link
                else
                {
                    Seniority NextSeniority = Seniorities.First();
                    string host = _httpContextAccessor.HttpContext.Request.Host.Value;
                    string link = $"{host}/Shiftbid/Responses/{report.ReportID}";
                    NextSeniority.SeniorityState = SeniorityState.Sent;
                    context.SaveChanges();
                    Console.WriteLine("Sending the next seniority an email");
                    Task.Delay(10000).Wait();
                }

            }
        }
    }
}