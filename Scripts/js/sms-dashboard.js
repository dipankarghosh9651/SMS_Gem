/* ============================================================================
   sms-dashboard.js
   Overview Dashboard & KPI Fetcher
   ========================================================================= */
(function ($, SMS) {
    "use strict";

    var SERVICE_URL = SMS.baseUrl + "StudentApi/";

    function loadDashboardMetrics() {
        SMS.callApi("GET", SERVICE_URL + "GetStudents", { search: "" })
            .done(function (students) {
                if (!Array.isArray(students)) {
                    students = [];
                }

                var total = students.length;
                var activeCount = 0;
                var inactiveCount = 0;
                var portalCount = 0;

                var tbodyHtml = "";

                // Compute counts and build top 6 recent rows
                $.each(students, function (idx, s) {
                    if (s.IsActive !== false) {
                        activeCount++;
                    } else {
                        inactiveCount++;
                    }

                    if (s.PortalAccess) {
                        portalCount++;
                    }

                    if (idx < 6) {
                        var fullName = [s.FirstName, s.MiddleName, s.LastName].filter(Boolean).join(" ");
                        var code = s.Student_Code || s.StudentCode || "--";
                        var admNo = s.AdmissionNo || "--";
                        var dateStr = s.EnrollmentDate ? SMS.formatDate(s.EnrollmentDate) : "--";
                        var isActive = s.IsActive !== false;

                        tbodyHtml += '<tr>' +
                            '<td><strong class="text-primary">' + SMS.escapeHtml(code) + '</strong></td>' +
                            '<td>' + SMS.escapeHtml(fullName) + '</td>' +
                            '<td>' + SMS.escapeHtml(admNo) + '</td>' +
                            '<td>' + SMS.escapeHtml(dateStr) + '</td>' +
                            '<td><span class="badge ' + (isActive ? 'bg-success' : 'bg-secondary') + ' bg-opacity-10 ' + (isActive ? 'text-success' : 'text-secondary') + '">' + (isActive ? 'Active' : 'Inactive') + '</span></td>' +
                            '<td class="text-end">' +
                            '<a href="../Students/StudentEntry_GEM.aspx?code=' + encodeURIComponent(code) + '" class="btn btn-sm btn-light py-0 px-2" title="Edit Record">' +
                            '<i class="fa-solid fa-pen-to-square text-primary"></i>' +
                            '</a>' +
                            '</td>' +
                            '</tr>';
                    }
                });

                // Update Metric Display Cards
                $("#statTotalStudents").text(total);
                $("#statNewAdmissions").text(activeCount);
                $("#statPortalUsers").text(portalCount);
                $("#statInactive").text(inactiveCount);

                if (students.length === 0) {
                    tbodyHtml = '<tr><td colspan="6" class="text-center py-4 text-muted">No student records found.</td></tr>';
                }

                $("#tbodyRecentStudents").html(tbodyHtml);
            })
            .fail(function () {
                $("#tbodyRecentStudents").html('<tr><td colspan="6" class="text-center text-danger py-4">Unable to load dashboard records.</td></tr>');
            });
    }

    $(document).ready(function () {
        loadDashboardMetrics();

        // Search in recent table
        $("#btnSearchRecent").on("click", function () {
            var term = $("#txtSearchRecent").val().toLowerCase();
            $("#tbodyRecentStudents tr").each(function () {
                var rowText = $(this).text().toLowerCase();
                $(this).toggle(rowText.indexOf(term) > -1);
            });
        });

        $("#txtSearchRecent").on("keyup", function () {
            $("#btnSearchRecent").trigger("click");
        });
    });

})(jQuery, window.SMS);