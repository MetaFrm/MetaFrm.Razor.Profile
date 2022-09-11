using MetaFrm.MVVM;
using MetaFrm.Razor.Models;

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
        public ProfileModel ProfileModel { get; set; } = new();
    }
}