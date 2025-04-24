function initializePagibigDataTable() {
    var tableId = '#pagibig-table';
    var lastSelectedRow = null;
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    var dtProperties = {
        ajax: {
            url: '/Deduction/GetPagibigList',
            type: "GET",
            data: {

            },
            dataType: "json",
            processing: true,
            serverSide: true,
            complete: function (xhr) {
                var url = new URL(window.location.href);
                var _currentPage = url.searchParams.get("page01") == null ? 1 : url.searchParams.get("page01");
                // console.log('table1', _currentPage);
                table.page(_currentPage - 1).draw('page');

            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        dom: 'Btip',
        columns: [
            {
                "title": "Tax From",
                "data": "tax_From",
                "orderable": false
            },
            {
                "title": "Tax To",
                "data": "tax_To",
                "orderable": false
            },
            {
                "title": "Employee Contribution Rate",
                "data": "employee_Contribution_Rate",
                "orderable": false
            }, 
            {
                "title": "Employer Contribution Rate",
                "data": "employer_Contribution_Rate",
                "orderable": false
            },
            {
                "title": "Total Contribution",
                "data": "total_Contribution",
                "orderable": false
            }
        ],
        order: [[0, 'desc']], // Sort the second column (index 1) by descending order
        columnDefs: [
            
            {

                //width: '50%', targets: 2
            }
        ],
        buttons: [
            {
                extend: 'pdf',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export PDF File</span>',
                title: 'Pag Ibig Deduction List', // Set custom title in the file
                filename: 'Pag Ibig Deduction List', // Custom file name
                className: 'btn btn-info',

            },
            {
                extend: 'excel',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export Excel File</span>',
                title: 'Pag Ibig Deduction List', // Set custom title in the file
                filename: 'Pag Ibig Deduction List', // Custom file name
                className: 'btn btn-info',

            },]
    };

    var table = $(tableId).DataTable(dtProperties);

    $('#pagibig-table').on('page.dt', function () {
        var info = table.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    $(tableId + '_filter input').attr('placeholder', 'Search Here');

    $(tableId + ' tbody').on('click', 'tr', function () {
        var data = table.row(this).data();
        // console.log(data);
        // Remove highlight from the previously selected row
        if (lastSelectedRow) {
            $(lastSelectedRow).removeClass('selected-row');
        }
        // Highlight the currently selected row
        $(this).addClass('selected-row');
        lastSelectedRow = this;
    });

}

function pagibigFileDrag() {
    const dropZone = document.getElementById("pagibig-drop-zone");
    const fileInput = document.getElementById("pagibig-upload-excel");

    // Open file picker on click
    dropZone.addEventListener("click", () => fileInput.click());

    // Highlight drop area on drag over
    dropZone.addEventListener("dragover", (e) => {
        e.preventDefault();
        dropZone.classList.add("dragover");
    });

    dropZone.addEventListener("dragleave", () => {
        dropZone.classList.remove("dragover");
    });

    dropZone.addEventListener("drop", (e) => {
        e.preventDefault();
        dropZone.classList.remove("dragover");

        const files = e.dataTransfer.files;
        pagibighandleFiles(files);
        if (files.length > 0) {
            console.log('Dropped file:', files[0]);

            // Optional: simulate setting the input's file list
            const dataTransfer = new DataTransfer(); // or ClipboardEvent().clipboardData
            for (let i = 0; i < files.length; i++) {
                dataTransfer.items.add(files[i]);
            }
            fileInput.files = dataTransfer.files;

            // Now fileInput.files has the dropped file(s)
            // You can trigger a function to handle upload or preview
        }
    });

    // Handle files from input
    fileInput.addEventListener("change", () => {
        pagibighandleFiles(fileInput.files);
    });
}
function pagibighandleFiles(files) {
    // You can upload the files here or just log them
    console.log("Files uploaded:", files[0].name);
    tlfilename = files[0].name;
    // Example: Upload to server or display preview
    document.getElementById("pagibig-drag-file-label").textContent = tlfilename;
    const text = document.getElementById("pagibig-drag-file-label");
    const colors = ["green"];
    let colorIndex = 0;
    let visible = true;
    text.style.fontWeight = "800";
    setInterval(() => {
        // Toggle visibility
        text.style.visibility = visible ? "hidden" : "visible";
        visible = !visible;

        // Change color on each blink
        if (visible) {
            text.style.color = colors[colorIndex];
            colorIndex = (colorIndex + 1) % colors.length;
        }
    }, 500); // Blink interval: 500ms

}

$("#import-pagibig").click(function () {
    document.getElementById('pagibig-filing-container').style.display = "block";
});
$("#pagibig-close-filing-container").click(function () {
    document.getElementById('pagibig-filing-container').style.display = "none";
});
function downloadPagibigTemplate() {
    // alert('Template Downloaded')
    location.replace('../Deduction/DownloaPagibigHeader');
}