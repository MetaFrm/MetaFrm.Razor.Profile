using MetaFrm.Alert;
using MetaFrm.Control;
using MetaFrm.Database;
using MetaFrm.Razor.Models;
using MetaFrm.Razor.ViewModels;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Timers;

namespace MetaFrm.Razor
{
    /// <summary>
    /// Profile
    /// </summary>
    public partial class Profile
    {
        #region Variable
        internal ProfileViewModel ProfileViewModel { get; set; } = Factory.CreateViewModel<ProfileViewModel>();

        private bool _isFocusElement = false;//등록 버튼 클릭하고 AccessCode로 포커스가 한번만 가도록

        private TimeSpan RemainTimeOrg { get; set; } = new TimeSpan(0, 5, 0);

        private TimeSpan RemainTime { get; set; }
        #endregion


        #region Init
        /// <summary>
        /// OnInitialized
        /// </summary>
        protected override void OnInitialized()
        {
            try
            {
                this.ProfileViewModel.ProfileModel.CssClassCard = this.GetAttribute(nameof(this.ProfileViewModel.ProfileModel.CssClassCard));

                string[] time = this.GetAttribute("RemainingTime").Split(":");

                this.RemainTimeOrg = new TimeSpan(time[0].ToInt(), time[1].ToInt(), time[2].ToInt());

                this.RemainTime = new TimeSpan(this.RemainTimeOrg.Ticks);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:올바르게 ValueTasks 사용", Justification = "<보류 중>")]
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.AuthState.IsLogin())
                    this.Navigation?.NavigateTo("/", true);

                this.Search();

                this.StateHasChanged();
            }

            if (this.ProfileViewModel.ProfileModel.AccessCodeVisible && !this._isFocusElement)
            {
                this.JSRuntime?.InvokeVoidAsync("ElementFocus", "inputaccesscode");
                this._isFocusElement = true;
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
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("Select.Profile");
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());

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
                    Token = this.AuthState.Token()
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
                    this.ToastShow("Completed", this.Localization["프로필이 성공적으로 등록되었습니다."], ToastDuration.Long);
                    this.OnAction(this, new MetaFrmEventArgs { Action = "ProfileImage", Value = null });
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

        private async Task<bool> OnWithdrawalClick()
        {
            try
            {
                this.ProfileViewModel.IsBusy = true;

                if (this.AuthState.IsLogin())
                {
                    Response response;


                    if (!this.ProfileViewModel.ProfileModel.EMAIL.IsNullOrEmpty() && !this.ProfileViewModel.ProfileModel.InputAccessCode.IsNullOrEmpty()
                        && this.ProfileViewModel.ProfileModel.AccessCode == this.ProfileViewModel.ProfileModel.InputAccessCode)
                    {
                        ServiceData serviceData = new()
                        {
                            TransactionScope = true,
                            Token = this.AuthState.Token()
                        };
                        serviceData["1"].CommandText = this.GetAttribute("Withdrawal");
                        serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.EMAIL), DbType.NVarChar, 100, this.ProfileViewModel.ProfileModel.EMAIL);
                        serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.USER_ID), DbType.Int, 3, this.ProfileViewModel.ProfileModel.USER_ID);
                        serviceData["1"].AddParameter("ACCESS_CODE", DbType.NVarChar, 10, this.ProfileViewModel.ProfileModel.InputAccessCode);

                        response = await this.ServiceRequestAsync(serviceData);

                        if (response.Status == Status.OK)
                        {
                            this.ToastShow("Withdrawal", this.Localization["탈퇴 완료 되었습니다."], ToastDuration.Long);

                            this.OnAction(this, new MetaFrmEventArgs { Action = "Logout" });
                            return true;
                        }
                        else
                        {
                            if (response.Message != null)
                            {
                                this.ModalShow("Withdrawal", response.Message, new() { { "Ok", Btn.Warning } }, EventCallback.Factory.Create<string>(this, OnClickFunctionAsync));
                            }
                        }
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
            }

