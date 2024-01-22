function submitForm(formId) {
    const form = document.getElementById(formId);
    
    const jsonObject = {};
    for (let i = 0; i < form.elements.length; i++) {
        const element = form.elements[i];
        if (element.type !== 'submit') {
            if (element.type === 'checkbox') {
                jsonObject[element.id] = element.checked;
            } else {
                jsonObject[element.id] = element.value;
            }
        }
    }

    const xhr = new XMLHttpRequest();
    xhr.open('POST', form.action, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onload = function () {
        if (xhr.status === 200) {
            alert("Page was created")
        } else {
            alert('Error:' + xhr.statusText);
        }
    };
    xhr.send(JSON.stringify(jsonObject));
}