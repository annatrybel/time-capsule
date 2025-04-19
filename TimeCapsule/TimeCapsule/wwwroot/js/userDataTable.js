$(document).ready(function () {
    const usersTable = $('#usersTable').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/pl.json',
        },
        autoWidth: false,
        scrollCollapse: true,
        scrollX: true,
        ajax: {
            url: '/AdminPanel/GetAllUsers',
            type: "POST",
            dataType: "json"
        },
        columns: [
            { data: "userId" },
            { data: "email" },
            { data: "userName" },
            {
                data: "role",
                render: function (data) {
                    if (data === "Admin") {
                        return '<span class="badge bg-primary">Administrator</span>';
                    } else {
                        return '<span class="badge bg-secondary">Użytkownik</span>';
                    }
                }
            },
            {
                data: "status",
                render: function (data) {
                    if (data === "Aktywny") {
                        return '<span class="badge bg-success">Aktywny</span>';
                    } else {
                        return '<span class="badge bg-danger">Zablokowany</span>';
                    }
                }
            },
            {
                data: "userId",
                orderable: false,
                className: "actions",
                render: function (data,type,row) {
                    return `
                                <div class="dropdown text-center">
                                <button class="btn btn-sm dropdown-toggle" type="button" id="dropdownMenuButton-${data}" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="fas fa-ellipsis-v"></i>
                                </button>
                                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data}">
                                <li>
                                    <a class="dropdown-item edit-user" href="/Users/GetUserById?id=${data}">
                                            <i class="fas fa-edit"></i> Edit user
                                        </a>
                                </li>
                                <li>
                                        <a class="dropdown-item delete-button" data-id="${data}" data-url="/Users/DeleteUser">
                                            <i class="fas fa-trash"></i> Delete
                                        </a>
                                </li>
                                <li>
                                        ${row.isLocked ?
                            `<a class="dropdown-item unlock-user" data-id="${data}">
                                            <i class="fas fa-unlock"></i> Unlock user
                                        </a>` :
                            `<a class="dropdown-item lock-user" data-id="${data}">
                                            <i class="fas fa-lock"></i> Lock user
                                        </a>`
                        }
                                </li>
                                </ul>
                            </div>`;
                }
            }
        ],
        columnDefs: [
            { "targets": 0, "width": "10%" },
            { "targets": 1, "width": "20%" },
            { "targets": 2, "width": "20%" },
            { "targets": 3, "width": "15%" },
            { "targets": 4, "width": "15%" },
            { "targets": 5, "width": "20%", "className": "text-center" }
        ],
        order: [[0, "asc"]]
    });
});
