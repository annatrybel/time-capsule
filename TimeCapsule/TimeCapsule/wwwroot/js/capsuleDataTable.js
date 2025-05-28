$(document).ready(function () {
    const statusClasses = {
        '0': 'badge-created',
        '1': 'badge-opened',
        '2': 'badge-deleted'
    };

    const capsuleTypeNames = {
        '0': 'Indywidualna',
        '1': 'Dla kogoś'
    };

    $('#tableCapsule').DataTable({
        processing: true,
        serverSide: true,
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/pl.json',
        },
        autoWidth: false,
        scrollCollapse: true,
        scrollX: true,
        ajax: {
            url: '/AdminPanel/Capsules/GetCapsules',
            type: "POST",
            dataType: "json",
            error: function (xhr, error, thrown) {
                console.error('Błąd w żądaniu AJAX DataTables:', error, thrown);
            }
        },
        columns: [
            { data: 'ownerName', name: 'OwnerName' },
            {
                data: 'recipients',
                name: 'Recipients',
                render: function (data) {
                    if (!data || data.length === 0) return 'Brak';
                    return data.join(', ');
                }
            },
            {
                data: 'capsuleType',
                name: 'CapsuleType',
                render: function (data) {
                    return capsuleTypeNames[data] || data;
                }
            },
            {
                data: 'createdAt',
                name: 'CreatedAt',
                render: function (data) {
                    return new Date(data).toLocaleString();
                }
            },
            {
                data: 'openingDate',
                name: 'OpeningDate',
                render: function (data) {
                    return new Date(data).toLocaleString();
                }
            },
            {
                data: 'status',
                name: 'Status',
                render: function (data) {
                    const statusName = data === 0 ? 'Utworzona' :
                        data === 1 ? 'Otwarta' :
                            data === 2 ? 'Usunięta' : 'Nieznany';

                    return `<span class="badge ${statusClasses[data]}">${statusName}</span>`;
                }
            },
            {
                data: 'id',
                name: 'Actions',
                orderable: false,
                className: 'actions',
                render: function (data, type, row) {
                    let dropdownItems = '';

                    if (row.status !== 1) {
                        dropdownItems += `
                        	<li>
                            	<button class="dropdown-item edit-opening-date" data-id="${data}" type="button">
                                	<i class="bi bi-calendar-event"></i> Zmień datę otwarcia
                            	</button>
                        	</li>
                    	`;
                    }

                    if (row.capsuleType === 1) {
                        dropdownItems += `
                        	<li>
                            	<button class="dropdown-item edit-recipients" data-id="${data}" type="button">
                                	<i class="bi bi-person-lines-fill"></i> Zmień listę odbiorców
                            	</button>
                        	</li>
                    	`;
                    }

                    dropdownItems += `
                    	<li>
                        	<form method="post" action="/AdminPanel/Capsules/DeactivateCapsule/${data}" style="margin:0; display: inline;">
                            	<button type="submit" class="dropdown-item deactivate-capsule">
                                	<i class="bi bi-toggle-off"></i> Dezaktywacja
                            	</button>
                        	</form>
                    	</li>
                	`;

                    if (dropdownItems === '') {
                        return `<div class="text-center">-</div>`;
                    }

                    return `
                    	<div class="dropdown text-center">
                        	<button class="btn btn-sm dropdown-toggle" type="button" id="dropdownMenuButton-${data}" data-bs-toggle="dropdown" aria-expanded="false">
                            	<i class="bi bi-list fs-4"></i>
                        	</button>
                        	<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data}">
                            	${dropdownItems}
                        	</ul>
                    	</div>
                	`;
                }
            }
        ],
        columnDefs: [
            { "targets": 0, "width": "15%" },
            { "targets": 1, "width": "15%" },
            { "targets": 2, "width": "10%" },
            { "targets": 3, "width": "15%" },
            { "targets": 4, "width": "15%" },
            { "targets": 5, "width": "10%" },
            { "targets": 6, "width": "15%", "className": "text-center" }
        ],
        order: [[4, 'desc']]
    });



    $(document).on('click', '.dropdown-item.edit-opening-date', function (event) {
        const capsuleId = $(this).data('id');

        $.ajax({
            url: `/AdminPanel/Capsules/GetCapsuleOpeningDate/${capsuleId}`,
            type: 'GET',
            success: function (response) {

                $('#editCapsuleId').val(response.capsuleId);
                $('#capsuleTitle').val(response.title);

                const currentDate = new Date(response.currentOpeningDate);
                $('#currentOpeningDate').val(currentDate.toLocaleString());

                const year = currentDate.getFullYear();
                const month = String(currentDate.getMonth() + 1).padStart(2, '0');
                const day = String(currentDate.getDate()).padStart(2, '0');
                const hours = String(currentDate.getHours()).padStart(2, '0');
                const minutes = String(currentDate.getMinutes()).padStart(2, '0');

                const formattedLocalDateTime = `${year}-${month}-${day}T${hours}:${minutes}`;
                $('#newOpeningDateDisplay').val(formattedLocalDateTime);

                $('#editOpeningDateModal').modal('show');
            },
            error: function (error) {
                console.error('Error fetching user data:', error);
                alert('Failed to fetch user data. Please try again.');
            }
        });
    });
});


