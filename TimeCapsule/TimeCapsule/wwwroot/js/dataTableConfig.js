// Globalne obiekty językowe dla DataTables
const dataTableLangPL = {
	"processing": "Przetwarzanie...",
	"search": "Szukaj:",
	"lengthMenu": "Pokaż _MENU_ pozycji",
	"info": "Pozycje od _START_ do _END_ z _TOTAL_ łącznie",
	"infoEmpty": "Pozycji 0 z 0 dostępnych",
	"infoFiltered": "(filtrowano z _MAX_ dostępnych pozycji)",
	"infoPostFix": "",
	"loadingRecords": "Wczytywanie...",
	"zeroRecords": "Nie znaleziono pasujących pozycji",
	"emptyTable": "Brak danych",
	"paginate": {
		"first": "Pierwsza",
		"previous": "Poprzednia",
		"next": "Następna",
		"last": "Ostatnia"
	},
	"aria": {
		"sortAscending": ": aktywuj, by posortować kolumnę rosnąco",
		"sortDescending": ": aktywuj, by posortować kolumnę malejąco"
	},
	"appTexts": {
		"editText": "Edytuj użytkownika",
		"lockUserText": "Zablokuj użytkownika",
		"unlockUserText": "Odblokuj użytkownika",
		"changeOpeningDateText": "Zmień datę otwarcia",
		"changeRecipientListText": "Zmień listę odbiorców",
		"deactivationText": "Dezaktywacja"
	}
};

const dataTableLangEN = {
	"processing": "Processing...",
	"search": "Search:",
	"lengthMenu": "Show _MENU_ entries",
	"info": "Showing _START_ to _END_ of _TOTAL_ entries",
	"infoEmpty": "Showing 0 to 0 of 0 entries",
	"infoFiltered": "(filtered from _MAX_ total entries)",
	"infoPostFix": "",
	"loadingRecords": "Loading...",
	"zeroRecords": "No matching records found",
	"emptyTable": "No data available in table",
	"paginate": {
		"first": "First",
		"previous": "Previous",
		"next": "Next",
		"last": "Last"
	},
	"aria": {
		"sortAscending": ": activate to sort column ascending",
		"sortDescending": ": activate to sort column descending"
	},
	"appTexts": {
		"editText": "Edit user",
		"lockUserText": "Lock user",
		"unlockUserText": "Unlock user",
		"changeOpeningDateText": "Change opening date",
		"changeRecipientListText": "Change recipient list",
		"deactivationText": "Deactivation"
	}
};

$(document).ready(function () {
	$.fn.dataTable.ext.errMode = 'throw'; 

	let currentDataTableLanguage;

	if (window.currentCulture === 'pl') {
		currentDataTableLanguage = dataTableLangPL;
	} else {
		currentDataTableLanguage = dataTableLangEN;
	}

	$.extend($.fn.dataTable.defaults, {
		stateSave: true,
		processing: true,
		serverSide: true,
		language: currentDataTableLanguage
	});
});
