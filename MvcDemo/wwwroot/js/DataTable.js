// Reference: https://github.com/DavidSuescunPelegay/jQuery-datatable-server-side-net-core

function BindDataTable(url, columns, columnDefs, languageJson = "/js/DataTableTR.json", allTextInPageLengthMenu = "", tableIdWithoutSharp = "dataTable", pageLength = 5) {
    var lengthMenuValues = [5, 10, 25, 50, 100];
    var lengthMenuTexts = [5, 10, 25, 50, 100];
    if (allTextInPageLengthMenu != null && allTextInPageLengthMenu != "") {
        lengthMenuValues = [5, 10, 25, 50, 100, -1];
        lengthMenuTexts = [5, 10, 25, 50, 100, allTextInPageLengthMenu];
    }
    $(document).ready(function () {
        $("#" + tableIdWithoutSharp).DataTable({
            language: {
                url: languageJson
            },
            scrollX: true,
            pagingType: "full_numbers",
            pageLength: pageLength,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: false },
            // Ajax Filter
            ajax: {
                url: url,
                type: "post",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: columns,
            // Column Definitions
            columnDefs: columnDefs,
            // Records Per Page Count Menu
            lengthMenu: [
                lengthMenuValues,
                lengthMenuTexts
            ]
        });
    });
}