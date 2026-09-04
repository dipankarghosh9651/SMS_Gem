<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="LandingPage.aspx.cs" Inherits="SMS_Gem.UI.DashBoard.LandingPage" %>



<%--<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/UI/MasterPages/SMSLanding.Master" AutoEventWireup="true" CodeBehind="LandingPage.aspx.cs" Inherits="SMS.UI.Dashboard.LandingPage" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .metric-card {
            transition: transform 0.2s ease, box-shadow 0.2s ease;
            position: relative;
            overflow: hidden;
        }
        .metric-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 28px rgba(31, 38, 135, 0.18);
        }
        .metric-icon-wrap {
            width: 48px;
            height: 48px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.4rem;
        }
        .quick-action-btn {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            padding: 0.85rem 1rem;
            border-radius: 0.75rem;
            background: rgba(255, 255, 255, 0.6);
            border: 1px solid rgba(255, 255, 255, 0.4);
            color: #1e293b;
            text-decoration: none;
            font-weight: 500;
            transition: all 0.2s ease;
        }
        .quick-action-btn:hover {
            background: rgba(255, 255, 255, 0.95);
            color: #2563eb;
            transform: translateX(4px);
        }
        .table-glass {
            background: transparent !important;
        }
        .table-glass th {
            background: rgba(241, 245, 249, 0.6) !important;
            font-size: 0.82rem;
            text-transform: uppercase;
            letter-spacing: 0.04em;
            color: #64748b;
        }
        .table-glass td {
            background: transparent !important;
            vertical-align: middle;
            font-size: 0.9rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-0 px-md-3">
        
        <!-- Welcome Header Banner -->
        <div class="glass-panel p-4 mb-4">
            <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
                <div>
                    <h3 class="fw-bold text-dark mb-1">
                        <i class="fa-solid fa-chart-line text-primary me-2"></i>School Overview
                    </h3>
                    <p class="text-muted mb-0">Branch: <span class="badge bg-primary bg-opacity-10 text-primary px-2 py-1" id="lblBranch">CAP</span> &bull; Academic Year 2026-2027</p>
                </div>
                <div>
                    <a href="../Students/StudentEntry_GEM.aspx" class="btn btn-primary px-3 py-2 shadow-sm d-inline-flex align-items-center gap-2">
                        <i class="fa-solid fa-user-plus"></i>
                        <span>New Admission</span>
                    </a>
                </div>
            </div>
        </div>

        <!-- Metric KPI Cards Row -->
        <div class="row g-3 mb-4">
            <!-- Total Students -->
            <div class="col-12 col-sm-6 col-xl-3">
                <div class="glass-panel p-3 metric-card">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <span class="text-muted small fw-semibold">Total Students</span>
                            <h3 class="fw-bold text-dark mt-1 mb-0" id="statTotalStudents">--</h3>
                            <small class="text-success"><i class="fa-solid fa-arrow-trend-up me-1"></i>Active Enrollment</small>
                        </div>
                        <div class="metric-icon-wrap bg-primary bg-opacity-10 text-primary">
                            <i class="fa-solid fa-user-graduate"></i>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Active Admissions -->
            <div class="col-12 col-sm-6 col-xl-3">
                <div class="glass-panel p-3 metric-card">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <span class="text-muted small fw-semibold">New This Month</span>
                            <h3 class="fw-bold text-dark mt-1 mb-0" id="statNewAdmissions">--</h3>
                            <small class="text-primary"><i class="fa-solid fa-calendar-check me-1"></i>Current Term</small>
                        </div>
                        <div class="metric-icon-wrap bg-success bg-opacity-10 text-success">
                            <i class="fa-solid fa-id-card"></i>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Portal Access -->
            <div class="col-12 col-sm-6 col-xl-3">
                <div class="glass-panel p-3 metric-card">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <span class="text-muted small fw-semibold">Portal Active</span>
                            <h3 class="fw-bold text-dark mt-1 mb-0" id="statPortalUsers">--</h3>
                            <small class="text-info"><i class="fa-solid fa-shield-halved me-1"></i>Parent Accounts</small>
                        </div>
                        <div class="metric-icon-wrap bg-info bg-opacity-10 text-info">
                            <i class="fa-solid fa-users"></i>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Inactive / TC -->
            <div class="col-12 col-sm-6 col-xl-3">
                <div class="glass-panel p-3 metric-card">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <span class="text-muted small fw-semibold">TC / Inactive</span>
                            <h3 class="fw-bold text-dark mt-1 mb-0" id="statInactive">--</h3>
                            <small class="text-danger"><i class="fa-solid fa-user-xmark me-1"></i>Archived</small>
                        </div>
                        <div class="metric-icon-wrap bg-danger bg-opacity-10 text-danger">
                            <i class="fa-solid fa-file-invoice"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Main Workspace Grid -->
        <div class="row g-4">
            <!-- Left: Recent Student Admissions Table -->
            <div class="col-12 col-lg-8">
                <div class="glass-panel p-3 p-md-4 h-100">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h5 class="fw-bold text-dark mb-0">Recent Admissions</h5>
                        <div class="input-group input-group-sm w-auto">
                            <input type="text" class="form-control" id="txtSearchRecent" placeholder="Search admission..." />
                            <button class="btn btn-outline-secondary" type="button" id="btnSearchRecent"><i class="fa-solid fa-magnifying-glass"></i></button>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table class="table table-hover table-glass align-middle mb-0">
                            <thead>
                                <tr>
                                    <th>Code</th>
                                    <th>Student Name</th>
                                    <th>Admission No</th>
                                    <th>Date</th>
                                    <th>Status</th>
                                    <th class="text-end">Action</th>
                                </tr>
                            </thead>
                            <tbody id="tbodyRecentStudents">
                                <tr>
                                    <td colspan="6" class="text-center py-4 text-muted">
                                        <div class="spinner-border spinner-border-sm text-primary me-2"></div>Loading recent records...
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <!-- Right: Quick Links & Summary Feed -->
            <div class="col-12 col-lg-4">
                <div class="d-flex flex-column gap-3">
                    
                    <!-- Quick Actions Card -->
                    <div class="glass-panel p-3 p-md-4">
                        <h5 class="fw-bold text-dark mb-3">Quick Navigation</h5>
                        <div class="d-flex flex-column gap-2">
                            <a href="../Students/StudentEntry_GEM.aspx" class="quick-action-btn">
                                <i class="fa-solid fa-user-plus text-primary fa-fw"></i>
                                <span>Student Entry Master</span>
                            </a>
                            <a href="../Students/StudentList_GEM.aspx" class="quick-action-btn">
                                <i class="fa-solid fa-table-list text-success fa-fw"></i>
                                <span>Student Master Register</span>
                            </a>
                            <a href="#" class="quick-action-btn text-muted" onclick="alert('Module available in next release.'); return false;">
                                <i class="fa-solid fa-receipt text-warning fa-fw"></i>
                                <span>Fee Collection Desk</span>
                            </a>
                            <a href="#" class="quick-action-btn text-muted" onclick="alert('Module available in next release.'); return false;">
                                <i class="fa-solid fa-calendar-check text-info fa-fw"></i>
                                <span>Daily Attendance</span>
                            </a>
                        </div>
                    </div>

                    <!-- System Status Card -->
                    <div class="glass-panel p-3">
                        <div class="d-flex align-items-center gap-3">
                            <div class="p-2 rounded-circle bg-success bg-opacity-10 text-success">
                                <i class="fa-solid fa-database fa-lg"></i>
                            </div>
                            <div>
                                <div class="fw-semibold small">Database Status</div>
                                <div class="text-muted small">Connected to MS SQL Server</div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </div>

    <script src="../../Scripts/js/sms-common.js"></script>
    <script src="../../Scripts/js/sms-dashboard.js"></script>
</asp:Content>

