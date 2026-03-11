# AhaSale Worklog (Codex)
Time zone: Asia/Ho_Chi_Minh (UTC+7).

Note: This log was created on 2026-03-11 07:13:32 +07. Last updated: 2026-03-11 08:10:50 +07. Earlier items are reconstructed from our collaboration and may not include exact timestamps.

Format: Problem -> Result (short).

## 2026-03-10
- Logged-in users could access login/register and referral pages -> Show logout-first modal (De sau + Dang xuat) and redirect to target link after logout.
- Home account dropdown menu layout issues -> Rebuilt toggle behavior; fixed positioning/z-index; pinned logout to footer like admin.
- Admin home bridge config needed (wallet/token/treasury) -> Added admin UI to update config; validation; update Web.config + scripts/usdt_bridge.env without System.Configuration.

## 2026-03-11
- Admin home bridge summary exposed sensitive info -> Only show A points; keep hidden labels to avoid compile errors.
- Bridge config needed extra protection + treasury selection -> Password-gated config panel; treasury account chosen from list (no free text).
- OTP admin account selection lacked search -> Metro select with filter + re-init on async postback for Home/Shop dropdowns.
- Compile error in HomeOtp_cl (TryVerifyFallbackOtp) -> Removed fallback verification call.
- Home login forced password/PIN change redirect -> Disabled forced redirect; always land on /home/default.aspx after login.

Files touched in this worklog (summary):
- /admin/Default.aspx
- /admin/Default.aspx.cs
- /admin/otp/Default.aspx
- /App_Code/check_login_cl.cs
- /App_Code/HomeOtp_cl.cs
- /home/login.aspx.cs
- /Uc/Home/Header_uc.ascx
- /assetscss/header_tabler_UI.css
- /css/header_tabler_UI.css
- /assetscss/admin-home-sync.css
- /css/admin-home-sync.css
- /home/dangky.aspx.cs
- /home/page/gioi-thieu-nguoi-dung.aspx.cs