            return false;
        }

        private async Task OnClickFunctionAsync(string action)
        {
            await Task.Delay(100);
#pragma warning disable CA2012 // 올바르게 ValueTasks 사용
            this.JSRuntime?.InvokeVoidAsync("ElementFocus", "inputaccesscode");
#pragma warning restore CA2012 // 올바르게 ValueTasks 사용
        }

        private void Withdrawal()
        {
            this.ModalShow($"Question", this.Localization["탈퇴하시겠습니까?"], new() { { this.Localization["탈퇴"], Btn.Danger }, { "Cancel", Btn.Primary } }, EventCallback.Factory.Create<string>(this, this.WithdrawalCheck));
        }

        private async void WithdrawalCheck(string action)
        {
            try
            {
                if (action != this.Localization["탈퇴"]) return;

                this.ProfileViewModel.IsBusy = true;

                if (this.AuthState.IsLogin())
                {
                    Response response;

                    if (this.ProfileViewModel.ProfileModel.USER_ID != null && this.ProfileViewModel.ProfileModel.USER_ID > 0)
                    {
                        ServiceData serviceData = new()
                        {
                            TransactionScope = true,
                            Token = this.AuthState.Token()
                        };
                        serviceData["1"].CommandText = this.GetAttribute("WithdrawalCheck");
                        serviceData["1"].AddParameter(nameof(this.ProfileViewModel.ProfileModel.USER_ID), DbType.Int, 3, this.ProfileViewModel.ProfileModel.USER_ID);

                        response = await this.ServiceRequestAsync(serviceData);

                        if (response.Status == Status.OK)
                        {
                            if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0)
                            {
                                if (response.DataSet.DataTables[0].DataRows[0].Int("CNT") > 0)
                                    this.ModalShow($"Question", this.Localization[this.GetAttribute("WithdrawalCheckQuestion")], new() { { this.Localization["탈퇴"], Btn.Danger }, { "Cancel", Btn.Primary } }, EventCallback.Factory.Create<string>(this, this.GetAccessCode));
                                else
                                    this.ModalShow($"Question", this.Localization["정말 탈퇴하시겠습니까?"], new() { { this.Localization["탈퇴"], Btn.Danger }, { "Cancel", Btn.Primary } }, EventCallback.Factory.Create<string>(this, this.GetAccessCode));
                            }
                        }
                        else
                        {
                            if (response.Message != null)
                            {
                                this.ModalShow("Withdrawal", response.Message, new() { { "Ok", Btn.Warning } }, null);
                            }
                        }
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
            }
        }

        private readonly System.Timers.Timer timer = new(1000);
        private async void GetAccessCode(string action)
        {
            try
            {
                if (action != this.Localization["탈퇴"]) return;

                if (this.ProfileViewModel.ProfileModel.EMAIL != null && !this.ProfileViewModel.ProfileModel.AccessCodeVisible)
                {
                    this.ProfileViewModel.ProfileModel.AccessCode = await this.AccessCodeServiceRequestAsync(this.AuthState.Token(), this.ProfileViewModel.ProfileModel.EMAIL, "WITHDRAWAL");
                    this._isFocusElement = false;
                    this.ProfileViewModel.ProfileModel.AccessCodeVisible = true;

                    try
                    {
                        this.timer.Elapsed -= Timer_Elapsed;
                    }
                    catch (Exception)
                    {
                    }
                    this.timer.Elapsed += Timer_Elapsed;
                    this.timer.Start();
                }
            }
            catch (Exception)
            {
            }
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                this.RemainTime = this.RemainTime.Add(new TimeSpan(0, 0, -1));

                if (this.RemainTime.Ticks <= 0)
                {
                    this.ProfileViewModel.ProfileModel.AccessCodeVisible = false;
                    this._isFocusElement = true;
                    this.ProfileViewModel.ProfileModel.AccessCode = null;
                    this.ProfileViewModel.ProfileModel.InputAccessCode = null;
                    this.ProfileViewModel.ProfileModel.AccessCodeConfirmVisible = false;
                    this.RemainTime = new TimeSpan(this.RemainTimeOrg.Ticks);
                    this.timer.Stop();
                }

                this.InvokeAsync(this.StateHasChanged);
            }
            catch (Exception)
            {
            }
        }
        private void HandleInvalidSubmit(EditContext context)
        {
            //this.PasswordResetViewModel.AccessCodeVisible = false;
        }
        private void InputAccessCodeKeydown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter" && this.ProfileViewModel.ProfileModel.AccessCodeVisible && this.ProfileViewModel.ProfileModel.AccessCode == this.ProfileViewModel.ProfileModel.InputAccessCode)
            {
#pragma warning disable CA2012 // 올바르게 ValueTasks 사용
                this.JSRuntime?.InvokeVoidAsync("ElementFocus", "password");
#pragma warning restore CA2012 // 올바르게 ValueTasks 사용
            }
        }
        #endregion


        #region Event
        private async Task InputFileChangeEventArgs(InputFileChangeEventArgs e)
        {
            IBrowserFile? DllFile;
            byte[] bytes;


            if (e.FileCount == 1)
            {
                DllFile = e.File;

                if (DllFile.Size > 2048000)
                {
                    this.ModalShow("Warning", this.Localization["최대 이미지 크기는 2MB입니다."], new() { { "Ok", Btn.Warning } }, null);
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