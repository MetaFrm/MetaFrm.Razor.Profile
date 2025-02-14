using MetaFrm.MVVM;
using MetaFrm.Razor.Essentials.ComponentModel.DataAnnotations;
using DisplayAttribute = System.ComponentModel.DataAnnotations.DisplayAttribute;

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
        [Display(Name = "별명")]
        [Required]
        [MinLength(3)]
        public string? NICKNAME { get; set; }

        /// <summary>
        /// FULLNAME
        /// </summary>
        [Display(Name = "성명")]
        [Required]
        [MinLength(3)]
        public string? FULLNAME { get; set; }

        /// <summary>
        /// PHONENUMBER
        /// </summary>
        [Display(Name = "전화번호")]
        public string? PHONENUMBER { get; set; }

        /// <summary>
        /// Org_FULLNAME_PHONENUMBER
        /// </summary>
        public string? Org_FULLNAME_PHONENUMBER { get; set; }

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
        /// PERSON_VERIFICATION
        /// </summary>
        public DateTime? PERSON_VERIFICATION { get; set; }

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
        /// AccessCodeConfirmVisible
        /// </summary>
        public bool AccessCodeConfirmVisible { get; set; }
    }
}