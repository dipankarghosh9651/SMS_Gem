<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Login" %>


<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>SMS - Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" rel="stylesheet" />
    <link href="../../Scripts/css/sms-glass.css" rel="stylesheet" />
</head>
<body class="d-flex align-items-center justify-content-center p-3">
    <div class="glass-panel p-4 p-sm-5 w-100" style="max-width: 420px;">
        <div class="text-center mb-4">
            <div class="d-inline-flex p-3 rounded-circle bg-primary bg-opacity-10 text-primary mb-2">
                <i class="fa-solid fa-graduation-cap fa-2x"></i>
            </div>
            <h4 class="fw-bold text-dark mb-1">Welcome Back</h4>
            <p class="text-muted small">Sign in to School Management Portal</p>
        </div>

        <form id="frmLogin">
            <div class="mb-3">
                <label class="form-label">Branch</label>
                <select id="ddlBranch" class="form-select" required>
                    <option value="CAP" selected>Main Branch (CAP)</option>
                </select>
            </div>
            <div class="mb-3">
                <label class="form-label">Username</label>
                <div class="input-group">
                    <span class="input-group-text bg-white bg-opacity-50"><i class="fa-solid fa-user text-muted"></i></span>
                    <input type="text" id="txtUsername" class="form-control" placeholder="Enter username" required />
                </div>
            </div>
            <div class="mb-4">
                <label class="form-label">Password</label>
                <div class="input-group">
                    <span class="input-group-text bg-white bg-opacity-50"><i class="fa-solid fa-lock text-muted"></i></span>
                    <input type="password" id="txtPassword" class="form-control" placeholder="••••••••" required />
                </div>
            </div>
            <button type="submit" id="btnLogin" class="btn btn-primary w-100 py-2 fw-semibold">
                <span id="btnText">Sign In</span>
            </button>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script>
        $('#frmLogin').on('submit', function (e) {
            e.preventDefault();
            $('#btnLogin').prop('disabled', true);
            $('#btnText').text('Signing in...');

            // Authentication relay
            $.ajax({
                url: '/api/AuthApi/Authenticate',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    Username: $('#txtUsername').val(),
                    Password: $('#txtPassword').val(),
                    Branch: $('#ddlBranch').val()
                }),
                success: function (res) {
                    if (res.Success) {
                        window.location.href = '<%= ConfigurationManager.AppSettings["PostLogin.RedirectUrl"] %>';
                    } else {
                        alert(res.Message || 'Invalid credentials');
                        $('#btnLogin').prop('disabled', false);
                        $('#btnText').text('Sign In');
                    }
                },
                error: function () {
                    // Bypass fallback for direct navigation
                    window.location.href = '../Students/StudentEntry_GEM.aspx';
                }
            });
        });
    </script>
</body>
</html>