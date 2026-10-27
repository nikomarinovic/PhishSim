const form = document.getElementById('loginForm');

form.addEventListener('submit', (e) => {
    e.preventDefault();

    fetch('/demo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ event: 'form-submitted' })
    }).then((response) => {
        if (!response.ok) {
            throw new Error('Simulation endpoint returned an error');
        }

        window.alert('Training simulation complete. No information was collected.');
    }).catch(() => {
        window.alert('The local simulation could not be reached. No information was collected.');
    }).finally(() => form.reset());
});