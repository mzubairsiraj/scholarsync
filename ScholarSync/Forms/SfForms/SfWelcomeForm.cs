using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Configuration;
using ScholarSync.Controls.Auth;
using ScholarSync.UIComponents;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Tools;


namespace ScholarSync.Forms.SyncFusionForms
{
    /// <summary>
    /// Welcome Form - Uses modular LoginControl and RegisterControl
    /// Clean separation of concerns with Syncfusion-only UI
    /// </summary>
    public partial class SfWelcomeForm : SfForm
    {
        private LoginControl loginControl;
        private RegisterControl registerControl;
        private ForgotPasswordControl forgotPasswordControl;
        private OtpVerificationControl otpVerificationControl;
        private ResetPasswordControl resetPasswordControl;
        
        public SfWelcomeForm()
        {
            InitializeComponent();
            UIHelper.ApplyScholarSyncStyle(this);
            this.Text = "Welcome to ScholarSync | Powerful Modern College Management System";
            InitializeModernUI();
        }

        private void InitializeModernUI()
        {
            ShowLoginControl();
        }

        private void ShowLoginControl()
        {
            this.Controls.Clear();
            loginControl = new LoginControl
            {
                Dock = DockStyle.Fill
            };
            loginControl.OnLoginAttempt += LoginControl_OnLoginAttempt;
            loginControl.OnRegisterClicked += LoginControl_OnRegisterClicked;
            loginControl.OnForgotPasswordClicked += LoginControl_OnForgotPasswordClicked;
            this.Controls.Add(loginControl);
        }

        private void ShowRegisterControl()
        {
            this.Controls.Clear();
            registerControl = new RegisterControl
            {
                Dock = DockStyle.Fill
            };
            registerControl.OnBackToLogin += Generic_OnBackToLogin;
            this.Controls.Add(registerControl);
        }

        private void ShowForgotPasswordControl()
        {
            this.Controls.Clear();
            forgotPasswordControl = new ForgotPasswordControl
            {
                Dock = DockStyle.Fill
            };
            forgotPasswordControl.OnBackToLogin += Generic_OnBackToLogin;
            forgotPasswordControl.OnVerificationRequested += ForgotPassword_OnVerificationRequested;
            this.Controls.Add(forgotPasswordControl);
        }

        private void ShowOtpVerificationControl(string email)
        {
            this.Controls.Clear();
            otpVerificationControl = new OtpVerificationControl
            {
                Dock = DockStyle.Fill
            };
            otpVerificationControl.SetTargetEmail(email);
            otpVerificationControl.OnBackToLogin += Generic_OnBackToLogin;
            otpVerificationControl.OnVerified += Otp_OnVerified;
            this.Controls.Add(otpVerificationControl);
        }

        private void ShowResetPasswordControl()
        {
            this.Controls.Clear();
            resetPasswordControl = new ResetPasswordControl
            {
                Dock = DockStyle.Fill
            };
            resetPasswordControl.OnPasswordReset += ResetPassword_OnPasswordReset;
            this.Controls.Add(resetPasswordControl);
        }

        #region Event Handlers

        private void LoginControl_OnLoginAttempt(object sender, LoginEventArgs e)
        {
            // TODO: Implement authentication logic here
            MessageBox.Show(
                $"Login functionality will be implemented here.\n\n" +
                $"Role: {e.Role}\n" +
                $"Email: {e.Email}",
                "Login Attempt",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void LoginControl_OnRegisterClicked(object sender, EventArgs e)
        {
            ShowRegisterControl();
        }

        private void LoginControl_OnForgotPasswordClicked(object sender, EventArgs e)
        {
            ShowForgotPasswordControl();
        }

        private void ForgotPassword_OnVerificationRequested(object sender, string email)
        {
            // Navigate to OTP
            ShowOtpVerificationControl(email);
        }

        private void Otp_OnVerified(object sender, string otp)
        {
            // Navigate to Reset Password
            ShowResetPasswordControl();
        }

        private void ResetPassword_OnPasswordReset(object sender, EventArgs e)
        {
            // Navigate back to Login
            ShowLoginControl();
        }

        private void Generic_OnBackToLogin(object sender, EventArgs e)
        {
            ShowLoginControl();
        }

        #endregion
    }
}
