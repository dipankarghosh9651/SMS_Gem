
<%@ Page Title="Guardian Mapping" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="StudentGuardianMapEntry_GEM.aspx.cs" Inherits="SMS_Gem.UI.Students.StudentGuardianMapEntry_GEM" %>

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
        .search-table tbody tr {
            cursor: pointer;
            transition: background 0.15s ease-in-out;
        }
        .search-table tbody tr:hover {
            background-color: rgba(37, 99, 235, 0.08);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        <div class="glass-panel p-3 p-md-4">

            <!-- Header -->
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                <div>
                    <h4 class="fw-bold text-dark mb-0">
                        <i class="fa-solid fa-people-arrows text-primary me-2"></i>Student Guardian Mapping
                    </h4>
                    <small class="text-muted">Link students with guardians, configure relationships, and configure permissions</small>
                </div>
            </div>

            <div id="mappingFormWrap">
                <input type="hidden" id="hdnMapID" value="0" />
                <input type="hidden" id="hdnStudentBranch" />
                <input type="hidden" id="hdnGuardianBranch" />
                <input type="hidden" id="hdnRelBranch" />

                <!-- SECTION 1: Student Selection -->
                <div class="section-header">
                    <i class="fa-solid fa-user-graduate"></i>Target Student Selection
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-md-4">
                        <label class="form-label">Student Code <span class="req">*</span></label>
                        <div class="input-group">
                            <input type="text" class="form-control bg-light" id="txtStudentCode" readonly required placeholder="Select Student..." />
                            <button class="btn btn-outline-primary" type="button" id="btnOpenStudentLookup" data-bs-toggle="modal" data-bs-target="#studentLookupModal">
                                <i class="fa-solid fa-magnifying-glass me-1"></i>Lookup
                            </button>
                        </div>
                    </div>
                    <div class="col-12 col-md-4">
                        <label class="form-label">Student Name</label>
                        <input type="text" class="form-control bg-light" id="txtStudentName" readonly disabled />
                    </div>
                    <div class="col-12 col-md-4">
                        <label class="form-label">Admission / Roll No</label>
                        <input type="text" class="form-control bg-light" id="txtAdmissionNo" readonly disabled />
                    </div>
                </div>

                <!-- SECTION 2: Guardian Details & Relationship -->
                <div class="section-header">
                    <i class="fa-solid fa-user-shield"></i>Guardian & Relationship Details
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Guardian ID <span class="req">*</span></label>
                        <input type="number" class="form-control" id="txtGuardianID" placeholder="Numeric ID" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Guardian Code <span class="req">*</span></label>
                        <input type="text" class="form-control" id="txtGuardianCode" maxlength="5" placeholder="e.g. G0001" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Relationship Type <span class="req">*</span></label>
                        <select class="form-select" id="ddlRelationship" required>
                            <option value="">-- Select --</option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-lg-3">
                        <label class="form-label">Contact Priority <span class="req">*</span></label>
                        <input type="number" class="form-control" id="txtContactPriority" value="1" min="1" max="10" required />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Specific Emergency Phone</label>
                        <input type="tel" class="form-control" id="txtSpecificPhone" maxlength="20" placeholder="+1234567890" />
                    </div>
                    <div class="col-12 col-sm-6 col-lg-4">
                        <label class="form-label">Machine / Biometric ID</label>
                        <input type="text" class="form-control" id="txtMachineId" maxlength="50" placeholder="Terminal/Card ID" />
                    </div>
                </div>

                <!-- SECTION 3: Permissions & Role Flags -->
                <div class="section-header">
                    <i class="fa-solid fa-sliders"></i>Permissions & Notification Flags
                </div>
                <div class="row g-2 g-md-3 mb-4">
                    <div class="col-12 col-sm-6 col-md-4">
                        <div class="form-check form-switch mt-2">
                            <input class="form-check-input" type="checkbox" id="chkIsPrimaryContact" checked />
                            <label class="form-check-label fw-semibold" for="chkIsPrimaryContact">Is Primary Contact</label>
                        </div>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <div class="form-check form-switch mt-2">
                            <input class="form-check-input" type="checkbox" id="chkIsEmergencyContact" checked />
                            <label class="form-check-label fw-semibold" for="chkIsEmergencyContact">Is Emergency Contact</label>
                        </div>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <div class="form-check form-switch mt-2">
                            <input class="form-check-input" type="checkbox" id="chkCanPickup" checked />
                            <label class="form-check-label fw-semibold" for="chkCanPickup">Authorized for Pickup</label>
                        </div>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <div class="form-check form-switch mt-2">
                            <input class="form-check-input" type="checkbox" id="chkCanViewReportCard" checked />
                            <label class="form-check-label fw-semibold" for="chkCanViewReportCard">Can View Report Cards</label>
                        </div>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <div class="form-check form-switch mt-2">
                            <input class="form-check-input" type="checkbox" id="chkCanReceiveSMS" checked />
                            <label class="form-check-label fw-semibold" for="chkCanReceiveSMS">Receive SMS Notifications</label>
                        </div>
                    </div>
                    <div class="col-12 col-sm-6 col-md-4">
                        <div class="form-check form-switch mt-2">
                            <input class="form-check-input" type="checkbox" id="chkCanReceiveEmail" checked />
                            <label class="form-check-label fw-semibold" for="chkCanReceiveEmail">Receive Email Notifications</label>
                        </div>
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="d-flex justify-content-end gap-2 mt-4 pt-3 border-top">
                    <button type="reset" class="btn btn-light px-4" id="btnResetForm">Reset</button>
                    <button type="button" id="btnSaveMapping" class="btn btn-primary px-4 shadow-sm">
                        <span id="btnSaveLabel">Save Mapping</span>
                    </button>
                </div>
            </div>

        </div>
    </div>

    <!-- Student Lookup Modal -->
    <div class="modal fade" id="studentLookupModal" tabindex="-1" aria-labelledby="studentLookupModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title fw-bold" id="studentLookupModalLabel">
                        <i class="fa-solid fa-users text-primary me-2"></i>Select Student
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="input-group mb-3">
                        <input type="text" id="txtStudentModalSearch" class="form-control" placeholder="Search by name, admission #, or code..." />
                        <button class="btn btn-primary" type="button" id="btnSearchStudents">
                            <i class="fa-solid fa-magnifying-glass"></i> Search
                        </button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-hover table-bordered search-table mb-0" id="tblStudentList">
                            <thead class="table-light">
                                <tr>
                                    <th>Code</th>
                                    <th>Admission No</th>
                                    <th>Student Name</th>
                                    <th>Branch</th>
                                    <th class="text-center">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="5" class="text-center text-muted py-3">Type a search term or click search to view students.</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Integration Scripts -->
    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            if (typeof jQuery === 'undefined') return;

            (function ($) {
                var API_BASE = '<%= ResolveUrl("~/api/StudentApi_CL/") %>';

                function loadRelationshipLookups() {
                    $.ajax({
                        type: "GET",
                        url: API_BASE + "GetLookup?id=RELATIONSHIP",
                        success: function (res) {
                            var items = res.Data || res;
                            var html = '<option value="">-- Select --</option>';
                            if (Array.isArray(items)) {
                                items.forEach(function (itm) {
                                    var code = itm.Code || itm.Rel_Code || "";
                                    var desc = itm.Desc || itm.Rel_Name || code;
                                    html += '<option value="' + code + '">' + desc + '</option>';
                                });
                            }
                            $("#ddlRelationship").html(html);
                        },
                        error: function (xhr) {
                            console.error("Failed to load relationships", xhr.responseText);
                        }
                    });
                }
                loadRelationshipLookups();

                function fetchStudents(query) {
                    var $tbody = $("#tblStudentList tbody");
                    $tbody.html('<tr><td colspan="5" class="text-center text-muted py-3"><div class="spinner-border spinner-border-sm text-primary me-2"></div>Loading...</td></tr>');

                    $.ajax({
                        type: "GET",
                        url: API_BASE + "GetStudentList?searchTerm=" + encodeURIComponent(query || ""),
                        success: function (res) {
                            var list = res.Data || res || [];
                            if (!Array.isArray(list) || list.length === 0) {
                                $tbody.html('<tr><td colspan="5" class="text-center text-muted py-3">No students found.</td></tr>');
                                return;
                            }
                            var html = '';
                            list.forEach(function (s) {
                                var code = s.Student_Code || '';
                                var branch = s.Branch || 'CAP';
                                var adm = s.AdmissionNo || '';
                                var name = [s.FirstName, s.MiddleName, s.LastName].filter(Boolean).join(' ');

                                html += '<tr data-code="' + code + '" data-branch="' + branch + '" data-adm="' + adm + '" data-name="' + name + '">';
                                html += '<td><strong>' + code + '</strong></td>';
                                html += '<td>' + adm + '</td>';
                                html += '<td>' + name + '</td>';
                                html += '<td><span class="badge bg-secondary">' + branch + '</span></td>';
                                html += '<td class="text-center"><button type="button" class="btn btn-sm btn-outline-primary btn-select-student">Select</button></td>';
                                html += '</tr>';
                            });
                            $tbody.html(html);
                        },
                        error: function () {
                            $tbody.html('<tr><td colspan="5" class="text-danger text-center py-3">Error fetching student records.</td></tr>');
                        }
                    });
                }

                $("#btnSearchStudents").on("click", function () {
                    fetchStudents($("#txtStudentModalSearch").val().trim());
                });

                $("#txtStudentModalSearch").on("keypress", function (e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        fetchStudents($(this).val().trim());
                    }
                });

                $('#studentLookupModal').on('shown.bs.modal', function () {
                    fetchStudents($("#txtStudentModalSearch").val().trim());
                });

                $(document).on("click", ".btn-select-student, #tblStudentList tbody tr", function (e) {
                    var $row = $(this).closest("tr");
                    var code = $row.data("code");
                    if (!code) return;

                    $("#txtStudentCode").val(code);
                    $("#hdnStudentBranch").val($row.data("branch"));
                    $("#txtStudentName").val($row.data("name"));
                    $("#txtAdmissionNo").val($row.data("adm"));

                    var modalEl = document.getElementById('studentLookupModal');
                    var modalInstance = bootstrap.Modal.getInstance(modalEl);
                    if (modalInstance) modalInstance.hide();
                });

                $("#btnSaveMapping").on("click", function () {
                    var studentCode = $("#txtStudentCode").val().trim();
                    var guardianId = $("#txtGuardianID").val().trim();
                    var guardianCode = $("#txtGuardianCode").val().trim();
                    var relCode = $("#ddlRelationship").val();

                    if (!studentCode || !guardianId || !guardianCode || !relCode) {
                        alert("Please select a Student and provide Guardian ID, Guardian Code, and Relationship.");
                        return;
                    }

                    var payload = {
                        MapID: parseInt($("#hdnMapID").val(), 10) || 0,
                        Student_Branch: $("#hdnStudentBranch").val(),
                        Student_Code: studentCode,
                        GuardianID: parseInt(guardianId, 10),
                        Guardian_Branch: $("#hdnGuardianBranch").val() || $("#hdnStudentBranch").val(),
                        Guardian_Code: guardianCode,
                        Rel_Branch: $("#hdnRelBranch").val() || $("#hdnStudentBranch").val(),
                        Rel_RID: "RL",
                        Rel_Code: relCode,
                        IsPrimaryContact: $("#chkIsPrimaryContact").is(":checked"),
                        IsEmergencyContact: $("#chkIsEmergencyContact").is(":checked"),
                        CanPickup: $("#chkCanPickup").is(":checked"),
                        CanViewReportCard: $("#chkCanViewReportCard").is(":checked"),
                        CanReceiveSMS: $("#chkCanReceiveSMS").is(":checked"),
                        CanReceiveEmail: $("#chkCanReceiveEmail").is(":checked"),
                        ContactPriority: parseInt($("#txtContactPriority").val(), 10) || 1,
                        SpecificPhone: $("#txtSpecificPhone").val().trim(),
                        Machine_Id: $("#txtMachineId").val().trim(),
                        DMLStatus: parseInt($("#hdnMapID").val(), 10) > 0 ? "U" : "I"
                    };

                    var $btn = $(this).prop("disabled", true);
                    $("#btnSaveLabel").text("Saving...");

                    $.ajax({
                        type: "POST",
                        url: API_BASE + "SaveGuardianMap",
                        data: JSON.stringify(payload),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (res) {
                            $btn.prop("disabled", false);
                            $("#btnSaveLabel").text("Save Mapping");
                            if (res && res.Success) {
                                alert(res.Message || "Mapping saved successfully!");
                                window.location.reload();
                            } else {
                                alert((res && res.Message) || "Mapping operation failed.");
                            }
                        },
                        error: function (xhr) {
                            $btn.prop("disabled", false);
                            $("#btnSaveLabel").text("Save Mapping");
                            alert("Server error occurred: " + xhr.responseText);
                        }
                    });
                });

            })(jQuery);
        });
    </script>
</asp:Content>

