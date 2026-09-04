<%@ Page Title="Student Entry" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="StudentEntry_GEM.aspx.cs" Inherits="SMS_Gem.UI.Students.StudentEntry_GEM" %>

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

        .student-avatar {
            width: 110px;
            height: 110px;
            border-radius: 50%;
            border: 2px dashed #94a3b8;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            background-color: #f8fafc;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        <div class="glass-panel p-3 p-md-4">

            <!-- Form Header & Actions -->
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                <div>
                    <h4 class="fw-bold text-dark mb-0" id="formTitle">
                        <i class="fa-solid fa-user-plus text-primary me-2"></i>New Student Admission
                    </h4>
                    <small class="text-muted">Fill in all academic and personal attributes</small>
                </div>
                <div class="d-flex gap-2">
                    <a href="StudentList_GEM.aspx" class="btn btn-outline-secondary btn-sm px-3">
                        <i class="fa-solid fa-list me-1"></i>View All
                    </a>
                </div>
            </div>

            <!-- Loading Spinner Skeleton -->
            <div id="formLoading" style="display: none;" class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <div class="mt-2 text-muted fw-semibold">Processing student data...</div>
            </div>

            <!-- Main Entry Form -->
            <div id="entryFormWrap">
                <div id="studentMasterForm">
                    <input type="hidden" id="hdnStudentCode" />
                    <input type="hidden" id="hdnIsNewRecord" value="true" />
                    <input type="hidden" id="hdnPhotoBase64" />

                    <!-- SECTION 1: Identifiers & Academic Info -->
                    <div class="section-header">
                        <i class="fa-solid fa-graduation-cap"></i>Identifiers & Academic Details
                    </div>
                    <div class="row g-2 g-md-3 mb-4">
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Student Code</label>
                            <input type="text" class="form-control bg-light" id="txtStudentCode" readonly disabled />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Admission No <span class="req">*</span></label>
                            <input type="text" class="form-control" id="txtAdmissionNo" placeholder="ADM-001" required />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Roll Number</label>
                            <input type="text" class="form-control" id="txtRollNumber" placeholder="e.g. 101" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Admission Category</label>
                            <select class="form-select" id="ddlAdmissionCategory">
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Enrollment Date</label>
                            <input type="date" class="form-control" id="txtEnrollmentDate" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Machine / Biometric ID</label>
                            <input type="text" class="form-control" id="txtMachineId" placeholder="BIO-001" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Account Status</label>
                            <select class="form-select" id="ddlIsActive">
                                <option value="true" selected>Active</option>
                                <option value="false">Inactive</option>
                            </select>
                        </div>
                    </div>

                    <!-- SECTION 2: Personal Details -->
                    <div class="section-header">
                        <i class="fa-solid fa-id-card"></i>Personal Details
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
                        <div class="col-12 col-sm-6 col-lg-6">
                            <label class="form-label">Father's Name</label>
                            <input type="text" class="form-control" id="txtFatherName" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-6">
                            <label class="form-label">Mother's Name</label>
                            <input type="text" class="form-control" id="txtMotherName" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Date of Birth</label>
                            <input type="date" class="form-control" id="txtDateOfBirth" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Gender <span class="req">*</span></label>
                            <select class="form-select" id="ddlGender" required>
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Blood Group</label>
                            <select class="form-select" id="ddlBloodGroup">
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                    </div>

                    <!-- SECTION 3: Demographics & Background -->
                    <div class="section-header">
                        <i class="fa-solid fa-earth-americas"></i>Demographics & Background
                    </div>
                    <div class="row g-2 g-md-3 mb-4">
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Nationality</label>
                            <select class="form-select" id="ddlNationality">
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Mother Tongue</label>
                            <select class="form-select" id="ddlMotherTongue">
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Religion</label>
                            <select class="form-select" id="ddlReligion">
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-3">
                            <label class="form-label">Caste</label>
                            <select class="form-select" id="ddlCaste_Category">
                                <option value="">-- Select --</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Government ID (National ID)</label>
                            <input type="text" class="form-control" id="txtAadhaarNumber" placeholder="[Redacted/Placeholder]" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Previous School Attended</label>
                            <input type="text" class="form-control" id="txtPreviousSchool" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Transfer Certificate (TC) No</label>
                            <input type="text" class="form-control" id="txtTcNumber" />
                        </div>
                    </div>

                    <!-- SECTION 4: Address & Contact Details -->
                    <div class="section-header">
                        <i class="fa-solid fa-address-book"></i>Address & Contact Information
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
                            <label class="form-label">Primary Contact Phone</label>
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

                    <!-- SECTION 5: Systems, Photos & Metadata -->
                    <div class="section-header">
                        <i class="fa-solid fa-camera"></i>Media & Systems Integration
                    </div>
                    <div class="row g-2 g-md-3 mb-4 align-items-center">
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">RFID Smartcard UID</label>
                            <input type="text" class="form-control" id="txtRfidTag" placeholder="e.g. A3F28B" />
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Parent Portal Access?</label>
                            <select class="form-select" id="ddlPortalAccess">
                                <option value="true" selected>Enabled</option>
                                <option value="false">Disabled</option>
                            </select>
                        </div>
                        <div class="col-12 col-sm-6 col-lg-4">
                            <label class="form-label">Cloud Photo URL (Optional)</label>
                            <input type="text" class="form-control" id="txtPhotoUrl" placeholder="https://..." />
                        </div>
                        <div class="col-12 col-sm-8 col-md-9">
                            <label class="form-label">Upload Student Photograph (Max 2MB)</label>
                            <input type="file" class="form-control" id="fileStudentPhoto" accept="image/*" />
                        </div>
                        <div class="col-12 col-sm-4 col-md-3 d-flex justify-content-center justify-content-sm-start mt-3 mt-sm-0">
                            <div class="student-avatar" id="photoPreview">
                                <i class="fa-solid fa-user fa-2x text-muted"></i>
                            </div>
                        </div>
                        <div class="col-12 mt-3">
                            <label class="form-label">Administrative Remarks</label>
                            <textarea class="form-control" id="txtRemarks" rows="2" placeholder="Special medical conditions, sports quota, or general remarks..."></textarea>
                        </div>
                    </div>

                    <!-- Form Controls -->
                    <div class="d-flex justify-content-end gap-2 mt-4 pt-3 border-top">
                        <button type="reset" class="btn btn-light px-4" id="btnResetForm">Reset</button>
                        <button type="button" id="btnSaveStudent" class="btn btn-primary px-4 shadow-sm">
                            <span id="btnSaveLabel">Save Complete Record</span>
                        </button>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!-- Self-Contained StudentService.asmx Integration Script -->
    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            if (typeof jQuery === 'undefined') {
                console.error("jQuery is missing!");
                return;
            }

            (function ($) {
                var ASMX_BASE = '<%= ResolveUrl("~/Services/StudentService.asmx/") %>';
                var defaultBranch = localStorage.getItem("SMS_BRANCH") || "CAP";

                function getUrlParam(param) {
                    var urlParams = new URLSearchParams(window.location.search);
                    return urlParams.get(param);
                }

                function fillSelect($el, items) {
                    if (!$el || $el.length === 0) return;
                    var html = '<option value="">-- Select --</option>';
                    if (Array.isArray(items) && items.length > 0) {
                        for (var i = 0; i < items.length; i++) {
                            var itm = items[i];
                            var code = itm.Code !== undefined ? itm.Code : "";
                            var desc = itm.Desc !== undefined ? itm.Desc : code;
                            html += '<option value="' + code + '">' + desc + '</option>';
                        }
                    }
                    $el.html(html);
                }

                function callService(methodName, payload, successCallback, errorCallback) {
                    $.ajax({
                        type: "POST",
                        url: ASMX_BASE + methodName,
                        data: JSON.stringify(payload || {}),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (res) {
                            var data = res.d !== undefined ? res.d : res;
                            if (typeof data === "string") {
                                try { data = JSON.parse(data); } catch (e) { }
                            }
                            if (successCallback) successCallback(data);
                        },
                        error: function (xhr) {
                            console.error("Service error on " + methodName + ":", xhr.responseText);
                            if (errorCallback) errorCallback(xhr);
                        }
                    });
                }

                // 1. Batch Load All Master Dropdowns
                callService("GetAllLookups", { branch: defaultBranch }, function (lookups) {
                    if (lookups) {
                        fillSelect($("#ddlBloodGroup"), lookups.BloodGroup);
                        fillSelect($("#ddlGender"), lookups.Gender);
                        fillSelect($("#ddlNationality"), lookups.Nationality);
                        fillSelect($("#ddlMotherTongue"), lookups.MotherTongue);
                        fillSelect($("#ddlAdmissionCategory"), lookups.AdmissionCategory);
                        fillSelect($("#ddlReligion"), lookups.Religion);
                        fillSelect($("#ddlCaste_Category"), lookups.Caste_Category);

                        // Once lookups are populated, bind existing record if in Edit mode
                        bindExistingStudentIfEdit();
                    }
                });

                // 2. Check for Edit Mode vs New Code Generation
                var editCode = getUrlParam("code");
                function bindExistingStudentIfEdit() {
                    if (editCode) {
                        $("#formTitle").html('<i class="fa-solid fa-user-pen text-primary me-2"></i>Edit Student Admission');
                        $("#btnSaveLabel").text("Update Record");
                        $("#hdnIsNewRecord").val("false");

                        callService("GetStudentList", { branch: defaultBranch, searchTerm: editCode }, function (list) {
                            if (Array.isArray(list) && list.length > 0) {
                                var s = list[0];
                                $("#txtStudentCode").val(s.Student_Code || "");
                                $("#hdnStudentCode").val(s.Student_Code || "");
                                $("#txtAdmissionNo").val(s.AdmissionNo || "");
                                $("#txtRollNumber").val(s.RollNumber || "");
                                $("#txtFirstName").val(s.FirstName || "");
                                $("#txtMiddleName").val(s.MiddleName || "");
                                $("#txtLastName").val(s.LastName || "");
                                $("#ddlGender").val(s.Gender || "");
                                $("#txtPhone").val(s.Phone || "");
                                $("#ddlIsActive").val(s.IsActive !== false ? "true" : "false");
                            }
                        });
                    } else {
                        // Generate Next Sequential Code for New Record
                        callService("GetNextStudentCode", { branch: defaultBranch }, function (code) {
                            $("#txtStudentCode").val(code || "");
                            $("#hdnStudentCode").val(code || "");
                        });
                    }
                }

                // 3. Photo File Preview and Base64 Encoding
                $(document).on("change", "#fileStudentPhoto", function (e) {
                    var file = e.target.files && e.target.files[0];
                    var $preview = $("#photoPreview");

                    if (!file) {
                        $preview.html('<i class="fa-solid fa-user fa-2x text-muted"></i>');
                        $("#hdnPhotoBase64").val("");
                        return;
                    }

                    if (file.size > 2 * 1024 * 1024) {
                        alert("Photograph must be under 2 MB.");
                        this.value = "";
                        return;
                    }

                    var reader = new FileReader();
                    reader.onload = function (evt) {
                        var dataUrl = evt.target.result;
                        var base64 = dataUrl.split(",")[1] || "";
                        $("#hdnPhotoBase64").val(base64);
                        $preview.html('<img src="' + dataUrl + '" alt="Preview" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                    };
                    reader.readAsDataURL(file);
                });

                // 4. Save Record Event Handler
                $("#btnSaveStudent").on("click", function () {
                    var firstName = $("#txtFirstName").val().trim();
                    var lastName = $("#txtLastName").val().trim();
                    var admNo = $("#txtAdmissionNo").val().trim();
                    var gender = $("#ddlGender").val();

                    if (!firstName || !lastName || !admNo || !gender) {
                        alert("Please fill in First Name, Last Name, Admission No, and Gender.");
                        return;
                    }

                    var model = {
                        Branch: defaultBranch,
                        Student_Code: $("#hdnStudentCode").val(),
                        AdmissionNo: admNo,
                        RollNumber: $("#txtRollNumber").val().trim(),
                        AdmissionCategory: $("#ddlAdmissionCategory").val(),
                        EnrollmentDate: $("#txtEnrollmentDate").val(),
                        Machine_Id: $("#txtMachineId").val().trim(),
                        IsActive: $("#ddlIsActive").val() === "true",

                        FirstName: firstName,
                        MiddleName: $("#txtMiddleName").val().trim(),
                        LastName: lastName,
                        F_Name: $("#txtFatherName").val().trim(),
                        M_Name: $("#txtMotherName").val().trim(),
                        DateOfBirth: $("#txtDateOfBirth").val(),
                        Gender: gender,
                        BloodGroup_Code: $("#ddlBloodGroup").val(),

                        Nationality: $("#ddlNationality").val(),
                        MotherTongue: $("#ddlMotherTongue").val(),
                        Religion: $("#ddlReligion").val(),
                        Caste_Category: $("#ddlCaste_Category").val(),
                        AadhaarNumber: $("#txtAadhaarNumber").val().trim(),
                        PreviousSchool: $("#txtPreviousSchool").val().trim(),
                        TC_Number: $("#txtTcNumber").val().trim(),

                        AddressLine1: $("#txtAddressLine1").val().trim(),
                        AddressLine2: $("#txtAddressLine2").val().trim(),
                        City: $("#txtCity").val().trim(),
                        State: $("#txtState").val().trim(),
                        PinCode: $("#txtPinCode").val().trim(),
                        Country: $("#txtCountry").val().trim(),

                        Phone: $("#txtPhone").val().trim(),
                        AlternatePhone: $("#txtAlternatePhone").val().trim(),
                        Email: $("#txtEmail").val().trim(),

                        RFID_Tag: $("#txtRfidTag").val().trim(),
                        PortalAccess: $("#ddlPortalAccess").val() === "true",
                        PhotoUrl: $("#txtPhotoUrl").val().trim(),
                        StudentPhoto: $("#hdnPhotoBase64").val(),
                        Remarks: $("#txtRemarks").val().trim()
                    };

                    var $btn = $(this).prop("disabled", true);
                    $("#btnSaveLabel").text("Saving...");

                    callService("SaveStudent", { model: model }, function (result) {
                        $btn.prop("disabled", false);
                        $("#btnSaveLabel").text("Save Complete Record");
                        if (result && result.Success) {
                            alert(result.Message || "Student record saved successfully!");
                            window.location.href = "StudentList_GEM.aspx";
                        } else {
                            alert((result && result.Message) || "Save failed.");
                        }
                    }, function () {
                        $btn.prop("disabled", false);
                        $("#btnSaveLabel").text("Save Complete Record");
                        alert("An error occurred while saving the record.");
                    });
                });

            })(jQuery);
        });


        window.addEventListener('DOMContentLoaded', function () {
            if (typeof jQuery === 'undefined') return;

            (function ($) {
                var ASMX_BASE = '<%= ResolveUrl("~/Services/StudentService.asmx/") %>';
                var defaultBranch = localStorage.getItem("SMS_BRANCH") || "CAP";

                function getUrlParam(param) {
                    var urlParams = new URLSearchParams(window.location.search);
                    return urlParams.get(param);
                }

                function fillSelect($el, items) {
                    if (!$el || $el.length === 0) return;
                    var html = '<option value="">-- Select --</option>';
                    if (Array.isArray(items)) {
                        for (var i = 0; i < items.length; i++) {
                            var code = items[i].Code !== undefined ? items[i].Code : "";
                            var desc = items[i].Desc !== undefined ? items[i].Desc : code;
                            html += '<option value="' + code + '">' + desc + '</option>';
                        }
                    }
                    $el.html(html);
                }

                function callService(methodName, payload, successCallback) {
                    $.ajax({
                        type: "POST",
                        url: ASMX_BASE + methodName,
                        data: JSON.stringify(payload || {}),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (res) {
                            var data = res.d !== undefined ? res.d : res;
                            if (typeof data === "string") {
                                try { data = JSON.parse(data); } catch (e) { }
                            }
                            if (successCallback) successCallback(data);
                        },
                        error: function (xhr) {
                            console.error("Service error on " + methodName + ":", xhr.responseText);
                        }
                    });
                }

                // 1. Fetch All Lookups first, then proceed to bind student data
                callService("GetAllLookups", { branch: defaultBranch }, function (lookups) {
                    if (lookups) {
                        fillSelect($("#ddlBloodGroup"), lookups.BloodGroup);
                        fillSelect($("#ddlGender"), lookups.Gender);
                        fillSelect($("#ddlNationality"), lookups.Nationality);
                        fillSelect($("#ddlMotherTongue"), lookups.MotherTongue);
                        fillSelect($("#ddlAdmissionCategory"), lookups.AdmissionCategory);
                        fillSelect($("#ddlReligion"), lookups.Religion);
                        fillSelect($("#ddlCaste_Category"), lookups.Caste_Category);

                        initFormState();
                    }
                });

                // 2. Populate form fields
                function initFormState() {
                    var editCode = getUrlParam("code");

                    if (editCode) {
                        $("#formTitle").html('<i class="fa-solid fa-user-pen text-primary me-2"></i>Edit Student Admission');
                        $("#btnSaveLabel").text("Update Complete Record");
                        $("#hdnIsNewRecord").val("false");

                        callService("GetStudentByCode", { branch: defaultBranch, studentCode: editCode }, function (s) {
                            if (s) {
                                // Section 1: Identifiers & Academic
                                $("#txtStudentCode").val(s.Student_Code || "");
                                $("#hdnStudentCode").val(s.Student_Code || "");
                                $("#txtAdmissionNo").val(s.AdmissionNo || "");
                                $("#txtRollNumber").val(s.RollNumber || "");
                                $("#ddlAdmissionCategory").val(s.AdmissionCategory || "");
                                $("#txtEnrollmentDate").val(s.EnrollmentDate || "");
                                $("#txtMachineId").val(s.Machine_Id || "");
                                $("#ddlIsActive").val(s.IsActive !== false ? "true" : "false");

                                // Section 2: Personal Details
                                $("#txtFirstName").val(s.FirstName || "");
                                $("#txtMiddleName").val(s.MiddleName || "");
                                $("#txtLastName").val(s.LastName || "");
                                $("#txtFatherName").val(s.F_Name || "");
                                $("#txtMotherName").val(s.M_Name || "");
                                $("#txtDateOfBirth").val(s.DateOfBirth || "");
                                $("#ddlGender").val(s.Gender || "");
                                $("#ddlBloodGroup").val(s.BloodGroup_Code || "");

                                // Section 3: Demographics & Background
                                $("#ddlNationality").val(s.Nationality || "");
                                $("#ddlMotherTongue").val(s.MotherTongue || "");
                                $("#ddlReligion").val(s.Religion || "");
                                $("#ddlCaste_Category").val(s.Caste_Category || "");
                                $("#txtAadhaarNumber").val(s.AadhaarNumber || "");
                                $("#txtPreviousSchool").val(s.PreviousSchool || "");
                                $("#txtTcNumber").val(s.TC_Number || "");

                                // Section 4: Address & Contact Details
                                $("#txtAddressLine1").val(s.AddressLine1 || "");
                                $("#txtAddressLine2").val(s.AddressLine2 || "");
                                $("#txtCity").val(s.City || "");
                                $("#txtState").val(s.State || "");
                                $("#txtPinCode").val(s.PinCode || "");
                                $("#txtCountry").val(s.Country || "India");
                                $("#txtPhone").val(s.Phone || "");
                                $("#txtAlternatePhone").val(s.AlternatePhone || "");
                                $("#txtEmail").val(s.Email || "");

                                // Section 5: Systems & Media
                                $("#txtRfidTag").val(s.RFID_Tag || "");
                                $("#ddlPortalAccess").val(s.PortalAccess !== false ? "true" : "false");
                                $("#txtPhotoUrl").val(s.PhotoUrl || "");
                                $("#txtRemarks").val(s.Remarks || "");

                                if (s.PhotoUrl) {
                                    $("#photoPreview").html('<img src="' + s.PhotoUrl + '" alt="Photo" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                                }
                            }
                        });
                    } else {
                        callService("GetNextStudentCode", { branch: defaultBranch }, function (code) {
                            $("#txtStudentCode").val(code || "");
                            $("#hdnStudentCode").val(code || "");
                        });
                    }
                }

                // --- Photo & Media Population ---
                var $preview = $("#photoPreview");

                if (data.StudentPhotoBase64) {
                    // If stored as raw base64 or byte array
                    var imgSrc = data.StudentPhotoBase64.startsWith("data:image")
                        ? data.StudentPhotoBase64
                        : "data:image/jpeg;base64," + data.StudentPhotoBase64;

                    $("#hdnPhotoBase64").val(data.StudentPhotoBase64);
                    $preview.html('<img src="' + imgSrc + '" alt="Student Photo" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                } else if (data.PhotoUrl) {
                    // Fallback to hosted/cloud URL if Base64 isn't populated
                    $preview.html('<img src="' + data.PhotoUrl + '" alt="Student Photo" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />');
                } else {
                    // Default placeholder icon
                    $preview.html('<i class="fa-solid fa-user fa-2x text-muted"></i>');
                    $("#hdnPhotoBase64").val("");
                }


            })(jQuery);
        });



    </script>
</asp:Content>