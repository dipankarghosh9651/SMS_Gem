<%@ Page Title="Guardian Entry" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="GuardianEntry_GEM.aspx.cs" Inherits="SMS_Gem.UI.Guardians.GuardianEntry_GEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .section-header { font-size: 0.95rem; font-weight: 700; color: #1e40af; border-bottom: 2px solid rgba(37, 99, 235, 0.2); padding-bottom: 0.4rem; margin-bottom: 1rem; display: flex; align-items: center; gap: 0.5rem; }
        .form-label span.req { color: #dc2626; font-weight: bold; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        <div class="glass-panel p-3 p-md-4">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                <div>
                    <h4 class="fw-bold text-dark mb-0" id="formTitle"><i class="fa-solid fa-user-shield text-primary me-2"></i>Guardian Information</h4>
                    <small class="text-muted">Parent and legal guardian contact master</small>
                </div>
                <a href="GuardianList_GEM.aspx" class="btn btn-outline-secondary btn-sm px-3"><i class="fa-solid fa-list me-1"></i>View All</a>
            </div>

            <div id="guardianFormWrap">
                <input type="hidden" id="hdnGuardianCode" />
                <input type="hidden" id="hdnGuardianID" value="0" />

                <!-- SECTION 1: Master Code & Identity -->
                <div class="section-header"><i class="fa-solid fa-id-card"></i>Identity & Basic Info</div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Guardian Code</label>
                        <input type="text" class="form-control bg-light" id="txtGuardianCode" readonly disabled />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">First Name <span class="req">*</span></label>
                        <input type="text" class="form-control" id="txtFirstName" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Middle Name</label>
                        <input type="text" class="form-control" id="txtMiddleName" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Last Name <span class="req">*</span></label>
                        <input type="text" class="form-control" id="txtLastName" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Gender</label>
                        <select class="form-select" id="ddlGender"><option value="">-- Select --</option></select>
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Machine / Gate ID</label>
                        <input type="text" class="form-control" id="txtMachineId" placeholder="BIO-G01" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Status</label>
                        <select class="form-select" id="ddlIsActive">
                            <option value="true" selected>Active</option>
                            <option value="false">Inactive</option>
                        </select>
                    </div>
                </div>

                <!-- SECTION 2: Professional Details -->
                <div class="section-header"><i class="fa-solid fa-briefcase"></i>Employment & Financial Details</div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Occupation</label>
                        <input type="text" class="form-control" id="txtOccupation" placeholder="e.g. Software Engineer" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Organization / Employer</label>
                        <input type="text" class="form-control" id="txtOrganization" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Annual Income (₹)</label>
                        <input type="number" step="0.01" class="form-control" id="txtAnnualIncome" placeholder="0.00" />
                    </div>
                </div>

                <!-- SECTION 3: Contact & Address -->
                <div class="section-header"><i class="fa-solid fa-map-location-dot"></i>Contact & Residential Details</div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Primary Phone <span class="req">*</span></label>
                        <input type="tel" class="form-control" id="txtPhone" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Alternate Phone</label>
                        <input type="tel" class="form-control" id="txtAlternatePhone" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">WhatsApp Number</label>
                        <input type="tel" class="form-control" id="txtWhatsAppNumber" />
                    </div>
                    <div class="col-12 col-md-6">
                        <label class="form-label">Email Address</label>
                        <input type="email" class="form-control" id="txtEmail" />
                    </div>
                    <div class="col-12 col-md-6">
                        <label class="form-label">Photo URL</label>
                        <input type="text" class="form-control" id="txtPhotoUrl" placeholder="https://..." />
                    </div>
                    <div class="col-12 col-md-6">
                        <label class="form-label">Address Line 1</label>
                        <input type="text" class="form-control" id="txtAddressLine1" />
                    </div>
                    <div class="col-12 col-md-6">
                        <label class="form-label">Address Line 2</label>
                        <input type="text" class="form-control" id="txtAddressLine2" />
                    </div>
                    <div class="col-12 col-sm-4">
                        <label class="form-label">City</label>
                        <input type="text" class="form-control" id="txtCity" />
                    </div>
                    <div class="col-12 col-sm-4">
                        <label class="form-label">State</label>
                        <input type="text" class="form-control" id="txtState" />
                    </div>
                    <div class="col-12 col-sm-4">
                        <label class="form-label">Pin Code</label>
                        <input type="text" class="form-control" id="txtPinCode" />
                    </div>
                </div>

                <div class="d-flex justify-content-end gap-2 mt-4 pt-3 border-top">
                    <button type="reset" class="btn btn-light px-4">Reset</button>
                    <button type="button" id="btnSaveGuardian" class="btn btn-primary px-4 shadow-sm">
                        <span id="btnSaveLabel">Save Guardian Record</span>
                    </button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            (function ($) {
                var SERVICE_BASE = '<%= ResolveUrl("~/Services/StaffService.asmx/") %>';

                function callAsmx(method, payload, cb) {
                    $.ajax({
                        type: "POST",
                        url: SERVICE_BASE + method,
                        data: JSON.stringify(payload || {}),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (res) {
                            var data = res.d !== undefined ? res.d : res;
                            if (typeof data === "string") { try { data = JSON.parse(data); } catch (e) { } }
                            if (cb) cb(data);
                        }
                    });
                }

                // Lookups
                callAsmx("GetLookup", { lookupType: "Gender" }, function (items) {
                    var html = '<option value="">-- Select --</option>';
                    if (Array.isArray(items)) items.forEach(function (i) { html += '<option value="' + i.Code + '">' + i.Description + '</option>'; });
                    $("#ddlGender").html(html);
                });

                var urlParams = new URLSearchParams(window.location.search);
                var editCode = urlParams.get('code');

                if (editCode) {
                    $("#formTitle").html('<i class="fa-solid fa-user-pen text-primary me-2"></i>Edit Guardian Details');
                    $("#btnSaveLabel").text("Update Record");

                    callAsmx("GetGuardianByCode", { guardianCode: editCode }, function (data) {
                        if (!data) return;
                        $("#hdnGuardianID").val(data.GuardianID || 0);
                        $("#hdnGuardianCode").val(data.Guardian_Code);
                        $("#txtGuardianCode").val(data.Guardian_Code);
                        $("#txtFirstName").val(data.FirstName);
                        $("#txtMiddleName").val(data.MiddleName);
                        $("#txtLastName").val(data.LastName);
                        $("#ddlGender").val(data.Gender);
                        $("#txtMachineId").val(data.Machine_Id);
                        $("#ddlIsActive").val(String(data.IsActive));

                        $("#txtOccupation").val(data.Occupation);
                        $("#txtOrganization").val(data.Organization);
                        $("#txtAnnualIncome").val(data.AnnualIncome);

                        $("#txtPhone").val(data.Phone);
                        $("#txtAlternatePhone").val(data.AlternatePhone);
                        $("#txtWhatsAppNumber").val(data.WhatsAppNumber);
                        $("#txtEmail").val(data.Email);
                        $("#txtPhotoUrl").val(data.PhotoUrl);

                        $("#txtAddressLine1").val(data.AddressLine1);
                        $("#txtAddressLine2").val(data.AddressLine2);
                        $("#txtCity").val(data.City);
                        $("#txtState").val(data.State);
                        $("#txtPinCode").val(data.PinCode);
                    });
                } else {
                    callAsmx("GetNextGuardianCode", {}, function (code) {
                        $("#txtGuardianCode").val(code);
                        $("#hdnGuardianCode").val(code);
                    });
                }

                $("#btnSaveGuardian").on("click", function () {
                    var model = {
                        GuardianID: parseInt($("#hdnGuardianID").val()) || 0,
                        Guardian_Code: $("#hdnGuardianCode").val(),
                        FirstName: $("#txtFirstName").val().trim(),
                        MiddleName: $("#txtMiddleName").val().trim(),
                        LastName: $("#txtLastName").val().trim(),
                        Gender: $("#ddlGender").val(),
                        Machine_Id: $("#txtMachineId").val().trim(),
                        IsActive: $("#ddlIsActive").val() === "true",
                        Occupation: $("#txtOccupation").val().trim(),
                        Organization: $("#txtOrganization").val().trim(),
                        AnnualIncome: parseFloat($("#txtAnnualIncome").val()) || null,
                        Phone: $("#txtPhone").val().trim(),
                        AlternatePhone: $("#txtAlternatePhone").val().trim(),
                        WhatsAppNumber: $("#txtWhatsAppNumber").val().trim(),
                        Email: $("#txtEmail").val().trim(),
                        PhotoUrl: $("#txtPhotoUrl").val().trim(),
                        AddressLine1: $("#txtAddressLine1").val().trim(),
                        AddressLine2: $("#txtAddressLine2").val().trim(),
                        City: $("#txtCity").val().trim(),
                        State: $("#txtState").val().trim(),
                        PinCode: $("#txtPinCode").val().trim()
                    };

                    callAsmx("SaveGuardian", { model: model }, function (res) {
                        if (res.Success) {
                            alert(res.Message);
                            window.location.href = "GuardianList_GEM.aspx";
                        } else {
                            alert(res.Message || "Error saving record.");
                        }
                    });
                });
            })(jQuery);
        });
    </script>
</asp:Content>
