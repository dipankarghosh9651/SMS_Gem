/* ============================================================================
   sms-common.js
   Shared application utilities, alerts, and API handlers
   ========================================================================= */
window.SMS = window.SMS || {};

(function ($, SMS) {
    "use strict";

    SMS.baseUrl = "/api/";

    // Resolve API Endpoint URLs
    SMS.resolveUrl = function (controller, action) {
        return SMS.baseUrl + controller + "/" + (action || "");
    };

    // Generic AJAX Caller
    SMS.callApi = function (method, url, data) {
        var ajaxOptions = {
            type: method || "GET",
            url: url,
            dataType: "json"
        };

        if (method === "GET" || method === "DELETE") {
            if (data) {
                ajaxOptions.data = data;
            }
        } else {
            ajaxOptions.contentType = "application/json; charset=utf-8";
            ajaxOptions.data = JSON.stringify(data || {});
        }

        return $.ajax(ajaxOptions);
    };

    // Bootstrap Glass Toast Notification
    SMS.showToast = function (message, variant) {
        var $toastEl = $("#smsToast");
        if ($toastEl.length === 0) {
            alert(message);
            return;
        }

        variant = variant || "success";
        var bgClass = "bg-success";
        var textClass = "text-white";

        if (variant === "danger") {
            bgClass = "bg-danger";
        } else if (variant === "warning") {
            bgClass = "bg-warning";
            textClass = "text-dark";
        } else if (variant === "info") {
            bgClass = "bg-info";
            textClass = "text-dark";
        }

        $toastEl
            .removeClass("bg-success bg-danger bg-warning bg-info text-white text-dark")
            .addClass(bgClass + " " + textClass);

        $("#smsToastBody").text(message);

        var toast = bootstrap.Toast.getOrCreateInstance($toastEl[0], { delay: 3500 });
        toast.show();
    };

    // HTML Escaping Utility
    SMS.escapeHtml = function (str) {
        if (str === null || typeof str === "undefined") return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    };

    // Query String Parameter Reader
    SMS.getQueryParam = function (name) {
        var params = new URLSearchParams(window.location.search);
        return params.get(name);
    };

    // Date to Input Format (YYYY-MM-DD)
    SMS.toDateInputValue = function (dateVal) {
        if (!dateVal) return "";
        var d = new Date(dateVal);
        if (isNaN(d.getTime())) return "";
        var month = String(d.getMonth() + 1).padStart(2, "0");
        var day = String(d.getDate()).padStart(2, "0");
        return d.getFullYear() + "-" + month + "-" + day;
    };

})(jQuery, window.SMS);
