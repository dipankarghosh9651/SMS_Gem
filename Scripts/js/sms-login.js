/* ============================================================================
   sms-login.js
   Mobile-first Glassmorphism Login Handler
   ========================================================================= */
(function ($, SMS) {
    "use strict";

    $(document).ready(function () {
        var $frmLogin = $("#frmLogin");
        var $btnLogin = $("#btnLogin");
        var $btnText = $("#btnText");

        $frmLogin.on("submit", function (e) {
            e.preventDefault();

            var username = $("#txtUsername").val().trim();
            var password = $("#txtPassword").val().trim();
            var branch = $("#ddlBranch").val();

            if (!username || !password) {
                SMS.showToast("Please enter both username and password.", "warning");
                return;
            }

            $btnLogin.prop("disabled", true);
            $btnText.text("Signing in...");

            var payload = {
                Username: username,
                Password: password,
                Branch: branch
            };

            var url = SMS.resolveUrl("AuthApi", "Authenticate");

            SMS.callApi("POST", url, payload)
                .done(function (res) {
                    if (res && res.Success) {
                        SMS.showToast("Login successful. Redirecting...", "success");
                        setTimeout(function () {
                            window.location.href = res.RedirectUrl || "../Students/StudentEntry_GEM.aspx";
                        }, 800);
                    } else {
                        SMS.showToast(res.Message || "Invalid username or password.", "danger");
                        $btnLogin.prop("disabled", false);
                        $btnText.text("Sign In");
                    }
                })
                .fail(function (xhr) {
                    $btnLogin.prop("disabled", false);
                    $btnText.text("Sign In");
                    SMS.showToast("Unable to connect to the authentication server.", "danger");
                });
        });
    });
})(jQuery, window.SMS);