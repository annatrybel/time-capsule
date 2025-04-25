$(document).ready(function () {
    const usersTable = $('#tableUser').DataTable({
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
            { data: "userId", name: "ID", },
            { data: "email", name: "Email", },
            { data: "userName", name: "Nazwa użytkownika", },
            {
                data: "roleName",
                name: "Rola",
                render: function (data) {
                    if (data === "Admin") {
                        return '<span class="badge bg-primary">Administrator</span>';
                    } else {
                        return '<span class="badge bg-secondary">Użytkownik</span>';
                    }
                }
            },
            {
                data: "isLocked",
                name: "Status",
                render: function (data) {
                    if (data === true) {
                        return '<span class="badge bg-danger">Zablokowany</span>';
                    } else {
                        return '<span class="badge bg-success">Aktywny</span>';
                    }
                }
            },
            {
                data: "userId",
                name: "Akcje",
                orderable: false,
                className: "actions",
                render: function (data, type, row) {
                    return `
                    	<div class="dropdown text-center">
                        	<button class="btn btn-sm dropdown-toggle" type="button" id="dropdownMenuButton-${data}" data-bs-toggle="dropdown" aria-expanded="false">
                            	<i class="fas fa-ellipsis-v"></i>
                        	</button>
                        	<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data}">
                            	<li>
                                	<button class="dropdown-item edit-user" data-id="${data}" type="button">
                                    	<i class="fas fa-edit"></i> Edit user
                                	</button>
                            	</li>
                            	<li>
                                	<a class="dropdown-item delete-button" data-id="${data}" data-url="/AdminPanel/DeleteUser">
                                    	<i class="fas fa-trash"></i> Delete
                                	</a>
                            	</li>
                            	<li>
                                	${row.isLocked ?
                            `<form method="post" action="/AdminPanel/UnlockUser/${data}" style="margin:0">
                                        	<button type="submit" class="dropdown-item unlock-user">
                                            	<i class="fas fa-unlock"></i> Unlock user
                                        	</button>
                                    	</form>` :
                            `<form method="post" action="/AdminPanel/LockUser/${data}" style="margin:0">
                                        	<button type="submit" class="dropdown-item lock-user">
                                            	<i class="fas fa-lock"></i> Lock user
                                        	</button>
                                     	</form>`
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


    $(document).on('click', '.dropdown-item.edit-user', function (event) {
        const userId = $(this).data('id');
        const url = '/AdminPanel/GetUserById?userId=' + userId;


        $.ajax({
            url: url,
            type: 'GET',
            success: function (response) {
                const modal = $('#editUserModal');

                modal.find('#EditId').val(response.userId);
                modal.find('#EditEmail').val(response.email);
                modal.find('#EditUserName').val(response.userName);
                modal.find('#EditRoleId').val(response.roleId);

                const modalInstance = new bootstrap.Modal(document.getElementById('editUserModal'));
                modalInstance.show();
            },
            error: function (error) {
                console.error('Error fetching user data:', error);
                alert('Failed to fetch user data. Please try again.');
            }
        });
    });
});

