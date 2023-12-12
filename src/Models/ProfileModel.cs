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

        /// <summary>
        /// AccessCodeVisible
        /// </summary>
        public bool AccessCodeVisible { get; set; }

        /// <summary>
        /// AccessCode
        /// </summary>
        public string? AccessCode { get; set; }

        private string? _inputAccessCode;
        /// <summary>
        /// InputAccessCode
        /// </summary>
        public string? InputAccessCode
        {
            get
            {
                return this._inputAccessCode;
            }
            set
            {
                this._inputAccessCode = value;

                this.AccessCodeConfirmVisible = this._inputAccessCode == this.AccessCode;

            }
        }

        /// <summary>
        /// CssClassCardBackground
        /// </summary>
        public string? CssClassCardBackground { get; set; }

        /// <summary>
        /// AccessCodeConfirmVisible
        /// </summary>
        public bool AccessCodeConfirmVisible { get; set; }
    }
}