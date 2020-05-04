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
    public class OpenPositionsController : Controller
    {
        private JobBoardEntities db = new JobBoardEntities();

        // GET: OpenPositions
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();

            if (!User.IsInRole("Manager"))
            {
                var openPositions = db.OpenPositions.Include(op => op.Location).Include(op => op.Position);

                ViewData.Add("LocationID", new SelectList(db.Locations, "LocationId", "FullLocation"));
                ViewData.Add("PositionID", new SelectList(db.Positions, "PositionId", "Title"));
                ViewBag.LocationID = new SelectList(db.Locations, "LocationId", "FullLocation");
                ViewBag.PositionID = new SelectList(db.Positions, "PositionId", "Title");

                return View(openPositions.ToList());
            }
            else
            {
                var mgrLocations = db.Locations.Where(l => l.ManagerId == userid);

                List<OpenPosition> openPositions = new List<OpenPosition>();
                foreach (var l in mgrLocations)
                {
                    var locationOpenPositions = l.OpenPositions;
                    foreach (var position in locationOpenPositions)
                    {
                        openPositions.Add(position);
                    }
                }
                string userId = User.Identity.GetUserId();
                var managerLocations = db.Locations.Where(l => l.ManagerId == userId);

                ViewBag.LocationID = new SelectList(managerLocations, "LocationId", "FullLocation");
                ViewBag.PositionID = new SelectList(db.Positions, "PositionId", "Title");

                return View(openPositions);
            }
        }

        // GET: OpenPositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            return View(openPosition);
        }

        // GET: OpenPositions/Create
        public ActionResult Create()
        {
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber");
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title");
            return View();
        }

        // POST: OpenPositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OpenPositionId,LocationId,PositionId")] OpenPosition openPosition)
        {
            if (ModelState.IsValid)
            {
                db.OpenPositions.Add(openPosition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Manager"))
            {
                string currentUserID = User.Identity.GetUserId();
                var managerLocations = db.Locations.Where(loc => loc.ManagerId == currentUserID);

                ViewBag.LocationID = new SelectList(managerLocations, "LocationId", "StoreNumber", openPosition.LocationId);
            }
            else
            {
                ViewBag.LocationID = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
            }
            return View(openPosition);
        }

        // GET: OpenPositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", openPosition.PositionId);
            return View(openPosition);
        }

        // POST: OpenPositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OpenPositionId,LocationId,PositionId")] OpenPosition openPosition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(openPosition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", openPosition.PositionId);
            return View(openPosition);
        }

        // GET: OpenPositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            return View(openPosition);
        }

        // POST: OpenPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OpenPosition openPosition = db.OpenPositions.Find(id);
            db.OpenPositions.Remove(openPosition);
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

        public ActionResult Apply(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
         
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if(openPosition == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            if(userId != null)
            {
                string userResume = db.UserDetails.Where(db => db.UserId == userId).FirstOrDefault().ResumeFilename;
                int newStatus = db.ApplicationStatus1.Where(a => a.StatusName == "New").FirstOrDefault().ApplicationStatusId;

                Application newApp = new Application();

                newApp.ApplicationDate = DateTime.Now;
                newApp.OpenPosition = openPosition;
                newApp.UserId = userId;
                newApp.ResumeFilename = userResume;
                newApp.ApplicationStatusId = newStatus;

                db.Applications.Add(newApp);
                db.SaveChanges();

                return RedirectToAction("Index", "Applications");

            } else
            {
                return RedirectToAction("Login", "Account");
            }



        }
    }
}
