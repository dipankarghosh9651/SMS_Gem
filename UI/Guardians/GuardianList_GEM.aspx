<%@ Page Title="Guardian Directory" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="GuardianList_GEM.aspx.cs" Inherits="SMS_Gem.UI.Guardians.GuardianList_GEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-card { background: #ffffff; border: 1px solid #e2e8f0; border-radius: 0.5rem; }
        .table thead th { background-color: #f8fafc; color: #1e293b; font-weight: 600; font-size: 0.85rem; border-bottom: 2px solid #e2e8f0; }
        .badge-active { background-color: #dcfce7; color: #15803d; }
        .badge-inactive { background-color: #fee2e2; color: #b91c1c; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        <div class="glass-panel p-3 p-md-4">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                <div>
                    <h4 class="fw-bold text-dark mb-0"><i class="fa-solid fa-people-roof text-primary me-2"></i>Guardian & Parent Directory</h4>
                    <small class="text-muted">Search, filter, and view parents/guardians</small>
                </div>
                <a href="GuardianEntry_GEM.aspx" class="btn btn-primary btn-sm px-3 shadow-sm">
                    <i class="fa-solid fa-user-plus me-1"></i>New Guardian Entry
                </a>
            </div>

            <!-- Filter Panel -->
            <div class="filter-card p-3 mb-4 shadow-sm">
                <div class="row g-2 g-md-3">
                    <div class="col-12 col-md-4">
                        <label class="form-label small fw-semibold">Search</label>
                        <input type="text" id="txtSearchGuardian" class="form-control" placeholder="Search by Name, Code, Phone, Organization..." />
                    </div>
                    <div class="col-12 col-sm-6 col-md-3">
                        <label class="form-label small fw-semibold">City</label>
                        <input type="text" id="txtFilterCity" class="form-control" placeholder="Filter by City" />
                    </div>
                    <div class="col-12 col-sm-6 col-md-2">
                        <label class="form-label small fw-semibold">Status</label>
                        <select class="form-select" id="ddlFilterStatus">
                            <option value="">All</option>
                            <option value="true">Active</option>
                            <option value="false">Inactive</option>
                        </select>
                    </div>
                    <div class="col-12 col-sm-6 col-md-3 d-flex align-items-end gap-2">
                        <button type="button" id="btnFilterGuardian" class="btn btn-primary w-100"><i class="fa-solid fa-filter me-1"></i>Filter</button>
                        <button type="button" id="btnResetFilter" class="btn btn-outline-secondary"><i class="fa-solid fa-rotate-left"></i></button>
                    </div>
                </div>
            </div>

            <!-- Table -->
            <div class="table-responsive bg-white shadow-sm border rounded">
                <table class="table table-hover mb-0" id="tblGuardians">
                    <thead>
                        <tr>
                            <th>Code</th>
                            <th>Full Name</th>
                            <th>Primary Phone</th>
                            <th>WhatsApp</th>
                            <th>Occupation</th>
                            <th>City</th>
                            <th class="text-center">Status</th>
                            <th class="text-end">Actions</th>
                        </tr>
                    </thead>
                    <tbody id="guardianTableBody"></tbody>
                </table>
            </div>

            <div id="guardianLoading" class="text-center py-5" style="display: none;">
                <div class="spinner-border text-primary" role="status"></div>
                <div class="mt-2 text-muted fw-semibold">Loading guardian profiles...</div>
            </div>
            <div id="noGuardianRecords" class="text-center py-5" style="display: none;">
                <h6 class="text-muted fw-semibold">No records match your criteria.</h6>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            (function ($) {
                var SERVICE_BASE = '<%= ResolveUrl("~/Services/StaffService.asmx/") %>';
                var allGuardians = [];

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

                function loadGuardians() {
                    $("#guardianLoading").show();
                    $("#tblGuardians").hide();
                    callAsmx("GetGuardianList", {}, function (data) {
                        $("#guardianLoading").hide();
                        $("#tblGuardians").show();
                        allGuardians = Array.isArray(data) ? data : [];
                        renderGuardians();
                    });
                }

                function renderGuardians() {
                    var search = $("#txtSearchGuardian").val().toLowerCase().trim();
                    var city = $("#txtFilterCity").val().toLowerCase().trim();
                    var status = $("#ddlFilterStatus").val();

                    var filtered = allGuardians.filter(function (g) {
                        var name = ((g.FirstName || '') + ' ' + (g.LastName || '')).toLowerCase();
                        var matchSearch = !search || name.includes(search) || (g.Guardian_Code && g.Guardian_Code.toLowerCase().includes(search)) || (g.Phone && g.Phone.includes(search)) || (g.Organization && g.Organization.toLowerCase().includes(search));
                        var matchCity = !city || (g.City && g.City.toLowerCase().includes(city));
                        var matchStatus = status === "" || String(g.IsActive) === status;
                        return matchSearch && matchCity && matchStatus;
                    });

                    var $tbody = $("#guardianTableBody").empty();
                    if (!filtered.length) {
                        $("#noGuardianRecords").show();
                        return;
                    }
                    $("#noGuardianRecords").hide();

                    filtered.forEach(function (g) {
                        var fullName = (g.FirstName || "") + " " + (g.MiddleName ? g.MiddleName + " " : "") + (g.LastName || "");
                        var badge = g.IsActive ? "badge-active" : "badge-inactive";
                        var status = g.IsActive ? "Active" : "Inactive";
                        var editUrl = "GuardianEntry_GEM.aspx?code=" + encodeURIComponent(g.Guardian_Code);

                        $tbody.append('<tr>' +
                            '<td class="fw-bold text-primary">' + g.Guardian_Code + '</td>' +
                            '<td class="fw-semibold text-dark">' + fullName.trim() + '</td>' +
                            '<td>' + (g.Phone || "-") + '</td>' +
                            '<td>' + (g.WhatsAppNumber || "-") + '</td>' +
                            '<td>' + (g.Occupation || "-") + '</td>' +
                            '<td>' + (g.City || "-") + '</td>' +
                            '<td class="text-center"><span class="badge ' + badge + ' px-2 py-1">' + status + '</span></td>' +
                            '<td class="text-end"><a href="' + editUrl + '" class="btn btn-outline-primary btn-sm"><i class="fa-solid fa-pen-to-square me-1"></i>Edit</a></td>' +
                            '</tr>');
                    });
                }

                $("#btnFilterGuardian").on("click", renderGuardians);
                $("#txtSearchGuardian, #txtFilterCity").on("keyup", renderGuardians);
                $("#ddlFilterStatus").on("change", renderGuardians);
                $("#btnResetFilter").on("click", function () {
                    $("#txtSearchGuardian, #txtFilterCity, #ddlFilterStatus").val("");
                    renderGuardians();
                });

                loadGuardians();
            })(jQuery);
        });
    </script>
</asp:Content>
