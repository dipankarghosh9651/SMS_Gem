/* ============================================================================
   sms-student.js
   Dropdown Binding, Photo Preview, and Form Processing
   ========================================================================= */
(function ($) {
    "use strict";

    var SERVICE_URL = window.SMS_SERVICE_URL || "/api/StudentApi/";
    var params = new URLSearchParams(window.location.search);
    var editCode = params.get("code");
    var isEditMode = !!editCode;

    // Helper: Safe HTML escape
    function escapeHtml(val) {
        if (val === null || typeof val === "undefined") return "";
        return String(val)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    // 1. Dynamic Dropdown Lookup Loader
    function loadLookup(lookupType, $select, selectedValue) {
        if (!$select || $select.length === 0) return $.Deferred().resolve().promise();

        var url = SERVICE_URL.replace(/\/+$/, "") + "/GetLookup/" + encodeURIComponent(lookupType);

        return $.ajax({
            url: url,
            type: "GET",
            dataType: "json"
        })
            .done(function (items) {
                var html = '<option value="">-- Select --</option>';

                if (Array.isArray(items) && items.length > 0) {
                    $.each(items, function (i, item) {
                        var keys = Object.keys(item);
                        var codeKey = keys.find(k => /code|id|val/i.test(k)) || keys[0];
                        var descKey = keys.find(k => /desc|name|text/i.test(k)) || keys[1] || keys[0];

                        var code = item[codeKey] || "";
                        var desc = item[descKey] || code;

                        html += '<option value="' + escapeHtml(code) + '">' + escapeHtml(desc) + '</option>';
                    });
                } else {
                    console.warn("No items returned from database for: " + lookupType);
                }

                $select.html(html);

                if (selectedValue !== undefined && selectedValue !== null && selectedValue !== "") {
                    $select.val(String(selectedValue));
                }
            })
            .fail(function (xhr) {
                console.error("Failed to load lookup: " + lookupType, xhr.status, xhr.responseText);
                $select.html('<option value="">-- Unable to Load --</option>');
            });
    }

    function loadAllDropdowns(preselect) {
        preselect = preselect || {};
        return Promise.all([
            loadLookup("BloodGroup", $("#ddlBloodGroup"), preselect.BloodGroupCode || preselect.bloodGroup),
            loadLookup("Gender", $("#ddlGender"), preselect.GenderCode || preselect.gender),
            loadLookup("Nationality", $("#ddlNationality"), preselect.NationalityCode || preselect.nationality),
            loadLookup("MotherTongue", $("#ddlMotherTongue"), preselect.MotherTongueCode || preselect.motherTongue),
            loadLookup("AdmissionCategory", $("#ddlAdmissionCategory"), preselect.AdmissionCategoryCode || preselect.admissionCategory),
            loadLookup("Religion", $("#ddlReligion"), preselect.ReligionCode || preselect.religion),
            loadLookup("Caste_Category", $("#ddlCaste_Category"), preselect.Caste_Category || preselect.Caste_Category)
        ]);
    }

    // 2. Photo Upload & Preview Handler
    function setPhotoPreview(src) {
        var $preview = $("#photoPreview");
        if (src) {
            $preview.html('<img src="' + src + '" alt="Student Photograph" style="width:100%; height:100%; object-fit:cover;" />');
        } else {
            $preview.html('<i class="fa-solid fa-user fa-2x text-muted"></i>');
        }
    }

    $("#fileStudentPhoto").on("change", function (e) {
        var file = e.target.files && e.target.files[0];
        if (!file) {
            setPhotoPreview(null);
            $("#hdnPhotoBase64").val("");
            return;
        }

        if (file.size > 2 * 1024 * 1024) {
            alert("Please choose an image under 2 MB.");
            this.value = "";
            return;
        }

        var reader = new FileReader();
        reader.onload = function (evt) {
            var dataUrl = evt.target.result;
            var base64 = dataUrl.split(",")[1] || "";
            $("#hdnPhotoBase64").val(base64);
            setPhotoPreview(dataUrl);
        };
        reader.readAsDataURL(file);
    });

    // 3. Auto Student Code Generation
    function initStudentCode() {
        var url = SERVICE_URL.replace(/\/+$/, "") + "/GetNextStudentCode";
        return $.getJSON(url)
            .done(function (code) {
                $("#txtStudentCode").val(code || "");
                $("#hdnStudentCode").val(code || "");
            })
            .fail(function (xhr) {
                console.error("Failed to generate student code:", xhr.status, xhr.responseText);
            });
    }

    // 4. Form Submission
    $("#btnSaveStudent").on("click", function () {
        var firstName = $("#txtFirstName").val().trim();
        var lastName = $("#txtLastName").val().trim();
        var admNo = $("#txtAdmissionNo").val().trim();
        var gender = $("#ddlGender").val();

        if (!firstName || !lastName || !admNo || !gender) {
            alert("Please fill in all mandatory fields marked with *.");
            return;
        }

        var payload = {
            Student_Code: $("#hdnStudentCode").val() || null,
            AdmissionNo: admNo,
            RollNumber: $("#txtRollNumber").val().trim(),
            AdmissionCategory: $("#ddlAdmissionCategory").val(),
            EnrollmentDate: $("#txtEnrollmentDate").val() || null,
            Machine_Id: $("#txtMachineId").val().trim(),
            IsActive: $("#ddlIsActive").val() === "true",

            FirstName: firstName,
            MiddleName: $("#txtMiddleName").val().trim(),
            LastName: lastName,
            F_Name: $("#txtFatherName").val().trim(),
            M_Name: $("#txtMotherName").val().trim(),
            DateOfBirth: $("#txtDateOfBirth").val() || null,
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
            StudentPhoto: $("#hdnPhotoBase64").val() || null,
            Remarks: $("#txtRemarks").val().trim(),

            IsNewRecord: $("#hdnIsNewRecord").val() === "true"
        };

        var $btn = $(this).prop("disabled", true);
        $("#btnSaveLabel").text("Saving...");

        var saveUrl = SERVICE_URL.replace(/\/+$/, "") + "/SaveStudent";

        $.ajax({
            url: saveUrl,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(payload)
        })
            .done(function (res) {
                $btn.prop("disabled", false);
                $("#btnSaveLabel").text("Save Complete Record");

                if (res && res.Success) {
                    alert(res.Message || "Student record saved successfully.");
                    window.location.reload();
                } else {
                    alert((res && res.Message) || "Failed to save record.");
                }
            })
            .fail(function () {
                $btn.prop("disabled", false);
                $("#btnSaveLabel").text("Save Complete Record");
                alert("Error communicating with the server.");
            });
    });

    // 5. Initial Page Bootstrap
    $(document).ready(function () {
        if (isEditMode) {
            $("#formTitle").html('<i class="fa-solid fa-user-pen text-primary me-2"></i>Edit Student');
            $("#btnSaveLabel").text("Update Student");
            $("#formLoading").show();
            $("#entryFormWrap").hide();

            var fetchUrl = SERVICE_URL.replace(/\/+$/, "") + "/GetStudentByCode/" + encodeURIComponent(editCode);

            $.getJSON(fetchUrl)
                .done(function (s) {
                    $("#formLoading").hide();
                    $("#entryFormWrap").show();

                    if (!s) {
                        alert("Student record was not found.");
                        return;
                    }

                    $("#hdnStudentCode").val(s.Student_Code || "");
                    $("#hdnIsNewRecord").val("false");
                    $("#txtStudentCode").val(s.Student_Code || "");
                    $("#txtAdmissionNo").val(s.AdmissionNo || "");
                    $("#txtRollNumber").val(s.RollNumber || "");
                    $("#txtFirstName").val(s.FirstName || "");
                    $("#txtMiddleName").val(s.MiddleName || "");
                    $("#txtLastName").val(s.LastName || "");
                    $("#txtFatherName").val(s.F_Name || "");
                    $("#txtMotherName").val(s.M_Name || "");
                    $("#txtAadhaarNumber").val(s.AadhaarNumber || "");
                    $("#txtAddressLine1").val(s.AddressLine1 || "");
                    $("#txtAddressLine2").val(s.AddressLine2 || "");
                    $("#txtCity").val(s.City || "");
                    $("#txtState").val(s.State || "");
                    $("#txtPinCode").val(s.PinCode || "");
                    $("#txtPhone").val(s.Phone || "");
                    $("#txtAlternatePhone").val(s.AlternatePhone || "");
                    $("#txtEmail").val(s.Email || "");
                    $("#txtRemarks").val(s.Remarks || "");

                    if (s.PhotoUrl) {
                        setPhotoPreview(s.PhotoUrl);
                    } else if (s.StudentPhoto) {
                        setPhotoPreview("data:image/jpeg;base64," + s.StudentPhoto);
                        $("#hdnPhotoBase64").val(s.StudentPhoto);
                    }

                    loadAllDropdowns({
                        bloodGroup: s.BloodGroup_Code,
                        gender: s.Gender,
                        nationality: s.Nationality,
                        motherTongue: s.MotherTongue,
                        admissionCategory: s.AdmissionCategory,
                        religion: s.Religion,
                        Caste_Category: s.Caste_Category
                    });
                })
                .fail(function () {
                    $("#formLoading").hide();
                    $("#entryFormWrap").show();
                    alert("Error loading student details.");
                });
        } else {
            initStudentCode();
            loadAllDropdowns();
        }
    });

})(jQuery);