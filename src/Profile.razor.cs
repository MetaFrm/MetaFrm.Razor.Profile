using MetaFrm.Database;
using MetaFrm.Razor.Models;
using MetaFrm.Razor.ViewModels;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;

namespace MetaFrm.Razor
{
    /// <summary>
    /// Profile
    /// </summary>
    public partial class Profile
    {
        #region Variable
        internal ProfileViewModel ProfileViewModel { get; set; } = Factory.CreateViewModel<ProfileViewModel>();
        #endregion


        #region Init
        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.IsLogin())
                    this.Navigation?.NavigateTo("/", true);

                //this.ProfileViewModel = await this.GetSession<ProfileViewModel>(nameof(this.ProfileViewModel));

                this.Search();

                this.StateHasChanged();
            }
        }
        #endregion


        #region IO
        private void Search()
        {
            Response response;

            try
            {
                if (this.ProfileViewModel.IsBusy) return;

                this.ProfileViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Select.Profile");
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.ProfileViewModel.ProfileModel = new();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0)
                    {
                        this.ProfileViewModel.ProfileModel.USER_ID = response.DataSet.DataTables[0].DataRows[0].Int(nameof(ProfileModel.USER_ID));
                        this.ProfileViewModel.ProfileModel.EMAIL = response.DataSet.DataTables[0].DataRows[0].String(nameof(ProfileModel.EMAIL));
                        this.ProfileViewModel.ProfileModel.NICKNAME = response.DataSet.DataTables[0].DataRows[0].String(nameof(ProfileModel.NICKNAME));
                        this.ProfileViewModel.ProfileModel.FULLNAME = response.DataSet.DataTables[0].DataRows[0].String(nameof(ProfileModel.FULLNAME));
                        this.ProfileViewModel.ProfileModel.PHONENUMBER = response.DataSet.DataTables[0].DataRows[0].String(nameof(ProfileModel.PHONENUMBER));
                        this.ProfileViewModel.ProfileModel.RESPONSIBILITY_NAME = response.DataSet.DataTables[0].DataRows[0].String(nameof(ProfileModel.RESPONSIBILITY_NAME));
                        this.ProfileViewModel.ProfileModel.PROFILE_IMAGE = response.DataSet.DataTables[0].DataRows[0].String(nameof(ProfileModel.PROFILE_IMAGE));
                        this.ProfileViewModel.ProfileModel.INACTIVE_DATE = response.DataSet.DataTables[0].DataRows[0].DateTime(nameof(ProfileModel.INACTIVE_DATE));
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.ProfileViewModel.IsBusy = false;
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                //this.SetSession(nameof(ProfileViewModel), this.ProfileViewModel);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            }
        }

        private void Save()
        {
            Response? response;

            response = null;

            try
            {
                if (this.ProfileViewModel.IsBusy)
                    return;

                if (this.ProfileViewModel.ProfileModel.USER_ID == null || this.ProfileViewModel.ProfileModel.USER_ID <= 0)
                    return;

                this.ProfileViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Save.Profile");
                serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.USER_ID), DbType.Int, 3, this.ProfileViewModel.ProfileModel.USER_ID);
                serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.NICKNAME), DbType.NVarChar, 50, this.ProfileViewModel.ProfileModel.NICKNAME);
                serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.FULLNAME), DbType.NVarChar, 200, this.ProfileViewModel.ProfileModel.FULLNAME);
                serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.PHONENUMBER), DbType.NVarChar, 50, this.ProfileViewModel.ProfileModel.PHONENUMBER);
                serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.PROFILE_IMAGE), DbType.Text, 0, this.ProfileViewModel.ProfileModel.PROFILE_IMAGE);

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.ToastShow("Completed", $"Profile registered successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.ProfileViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }
        #endregion


        #region Event
        private async Task InputFileChangeEventArgs(Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs e)
        {
            Microsoft.AspNetCore.Components.Forms.IBrowserFile? DllFile;
            byte[] bytes;


            if (e.FileCount == 1)
            {
                DllFile = e.File;

                if (DllFile.Size > 2048000)
                {
                    this.ModalShow("Warning", "The maximum image size is 2MB.", new() { { "Ok", Btn.Warning } }, null);
                    return;
                }

                MemoryStream memoryStream = new();
                await DllFile.OpenReadStream(10000 * 1024).CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                bytes = new byte[DllFile.Size];
                
                await memoryStream.ReadAsync(bytes.AsMemory(0, (int)DllFile.Size));

                this.ProfileViewModel.ProfileModel.PROFILE_IMAGE = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes));
                //this.StateHasChanged();
            }
        }
        #endregion
    }
}