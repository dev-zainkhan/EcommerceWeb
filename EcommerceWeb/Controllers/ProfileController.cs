using BC = BCrypt.Net.BCrypt;
using EcommerceWeb.DAL;
using EcommerceWeb.DAL.Profile;
using EcommerceWeb.Models.Profile;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileDAL _profileDAL;
        private readonly S3Helper _s3Helper;
        private readonly DbHelper _db;

        public ProfileController()
        {
            _db = new DbHelper();
            _profileDAL = new ProfileDAL(_db);
            _s3Helper = new S3Helper();
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int userId = HttpContext.Session.GetInt32("UserId").Value;
            var user = await _profileDAL.GetByIdAsync(userId);
            var profile = new UserProfile
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                Role = user.Role
            };

            return View(profile);
        }

        // POST: /Profile/UpdateProfile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfile model, IFormFile profileImage)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int userId = HttpContext.Session.GetInt32("UserId").Value;

            try
            {
                // Update name and email
                await _profileDAL.UpdateProfileAsync(userId, model.Name, model.Email);
                // Update session name
                HttpContext.Session.SetString("UserName", model.Name);

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        TempData["Error"] = "Passwords do not match.";
                        return RedirectToAction("Index");
                    }

                    var user = await _profileDAL.GetByIdAsync(userId); if (!BC.Verify(model.CurrentPassword, user.PasswordHash))
                    {
                        TempData["Error"] = "Current password is incorrect.";
                        return RedirectToAction("Index");
                    }

                    await _profileDAL.UpdatePasswordAsync(userId, BC.HashPassword(model.NewPassword));
                }

                // Update profile image if provided
                // Update profile image if provided
                if (profileImage != null && profileImage.Length > 0)
                {
                    // Delete old image from S3 first
                    var existingUser = await _profileDAL.GetByIdAsync(userId); if (!string.IsNullOrWhiteSpace(existingUser.ProfileImage))
                        _s3Helper.DeleteImage(existingUser.ProfileImage);

                    // Upload new image
                    string imageUrl = _s3Helper.UploadImage(profileImage);
                    await _profileDAL.UpdateProfileImageAsync(userId, imageUrl);
                    HttpContext.Session.SetString("ProfileImage", imageUrl);
                }

                TempData["Success"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Update failed: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: /Profile/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}