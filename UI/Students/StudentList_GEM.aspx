<%@ Page Title="Student Directory" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="StudentList_GEM.aspx.cs" Inherits="SMS_Gem.UI.Students.StudentList_GEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-hover tbody tr {
            cursor: pointer;
        }
        .badge-active {
            background-color: #dcfce7;
            color: #15803d;
        }
        .badge-inactive {
            background-color: #fee2e2;
            color: #b91c1c;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        <div class="glass-panel p-3 p-md-4">
            
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                <div>
                    <h4 class="fw-bold text-dark mb-0">
                        <i class="fa-solid fa-users text-primary me-2"></i>Student Directory
                    </h4>
                    <small class="text-muted">Search, filter, and manage registered student records</small>
                </div>
                <div class="d-flex gap-2">
                    <a href="StudentEntry_GEM.aspx" class="btn btn-primary btn-sm px-3 shadow-sm">
                        <i class="fa-solid fa-user-plus me-1"></i>New Admission
                    </a>
                </div>
            </div>

            <!-- Search Filter Bar -->
            <div class="row g-2 mb-4">
                <div class="col-12 col-md-6 col-lg-4">
                    <div class="input-group">
                        <span class="input-group-text bg-light"><i class="fa-solid fa-magnifying-glass"></i></span>
                        <input type="text" id="txtSearchTerm" class="form-control" placeholder="Search by name, roll no, admission no..." />
                    </div>
                </div>
                <div class="col-6 col-md-3 col-lg-2">
                    <button type="button" id="btnSearch" class="btn btn-primary w-100">
                        Search
                    </button>
                </div>
                <div class="col-6 col-md-3 col-lg-2">
                    <button type="button" id="btnResetSearch" class="btn btn-light w-100 border">
                        Reset
                    </button>
                </div>
            </div>

            <!-- Student Grid Table -->
            <div class="table-responsive">
                <table class="table table-hover align-middle border" id="tblStudents">
                    <thead class="table-light">
                        <tr>
                            <th>Student Code</th>
                            <th>Admission No</th>
                            <th>Full Name</th>
                            <th>Roll No</th>
                            <th>Gender</th>
                            <th>Contact Phone</th>
                            <th>Status</th>
                            <th class="text-end">Actions</th>
                        </tr>
                    </thead>
                    <tbody id="tblStudentBody">
                        <tr>
                            <td colspan="8" class="text-center py-4 text-muted">
                                <div class="spinner-border spinner-border-sm text-primary me-2" role="status"></div> Loading student directory...
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Pagination Bar -->
            <div class="d-flex justify-content-between align-items-center mt-3 flex-wrap gap-2">
                <small class="text-muted" id="lblTotalRecords">Showing records</small>
                <nav aria-label="Page navigation">
                    <ul class="pagination pagination-sm mb-0" id="paginationControls"></ul>
                </nav>
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
                var ASMX_BASE = '<%= ResolveUrl("~/Services/StudentService.asmx/") %>';
                var defaultBranch = localStorage.getItem("SMS_BRANCH") || "CAP";
                var allStudents = [];
                var currentPage = 1;
                var pageSize = 10;

                function loadStudents(searchTerm) {
                    var $tbody = $("#tblStudentBody");
                    $tbody.html('<tr><td colspan="8" class="text-center py-4 text-muted"><div class="spinner-border spinner-border-sm text-primary me-2"></div> Loading...</td></tr>');

                    $.ajax({
                        type: "POST",
                        url: ASMX_BASE + "GetStudentList",
                        data: JSON.stringify({ branch: defaultBranch, searchTerm: searchTerm || "" }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (res) {
                            // Extract array safely from .d envelope
                            var data = (res && res.d !== undefined) ? res.d : res;
                            if (typeof data === "string") {
                                try { data = JSON.parse(data); } catch (e) { }
                            }

                            // If nested inside a Data property or direct list
                            if (data && data.Data && Array.isArray(data.Data)) {
                                allStudents = data.Data;
                            } else if (Array.isArray(data)) {
                                allStudents = data;
                            } else {
                                allStudents = [];
                            }

                            currentPage = 1;
                            renderGrid();
                        },
                        error: function (xhr) {
                            console.error("Failed to load students:", xhr.responseText);
                            $tbody.html('<tr><td colspan="8" class="text-center text-danger py-4">Failed to fetch students. Check network tab for details.</td></tr>');
                        }
                    });
                }

                function renderGrid() {
                    var $tbody = $("#tblStudentBody");
                    if (!allStudents || allStudents.length === 0) {
                        $tbody.html('<tr><td colspan="8" class="text-center text-muted py-4">No matching student records found.</td></tr>');
                        $("#lblTotalRecords").text("Showing 0 records");
                        $("#paginationControls").empty();
                        return;
                    }

                    var totalRecords = allStudents.length;
                    var totalPages = Math.ceil(totalRecords / pageSize);
                    var startIndex = (currentPage - 1) * pageSize;
                    var endIndex = Math.min(startIndex + pageSize, totalRecords);
                    var pageData = allStudents.slice(startIndex, endIndex);

                    var html = "";
                    pageData.forEach(function (s) {
                        var code = s.Student_Code || "";
                        var admNo = s.AdmissionNo || "-";
                        var name = s.FullName || [s.FirstName, s.MiddleName, s.LastName].filter(Boolean).join(" ") || "-";
                        var roll = s.RollNumber || "-";
                        var gender = s.Gender || "-";
                        var phone = s.Phone || "-";
                        var isActive = s.IsActive !== false;
                        var statusBadge = isActive
                            ? '<span class="badge badge-active px-2 py-1">Active</span>'
                            : '<span class="badge badge-inactive px-2 py-1">Inactive</span>';

                        html += '<tr>' +
                            '<td><strong>' + code + '</strong></td>' +
                            '<td>' + admNo + '</td>' +
                            '<td class="fw-semibold">' + name + '</td>' +
                            '<td>' + roll + '</td>' +
                            '<td>' + gender + '</td>' +
                            '<td>' + phone + '</td>' +
                            '<td>' + statusBadge + '</td>' +
                            '<td class="text-end">' +
                            '<a href="StudentEntry_GEM.aspx?code=' + encodeURIComponent(code) + '" class="btn btn-sm btn-outline-primary me-1" title="Edit Student">' +
                            '<i class="fa-solid fa-pen-to-square"></i>' +
                            '</a>' +
                            '<a href="StudentGuardianMapEntry_GEM.aspx?code=' + encodeURIComponent(code) + '" class="btn btn-sm btn-outline-secondary" title="Guardian Mapping">' +
                            '<i class="fa-solid fa-people-arrows"></i>' +
                            '</a>' +
                            '</td>' +
                            '</tr>';
                    });

                    $tbody.html(html);
                    $("#lblTotalRecords").text("Showing " + (startIndex + 1) + " to " + endIndex + " of " + totalRecords + " entries");
                    buildPagination(totalPages);
                }

                function buildPagination(totalPages) {
                    var $pagination = $("#paginationControls").empty();
                    if (totalPages <= 1) return;

                    var prevDisabled = currentPage === 1 ? " disabled" : "";
                    $pagination.append('<li class="page-item' + prevDisabled + '"><a class="page-link" href="#" data-page="' + (currentPage - 1) + '">Previous</a></li>');

                    for (var i = 1; i <= totalPages; i++) {
                        var active = i === currentPage ? " active" : "";
                        $pagination.append('<li class="page-item' + active + '"><a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>');
                    }

                    var nextDisabled = currentPage === totalPages ? " disabled" : "";
                    $pagination.append('<li class="page-item' + nextDisabled + '"><a class="page-link" href="#" data-page="' + (currentPage + 1) + '">Next</a></li>');
                }

                $(document).on("click", "#paginationControls .page-link", function (e) {
                    e.preventDefault();
                    var page = parseInt($(this).data("page"), 10);
                    if (page >= 1 && page <= Math.ceil(allStudents.length / pageSize)) {
                        currentPage = page;
                        renderGrid();
                    }
                });

                $("#btnSearch").on("click", function () {
                    loadStudents($("#txtSearchTerm").val().trim());
                });

                $("#txtSearchTerm").on("keypress", function (e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        loadStudents($(this).val().trim());
                    }
                });

                $("#btnResetSearch").on("click", function () {
                    $("#txtSearchTerm").val("");
                    loadStudents("");
                });

                // Initial Load
                loadStudents("");
            })(jQuery);
        });
    </script>
</asp:Content>
