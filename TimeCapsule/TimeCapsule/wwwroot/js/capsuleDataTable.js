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

    $('#tableCapsule')
        .on('xhr.dt', function (e, settings, json, xhr) {
            if (xhr.responseJSON.traceId) {
                alert(xhr.responseJSON.detail);
            }
        })
        .DataTable({
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
                                data === 2 ? 'Dezaktywowana' : 'Nieznany';

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


    $(document).on('click', '.dropdown-item.deactivate-capsule', function (event) {
        event.preventDefault();
        const form = $(this).closest('form');
        const capsuleId = form.attr('action').split('/').pop();

        $('#deactivateCapsuleForm').attr('action', `/AdminPanel/Capsules/DeactivateCapsule/${capsuleId}`);
        $('#deactivateCapsuleModal').modal('show');
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

    $(document).on('click', '.dropdown-item.edit-recipients', function (event) {
        const capsuleId = $(this).data('id');

        $.ajax({
            url: `/AdminPanel/Capsules/GetCapsuleRecipients/${capsuleId}`,
            type: 'GET',
            success: function (response) {
                $('#recipientsCapsuleId').val(response.capsuleId);
                $('#recipientsCapsuleTitle').val(response.title);

                const container = $('#recipientsContainer');
                container.empty();

                if (response.recipients && response.recipients.length > 0) {
                    response.recipients.forEach((recipient, index) => {
                        const fieldHtml = `
                            <div class="input-group mb-2 recipient-input-group">
                                <input type="email" name="Recipients" class="form-control" value="${recipient.email}" required />
                                <button type="button" class="btn btn-outline-danger remove-recipient-btn">
                                    <i class="bi bi-x-lg"></i>
                                </button>
                            </div>
                        `;
                        container.append(fieldHtml);
                    });
                } else {
                    $('#addRecipientFieldBtn').click();
                }

                $('#editRecipientsModal').modal('show');
            },
            error: function (error) {
                console.error('Error fetching recipient data:', error);
                alert('Nie udało się pobrać listy odbiorców.');
            }
        });
    });

    $(document).on('click', '#addRecipientFieldBtn', function () {
        const newField = `
            <div class="input-group mb-2 recipient-input-group">
                <input type="email" name="Recipients" class="form-control" placeholder="email@domena.com" required />
                <button type="button" class="btn btn-outline-danger remove-recipient-btn">
                    <i class="bi bi-x-lg"></i>
                </button>
            </div>
        `;
        $('#recipientsContainer').append(newField);
    });

    $(document).on('click', '.remove-recipient-btn', function () {
        $(this).closest('.recipient-input-group').remove();
    });
});
