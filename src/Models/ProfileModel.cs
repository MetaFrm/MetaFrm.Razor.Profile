using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Razor.Models
{
    /// <summary>
    /// ProfileModel
    /// </summary>
    public class ProfileModel
    {
        /// <summary>
        /// USER_ID
        /// </summary>
        public int? USER_ID { get; set; }

        /// <summary>
        /// EMAIL
        /// </summary>
        public string? EMAIL { get; set; }

        /// <summary>
        /// NICKNAME
        /// </summary>
        [Required]
        [MinLength(3)]
        [Display(Name = "Nickname")]
        public string? NICKNAME { get; set; }

        /// <summary>
        /// FULLNAME
        /// </summary>
        [Required]
        [MinLength(3)]
        [Display(Name = "Full name")]
        public string? FULLNAME { get; set; }

        /// <summary>
        /// PHONENUMBER
        /// </summary>
        [Required]
        [Display(Name = "Phone Number")]
        public string? PHONENUMBER { get; set; }

        /// <summary>
        /// RESPONSIBILITY_NAME
        /// </summary>
        public string? RESPONSIBILITY_NAME { get; set; }

        /// <summary>
        /// PROFILE_IMAGE
        /// </summary>
        public string? PROFILE_IMAGE { get; set; }

        /// <summary>
        /// INACTIVE_DATE
        /// </summary>
        public DateTime? INACTIVE_DATE { get; set; }
    }
}