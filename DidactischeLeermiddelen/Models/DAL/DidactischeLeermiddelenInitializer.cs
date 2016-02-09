//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity.Validation;
//using System.Linq;
//using System.Web;
//using DidactischeLeermiddelen.Models;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

//namespace DidactischeLeermiddelen.Models.DAL
//{
//    public class DidactischeLeermiddelenInitializer : DropCreateDatabaseAlways<DidactischeLeermiddelenContext>
//    {
//        protected override void Seed(DidactischeLeermiddelenContext context)
//        {
//            try
//            {
//                CreateRolesAndAdmin(context);

//                //Create students
//                string email = "test@hogent.be";
//                ApplicationUser user = new ApplicationUser { UserName = email, Email = email };
//                CreateAccount(user, context);
//            }
//            catch (DbEntityValidationException e)
//            {
//                string s = "Fout creatie database ";
//                foreach (var eve in e.EntityValidationErrors)
//                {
//                    s += String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
//                        eve.Entry.Entity.GetType().Name, eve.Entry.GetValidationResult());
//                    foreach (var ve in eve.ValidationErrors)
//                    {
//                        s += String.Format("- Property: \"{0}\", Error: \"{1}\"",
//                            ve.PropertyName, ve.ErrorMessage);
//                    }
//                }
//                throw new Exception(s);
//            }
//        }
//        private void CreateAccount(ApplicationUser user, DidactischeLeermiddelenContext context)
//        {
//            var userStore = new UserStore<ApplicationUser>(context);
//            var userManager = new UserManager<ApplicationUser>(userStore);
//            var roleStore = new RoleStore<IdentityRole>(context);
//            var roleManager = new RoleManager<IdentityRole>(roleStore);
//            var role = roleManager.FindByName("student");

//            var result = userManager.Create(user, "P@ssword1");
//            result = userManager.SetLockoutEnabled(user.Id, false);
//            result = userManager.AddToRole(user.Id, role.Name);
//        }


//        private void CreateRolesAndAdmin(DidactischeLeermiddelenContext context)
//        {
//            //var userManager =
//            //    HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

//            //var roleManager =
//            //    HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
//            var userStore = new UserStore<ApplicationUser>(context);
//            var userManager = new UserManager<ApplicationUser>(userStore);
//            var roleStore = new RoleStore<IdentityRole>(context);
//            var roleManager = new RoleManager<IdentityRole>(roleStore);

//            //Create Roles
//            var role = new IdentityRole("docent");
//            var roleresult = roleManager.Create(role);
//            role = new IdentityRole("student");
//            roleresult = roleManager.Create(role);

//            //Create user Admin
//            const string name = "admin@hogent.be";
//            const string password = "Admin@1";
//            ApplicationUser admin = new ApplicationUser { UserName = name, Email = name };
//            var result = userManager.Create(admin, password);
//            result = userManager.SetLockoutEnabled(admin.Id, false);
//            result = userManager.AddToRole(admin.Id, "docent");

//        }
//    }
//}