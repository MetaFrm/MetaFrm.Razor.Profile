using MetaFrm.MVVM;
using MetaFrm.Razor.Models;
using Microsoft.Extensions.Localization;

namespace MetaFrm.Razor.ViewModels
{
    /// <summary>
    /// ProfileViewModel
    /// </summary>
    public partial class ProfileViewModel : BaseViewModel
    {
        /// <summary>
        /// ProfileModel
        /// </summary>
        public ProfileModel ProfileModel { get; set; } = new(null);

        /// <summary>
        /// ProfileViewModel
        /// </summary>
        public ProfileViewModel() : base() { }

        /// <summary>
        /// ProfileViewModel
        /// </summary>
        /// <param name="localization"></param>
        public ProfileViewModel(IStringLocalizer? localization) : base(localization) { }
    }
}