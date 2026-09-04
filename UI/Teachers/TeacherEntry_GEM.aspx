<%@ Page Title="Teacher Entry" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="TeacherEntry_GEM.aspx.cs" Inherits="SMS_Gem.UI.Teachers.TeacherEntry_GEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .section-header {
            font-size: 0.95rem;
            font-weight: 700;
            color: #1e40af;
            border-bottom: 2px solid rgba(37, 99, 235, 0.2);
            padding-bottom: 0.4rem;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .form-label span.req {
            color: #dc2626;
            font-weight: bold;
        }

        .avatar-box {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            border: 2px dashed #cbd5e1;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            background: #f8fafc;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        <div class="glass-panel p-3 p-md-4">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                <div>
                    <h4 class="fw-bold text-dark mb-0" id="formTitle">
                        <i class="fa-solid fa-user-plus text-primary me-2"></i>Faculty Registration
                    </h4>
                    <small class="text-muted">Fill in all personal, academic, and financial payroll details</small>
                </div>
                <a href="TeacherList_GEM.aspx" class="btn btn-outline-secondary btn-sm px-3">
                    <i class="fa-solid fa-list me-1"></i>View All
                </a>
            </div>

            <div id="teacherFormWrap">
                <input type="hidden" id="hdnTeacherCode" />
                <input type="hidden" id="hdnPhotoBase64" />

                <!-- SECTION 1: Department & Identification -->
                <div class="section-header">
                    <i class="fa-solid fa-building-columns"></i>Department & Core Identifiers
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Teacher Code</label>
                        <input type="text" class="form-control bg-light" id="txtTeacherCode" readonly disabled />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Department <span class="req">*</span></label>
                        <select class="form-select" id="ddlDeptCode" required>
                            <option value="">-- Select --</option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Hire Date <span class="req">*</span></label>
                        <input type="date" class="form-control" id="txtHireDate" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Account Status</label>
                        <select class="form-select" id="ddlIsActive">
                            <option value="true" selected>Active</option>
                            <option value="false">Inactive</option>
                        </select>
                    </div>
                </div>

                <!-- SECTION 2: Personal Information -->
                <div class="section-header">
                    <i class="fa-solid fa-id-card"></i>Personal Particulars
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">First Name <span class="req">*</span></label>
                        <input type="text" class="form-control" id="txtFirstName" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Middle Name</label>
                        <input type="text" class="form-control" id="txtMiddleName" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Last Name <span class="req">*</span></label>
                        <input type="text" class="form-control" id="txtLastName" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Date of Birth <span class="req">*</span></label>
                        <input type="date" class="form-control" id="txtDateOfBirth" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Gender <span class="req">*</span></label>
                        <select class="form-select" id="ddlGender" required>
                            <option value="">-- Select --</option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Machine / Biometric ID</label>
                        <input type="text" class="form-control" id="txtMachineId" placeholder="BIO-T01" />
                    </div>
                </div>

                <!-- SECTION 3: Qualification & Experience -->
                <div class="section-header">
                    <i class="fa-solid fa-award"></i>Professional Profile
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Highest Qualification</label>
                        <input type="text" class="form-control" id="txtQualification" placeholder="e.g. M.Sc, B.Ed" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Specialization / Subject</label>
                        <input type="text" class="form-control" id="txtSpecialization" placeholder="e.g. Physics" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Total Experience (Years)</label>
                        <input type="number" class="form-control" id="txtExperienceYears" min="0" />
                    </div>
                </div>

                <!-- SECTION 4: Contact & Address -->
                <div class="section-header">
                    <i class="fa-solid fa-address-book"></i>Contact & Address Information
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-md-6">
                        <label class="form-label">Address Line 1</label>
                        <input type="text" class="form-control" id="txtAddressLine1" />
                    </div>
                    <div class="col-12 col-md-6">
                        <label class="form-label">Address Line 2</label>
                        <input type="text" class="form-control" id="txtAddressLine2" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">City</label>
                        <input type="text" class="form-control" id="txtCity" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">State</label>
                        <input type="text" class="form-control" id="txtState" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Pin Code</label>
                        <input type="text" class="form-control" id="txtPinCode" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Country</label>
                        <input type="text" class="form-control" id="txtCountry" value="India" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Primary Phone</label>
                        <input type="tel" class="form-control" id="txtPhone" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Alternate Phone</label>
                        <input type="tel" class="form-control" id="txtAlternatePhone" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Email Address</label>
                        <input type="email" class="form-control" id="txtEmail" />
                    </div>
                </div>

                <!-- SECTION 5: Banking & Regulatory Identifiers -->
                <div class="section-header">
                    <i class="fa-solid fa-money-check-dollar"></i>Statutory, Identity & Payroll Details
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Government ID (National ID)</label>
                        <input type="text" class="form-control" id="txtAadhaarNumber" placeholder="[Redacted/Placeholder]" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">PAN Number</label>
                        <input type="text" class="form-control" id="txtPanNumber" placeholder="ABCDE1234F" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">PF Number</label>
                        <input type="text" class="form-control" id="txtPfNumber" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-6">
                        <label class="form-label">Bank Account Number</label>
                        <input type="text" class="form-control" id="txtBankAccountNo" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-6">
                        <label class="form-label">Bank IFSC Code</label>
                        <input type="text" class="form-control" id="txtIfscCode" />
                    </div>
                </div>

                <!-- SECTION 6: Media & Photograph -->
                <div class="section-header">
                    <i class="fa-solid fa-camera"></i>Profile Image
                </div>
                <div class="row g-2 g-md-3 mb-4 align-items-center">
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Cloud Photo URL</label>
                        <input type="text" class="form-control" id="txtPhotoUrl" placeholder="https://..." />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-5">
                        <label class="form-label">Upload Photograph (Max 2MB)</label>
                        <input type="file" class="form-control" id="fileTeacherPhoto" accept="image/*" />
                    </div>
                    <div class="col-12 col-sm-12 col-lg-3 d-flex justify-content-center">
                        <div class="avatar-box" id="teacherPhotoPreview">
                            <i class="fa-solid fa-user fa-2x text-muted"></i>
                        </div>
                    </div>
                </div>

                <div class="d-flex justify-content-end gap-2 mt-4 pt-3 border-top">
                    <button type="reset" class="btn btn-light px-4">Reset</button>
                    <button type="button" id="btnSaveTeacher" class="btn btn-primary px-4 shadow-sm">
                        <span id="btnSaveLabel">Save Teacher Record</span>
                    </button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            if (typeof jQuery === 'undefined') {
                console.error("jQuery is missing!");
                return;
            }

            (function ($) {
                var API_BASE = '<%= ResolveUrl("~/api/TeacherApi/") %>';

                function loadLookupFromApi(lookupType, $dropdown) {
                    return $.ajax({
                        type: "GET",
                        url: API_BASE + "GetLookup?id=" + encodeURIComponent(lookupType),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json"
                    }).then(function (res) {
                        var items = (res && res.Data !== undefined) ? res.Data : res;
                        if (typeof items === "string") {
                            try { items = JSON.parse(items); } catch (e) { }
                        }

                        var optionsHtml = '<option value="">-- Select --</option>';
                        if (Array.isArray(items) && items.length > 0) {
                            for (var i = 0; i < items.length; i++) {
                                var item = items[i];
                                var code = item.Code !== undefined ? item.Code : "";
                                var desc = item.Description || item.Desc || code;
                                optionsHtml += '<option value="' + code + '">' + desc + '</option>';
                            }
                        }
                        $dropdown.html(optionsHtml);
                    }).fail(function (xhr) {
                        console.error("Failed to load lookup: " + lookupType, xhr.responseText);
                    });
                }

                // 1. Resolve Dropdowns via Promises first
                var deptPromise = loadLookupFromApi("Department", $("#ddlDeptCode"));
                var genderPromise = loadLookupFromApi("Gender", $("#ddlGender"));

                // 2. Initialize Edit or New Mode
                $.when(deptPromise, genderPromise).always(function () {
                    var urlParams = new URLSearchParams(window.location.search);
                    var editCode = urlParams.get('code');

                    if (editCode) {
                        $("#formTitle").html('<i class="fa-solid fa-user-pen text-primary me-2"></i>Edit Faculty Record');
                        $("#btnSaveLabel").text("Update Record");

                        $.ajax({
                            type: "GET",
                            url: API_BASE + "GetTeacherByCode?code=" + encodeURIComponent(editCode),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (res) {
                                if (!res || !res.Success || !res.Data) return;
                                var data = res.Data;

                                $("#hdnTeacherCode").val(data.Teacher_Code);
                                $("#txtTeacherCode").val(data.Teacher_Code);
                                $("#ddlDeptCode").val(data.Dept_Code);
                                $("#txtHireDate").val(data.HireDate ? data.HireDate.split('T')[0] : "");
                                $("#ddlIsActive").val(String(data.IsActive));

                                $("#txtFirstName").val(data.FirstName);
                                $("#txtMiddleName").val(data.MiddleName);
                                $("#txtLastName").val(data.LastName);
                                $("#txtDateOfBirth").val(data.DateOfBirth ? data.DateOfBirth.split('T')[0] : "");
                                $("#ddlGender").val(data.Gender);
                                $("#txtMachineId").val(data.Machine_Id);

                                $("#txtQualification").val(data.Qualification);
                                $("#txtSpecialization").val(data.Specialization);
                                $("#txtExperienceYears").val(data.Experience_Years);

                                $("#txtAddressLine1").val(data.AddressLine1);
                                $("#txtAddressLine2").val(data.AddressLine2);
                                $("#txtCity").val(data.City);
                                $("#txtState").val(data.State);
                                $("#txtPinCode").val(data.PinCode);
                                $("#txtCountry").val(data.Country || "India");

                                $("#txtPhone").val(data.Phone);
                                $("#txtAlternatePhone").val(data.AlternatePhone);
                                $("#txtEmail").val(data.Email);

                                $("#txtAadhaarNumber").val(data.AadhaarNumber || "");
                                $("#txtPanNumber").val(data.PAN_Number);
                                $("#txtPfNumber").val(data.PF_Number);
                                $("#txtBankAccountNo").val(data.BankAccountNo);
                                $("#txtIfscCode").val(data.IFSC_Code);

                                // Image Preview Population
                                var $preview = $("#teacherPhotoPreview");
                                if (data.TeacherPhotoBase64) {
                                    var src = data.TeacherPhotoBase64.startsWith("data:image")
                                        ? data.TeacherPhotoBase64
                                        : "data:image/jpeg;base64," + data.TeacherPhotoBase64;
                                    $("#hdnPhotoBase64").val(data.TeacherPhotoBase64);
                                    $preview.html('<img src="' + src + '" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                                } else if (data.PhotoUrl) {
                                    $preview.html('<img src="' + data.PhotoUrl + '" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                                }
                            }
                        });
                    } else {
                        $.ajax({
                            type: "GET",
                            url: API_BASE + "GetNextCode",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (res) {
                                if (res && res.Success) {
                                    $("#txtTeacherCode").val(res.Data);
                                    $("#hdnTeacherCode").val(res.Data);
                                }
                            }
                        });
                    }
                });

                // 3. Photo Upload Handler
                $("#fileTeacherPhoto").on("change", function (e) {
                    var file = e.target.files[0];
                    if (!file) return;

                    if (file.size > 2 * 1024 * 1024) {
                        alert("File size must be under 2MB");
                        this.value = "";
                        return;
                    }

                    var reader = new FileReader();
                    reader.onload = function (evt) {
                        var base64 = evt.target.result.split(",")[1];
                        $("#hdnPhotoBase64").val(base64);
                        $("#teacherPhotoPreview").html('<img src="' + evt.target.result + '" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                    };
                    reader.readAsDataURL(file);
                });

                // 4. Save Record
                $("#btnSaveTeacher").on("click", function () {
                    var model = {
                        Teacher_Code: $("#hdnTeacherCode").val(),
                        Dept_Code: $("#ddlDeptCode").val(),
                        Dept_Branch: "CAP",
                        Dept_RID: "FN",
                        HireDate: $("#txtHireDate").val(),
                        IsActive: $("#ddlIsActive").val() === "true",
                        FirstName: $("#txtFirstName").val().trim(),
                        MiddleName: $("#txtMiddleName").val().trim(),
                        LastName: $("#txtLastName").val().trim(),
                        DateOfBirth: $("#txtDateOfBirth").val(),
                        Gender: $("#ddlGender").val(),
                        Machine_Id: $("#txtMachineId").val().trim(),
                        Qualification: $("#txtQualification").val().trim(),
                        Specialization: $("#txtSpecialization").val().trim(),
                        Experience_Years: parseInt($("#txtExperienceYears").val()) || 0,
                        AddressLine1: $("#txtAddressLine1").val().trim(),
                        AddressLine2: $("#txtAddressLine2").val().trim(),
                        City: $("#txtCity").val().trim(),
                        State: $("#txtState").val().trim(),
                        PinCode: $("#txtPinCode").val().trim(),
                        Country: $("#txtCountry").val().trim(),
                        Phone: $("#txtPhone").val().trim(),
                        AlternatePhone: $("#txtAlternatePhone").val().trim(),
                        Email: $("#txtEmail").val().trim(),
                        AadhaarNumber: $("#txtAadhaarNumber").val().trim(),
                        PAN_Number: $("#txtPanNumber").val().trim(),
                        PF_Number: $("#txtPfNumber").val().trim(),
                        BankAccountNo: $("#txtBankAccountNo").val().trim(),
                        IFSC_Code: $("#txtIfscCode").val().trim(),
                        PhotoUrl: $("#txtPhotoUrl").val().trim(),
                        TeacherPhotoBase64: $("#hdnPhotoBase64").val()
                    };

                    var $btn = $(this).prop("disabled", true);
                    $("#btnSaveLabel").text("Saving...");

                    $.ajax({
                        type: "POST",
                        url: API_BASE + "SaveTeacher",
                        data: JSON.stringify(model),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (res) {
                            $btn.prop("disabled", false);
                            $("#btnSaveLabel").text("Save Teacher Record");
                            if (res && res.Success) {
                                alert(res.Message);
                                window.location.href = "TeacherList_GEM.aspx";
                            } else {
                                alert((res && res.Message) || "Error saving record.");
                            }
                        },
                        error: function (xhr) {
                            $btn.prop("disabled", false);
                            $("#btnSaveLabel").text("Save Teacher Record");
                            alert("Server error: " + xhr.responseText);
                        }
                    });
                });

            })(jQuery);
        });
    </script>
</asp:Content>
