using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace DidactischeLeermiddelen.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
           // var i = 0;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
    }


    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;
        protected override void Seed(ApplicationDbContext context)
        {
            userManager =
              HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            roleManager =
               HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            // InitializeIdentity();
            //InitializeIdentityAndRoles();
            InitializeRole("Student");
            InitializeRole("Lector");
            base.Seed(context);
        }

        private void InitializeIdentity()
        {
            //CreateUser("lector@hogent.be", "P@ssword1"); //Create user Admin
            //CreateUser("student@student.hogent.be", "P@ssword1");  //Create User Student
        }

        private void InitializeRole(string roleName)
        {
            IdentityRole role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                IdentityResult result = roleManager.Create(role);
                if (!result.Succeeded)
                    throw new ApplicationException(result.Errors.ToString());
            }
        }

        private void InitializeIdentityAndRoles()
        {

            //CreateUserAndRoles("lector@hogent.be", "P@ssword1", "lector");
            //CreateUserAndRoles("student@student.hogent.be", "P@ssword1", "student");
        }

        private void CreateUser(string name, string password)
        {
            ApplicationUser user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name, LockoutEnabled = false };
                IdentityResult result = userManager.Create(user, password);
                if (!result.Succeeded)
                    throw new ApplicationException(result.Errors.ToString());
            }
        }

        private void CreateUserAndRoles(string name, string password, string roleName)
        {
            //Create user
            ApplicationUser user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name, LockoutEnabled = false };
                IdentityResult result = userManager.Create(user, password);
                if (!result.Succeeded)
                    throw new ApplicationException(result.Errors.ToString());
            }

            //Create roles
            IdentityRole role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                IdentityResult result = roleManager.Create(role);
                if (!result.Succeeded)
                    throw new ApplicationException(result.Errors.ToString());
            }

            //Associate user with role
            IList<string> rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                IdentityResult result = userManager.AddToRole(user.Id, roleName);
                if (!result.Succeeded)
                    throw new ApplicationException(result.Errors.ToString());
            }
        }
    }



}
