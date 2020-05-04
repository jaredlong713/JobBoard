using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobBoard.DATA.EF;
using Microsoft.AspNet.Identity;

namespace JobBoard.UI.MVC.Controllers
{
    public class ApplicationsController : Controller
    {
        private JobBoardEntities db = new JobBoardEntities();

        // GET: Applications
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            dynamic applications = null;

            if (User.IsInRole("Employee"))
            {
                applications = db.Applications.Where(a => a.UserId == user).OrderByDescending(a => a.ApplicationDate).Include(a => a.OpenPosition).ToList();

            } else if (User.IsInRole("Manager"))
            {
                var managerLocations = db.Locations.Where(l => l.ManagerId == user);

                List<OpenPosition> managerPositions = new List<OpenPosition>();
                List<Application> managerApplications = new List<Application>();

                foreach (Location item in managerLocations)
                {
                    var singleOpenPosition = item.OpenPositions;
                    foreach(OpenPosition op in singleOpenPosition)
                    {
                        managerPositions.Add(op);
                    }
                }

                foreach(OpenPosition op in managerPositions)
                {
                    var singleApplications = op.Applications;
                    foreach(Application a in singleApplications)
                    {
                        managerApplications.Add(a);
                    }
                }

                applications = managerApplications.OrderByDescending(a => a.ApplicationDate).ThenBy(a => a.UserId).ToList();

            } else
            {
                applications = db.Applications.Include(a => a.OpenPosition).Include(a => a.UserDetail).ToList();
            }


            return View(applications);
        }

        // GET: Applications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // GET: Applications/Create
        public ActionResult Create()
        {
            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId");
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName");
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApplicationId,UserId,OpenPositionId,ApplicationDate,ManagerNotes,ApplicationStatusId,ResumeFilename")] Application application)
        {
            if (ModelState.IsValid)
            {
                db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
            return View(application);
        }

        // GET: Applications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApplicationId,UserId,OpenPositionId,ApplicationDate,ManagerNotes,ApplicationStatusId,ResumeFilename")] Application application)
        {
            if (ModelState.IsValid)
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
            return View(application);
        }

        // GET: Applications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Reject(int? appID)
        {

            if (appID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(appID);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);

        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public ActionResult RejectConfrimed(int? appID)
        {

            int reject = db.ApplicationStatus1.Where(a => a.StatusName == "Declined").FirstOrDefault().ApplicationStatusId;
            Application application = db.Applications.Find(appID);
            application.ApplicationStatusId = reject;
            db.Entry(application).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Approve(int? appID)
        {

            if (appID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(appID);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);

        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveConfirmed(int appID)
        {
            int approved = db.ApplicationStatus1.Where(a => a.StatusName == "Approved").FirstOrDefault().ApplicationStatusId;
            Application application = db.Applications.Find(appID);
            application.ApplicationStatusId = approved;
            db.Entry(application).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
