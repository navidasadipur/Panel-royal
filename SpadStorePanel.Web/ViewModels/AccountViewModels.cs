using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpadStorePanel.Web.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری یا ایمیل خود را وارد کنید")]
        [Display(Name = "نام کاربری یا ایمیل: *")]
        //[EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور خود را وارد کنید")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور: *")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "ایمیل خود را وارد کنید")]
        [EmailAddress]
        [Display(Name = "ایمیل: *")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, ErrorMessage = " {0} حداقل باید دارای {2} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور: *")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور: *")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار رمز عبور یکسان نیستند")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} وارد شده معتبر نیست")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} خود را وارد کنید")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل 6 کارکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} خود را وارد کنید")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [Compare("Password", ErrorMessage = "عدم تطابق رمز عبور و تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} وارد شده معتبر نیست")]
        public string Email { get; set; }
    }
    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Access { get; set; }

    }

    public class RolePermissionsViewModel
    {

        public int PermissionID { get; set; }
        public string ControllerName { get; set; }
        public string PermissionTitle { get; set; }
        public bool Access { get; set; }

    }
}
