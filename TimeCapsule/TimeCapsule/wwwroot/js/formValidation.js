function handleFormSubmit(formId) {
    const form = document.getElementById(formId);

    const existingSummaries = form.querySelectorAll('.validation-summary-errors');
    existingSummaries.forEach(summary => summary.remove());

    const errorFields = form.querySelectorAll('.input-validation-error');
    errorFields.forEach(field => field.classList.remove('input-validation-error'));

    const fieldErrors = form.querySelectorAll('.field-validation-error');
    fieldErrors.forEach(error => {
        error.classList.remove('field-validation-error');
        error.classList.add('field-validation-valid');
        error.textContent = '';
    });

    if (window.jQuery && window.jQuery.validator && jQuery(form).valid && !jQuery(form).valid()) {
        return false;
    }

    const formData = new FormData(form);

    fetch(form.action, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(data => {
                    throw data;
                });
            }
            location.reload();
        })
        .catch(errorData => {
            let errorMsg = 'Error: ' + (errorData.message || 'Unknown error');

            try {
                if (errorData && errorData.error && errorData.error.description) {
                    errorMsg = errorData.error.description;

                    const errorLines = errorMsg.split('\n');
                    errorLines.forEach(line => {
                        const fieldMatch = line.match(/The\s+(\w+)\s+field/i);
                        if (fieldMatch && fieldMatch[1]) {
                            const fieldName = fieldMatch[1];

                            const inputField = form.querySelector(`[name="${fieldName}"]`);
                            if (inputField) {
                                inputField.classList.add('input-validation-error');

                                const errorSpan = form.querySelector(`[data-valmsg-for="${fieldName}"]`);
                                if (errorSpan) {
                                    errorSpan.textContent = line;
                                    errorSpan.classList.add('field-validation-error');
                                    errorSpan.classList.remove('field-validation-valid');
                                }
                            }
                        }
                    });
                }
            } catch (ex) {
                console.error('Error parsing error response', ex);
            }

            const errorSummary = document.createElement('div');
            errorSummary.className = 'validation-summary-errors text-danger';
            errorSummary.setAttribute('data-valmsg-summary', 'true');

            const errorList = document.createElement('ul');
            const errorItem = document.createElement('li');
            errorItem.textContent = errorMsg;

            errorList.appendChild(errorItem);
            errorSummary.appendChild(errorList);
            form.prepend(errorSummary);
        });

    return false;
}

function prepareAndSubmitOpeningDateForm() {
    const displayDateField = document.getElementById('newOpeningDateDisplay');
    const utcHiddenField = document.getElementById('newOpeningDateUtc');

    if (displayDateField.value) {
        const localDateTimeString = displayDateField.value;

        const localDate = new Date(localDateTimeString);
        utcHiddenField.value = localDate.toISOString();
    } else {
        utcHiddenField.value = '';
    }
    return handleFormSubmit('editOpeningDateForm');
}



