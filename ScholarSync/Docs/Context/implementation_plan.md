# Implementation Plan: Strictly Syncfusion Login & Registration

## Goal Description
Refactor the authentication controls (`LoginControl` and `RegisterControl`) to strictly use Syncfusion controls (`AutoLabel`, `SfTextBox`, `SfButton`, `SfComboBox`) via the updated `SfUIHelper`. Update `SfWelcomeForm` to orchestrate navigation between these two controls.

## User Review Required
> [!IMPORTANT]
> This change replaces standard Windows Forms controls with Syncfusion User Interface controls. Ensure that the Syncfusion licensing is correctly configured if not using the community version, although the code assumes valid packages are present.

## Proposed Changes

### Controls/Auth
#### [MODIFY] [LoginControl.cs](file:///c:/Users/zubai/OneDrive/Desktop/scholarsync/ScholarSync/Controls/Auth/LoginControl.cs)
- Change field types:
  - `Label` -> `AutoLabel`
  - `TextBox` -> `SfTextBox`
  - `ComboBox` -> `SfComboBox`
  - `Button` -> `SfButton`
- Update `InitializeUI` and helper methods to use `SfUIHelper` factory methods returning these new types.
- Fix event handlers to work with Syncfusion control events if they differ (mostly standard Click/TextChanged are compatible).

#### [MODIFY] [RegisterControl.cs](file:///c:/Users/zubai/OneDrive/Desktop/scholarsync/ScholarSync/Controls/Auth/RegisterControl.cs)
- Change field types similar to LoginControl.
- Ensure `ProfileImage` section uses strictly permitted controls (PictureBox is likely standard, but check if `SfUIHelper` allows it or if there's an Sf equivalent - usually PictureBox is fine as it's not a "control" in the same sense, but I will stick to standard PictureBox as wrapped in `SfUIHelper` or just standard). Note: `SfUIHelper` doesn't seem to wrap PictureBox, so standard is fine, but I will check if I can wrap it for consistency if needed.

#### [NEW] [ForgotPasswordControl.cs](file:///c:/Users/zubai/OneDrive/Desktop/scholarsync/ScholarSync/Controls/Auth/ForgotPasswordControl.cs)
- Input: Email Address (Rounded TextBox)
- Action: "Send Verification Code" button (Windows 11 Style)
- Navigation: "Back to Login" link.

#### [MODIFY] [OtpVerificationControl.cs](file:///c:/Users/zubai/OneDrive/Desktop/scholarsync/ScholarSync/Controls/Auth/OtpVerificationControl.cs)
- Input: Numeric Verification Code (Rounded TextBox)
- Action: "Verify" button (Windows 11 Style)
- **Fix**: "Resend Code" button styling (ensure text is visible on white background).
- Logic: Used for both Registration verification and Password Reset verification.

#### [NEW] [ResetPasswordControl.cs](file:///c:/Users/zubai/OneDrive/Desktop/scholarsync/ScholarSync/Controls/Auth/ResetPasswordControl.cs)
- Input: New Password (Rounded TextBox)
- Input: Confirm New Password (Rounded TextBox)
- Action: "Reset Password" button.
- Logic: Validates passwords match and redirects to Login on success.

### Forms/SfForms
#### [MODIFY] [SfWelcomeForm.cs](file:///c:/Users/zubai/OneDrive/Desktop/scholarsync/ScholarSync/Forms/SfForms/SfWelcomeForm.cs)
- Implement logic to switch between `LoginControl`, `RegisterControl`, `ForgotPasswordControl`, and `OtpVerificationControl`.
- Handle `OnLoginAttempt` (placeholder or actual auth).
- Handle `OnRegisterClicked` to show `RegisterControl`.
- Handle `OnBackToLogin` event from all controls.
- Handle `OnForgotPasswordClicked` -> Show `ForgotPasswordControl`.
- Handle `OnVerificationRequested` -> Show `OtpVerificationControl`.
- Handle `OnVerified` -> Show `ResetPasswordControl`.
- Handle `OnPasswordReset` -> Show `LoginControl`.

## Verification Plan

### Manual Verification
1.  **Build**: Compile the project to ensure no type mismatches.
2.  **Run**: Launch the application.
3.  **Login View**: Verify `LoginControl` renders with Syncfusion styles.
    - Check hover effects on buttons.
    - Check watermarks on TextBoxes.
4.  **Navigation**: Click "Create New Account" -> Verify `RegisterControl` loads.
5.  **Register View**: Verify `RegisterControl` renders correctly.
    - Click "Already have an account?" -> Verify it returns to `LoginControl`.
6.  **Functionality**:
    - Try logging in (mock or real).
    - Try navigating back and forth.

### Debugging & Fixes
- **Version Alignment**: Fixed `CS0246` and `MethodNotFoundException` by aligning all Syncfusion packages to version `32.2.4` and performing a clean build.
- **Exception Handling**: Added global exception handling in `Program.cs` to capture startup crashes.
